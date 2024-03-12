using AAT.API.Filters;
using HPVTesting.Business.Enums.General;
using HPVTesting.Business.ViewModels.General;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.Threading.Tasks;

namespace HPVTesting.API.Controllers
{
    [JwtAuthenticationFilter]
    public class BaseApiController : Controller
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public BaseApiController()
        {
        }

        protected async Task<ResponseDetail<T>> GetDataWithMessage<T>(Func<Task<Tuple<T, string, DropMessageType>>> getDataFunc)
        {
            var output = new ResponseDetail<T>();
            try
            {                
                var result = await getDataFunc();
                output.Data = result.Item1;
                output.Message = result.Item2;
                output.MessageType = result.Item3;               
                return await Task.FromResult(output);
            }
            catch (Exception ex)
            {
                output.MessageType = DropMessageType.Error;
                output.Error = new Error
                {
                    Code = ErrorCode.SERVICE_EXECUTION_FAILED,
                    Message = ex.Message
                };
                output.Message = "Something went wrong!! Please Try again later";
                Logger.Error($"An error has occuerd on {Convert.ToString(ControllerContext.RouteData.Values["controller"])+ " controller &" + Convert.ToString(ControllerContext.RouteData.Values["action"])+" Method"}Message:{ex.Message}");
                return await Task.FromResult(output);
            }
        }

        protected new Tuple<T, string, DropMessageType> Response<T>(T data, string message, DropMessageType messageType = DropMessageType.Success)
        {
            return new Tuple<T, string, DropMessageType>(data, message, messageType);
        }
    }
}