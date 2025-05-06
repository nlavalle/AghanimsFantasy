using System.Text.Json;
using csharp_ef_webapi.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace csharp_ef_webapi.Extensions;

/***
*** Use the [AuthenticatedETag] attribute to perform ETag matching and return a 304 manually
*** This is intended to be used for authenticated scenarios to avoid OutputCache providing sensitive data
*** to users that shouldn't have access to it
***/
public class AuthenticatedETagAttribute : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult objectResult &&
            objectResult.Value != null)
        {
            var json = JsonSerializer.Serialize(objectResult.Value);
            var hash = ETagGenerator.GenerateETag(json);

            var request = context.HttpContext.Request.Headers.IfNoneMatch;
            context.HttpContext.Response.Headers.ETag = $"\"{hash}\"";

            if (!string.IsNullOrEmpty(request) && request == $"\"{hash}\"")
            {
                context.Result = new StatusCodeResult(StatusCodes.Status304NotModified);
            }
        }
    }
}
