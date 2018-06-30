using SNROI.Models;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Controls;

namespace SNROI.Views.Utilities
{
    public class IllegalPathCharsValidationRule : ValidationRule
    {
        public static string GetErrorMessage(string fieldName, object fieldValue)
        {
            if ((fieldValue == null) || (string.IsNullOrWhiteSpace(fieldValue.ToString())))
                return string.Format("You cannot leave the {0} field empty.", fieldName);

            var invalidChars = Path.GetInvalidFileNameChars();

            if (fieldValue.ToString().IndexOfAny(invalidChars) >= 0)
                return string.Format("The {0} cannot contain any of the following characters:" + Environment.NewLine + "\\/:*?\"<>|", fieldName);

            return string.Empty;
        }

        public string FieldName { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var error = GetErrorMessage(FieldName, value);
            if (!string.IsNullOrEmpty(error))
                return new ValidationResult(false, error);

            return ValidationResult.ValidResult;
        }
    }

    public class FileNameExistsValidationRule : ValidationRule
    {
        public static string GetErrorMessage(object fieldValue)
        {
            // Don't care if it's null, only checking for existing file name
            if ((fieldValue == null) || (string.IsNullOrWhiteSpace(fieldValue.ToString())))
                return string.Empty;

            var docFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Constants.DataFolderName, fieldValue.ToString() + ".xml");

            if (File.Exists(docFilePath))
                return string.Format("The name {0} already exists." + Environment.NewLine + "Please choose another name.", fieldValue.ToString());

            return string.Empty;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var error = GetErrorMessage(value);
            if (!string.IsNullOrEmpty(error))
                return new ValidationResult(false, error);

            return ValidationResult.ValidResult;
        }
    }
}