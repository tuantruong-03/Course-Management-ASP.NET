using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace api.Filters
{   // (THIS FILTER IS STILL NOT USED)
    // Validate "int" in controllers
    public class ValidateIntegerActionFilter : IActionFilter
    {


        public void OnActionExecuting(ActionExecutingContext context)
        {
            // kvp is key-value-pair
            foreach (var kvp in context.ActionArguments)
            {
                if (kvp.Value is int)
                {
                    // Validate if the value is a valid integer
                    // out _ : assign to nothing
                    if (!int.TryParse(kvp.Value.ToString(), out _))
                    {
                        System.Console.WriteLine("ValidateIntegerActionFilter  ");
                        context.Result = new BadRequestObjectResult($"Parameter '{kvp.Key}' must be a valid integer.");
                        throw new AppException($"Parameter '{kvp.Key}' must be a valid integer.", (int)HttpStatusCode.BadRequest);
                    }
                }
            }

        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}