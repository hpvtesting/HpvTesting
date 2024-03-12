using HPVTesting.Business.Enums.General;
using HPVTesting.Business.ViewModels.General;

namespace HPVTesting.Helpers
{
    public class PostCompleteJsonResult : PostJsonResult<PostCompleteJsonResultData>
    {
        public PostCompleteJsonResult()
        {
            Data.ContentType = "json";
            Data.Refresh = false;
        }

        public PostCompleteJsonResult(object content) : this()
        {
            Data.Content = content;
        }

        public PostCompleteJsonResult WithDropMessage(string message, string description, DropMessageType dropMessageType)
        {
            Data.DropMessage = new DropMessageModel { Message = message, Description = description, DropMessageType = dropMessageType };
            return this;
        }

        public PostCompleteJsonResult WithDropMessage(string message, string description = null)
        {
            return WithDropMessage(message, description, DropMessageType.Success);
        }

        public PostCompleteJsonResult WithRefresh()
        {
            Data.Refresh = true;
            return this;
        }
    }

    public class PostCompleteJsonResultData : PostJsonResultData
    {
        public string RedirectUrl { get; set; }

        public bool Refresh { get; set; }

        public object Content { get; set; }

        public string ContentType { get; set; } // json, xml, html, text (not used anywhere right now)

        public bool success => true;
    }
}