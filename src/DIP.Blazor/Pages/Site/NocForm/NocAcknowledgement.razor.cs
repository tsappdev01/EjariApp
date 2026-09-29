using Blazorise;
using DIP.EServices;
using DIP.PageInfos;
using DIP.QrService;
using DIP.UaePassService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using StgDipService;
using SweetAlertBlazor;
using System.Globalization;
using System.Web;
using static DIP.Blazor.Pages.Site.NocForm.StepProgress;
using DIP.SoapServices;

namespace DIP.Blazor.Pages.Site.NocForm
{
    public partial class NocAcknowledgement : IDisposable
    {
        [Parameter]
        public string Lang { get; set; }

        [Parameter]
        public string ReferenceNumber { get; set; }

        private string encryptedReferenceNumber;

        #region StepProgress
        private int currentStep = 3;
        private static readonly List<StepModel> stepModels = StepProgress.GetRegistrationSteps();
        private List<StepModel> registrationSteps = stepModels;

        private void HandleStepChanged(int newStep)
        {
            currentStep = newStep;
        }
        #endregion

        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }
        [Inject]
        public IConfiguration Configuration { get; set; }
        [Inject]
        public IUaePassClient IUaePassClient { get; set; }
        [Inject]
        public IQrServiceClient IQrServiceClient { get; set; }
        public EServiceFrontEnd PageInfoFrontEnd { get; set; }
        [Inject]
        public IPageInfosAppService PageInfosAppService { get; set; }
        [Inject]
        public INOCServiceWrapper NOCService { get; set; }
        [Inject]
        ILogger<NocAcknowledgement> _logger { get; set; }
        public ClsEOGETTenantEmail _clsEOGETTenant { get; set; }
        public string StatusName { get; set; }
        [Inject]
        public IEServicesAppService EServicesAppService { get; set; }
        public List<EServiceFrontEnd> EServiceList { get; set; }
        public ClsRegistration Info { get; private set; }
        public Modal modalRef;
        public string _otpsendDetails { get; set; } = string.Empty;
        public string _otpsentContact { get; set; } = string.Empty;
        public string[] Otpcode { get; set; } = Enumerable.Repeat(string.Empty, 6).ToArray();
        public bool OtpSending { get; set; }
        public bool OtpVerifyLoading { get; set; } = false;
        public string OtpErrorMessage { get; set; } = string.Empty;
        public bool AgreeCheckbox { get; set; } = false;
        public bool NocApplicationProceed = false;
        public bool contactSelction = true;
        public bool modelclose { get; set; } = true;
        public string NOCFor { get; set; } = string.Empty;
        public bool _reviewDocument { get; set; } = true;

        public bool resendOTPEnable = false;
        private int countdown = 60;
        private System.Timers.Timer? _resendOtpTimer;
        string selectedTab = "Tenant";//Tenant or Landlord

        private DotNetObjectReference<NocAcknowledgement>? _otpDotNetRef;
        private bool _otpJsWired;

        #region prevent initial load
        [Inject]
        private PersistentComponentState ApplicationState { get; set; }
        private PersistingComponentStateSubscription persistingSubscription;
        #endregion
        public NocAcknowledgement()
        {
        }

