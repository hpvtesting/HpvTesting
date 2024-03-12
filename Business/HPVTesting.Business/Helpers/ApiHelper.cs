using HPVTesting.Business.ViewModels.General;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace HPVTesting.Business.Helpers
{
    public class ApiHelper
    {
        public static async Task<ResponseDetail<T>> SendApiRequest<T>(object data, string url, HttpMethod httpMethod, string apiToken, string culture)
        {
            var responseDetail = new ResponseDetail<T>();
            try
            {
                var baseUrl = new AppSettings().ApiUrl + url;
                if (httpMethod == HttpMethod.Get || httpMethod == HttpMethod.Delete)
                {
                    baseUrl += Convert.ToString(data);
                }
                var client = new HttpClient { BaseAddress = new Uri(baseUrl) };
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
                client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue(culture));
                client.Timeout = TimeSpan.FromMinutes(10);

                HttpResponseMessage response;

                if (httpMethod == HttpMethod.Get)
                {
                    response = await client.GetAsync(baseUrl).ConfigureAwait(false);
                }
                else if (httpMethod == HttpMethod.Post)
                {
                    response = await client.PostAsJsonAsync(baseUrl, data).ConfigureAwait(false);
                }
                else if (httpMethod == HttpMethod.Delete)
                {
                    response = await client.DeleteAsync(baseUrl).ConfigureAwait(false);
                }
                else if (httpMethod == HttpMethod.Put)
                {
                    response = await client.PutAsJsonAsync(baseUrl, data).ConfigureAwait(false);
                }
                else
                {
                    throw new NotSupportedException($"Method {httpMethod} is not supported.");
                }

                return await GetResponseDetail<T>(response).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //throw ex;
                responseDetail.Message = "Oops Something went wrong. Please try again leter.";
                responseDetail.MessageType = Enums.General.DropMessageType.Error;
            }
            return responseDetail;
        }

        private static async Task<ResponseDetail<T>> GetResponseDetail<T>(HttpResponseMessage response)
        {
            var content = response.Content;
            var result = await content.ReadAsStringAsync().ConfigureAwait(false);
            dynamic returnObj = JObject.Parse(result);
            if (returnObj != null)
            {
                return JsonConvert.DeserializeObject<ResponseDetail<T>>(returnObj.ToString());
            }
            return default(ResponseDetail<T>);
        }
    }
}