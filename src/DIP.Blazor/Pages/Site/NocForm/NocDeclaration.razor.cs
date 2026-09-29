using Blazorise;
using DIP.EServices;
using DIP.SoapServices;
using DIP.UaePassService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using StgDipService;
using SweetAlertBlazor;
using System.Globalization;
using System.Text.RegularExpressions;
using Volo.Abp;

namespace DIP.Blazor.Pages.Site.NocForm
{
    public partial class NocDeclaration
    {
        [Parameter]
        public string Lang { get; set; }

        [Parameter]
        public string Landlord { get; set; }
        private string encryptedReferenceNumber;
        [Inject]
        public IUaePassClient IUaePassClient { get; set; }
        [Inject]
        IDistributedCache Cache { get; set; }
        private string DataUrl { get; set; }
        public string ReferenceNumber { get; set; }
        public bool InvalidAttempt { get; set; } = false;
        [Inject]
        NavigationManager NavigationManager { get; set; }
        public EServiceFrontEnd PageInfoFrontEnd { get; set; }
        [Inject]
        public INOCServiceWrapper NOCService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }
        public List<ClsEODocument> DocumentsGrid { get; private set; }
        public List<ClsEODocument> DocumentsGridDMSEJAPP { get; private set; }
        public ClsRegistration Info { get; private set; }
        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public string ErrorMessageMissingDocuments { get; set; }
        public string ErrorMessageUpload { get; set; }
        public byte[] FormFileContent { get; set; }
        public string FormFile { get; set; } = "";
        public string Designation { get; set; } = "";
        public bool fileUploaded { get; set; } = false;
        public bool ButtonDisable { get; set; } = false;
        private string _accessToken { get; set; }
        private string _uaepassRedirectionLink { get; set; }
        private string _uaepassloginRedirectionLink { get; set; }
        private bool _isLoading { get; set; } = true;

        private bool isCaptchaValid = false;
        public string _siggnExist { get; set; } = string.Empty;

        private string defaultImage = "/assets/images/UAEPASS_Sign_in_Active.png";
        private string hoverImage = "/assets/images/UAEPASS_Sign_in_Focus.png";
        private string clickedImage = "/assets/images/UAEPASS_Sign_in_Pressed.png";

        public bool UAEPassAuthorized { get; set; } = false;
        public string UAEPassAuthorizedToken { get; set; } = string.Empty;
        public UserInfoResponse UserInfoResponse { get; set; }
        public ClsDocumentDetailDTO SignclsDocumentDetail { get; set; }
        private EOCheckStatusOfDocumentSigningResult EOCheckStatusOfDocumentSigningResult { get; set; }
        private EOGETCurrentLandlordSignatureDetailsResult CurrentSignature { get; set; }

        private string currentImage;

        private Modal videoModalRef;
        private bool isVideoModalOpen;

