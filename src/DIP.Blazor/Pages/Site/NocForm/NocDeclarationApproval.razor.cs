using DIP.EServices;
using DIP.QrService;
using DIP.UaePassService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using StgDipService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIP.SoapServices;

namespace DIP.Blazor.Pages.Site.NocForm
{
    public partial class NocDeclarationApproval
    {
        [Parameter]
        public string Lang { get; set; }

        public string Landlord { get; set; }
        [Inject]
        public IUaePassClient IUaePassClient { get; set; }
        [Inject]
        IDistributedCache Cache { get; set; }
        public string ReferenceNumber { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        public EServiceFrontEnd PageInfoFrontEnd { get; set; }
        [Inject]
        public INOCServiceWrapper NOCService { get; set; }
        public int InvalidAttempt { get; set; } = 0; //0-Initializing,1-failed,2-canceled,3-finished,4-InvalidActions, 5- Sessionout,6-Already singed,7-Rejected
        public List<EServiceFrontEnd> EServiceList { get; set; }
        public ClsRegistration Info { get; private set; }
        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public string _tryAgainUrl { get; set; }
        private string _accessToken { get; set; }
        public string _status { get; set; }
        public string _signerProcessId { get; set; }
        public DeclarationCacheDTO _declarationCacheDto { get; set; }
        public SignProcessResponse _signProcessResponse { get; set; }
        private EOCheckStatusOfDocumentSigningResult EOCheckStatusOfDocumentSigningResult { get; set; }
        public string _siggnExist { get; set; } = string.Empty;
        public NocDeclarationApproval()
        {
        }

        protected override async Task OnInitializedAsync()
        {
            InvalidAttempt = 0;
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);
            _status = queryParams["status"];
            _signerProcessId = queryParams["signer_process_id"];
            _accessToken = await IUaePassClient.GetAccessTokenAsync();
            GetEServicesInput getEServicesInput = new()
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };
            EServiceList = await EServicesAppService.GetListFrontEndAsync(getEServicesInput);

            if (EServiceList != null && EServiceList.Count > 0)
            {
                PageInfoFrontEnd = EServiceList.FirstOrDefault(x => x.Slug.Equals("Ejari"));
            }
            await PrepareData();
        }

        private async Task PrepareData()
        {
            if (string.IsNullOrEmpty(_signerProcessId))
            {
                InvalidAttempt = 4;
                return;
            }
            var cacheresponseBytes = await Cache.GetAsync(_signerProcessId);
            if (cacheresponseBytes == null)
            {
                InvalidAttempt = 5;
                return;
            }

            string json = Encoding.UTF8.GetString(cacheresponseBytes);

            _declarationCacheDto = JsonConvert.DeserializeObject<DeclarationCacheDTO>(json) ?? throw new NullReferenceException("Argument not be nul in the response value.(C1)");
            _signProcessResponse = _declarationCacheDto?.signProcessResponse ?? throw new NullReferenceException("Argument not be nul in the response value.(C2)");

            string refnumber = await NOCService.VerifiedDeclarationLandlordAsync(_declarationCacheDto.EncryptedRefNo);

            ReferenceNumber = refnumber;
            Landlord = string.IsNullOrEmpty(refnumber) ? string.Empty : EncryptionHelper.EncryptUrlSafe(ReferenceNumber);

            if (string.IsNullOrEmpty(refnumber) || string.IsNullOrEmpty(_status) || string.IsNullOrEmpty(_signerProcessId))
            {
                InvalidAttempt = 4;
                return;
            }
            switch (_status)
            {
                case "failed":
                    InvalidAttempt = 1;
                    break;
                case "canceled":
                    InvalidAttempt = 2;
                    break;
                case "finished":
                    InvalidAttempt = 3;
                    break;
                default:
                    InvalidAttempt = 4;
                    break;
            }
            if (_status == "finished")
            {
                //ReferenceNumber = refnumber;
                //Landlord = EncryptionHelper.EncryptUrlSafe(ReferenceNumber);
                Info = await NOCService.GetRegistrationInfoAsync(ReferenceNumber);
                byte[] signedDocumentBytes = await IUaePassClient.FetchSignedDocumentAsync(_signProcessResponse.Documents[0].Id, _accessToken);
                string base64String = Convert.ToBase64String(signedDocumentBytes);

                EOCheckStatusOfDocumentSigningResult = await NOCService.EOCheckStatusOfDocumentSigningAsync(ReferenceNumber, _declarationCacheDto.UserInfo.Email);
                _siggnExist = EOCheckStatusOfDocumentSigningResult.ApllicationSigned ? "Yes" : "No";
                var status = await NOCService.CheckProcessingPageAsync(ReferenceNumber);
                _siggnExist = status.StatusName == "Rejected" ? "Rejected" : _siggnExist;

                if (_siggnExist == "Yes")
                {
                    InvalidAttempt = 6;
                    return;
                }
                else if (_siggnExist == "Rejected")
                {
                    InvalidAttempt = 7;
                    return;
                }
                else
                {
                    var obj = new ClsEODocument()
                    {
                        RefNo = ReferenceNumber.ToString(),
                        AttachmentName = ReferenceNumber + DateTime.Now.ToString("yyyyy") + ".pdf",
                        DocName = ReferenceNumber + DateTime.Now.ToString("yyyyy") + ".pdf",
                        DMSDocument = base64String,
                        DocSize = "0",
                        DocType = "pdf",
                        DocumentCode = string.Empty,
                        Id = 0,
                        IsMandatory = false,
                        IsProcessed = false,
                        IsUploaded = true,
                        IdSpecified = true,
                        UploadedDateTime = DateTime.Now.ClearTime(),

                    };
                    bool isuploaded = await NOCService.UploadSignedDeclarationDocumentForNOCAsync(obj, _declarationCacheDto.UserInfo.Email);
                    if (!isuploaded)
                    {
                        InvalidAttempt = 4;
                        return;
                    }
                    //bool response = await NOCService.ApproveNocApplicationByLandLordAsync(string.Empty, ReferenceNumber);
                    //if (!response)
                    //{
                    //    InvalidAttempt = 4;
                    //    return;
                    //}
                }
            }
        }
    }
}
