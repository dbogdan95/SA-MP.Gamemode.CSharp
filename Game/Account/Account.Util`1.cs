using System.Text.RegularExpressions;

namespace Game.Account
{
    partial class Account
    {
        public static bool ValidatePassword(string password, out string ErrorMessage)
        {
            var input = password;
            ErrorMessage = string.Empty;

            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMiniMaxChars = new Regex(@".{8,15}");
            var hasLowerChar = new Regex(@"[a-z]+");
            var hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
            bool succes = true;

            if (string.IsNullOrWhiteSpace(input))
            {
                ErrorMessage += "Password should not be empty.\n";
                succes = false;
            }

            if (!hasLowerChar.IsMatch(input))
            {
                ErrorMessage += "Password should contain At least one lower case letter.\n";
                succes = false;
            }
            if (!hasUpperChar.IsMatch(input))
            {
                ErrorMessage += "Password should contain At least one upper case letter.\n";
                succes = false;
            }
            if (!hasMiniMaxChars.IsMatch(input))
            {
                ErrorMessage += "Password should not be less than 8 or greater than 12 characters.\n";
                succes = false;
            }
            if (!hasNumber.IsMatch(input))
            {
                ErrorMessage += "Password should contain at least one numeric value.\n";
                succes = false;
            }
            return succes;
        }

        public static bool IsValidEmailAddress(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;
            else
            {
                var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
                return regex.IsMatch(s) && !s.EndsWith(".");
            }
        }
    }
}
