using FirebaseAuthDemo.Utils.Custom_Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirebaseAuthDemo.Filters
{
    public class HttpResponseExceptionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception == null)
            {
                return;
            }
            else if (context.Exception is ArgumentException ex)
            {
                context.Result = new BadRequestObjectResult(ex.Message);
                context.ExceptionHandled = true;
                return;
            }
            else if (context.Exception is NoResultsFoundException NRF)
            {
                context.Result = new NotFoundObjectResult(NRF.Message);
                context.ExceptionHandled = true;
                return;
            }
            else if (context.Exception is UnauthorizedAccessException UA)
            {
                context.Result = new UnauthorizedResult();
                context.ExceptionHandled = true;
                return;
            }
            else if (context.Exception is ResourceAlreadyExistsException RAE)
            {
                context.Result = new ConflictObjectResult(RAE.Message);
                context.ExceptionHandled = true;
                return;
            }
            else
            {
                context.Result = new ObjectResult("Error")
                {
                    StatusCode = 500,
                    Value = "There was a problem while processing the request."
                };
                context.ExceptionHandled = true;
                return;
            }
        }
    }
}
