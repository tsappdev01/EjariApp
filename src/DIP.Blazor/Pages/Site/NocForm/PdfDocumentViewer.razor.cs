using DIP.EServices;
using DIP.UaePassService;
using k8s.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using StgDipService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StgDipService.NOCServiceClient; 

namespace DIP.Blazor.Pages.Site.NocForm
{
    public partial class PdfDocumentViewer : IDisposable
    {

        [Parameter]
        public string Lang { get; set; }

        [Parameter]
        public string ReferenceNumber { get; set; }

        private string encryptedReferenceNumber;

        [Parameter]
        public string DocumentId { get; set; }
        private int _DocumentId;
        [Inject]
        NavigationManager NavigationManager { get; set; }
        public EServiceFrontEnd PageInfoFrontEnd { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }
        private string DataUrl { get; set; }

        [Inject]
        private PersistentComponentState ApplicationState { get; set; }
        private PersistingComponentStateSubscription persistingSubscription;

        protected override async Task OnInitializedAsync()
        {
            persistingSubscription = ApplicationState.RegisterOnPersisting(PersistData);

            var decoded = Uri.UnescapeDataString(ReferenceNumber);
            var decodedId = Uri.UnescapeDataString(DocumentId);
            ReferenceNumber = EncryptionHelper.DecryptUrlSafe(decoded);
            _DocumentId = Convert.ToInt32(EncryptionHelper.DecryptUrlSafe(decodedId));
            encryptedReferenceNumber = EncryptionHelper.EncryptUrlSafe(ReferenceNumber);

            if (!ApplicationState.TryTakeFromJson<List<EServiceFrontEnd>>("EServiceList", out var restored))
            {
                GetEServicesInput getEServicesInput = new()
                {
                    MaxResultCount = 1000,
                    SkipCount = 0,
                    IsActive = true,
                    Sorting = "Order"
                };
                EServiceList = await EServicesAppService.GetListFrontEndAsync(getEServicesInput);
            }
            else
            {
                EServiceList = restored;
            }

            if (EServiceList != null && EServiceList.Count > 0)
            {
                PageInfoFrontEnd = EServiceList.FirstOrDefault(x => x.Slug.Equals("Ejari"));
            }
        }

        private Task PersistData()
        {
            ApplicationState.PersistAsJson("EServiceList", EServiceList);
            return Task.CompletedTask;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Point to the controller API
                // Use encryptedReferenceNumber because the API expects the encrypted value (which it then decrypts)
                // Use DocumentId (original param) because it is already the encrypted string (API expects encrypted)
                DataUrl = $"/api/PdfViewer/GetPdf?referenceNumber={Uri.EscapeDataString(encryptedReferenceNumber)}&documentId={Uri.EscapeDataString(DocumentId)}";
                StateHasChanged();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public void Dispose()
        {
            persistingSubscription.Dispose();
        }


    }
}
