using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace api.Validations
{
    public class RegexValidationAttribute : ValidationAttribute
{
    private readonly string _pattern;

    public RegexValidationAttribute(string pattern)
    {
        _pattern = pattern;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
        {
            return ValidationResult.Success;
        }

        var input = value.ToString();
        var regex = new Regex(_pattern);

        if (!regex.IsMatch(input.ToString()))
        {
            return new ValidationResult($"The {validationContext.DisplayName} field is not valid.");
        }

        return ValidationResult.Success;
    }
}
}