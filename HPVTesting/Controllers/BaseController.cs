using HPVTesting.Business.Enums.General;
using HPVTesting.Business.Helpers;
using HPVTesting.Business.ViewModels.General;
using HPVTesting.Extensions;
using HPVTesting.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HPVTesting.Controllers
{
    public class BaseController : Controller
    {
        private readonly ILogger<BaseController> _logger;
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected async Task<ResponseDetail<T>> DoActionForGet<T>(object data, string method)
        {
            return await DoActionForRequest<T>(data, method, HttpMethod.Get);
        }

        protected async Task<ResponseDetail<T>> DoActionForPost<T>(object data, string method)
        {
            return await DoActionForRequest<T>(data, method, HttpMethod.Post);
        }

        protected async Task<ResponseDetail<T>> DoActionForDelete<T>(object data, string method)
        {
            return await DoActionForRequest<T>(data, method, HttpMethod.Delete);
        }

        private async Task<ResponseDetail<T>> DoActionForRequest<T>(object data, string method, HttpMethod httpMethod)
        {
            var result = new ResponseDetail<T>();
            try
            {
                var apiToken = SessionHelper.GetUserToken(HttpContext);               
                var culture = (Request.Cookies["Curentlanguage"]==null)? "en": Request.Cookies["Curentlanguage"];
                result = await ApiHelper.SendApiRequest<T>(data, method, httpMethod, apiToken,culture);
                if (!string.IsNullOrWhiteSpace(result.Message) && result.MessageType != DropMessageType.None)
                {
                    this.DropMessage(result.Message, string.Empty, result.MessageType);
                }
                if (result.Data != null)
                {
                    result.Data = JsonConvert.DeserializeObject<T>(result.Data.ToString());
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"An error has occuerd on {nameof(DoActionForDelete)}. {nameof(method)} - {method}, {nameof(data)} - {data}. {ex}");
            }
            return result;
        }

        protected PostCompleteJsonResult PostComplete()
        {
            return new PostCompleteJsonResult();
        }

        protected PostCompleteJsonResult PostComplete(object content)
        {
            return new PostCompleteJsonResult(content);
        }

        protected PostFailJsonResult PostFailed()
        {
            return new PostFailJsonResult();
        }

        protected PostFailJsonResult PostFailed(string message)
        {
            return new PostFailJsonResult(message);
        }

        protected PostCompleteJsonResult PostCompleteRedirect(string url)
        {
            var jsonResult = new PostCompleteJsonResult();
            jsonResult.Data.RedirectUrl = url;
            return jsonResult;
        }

        protected PostCompleteJsonResult PostCompleteRefresh()
        {
            var jsonResult = new PostCompleteJsonResult();
            jsonResult.Data.Refresh = true;
            return jsonResult;
        }

        protected PostCompleteJsonResult PostCompleteView(Controller controller, object model)
        {
            return PostCompleteView(controller, RouteData.GetRequiredString("action"), model);
        }

        protected PostCompleteJsonResult PostCompleteView(Controller controller, string viewName, object model)
        {
            var view = ControllerExtensions.RenderViewToStringAsync(controller, viewName, model);
            var result = new PostCompleteJsonResult(view);
            result.Data.ContentType = "html";
            return result;
        }
    }
}