        public NocDeclaration()
        {
        }
        protected override async Task OnInitializedAsync()
        {

            currentImage = defaultImage;
            var decoded = Uri.UnescapeDataString(Landlord);
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

            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var query = QueryHelpers.ParseQuery(uri.Query);
            if (query.TryGetValue("UAEPassAuthorized", out var authorized) && authorized == "true")
            {
                UAEPassAuthorized = true;
            }
            if (query.TryGetValue("UAEPassAuthorizedToken", out var token) && !string.IsNullOrEmpty(token))
            {
                UAEPassAuthorizedToken = token;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            await PrepareData();
            StateHasChanged();
        }


        private async Task PrepareData()
        {
            try
            {
                if (string.IsNullOrEmpty(UAEPassAuthorizedToken))
                {
                    UAEPassAuthorized = false;
                    await PopulateLoginUrl();
                }
                try
                {
                    var userInfoCacheData = await Cache.GetStringAsync(UAEPassAuthorizedToken);
                    var declarationUserInffoCache = JsonConvert.DeserializeObject<UserInfoResponse?>(userInfoCacheData ?? string.Empty);
                    if (declarationUserInffoCache == null)
                    {
                        UAEPassAuthorized = false;
                        await PopulateLoginUrl();
                        return;
                    }
                    UserInfoResponse = declarationUserInffoCache;
                }
                catch
                {
                    UAEPassAuthorized = false;
                    await PopulateLoginUrl();
                }
                string refnumber = await NOCService.VerifiedDeclarationLandlordAsync(Landlord);
                if (string.IsNullOrEmpty(refnumber))
                {
                    InvalidAttempt = true;
                }
                ReferenceNumber = refnumber;
                encryptedReferenceNumber = EncryptionHelper.EncryptUrlSafe(ReferenceNumber);
                EOCheckStatusOfDocumentSigningResult = await NOCService.EOCheckStatusOfDocumentSigningAsync(ReferenceNumber, UserInfoResponse.Email);
                _siggnExist = EOCheckStatusOfDocumentSigningResult.ApllicationSigned ? "Yes" : "No";
                var status = await NOCService.CheckProcessingPageAsync(ReferenceNumber);
                _siggnExist = status.StatusName == "Rejected" ? "Rejected" : _siggnExist;
                Info = await NOCService.GetRegistrationInfoAsync(ReferenceNumber);
                if (_siggnExist == "Yes" || _siggnExist == "Rejected" || UAEPassAuthorized || !string.IsNullOrEmpty(UAEPassAuthorizedToken))
                {
                    UAEPassAuthorized = true;
                    await PopulateDocumentsList();
                }
                else
                {
                    UAEPassAuthorized = false;
                    await PopulateLoginUrl();
                }
            }
            finally
            {
                _isLoading = false;
            }
        }


        private async Task PopulateLoginUrl()
        {
            string CacheId = Guid.NewGuid().ToString();
            UAEpassLoginCacheDTO declarationCacheDTO = new()
            {
                EncryptedRefNo = Landlord
            };
            await Cache.SetStringAsync(CacheId, JsonConvert.SerializeObject(declarationCacheDTO),
                         new DistributedCacheEntryOptions
                         {
                             AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)
                         });
            _uaepassloginRedirectionLink = IUaePassClient.GetuaepassloginRedirectionLink(CacheId);
        }

        private async Task PopulateDocumentsList()
        {
            CurrentSignature = await NOCService.EOGETCurrentLandlordSignatureDetailsAsync(ReferenceNumber);
            var documents = await NOCService.GetDocumentsListAsync(ReferenceNumber);
            DocumentsGrid = [.. documents.Where(x => x.IsUploaded == true && x.DocumentCode != "DMSEJAPP")];
            DocumentsGridDMSEJAPP = [.. documents.
                                                Where(x => x.DocumentCode == ( CurrentSignature.CurrentSignerLevel > 1 ? "DMSEJAPPV2" :  _siggnExist == "No" || _siggnExist == "Rejected"  ? "DMSEJAPP" : "DMSEJAPPV2"))];

            SignclsDocumentDetail = await NOCService.GetDocumentForRefNoNEWAsync(ReferenceNumber, DocumentsGridDMSEJAPP[0]?.Id ?? 0);
            DataUrl = $"data:application/pdf;base64,{SignclsDocumentDetail?.DMSDocument ?? string.Empty ?? string.Empty} ";
        }

        private async Task CreateUAEPassSignDocProcessAsync()
        {
            if (SignclsDocumentDetail != null && _siggnExist == "No")
            {
                _accessToken = await IUaePassClient.GetAccessTokenAsync();
                SignProcessResponse response = await IUaePassClient.CreateSignProcessAsync(SignclsDocumentDetail.DMSDocument ?? string.Empty,
                Landlord, Lang, DocumentsGridDMSEJAPP[0]?.DocName, _accessToken, CurrentSignature);
                if (response != null)
                {
                    _uaepassRedirectionLink = response.Tasks.Pending[0].Url;
                    DeclarationCacheDTO declarationCacheDTO = new()
                    {
                        EncryptedRefNo = Landlord,
                        signProcessResponse = response,
                        UserInfo = UserInfoResponse,
                    };

                    await Cache.SetStringAsync(response.Id, JsonConvert.SerializeObject(declarationCacheDTO),
                           new DistributedCacheEntryOptions
                           {
                               AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                           });
                }
            }
        }

