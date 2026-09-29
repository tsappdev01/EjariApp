
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using Volo.Abp.AspNetCore.Components.Server.LeptonXTheme;

namespace DIP.Blazor.Shared
{
    public class UtilClass
    {

        [Inject]
        NavigationManager NavigationManager { get; set; }

        public static object GetPropValue(object src, string propName, string lang)
        {
            propName = propName + lang[0].ToString().ToUpper() + lang.Substring(1);
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        public static string UpdateUriByLang(string link)
        {
            if (link.EndsWith("/en"))
            {
                link = link.Replace("/en", "");
            }

            if (link.EndsWith("/ar"))
            {
                link = link.Replace("/ar", "");
            }


            if (CultureInfo.CurrentCulture.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase))
                link = link.Replace("/en/", "/ar/");
            else
                link = link.Replace("/ar/", "/en/");

            if (CultureInfo.CurrentCulture.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase))
            {
                if (!link.Contains("/ar"))
                {
                    if (link.EndsWith("/"))
                        link = link + "ar";
                    else
                        link = link + "/ar";
                }
            }
            return link;

        }

        public static string GetHomeUrlByLang()
        {

            if (CultureInfo.CurrentCulture.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase))
            {
                return "/";
            }
            else
                return "/ar";


        }
    }

    public class NocRegistrationModel
    {
        public string Type { get; set; }
        public string PropertyCode { get; set; }
        public string PropertyValue { get; set; }
        public string SubtenantName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string IssuerName { get; set; }
        public string IssuerType { get; set; }
        public string InitialApproval { get; set; }
        public string EmiratesId { get; set; }
        public string TradeLicense { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public string RegisterType { get; set; }

    }

    public class NocOSRegistrationModel
    {
        //List<OSPayments> _lstOSPayments = new List<OSPayments>();
        public string Type { get; set; }
        public string PropertyCode { get; set; }
        public string PropertyValue { get; set; }
        public string SubtenantName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string IssuerName { get; set; }
        public string IssuerType { get; set; }
        public string InitialApproval { get; set; }
        public string EmiratesId { get; set; }
        public string TradeLicense { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public string RegisterType { get; set; }
        // public List<OSPayments> SelectedPayments { get; set; }
        //  public List<OSPayments> SelectedPayments { get { return _lstOSPayments; } set { _lstOSPayments = value; } }
    }

    public class OSPayments
    {
        public string PaymentFor { get; set; }
        public string AccCode { get; set; }
        public string Amount { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Comment { get; set; }
        public string RefNo { get; set; }
    }

    public class OtherChargesPayInfo
    {
        public int SId { get; set; }
        public string ServiceName { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string Comment { get; set; }
        public string Amount { get; set; }
        public string AccCode { get; set; }
    }

    public class FeedbackViewModel
    {
        public string Comment { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int? Select { get; set; }
        public List<Answer> Answers { get; set; }
        public bool SelectService { get; set; }
        public List<Service> Services { get; set; }
        public bool IsPopup { get; set; }
        public string RefNo { get; set; }
    }

    public class Answer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Css { get; set; }
        public bool  IsClick { get; set; }
    }

    public class Service
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }


    public class NocDetailsModel
    {
        public string ReferenceNumber { get; set; }
        public string NocType { get; set; }
        public double Rent { get; set; }
        public double SecurityDeposit { get; set; }
        public string Units { get; set; }
        public int NumberOfUnits { get; set; }
        public string NOCFor { get; set; }
        public string BuildingName { get; set; }
        public string ContractType { get; set; }
        public DateTime ContractFromDate { get; set; }
        public DateTime ContractEndDate { get; set; }
    }
}
