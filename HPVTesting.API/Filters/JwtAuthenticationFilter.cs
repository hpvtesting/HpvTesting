using HPVTesting.Business.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace AAT.API.Filters
{
    [AttributeUsage(AttributeTargets.Class)]
    public class JwtAuthenticationFilter : Attribute, IAuthorizationFilter, IActionFilter
    {
        public static PersonModel ApplicationUserApiRequest { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            try
            {
                var IsAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;
                ApplicationUserApiRequest = new PersonModel();
                if (IsAuthenticated)
                {
                    var claimsIndentity = context.HttpContext.User.Identity as ClaimsIdentity;
                    ApplicationUserApiRequest = new PersonModel
                    {
                        Id = Guid.Parse(context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value),
                        Name = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "Name")?.Value,
                    };
                }
            }
            catch (Exception)
            {
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                if (ApplicationUserApiRequest != null)
                {
                    dynamic valuesCntlr = context.Controller as dynamic;
                    valuesCntlr.ApplicationUserApiRequest = ApplicationUserApiRequest;
                }
            }
            catch (Exception)
            {
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }
    }
}