        public async Task HandleReject()
        {
            int updateApplicationStatus = await NOCService.UpdateNocApplicationStatusByPropsAsync(ReferenceNumber, "Rejected");
            if (updateApplicationStatus == 1)
            {
                _siggnExist = "Rejected";
                StateHasChanged();
                await JS.InvokeVoidAsync("eval", "window.scrollTo({ top: 50, behavior: 'smooth' })");
            }
            return;
        }

        public async Task HandleUAEPassSignatureFow()
        {
            EOCheckStatusOfDocumentSigningResult = await NOCService.EOCheckStatusOfDocumentSigningAsync(ReferenceNumber, UserInfoResponse.Email);
            _siggnExist = EOCheckStatusOfDocumentSigningResult.ApllicationSigned ? "Yes" : "No";
            var status = await NOCService.CheckProcessingPageAsync(ReferenceNumber);
            _siggnExist = status.StatusName == "Rejected" ? "Rejected" : _siggnExist;
            if (_siggnExist == "Yes")
            {
                await JS.InvokeVoidAsync("Swal.fire", new
                {
                    icon = "warning",
                    html = "This document has already been signed by.<br/>" +
                    $"{EOCheckStatusOfDocumentSigningResult?.SignerEmail} At {EOCheckStatusOfDocumentSigningResult?.SignerSignedAt:yyyy-MM-dd HH:mm:ss}.",
                    showConfirmButton = true,
                    confirmButtonText = "Ok",
                    confirmButtonColor = "#6f6259",
                    allowOutsideClick = true,
                    allowEscapeKey = true,
                    timer = 10000,
                    timerProgressBar = true
                });
                await InvokeAsync(StateHasChanged);
                return;
            }
            else if (_siggnExist == "Rejected")
            {
                await JS.InvokeVoidAsync("Swal.fire", new
                {
                    icon = "warning",
                    html = "This document has been rejected by landlord.<br/>",
                    showConfirmButton = true,
                    confirmButtonText = "Ok",
                    confirmButtonColor = "#6f6259",
                    allowOutsideClick = true,
                    allowEscapeKey = true,
                    timer = 10000,
                    timerProgressBar = true
                });
                await InvokeAsync(StateHasChanged);
                return;
            }
            await CreateUAEPassSignDocProcessAsync();
            await Navigat(_uaepassRedirectionLink);
        }

        public async Task OnProcced()
        {
            ButtonDisable = true;
            ErrorMessageUpload = null;
            ErrorMessageMissingDocuments = string.Empty;

            if (fileUploaded is bool V && V == true)
            {
                bool response = await NOCService.ApproveNocApplicationByLandLordAsync(Designation, ReferenceNumber);
                if (response)
                {
                    var swalAlertModel = new SwalModel("Tenant Contract Application Approved successfully.\nThank you for choosing Dubai Investments Park.", "")
                        .WithIcon(SweetAlert.Icon.Success) // Other Icons are Success, Error and Warning
                        .WithButton(SweetAlert.Button.Ok())
                        .SetClosingOptions(closeOnEscButton: true, closeOnOutsideClick: true);

                    await JS.ShowSwalAsync(swalAlertModel);
                }
                ButtonDisable = false;
            }
            else
            {
                ButtonDisable = false;
                ErrorMessageUpload = "Please upload signed Tenant Contract Document.";
            }
        }

        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }

        private void OnSuccessCaptcha(bool isValid)
        {
            isCaptchaValid = isValid;
            if (isValid)
            {
                StateHasChanged();
            }
        }

        #region OnChange Methods 
        private async Task OpenVideoModal()
        {
            isVideoModalOpen = true;
            await videoModalRef.Show();
        }
        private async Task OnVideoModalClosed()
        {
            isVideoModalOpen = false;
        }
        #endregion
    }
}
