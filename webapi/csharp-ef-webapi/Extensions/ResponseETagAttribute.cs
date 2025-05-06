using System.Text.Json;
using csharp_ef_webapi.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace csharp_ef_webapi.Extensions;

/***
*** Use the [ResponseETag] attribute to generate an ETag header for a static response (doesn't change whether authenticated or not), 
*** so that the vuejs can pass the ETag back in an If-None-Match for OutputCache
***/
public class ResponseETagAttribute : ActionFilterAttribute
{
    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult objectResult &&
            objectResult.Value != null)
        {
            var json = JsonSerializer.Serialize(objectResult.Value);
            var hash = ETagGenerator.GenerateETag(json);
            context.HttpContext.Response.Headers.ETag = $"\"{hash}\"";
        }
    }
}
