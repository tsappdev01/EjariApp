//using Microsoft.AspNetCore.Components;
//using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.Security.Cryptography.Xml;
//using System.Threading.Tasks;

//namespace DIP.Blazor.Pages
//{
//    public class MyLoginContainer
//    {
//        public int Value { get; set; }
    
//         ProtectedLocalStorage LocalStorage { get; set; }



//        public async void SetValue(int value)
//        {
//            this.Value = value;
//           await LocalStorage.SetAsync("MyLoginContainer", value);
//            //NotifyStateChanged();
//        }

//        public async Task<int> GetValue()
//        {
//            var res = await LocalStorage.GetAsync<int>("MyLoginContainer");
//            if (res.Success)
//            {
//                return res.Value;
//            }
//            else
//            {

//                return default;
//            }
//        }
//        //private void NotifyStateChanged() => OnStateChange?.Invoke();
//    }
//}
