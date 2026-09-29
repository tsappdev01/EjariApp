//using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
//using Microsoft.AspNetCore.Components;
//using Microsoft.AspNetCore.Http;
//using System;
//using System.Threading.Tasks;

//namespace DIP.Blazor.Pages
//{
//    public class MyStateContainer
//    {
//        public IFormCollection Value { get; set; }

     
//        ProtectedLocalStorage LocalStorage { get; set; }
//        public async void SetValue(IFormCollection value)
//        {
//            this.Value = value;
//            await LocalStorage.SetAsync("MyStateContainer", value);

//        }

//        public async Task<IFormCollection> GetValue()
//        {
//            var res = await LocalStorage.GetAsync<IFormCollection>("MyStateContainer");
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