        protected override async Task OnInitializedAsync()
        {
            persistingSubscription = ApplicationState.RegisterOnPersisting(PersistData);

            var decoded = Uri.UnescapeDataString(ReferenceNumber);
            ReferenceNumber = EncryptionHelper.DecryptUrlSafe(decoded);
            encryptedReferenceNumber = EncryptionHelper.EncryptUrlSafe(ReferenceNumber);

            GetEServicesInput getEServicesInput = new GetEServicesInput
            {
                MaxResultCount = 1000,
                SkipCount = 0,
                IsActive = true,
                Sorting = "Order"
            };

            Otpcode = Enumerable.Repeat(string.Empty, 6).ToArray();
            EServiceList = await EServicesAppService.GetListFrontEndAsync(getEServicesInput);

            if (EServiceList != null && EServiceList.Count > 0)
            {
                PageInfoFrontEnd = EServiceList.FirstOrDefault(x => x.Slug.Equals("Ejari"));
            }
            await PrepareData();
        }
        private Task PersistData()
        {
            ApplicationState.PersistAsJson("EServiceList", EServiceList);
            return Task.CompletedTask;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await TryWireOtpJsAsync();
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task TryWireOtpJsAsync()
        {
            var otpEntryVisible = !contactSelction && !NocApplicationProceed && !OtpVerifyLoading;

            if (!otpEntryVisible)
            {
                if (_otpJsWired)
                {
                    await JS.InvokeVoidAsync("dipNocOtp.unwire");
                    _otpJsWired = false;
                }
                return;
            }

            if (_otpJsWired)
                return;

            try
            {
                _otpDotNetRef ??= DotNetObjectReference.Create(this);
                var wired = await JS.InvokeAsync<bool>("dipNocOtp.wire", _otpDotNetRef);
                _otpJsWired = wired;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "dipNocOtp.wire failed");
                _otpJsWired = false;
            }
        }
        private async Task PrepareData()
        {
            var status = await NOCService.CheckProcessingPageAsync(ReferenceNumber);
            StatusName = status.StatusName;
            await RedirectionToCorrespondingPage();
            Info = await NOCService.GetRegistrationInfoAsync(ReferenceNumber);
            var details = await NOCService.GetRegistrationDetailsAsync(ReferenceNumber);
            NOCFor = HttpUtility.HtmlEncode(details.NOCFor);
            _logger.LogInformation("NocAcknowledgement Noc for values : {NOCFor}", NOCFor);
            _clsEOGETTenant = await NOCService.EOGETTenantEmailAndMobileAsync(ReferenceNumber);
            _reviewDocument = await NOCService.CheckDocumentBackOfficeReviewCountAsync(ReferenceNumber);
        }


