using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Json;
using Microsoft.Extensions.Logging;

namespace DIP.MakaniNumber
{
    [RemoteService(IsEnabled = false)]

    public class MakaniNumberAppService : ApplicationService, IMakaniNumberAppService
    {

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly IJsonSerializer _jsonSerializer;
        public MakaniNumberAppService(
      IHttpClientFactory httpClientFactory,
      IJsonSerializer jsonSerializer
      )
        {
            _httpClientFactory = httpClientFactory;
            _jsonSerializer = jsonSerializer;
        
        }

        public async Task<MakaniNumberPropertyDto> GetMakaniNumberAsync(string plotNumber)
        {

            //var svc = new MakaniNumberService.MakaniPhase2ProxyClient();
            //MakaniNumberService.SearchResult searchResult = new MakaniNumberService.SearchResult();
            //searchResult.featureclass_id = "5";
            //searchResult.dgis_id = plotNumber;
            //searchResult.userid = "";
            //searchResult.sessionid = "";

            //var result = await svc.SmartSearchResultAsync(searchResult, "l4f76613re4junvvbbd4ufki1j!=+=o6JGpy02dppzxKfVibRYfQ==", "MAKANI PHASE 2");
            //var makaniNumber = _jsonSerializer.Deserialize<MakaniNumberPropertyDto>(result.ToString());

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var url = $"https://www.makani.ae/MakaniPhase2ProxyWebService/MakaniPhase2Proxy.svc/SmartSearchResult";

            httpClient.Timeout = TimeSpan.FromSeconds(1000);

            MakaniNumberService.SearchResult searchResult = new MakaniNumberService.SearchResult();
            searchResult.featureclass_id = "5";
            searchResult.dgis_id = plotNumber;
            searchResult.userid = "";
            searchResult.sessionid = "";

            CallMakaniNumberDto callMakaniNumberDto = new CallMakaniNumberDto();

            callMakaniNumberDto.InputJson = new InputJson();

            callMakaniNumberDto.InputJson.featureclass_id="5";
            callMakaniNumberDto.InputJson.dgis_id = plotNumber;
            callMakaniNumberDto.InputJson.sessionid = "";
            callMakaniNumberDto.InputJson.userid = "";
            callMakaniNumberDto.Token = "l4f76613re4junvvbbd4ufki1j!=+=o6JGpy02dppzxKfVibRYfQ==";

            callMakaniNumberDto.Remarks = "MAKANI PHASE 2";

            var json = _jsonSerializer.Serialize(callMakaniNumberDto);

            HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");



            using var response = await httpClient.PostAsync(url, httpContent);

            if (!response.IsSuccessStatusCode)
            {
                //return null;
                var errorRawResponse = await response.Content.ReadAsStringAsync();


                throw new UserFriendlyException("Please try again");


            }
            else
            {
                var rawResponse = await response.Content.ReadAsStringAsync();

                var result = _jsonSerializer.Deserialize<MakaniNumberPropertyDto>(rawResponse);
                if (result.IsExp)
                {
                    return result;

                }
                else
                    return result;

            }




            throw new UserFriendlyException("Please try again");



        }
    }
}
