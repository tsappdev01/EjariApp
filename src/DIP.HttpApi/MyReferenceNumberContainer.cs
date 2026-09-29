//using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
//using Microsoft.AspNetCore.Components;
//using Microsoft.AspNetCore.Http;
//using System;
//using Scriban.Runtime.Accessors;
//using System.Threading.Tasks;

//namespace DIP.Blazor.Pages
//{
//    public class MyReferenceNumberContainer
//    {
//        public string Value { get; set; }

//        ProtectedLocalStorage LocalStorage { get; set; }

//        public async void SetValue(string value)
//        {
//            this.Value = value;
//            await LocalStorage.SetAsync("MyReferenceNumberContainer", value);
//        }

//        public async Task<string> GetValue()
//        {
//          var  res =  await LocalStorage.GetAsync<string>("MyReferenceNumberContainer");
//            if (res.Success)
//            {
//                return res.Value;
//            }
//            else
//            {

//                return null;
//            }
//        }
//    }
//}




