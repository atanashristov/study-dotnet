using System.Text.RegularExpressions;

namespace CreditCardMasking.CreditCardMasker
{
    /// <summary>
    /// Provides functionality to find and mask valid credit card PANs within a text block.
    /// </summary>
    public static class CreditCardMasker
    {
        // Regex to find potential credit card numbers (13 to 19 digits).
        // \b ensures we match whole numbers, not parts of longer digit strings.
        private static readonly Regex _panRegex = new Regex(@"\b\d{13,19}\b", RegexOptions.Compiled);

        /// <summary>
        /// Masks valid credit card numbers found within a given input string.
        /// </summary>
        /// <param name="inputText">The text to scan for credit card numbers.</param>
        /// <returns>A new string with valid PANs masked.</returns>
        public static string MaskValidPANs(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
            {
                return inputText;
            }

            // We use Regex.Replace with a MatchEvaluator delegate.
            // This delegate (the 'Masker' method) is called for each match found.
            return _panRegex.Replace(inputText, Masker);
        }

        /// <summary>
        /// This method is called for each match found by the Regex.
        /// It validates the match using the Luhn algorithm and returns the masked string if valid.
        /// </summary>
        private static string Masker(Match match)
        {
            string potentialPAN = match.Value;

            // Step 1: Validate the potential PAN using the Luhn algorithm.
            if (IsValidLuhn(potentialPAN))
            {
                // Step 2: If valid, apply the masking.
                // Keep the last 4 digits and replace the rest with asterisks.
                int length = potentialPAN.Length;
                int keepLast = 4;
                int maskLength = length - keepLast;

                // Create a masked string: e.g., "************1234"
                return new string('*', maskLength) + potentialPAN.Substring(maskLength);
            }
            else
            {
                // Step 3: If not a valid Luhn number, return it unchanged.
                // This prevents masking other long numbers (e.g., tracking IDs)
                return potentialPAN;
            }
        }

        /// <summary>
        /// Validates a number string using the Luhn (Mod-10) algorithm.
        /// </summary>
        /// <param name="number">The number string to validate.</param>
        /// <returns>True if the number is valid according to Luhn, false otherwise.</returns>
        public static bool IsValidLuhn(string number)
        {
            if (string.IsNullOrEmpty(number))
            {
                return false;
            }

            int sum = 0;
            bool alternate = false;

            // Iterate from right to left (end of the string to the start)
            for (int i = number.Length - 1; i >= 0; i--)
            {
                // Ensure the character is a digit
                if (!char.IsDigit(number[i]))
                {
                    // This shouldn't happen with our Regex, but it's good practice.
                    return false;
                }

                int digit = (int)char.GetNumericValue(number[i]);

                if (alternate)
                {
                    // Double the digit
                    int doubled = digit * 2;

                    // If doubled > 9, subtract 9 (which is the same as summing its digits: e.g., 14 -> 1+4=5)
                    sum += (doubled > 9) ? (doubled - 9) : doubled;
                }
                else
                {
                    // Add the digit as is
                    sum += digit;
                }

                // Flip the 'alternate' flag for the next iteration
                alternate = !alternate;
            }

            // The number is valid if the total sum is a multiple of 10.
            return (sum % 10 == 0);
        }
    }
}