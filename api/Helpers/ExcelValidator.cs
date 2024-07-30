using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace api.Helpers
{
    public class ExcelValidator
    {
         private static bool ValidateFileHeaders(string[] fileHeaders, string[] requiredHeaders)
        {
            return fileHeaders.All(header => requiredHeaders.Contains(header, StringComparer.OrdinalIgnoreCase));
        }
        private static async Task<bool> ValidateFile(IFormFile file, string[] requiredHeaders)
        {
            string extension = Path.GetExtension(file.FileName).ToLower();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                if (extension == ".csv")
                {
                    using (var reader = new StreamReader(stream))
                    {
                        stream.Position = 0;
                        var headerLine = await reader.ReadLineAsync();
                        if (string.IsNullOrEmpty(headerLine))
                        {
                            return false;
                        }

                        var fileHeaders = headerLine.Split(',');
                        return ValidateFileHeaders(fileHeaders, requiredHeaders);
                    }
                }
                else if (extension == ".xlsx")
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.First();
                        var fileHeaders = new List<string>();

                        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                        {
                            System.Console.WriteLine("Column: " + worksheet.Cells[1, col].Value?.ToString());
                            fileHeaders.Add(worksheet.Cells[1, col].Value?.ToString().Trim());
                        }

                        return ValidateFileHeaders(fileHeaders.ToArray(), requiredHeaders);
                    }
                }
                return false;
            }
        }
       
        public static async Task<bool> ValidateUsersFile(IFormFile file)
        {
            var requiredHeaders = new[] { "UserName", "Email", "First Name", "Last Name", "Roles" };
            return await ValidateFile(file, requiredHeaders);
           
        }
        public static async Task<bool> ValidateScoresFile(IFormFile file)
        {
            var requiredHeaders = new[] { "UserName", "Value" };
            return await ValidateFile(file, requiredHeaders);
        }

        public static async Task<bool> ValidateCourseUsersFile(IFormFile file) {
            var requiredHeaders = new[] { "UserName" };
            return await ValidateFile(file, requiredHeaders);
        }

    }
}