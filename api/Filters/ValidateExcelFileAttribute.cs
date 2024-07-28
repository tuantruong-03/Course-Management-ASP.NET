using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace api.Filters
{
    public class ValidateExcelFileAttribute : ActionFilterAttribute
    {
        private readonly string[] _allowedExtensions = [".csv", ".xlsx"];
        private readonly string _parameterName;

        public ValidateExcelFileAttribute(string parameterName)
        {
            _parameterName = parameterName;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.ContainsKey(_parameterName) && context.ActionArguments[_parameterName] is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!_allowedExtensions.Contains(extension))
                {
                    context.Result = new BadRequestObjectResult($"Invalid file format. Only {string.Join(", ", _allowedExtensions)} files are allowed.");
                    return;
                }
            }
            else
            {
                context.Result = new BadRequestObjectResult("File is required.");
            }

            base.OnActionExecuting(context);
        }
    }
}