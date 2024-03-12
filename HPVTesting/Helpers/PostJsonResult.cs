using HPVTesting.Business.ViewModels.General;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace HPVTesting.Helpers
{
    public class PostJsonResult : ActionResult
    {
        public override void ExecuteResult(ActionContext context)
        {
        }
    }

    public abstract class PostJsonResult<T> : PostJsonResult where T : class, new()
    {
        private T data;

        public T Data => data ?? (data = new T());

        protected int? StatusCode { get; set; }

        public override void ExecuteResult(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            if (StatusCode.HasValue)
            {
                response.StatusCode = StatusCode.Value;
            }

            if (Data == null)
            {
                return;
            }
            var settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            response.WriteAsync(JsonConvert.SerializeObject(Data, settings));
        }
    }

    public abstract class PostJsonResultData
    {
        public string Result => "PostJsonResult";

        public DropMessageModel DropMessage { get; set; }
    }
}