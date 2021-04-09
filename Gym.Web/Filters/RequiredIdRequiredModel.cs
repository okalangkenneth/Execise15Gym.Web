using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gym.Web.Filters
{
    public class RequiredIdRequiredModel : ActionFilterAttribute
    {
        private readonly string parameterName;

        public RequiredIdRequiredModel(string parameterName)
        {
            this.parameterName = parameterName;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.RouteData.Values[parameterName] == null)
            {
                context.Result = new BadRequestResult();
            }
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ViewResult viewResult)
            {
                if (viewResult.Model is null) context.Result = new NotFoundResult();
            }
        }
    }
}