        #region
        private Task OnSelectedTabChanged(string name)
        {
            if (selectedTab != name)
            {
                _otpsendDetails = string.Empty;
            }
            selectedTab = name;

            return Task.CompletedTask;
        }
        public async Task HandleProceedAgree()
        {
            OtpErrorMessage = string.Empty;
            if (AgreeCheckbox)
            {
                await modalRef.Show();
                contactSelction = true;
            }
        }
        public async Task HandleTriggerOtp()
        {
            if (string.IsNullOrEmpty(_otpsendDetails) || OtpSending)
            {
                return;
            }

            OtpSending = true;
            OtpErrorMessage = string.Empty;
            await InvokeAsync(StateHasChanged);

            try
            {
                _otpsentContact = ExtractOtpContactValue(_otpsendDetails);
                int sendotpresponse = await NOCService.SendVerificationOTPToNocApplicationAsync(
                    ReferenceNumber,
                    _otpsentContact,
                    _otpsendDetails.Contains("Mobile", StringComparison.Ordinal) ? 1 : 2);

                if (sendotpresponse == 1)
                {
                    contactSelction = false;
                    Otpcode = Enumerable.Repeat(string.Empty, 6).ToArray();
                    OtpErrorMessage = string.Empty;
                    StartResendCountdown();
                    try
                    {
                        await JS.InvokeVoidAsync("dipNocOtp.clearAll");
                    }
                    catch
                    {
                        // OTP inputs may not exist until next paint
                    }
                    await modalRef.Show();
                }
                else
                {
                    OtpErrorMessage = "Unable to send OTP. Please try again or pick a different contact option.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Send OTP failed for reference {Ref}", ReferenceNumber);
                OtpErrorMessage = "Unable to send OTP. Please check your connection and try again.";
            }
            finally
            {
                OtpSending = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        private static string ExtractOtpContactValue(string otpSendDetails)
        {
            var marker = "%%";
            var idx = otpSendDetails.IndexOf(marker, StringComparison.Ordinal);
            var raw = idx >= 0 ? otpSendDetails[..idx] : otpSendDetails;
            if (otpSendDetails.Contains("Mobile", StringComparison.Ordinal) && !raw.Contains('+'))
            {
                return "+" + raw.TrimStart();
            }
            return raw;
        }


        public async Task HandleChangeOtpOption()
        {
            if (OtpSending)
            {
                return;
            }

            contactSelction = true;
            _otpsendDetails = string.Empty;
            Otpcode = Enumerable.Repeat(string.Empty, 6).ToArray();
            OtpErrorMessage = string.Empty;
            StopResendTimer();
            resendOTPEnable = false;
            await InvokeAsync(StateHasChanged);
            return;
        }

        public async Task HandleResendOtpClick()
        {
            if (!resendOTPEnable || OtpSending)
            {
                return;
            }

            await HandleTriggerOtp();
        }

        [JSInvokable]
        public Task SyncOtpFromClient(string[]? digits)
        {
            if (digits == null || digits.Length != 6)
                return Task.CompletedTask;

            for (var i = 0; i < 6; i++)
            {
                var d = digits[i] ?? string.Empty;
                Otpcode[i] = d.Length > 0 && char.IsDigit(d[0])
                    ? d[0].ToString()
                    : string.Empty;
            }

            return InvokeAsync(StateHasChanged);
        }

        private async Task ApplyOtpFromDomAsync()
        {
            try
            {
                var digits = await JS.InvokeAsync<string[]>("dipNocOtp.readDigits");
                if (digits == null || digits.Length < 6)
                    return;

                for (var i = 0; i < 6; i++)
                {
                    var d = digits[i] ?? string.Empty;
                    Otpcode[i] = d.Length > 0 && char.IsDigit(d[0])
                        ? d[0].ToString()
                        : string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "dipNocOtp.readDigits failed before verify");
            }
        }

        public async Task HandleVerificationOtp()
        {
            if (OtpVerifyLoading)
            {
                return;
            }

            await ApplyOtpFromDomAsync();

            OtpErrorMessage = string.Empty;
            NocApplicationProceed = false;

            if (Otpcode.Any(x => string.IsNullOrEmpty(x)))
            {
                OtpErrorMessage = "Please enter all 6 digits of the OTP";
                await InvokeAsync(StateHasChanged);
                return;
            }

            var otpString = string.Concat(Otpcode);
            if (!int.TryParse(otpString, NumberStyles.None, CultureInfo.InvariantCulture, out var otpInt))
            {
                OtpErrorMessage = "Please enter the complete 6-digit OTP";
                await InvokeAsync(StateHasChanged);
                return;
            }

            OtpVerifyLoading = true;
            await InvokeAsync(StateHasChanged);
            int verifyresponse;
            try
            {
                verifyresponse = await NOCService.VerificationSubTenatOTPVerificationAsync(ReferenceNumber, otpInt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Verify OTP failed for reference {Ref}", ReferenceNumber);
                OtpVerifyLoading = false;
                OtpErrorMessage = "Unable to verify OTP. Please try again.";
                await InvokeAsync(StateHasChanged);
                return;
            }

            if (verifyresponse == 3)
            {
                OtpVerifyLoading = false;
                OtpErrorMessage = "OTP Expired";
                await InvokeAsync(StateHasChanged);
                return;
            }

            if (verifyresponse == 2)
            {
                OtpVerifyLoading = false;
                OtpErrorMessage = "Invalid OTP. If you requested a new code, enter only the latest OTP.";
                await InvokeAsync(StateHasChanged);
                return;
            }

            if (verifyresponse == 1)
            {
                OtpVerifyLoading = false;
                NocApplicationProceed = true;
                string NocDeclarationEndpoint = await IUaePassClient.GetNOcDeclarationEnpoint();
                //String responseQR = await QrCodeHelper.ConvertStringToQrBase64Async(ReferenceNumber);
                String responseQR = await QrCodeHelper.ConvertStringToQrBase64Async(NocDeclarationEndpoint + "/" + encryptedReferenceNumber);
                string base64StringQR = ("data:image/png;base64," + responseQR);
                bool response = await NOCService.GenerateNocPdfApplicationAuthAsync(ReferenceNumber, base64StringQR);
                if(Info.ZzApplicationType == "OldUr")
                {
                    int updateApplicationStatus = await NOCService.UpdateNocApplicationStatusByPropsAsync(ReferenceNumber, "Submitted");
                    if (updateApplicationStatus == 1 && response)
                    {
                        modelclose = false;
                        await modalRef.Hide();
                        // Auto-close alert after 5 seconds
                        await JS.InvokeVoidAsync("Swal.fire", new
                        {
                            icon = "info",
                            html = "Application submitted successfuly... processing takes 3-5 working days.<br/>Thank you for choosing Dubai Investments Park.",
                            showConfirmButton = true,
                            confirmButtonText = "Ok",
                            confirmButtonColor = "#6f6259",
                            allowOutsideClick = true,
                            allowEscapeKey = true,
                            timer = 10000,
                            timerProgressBar = true
                        });

                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}");
                    }
                    else
                    {
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}");
                    }
                }else if (_reviewDocument)
                {
                    int updateApplicationStatus = await NOCService.UpdateNocApplicationStatusByPropsAsync(ReferenceNumber, "DocReview");
                    if (updateApplicationStatus == 1 && response)
                    {
                        modelclose = false;
                        await modalRef.Hide();
                        // Auto-close alert after 5 seconds
                        await JS.InvokeVoidAsync("Swal.fire", new
                        {
                            icon = "info",
                            html = "Your submitted documents are currently under review.<br/>You will be notified once the verification is complete.<br/>Thank you for choosing Dubai Investments Park.",
                            showConfirmButton = true,
                            confirmButtonText = "Ok",
                            confirmButtonColor = "#6f6259",
                            allowOutsideClick = true,
                            allowEscapeKey = true,
                            timer = 10000,
                            timerProgressBar = true
                        });

                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}");
                    }
                    else
                    {
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}");
                    }
                }
                else
                {
                    bool uaePassSignatureBypass = Configuration.GetValue<bool>("UaePassSignatureBypass");
                    if (uaePassSignatureBypass)
                    {
                        bool responsebypass = await NOCService.ApproveNocApplicationByLandLordAsync(string.Empty, ReferenceNumber);
                        if (response && responsebypass)
                        {
                            modelclose = false;
                            await modalRef.Hide();
                            // Auto-close alert after 5 seconds
                            await JS.InvokeVoidAsync("Swal.fire", new
                            {
                                icon = "success",
                                html = "NOC application submitted successfully.<br/>Please wait for landlord approval.<br/>Thank you for choosing Dubai Investments Park.",
                                showConfirmButton = true,
                                confirmButtonText = "Ok",
                                confirmButtonColor = "#6f6259",
                                allowOutsideClick = true,
                                allowEscapeKey = true,
                                timer = 10000,
                                timerProgressBar = true
                            });

                            await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}");
                        }
                        else
                        {
                            await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}");
                        }
                    }
                    else
                    {
                        OtpVerifyLoading = false;
                        bool resultoflandlord = await NOCService.UpdateNocApplicationToPendingAuthAsync(ReferenceNumber);
                        if (response && resultoflandlord)
                        {
                            modelclose = false;
                            await modalRef.Hide();
                            // Auto-close alert after 5 seconds
                            await JS.InvokeVoidAsync("Swal.fire", new
                            {
                                icon = "success",
                                html = "NOC application submitted successfully.<br/>Please wait for landlord approval.<br/>Thank you for choosing Dubai Investments Park.",
                                showConfirmButton = true,
                                confirmButtonText = "Ok",
                                confirmButtonColor = "#6f6259",
                                allowOutsideClick = true,
                                allowEscapeKey = true,
                                timer = 10000,
                                timerProgressBar = true
                            });

                            await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}");
                        }
                        else
                        {
                            await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}");
                        }
                    }
                }

                return;
            }

            OtpVerifyLoading = false;
            OtpErrorMessage = "Verification could not be completed. Please try again.";
            await InvokeAsync(StateHasChanged);
        }


        public async Task handleBackAction()
        {
            int updateApplicationStatus = await NOCService.UpdateNocApplicationStatusByPropsAsync(ReferenceNumber, "Upload");
            if (updateApplicationStatus == 1)
            {
                await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDocuments/Upload/{encryptedReferenceNumber}");
            }
            return;
        }
        #endregion

        private void StopResendTimer()
        {
            if (_resendOtpTimer == null)
            {
                return;
            }

            _resendOtpTimer.Stop();
            _resendOtpTimer.Elapsed -= OnResendTimerElapsed;
            _resendOtpTimer.Dispose();
            _resendOtpTimer = null;
        }

        private void OnResendTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            _ = InvokeAsync(OnResendTickAsync);
        }

        private Task OnResendTickAsync()
        {
            countdown--;
            if (countdown <= 0)
            {
                resendOTPEnable = true;
                StopResendTimer();
            }

            StateHasChanged();
            return Task.CompletedTask;
        }

        private void StartResendCountdown()
        {
            StopResendTimer();
            resendOTPEnable = false;
            countdown = 60;

            _resendOtpTimer = new System.Timers.Timer(1000) { AutoReset = true };
            _resendOtpTimer.Elapsed += OnResendTimerElapsed;
            _resendOtpTimer.Start();
        }
        private async Task RedirectionToCorrespondingPage()
        {
            if (!StatusName.Equals("ApplicantAcknowledgement"))
            {
                switch (StatusName)
                {
                    case "Registered":
                        //await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Details/{encryptedReferenceNumber}"); //old flow
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}"); //new flow
                        break;
                    case "Verified":
                        //await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Details/{encryptedReferenceNumber}"); //old flow
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocBasicForm/{encryptedReferenceNumber}"); //new flow
                        break;
                    case "Upload":
                        //await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Uploads/{encryptedReferenceNumber}"); //old flow
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDocuments/Upload/{encryptedReferenceNumber}"); //new flow
                        break;
                    case "ApplicantAcknowledgement":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocAcknowledgement/{encryptedReferenceNumber}"); //new flow
                        break;
                    case "PendingAuth":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}");
                        break;
                    case "Payment":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Payment/{encryptedReferenceNumber}");
                        break;

                    case "PaymentProcess":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/UaepgsGateway/PaymentProcess/{encryptedReferenceNumber}");
                        break;

                    case "Submitted":
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/Status/{encryptedReferenceNumber}");
                        break;
                    default:
                        await Navigat($"{CultureInfo.CurrentCulture.Name}/Ejari/NocDashboard/{encryptedReferenceNumber}");
                        break;
                }
            }
        }
        private async Task Navigat(string url)
        {
            NavigationManager.NavigateTo(url);
        }
        private Task OnModalClosing(ModalClosingEventArgs e)
        {
            // just set Cancel to prevent modal from closing
            e.Cancel = modelclose
                || e.CloseReason != CloseReason.UserClosing;

            return Task.CompletedTask;
        }

        private async Task OnModalCloseManual()
        {
            modelclose = false;
            await modalRef.Hide();
        }

        public void Dispose()
        {
            StopResendTimer();
            persistingSubscription.Dispose();
            try
            {
                _ = JS.InvokeVoidAsync("dipNocOtp.unwire");
            }
            catch (ObjectDisposedException) { /* host shutting down */ }
            catch { /* ignore JS during teardown */ }

            _otpDotNetRef?.Dispose();
            _otpDotNetRef = null;
        }
    }
}
