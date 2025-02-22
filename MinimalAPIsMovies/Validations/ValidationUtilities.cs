﻿namespace MinimalAPIsMovies.Validations
{
    public static class ValidationUtilities
    {
        public static string NonEmptyMessage = "The field {PropertyName} is requied";
        public static string MaximumLengthMessage = "The field {PropertyName} should be less than {MaxLength} characters";
        public static string FirstLetterIsUpperCaseMessage = "The field {PropertyName} should start with uppercase";
        public static string EmailAddressMessage="The filed {PropertyName} is not a valid email address";

        public static string GreaterThanDate(DateTime value) => "The field {PropertyName} should be greater than " + value.ToString("dd/MM/yyyy");


        public static bool FirstLetterIsUppercase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            var firstLetter = value[0].ToString();
            return firstLetter == firstLetter.ToUpper();
        }
    }
}
