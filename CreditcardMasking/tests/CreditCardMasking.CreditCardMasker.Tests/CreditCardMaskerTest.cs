namespace CreditCardMasking.CreditCardMasker.Tests
{
    public class CreditCardMaskerTest
    {
        [SetUp]
        public void Setup()
        {
        }

        #region MaskValidPANs Tests

        [Test]
        public void MaskValidPANs_WithValidPAN_ReturnsMaskedText()
        {
            string validPANText = "Transaction to AMZN MKTPLACE 4111111111111111 for $19.99";
            string maskedText = CreditCardMasker.MaskValidPANs(validPANText);
            Assert.That(maskedText,
                Is.EqualTo("Transaction to AMZN MKTPLACE ************1111 for $19.99"),
                "A valid PAN (passes Luhn) should be masked");
        }

        [Test]
        public void MaskValidPANs_WithInvalidPAN_ReturnsOriginalText()
        {
            string invalidPANText = "Payment to VENDOR 1234567890123456 (Ref: 98765)";
            string maskedText = CreditCardMasker.MaskValidPANs(invalidPANText);
            Assert.That(maskedText,
                Is.EqualTo(invalidPANText),
                "An invalid PAN (fails Luhn) should NOT be masked");
        }

        [Test]
        public void MaskValidPANs_WithLongNumber_ReturnsOriginalText()
        {
            string longTrackingId = "Order confirmation 888777666555444333222 (20 digits)";
            string maskedText = CreditCardMasker.MaskValidPANs(longTrackingId);
            Assert.That(maskedText,
                Is.EqualTo(longTrackingId),
                "A long number that is NOT a PAN should NOT be masked");
        }

        [Test]
        public void MaskValidPANs_WithShortNumber_ReturnsOriginalText()
        {
            string shortTrackingId = "Order confirmation 12345678 (8 digits)";
            string maskedText = CreditCardMasker.MaskValidPANs(shortTrackingId);
            Assert.That(maskedText,
                Is.EqualTo(shortTrackingId),
                "A short number that is NOT a PAN should NOT be masked");
        }

        [Test]
        public void MaskValidPANs_WithComplexText_ReturnsProperlyMaskedText()
        {
            string originalMixedText = @"
PayeeName: WALMART STORE #1234, 4242424242424242 (valid credit card PAN)
Description: Purchase of $50. Reference ID 99988877766655.
GeneralDescription: Payment for account 1234567890.
PayeeName: TAXI 5555444433332222 (16 digit PAN, fails the Luhn check)
Description: Hold for 12345678. (Old logic match, now ignored)
PayeeName: VENDOR_ABC 1111222233334444 (16 digit PAN, passes the Luhn check)
";

            string expectedMaskedText = @"
PayeeName: WALMART STORE #1234, ************4242 (valid credit card PAN)
Description: Purchase of $50. Reference ID 99988877766655.
GeneralDescription: Payment for account 1234567890.
PayeeName: TAXI 5555444433332222 (16 digit PAN, fails the Luhn check)
Description: Hold for 12345678. (Old logic match, now ignored)
PayeeName: VENDOR_ABC ************4444 (16 digit PAN, passes the Luhn check)
";
            string maskedText = CreditCardMasker.MaskValidPANs(originalMixedText);
            Assert.That(maskedText,
                Is.EqualTo(expectedMaskedText),
                "Should properly mask valid PANs in complex text");
        }

        [Test]
        public void MaskValidPANs_WithNullInput_ReturnsNull()
        {
            string? result = CreditCardMasker.MaskValidPANs(null!);
            Assert.That(result, Is.Null, "Null input should return null");
        }

        [Test]
        public void MaskValidPANs_WithEmptyInput_ReturnsEmpty()
        {
            string result = CreditCardMasker.MaskValidPANs("");
            Assert.That(result, Is.EqualTo(""), "Empty input should return empty string");
        }

        [Test]
        public void MaskValidPANs_WithWhitespaceInput_ReturnsWhitespace()
        {
            string input = "   \t\n  ";
            string result = CreditCardMasker.MaskValidPANs(input);
            Assert.That(result, Is.EqualTo(input), "Whitespace-only input should be unchanged");
        }

        [Test]
        public void MaskValidPANs_WithMultiplePANs_MasksAllValid()
        {
            // Using multiple valid test PANs
            string input = "Card 1: 4111111111111111 and Card 2: 5555555555554444 are both valid";
            string result = CreditCardMasker.MaskValidPANs(input);
            Assert.That(result, Is.EqualTo("Card 1: ************1111 and Card 2: ************4444 are both valid"));
        }

        [Test]
        public void MaskValidPANs_WithMixedValidInvalidPANs_MasksOnlyValid()
        {
            string input = "Valid: 4111111111111111 Invalid: 1234567890123456 Valid: 5555555555554444";
            string result = CreditCardMasker.MaskValidPANs(input);
            Assert.That(result, Is.EqualTo("Valid: ************1111 Invalid: 1234567890123456 Valid: ************4444"));
        }

        [Test]
        public void MaskValidPANs_WithNoPotentialPANs_ReturnsOriginal()
        {
            string input = "This text has no long numbers at all!";
            string result = CreditCardMasker.MaskValidPANs(input);
            Assert.That(result, Is.EqualTo(input));
        }

        [Test]
        public void MaskValidPANs_WithBoundaryLengthNumbers_HandlesCorrectly()
        {
            // Test 13-digit valid PAN (minimum length)
            string input13 = "Card: 4000000000006"; // Valid 13-digit test PAN
            string result13 = CreditCardMasker.MaskValidPANs(input13);
            Assert.That(result13, Is.EqualTo("Card: *********0006"));

            // Test that numbers outside range (12 and 20 digits) are not processed
            string input12 = "Card: 400000000006"; // 12 digits - too short
            string result12 = CreditCardMasker.MaskValidPANs(input12);
            Assert.That(result12, Is.EqualTo("Card: 400000000006"), "12-digit numbers should not be processed");

            string input20 = "Card: 12345678901234567890"; // 20 digits - too long
            string result20 = CreditCardMasker.MaskValidPANs(input20);
            Assert.That(result20, Is.EqualTo("Card: 12345678901234567890"), "20-digit numbers should not be processed");
        }

        [Test]
        public void MaskValidPANs_WithNumbersContainingWordBoundaries_HandlesCorrectly()
        {
            // Numbers at start and end of string
            string input = "4111111111111111 some text 5555555555554444";
            string result = CreditCardMasker.MaskValidPANs(input);
            Assert.That(result, Is.EqualTo("************1111 some text ************4444"));
        }

        #endregion

        #region IsValidLuhn Tests

        [Test]
        public void IsValidLuhn_WithValidLuhnNumber_ReturnsTrue()
        {
            // Known valid Luhn test numbers
            Assert.That(CreditCardMasker.IsValidLuhn("4111111111111111"), Is.True, "Valid Visa test number");
            Assert.That(CreditCardMasker.IsValidLuhn("5555555555554444"), Is.True, "Valid Mastercard test number");
            Assert.That(CreditCardMasker.IsValidLuhn("378282246310005"), Is.True, "Valid Amex test number");
            Assert.That(CreditCardMasker.IsValidLuhn("6011111111111117"), Is.True, "Valid Discover test number");
        }

        [Test]
        public void IsValidLuhn_WithInvalidLuhnNumber_ReturnsFalse()
        {
            Assert.That(CreditCardMasker.IsValidLuhn("4111111111111112"), Is.False, "Invalid Luhn checksum");
            Assert.That(CreditCardMasker.IsValidLuhn("1234567890123456"), Is.False, "Sequential digits");
            Assert.That(CreditCardMasker.IsValidLuhn("1111111111111111"), Is.False, "All same digits");
        }

        [Test]
        public void IsValidLuhn_WithNullInput_ReturnsFalse()
        {
            Assert.That(CreditCardMasker.IsValidLuhn(null!), Is.False, "Null should be invalid");
        }

        [Test]
        public void IsValidLuhn_WithEmptyInput_ReturnsFalse()
        {
            Assert.That(CreditCardMasker.IsValidLuhn(""), Is.False, "Empty string should be invalid");
        }

        [Test]
        public void IsValidLuhn_WithNonDigitCharacters_ReturnsFalse()
        {
            Assert.That(CreditCardMasker.IsValidLuhn("411111111111111a"), Is.False, "Should reject letters");
            Assert.That(CreditCardMasker.IsValidLuhn("4111-1111-1111-1111"), Is.False, "Should reject dashes");
            Assert.That(CreditCardMasker.IsValidLuhn("4111 1111 1111 1111"), Is.False, "Should reject spaces");
            Assert.That(CreditCardMasker.IsValidLuhn("4111.1111.1111.1111"), Is.False, "Should reject dots");
        }

        [Test]
        public void IsValidLuhn_WithSingleDigit_HandlesCorrectly()
        {
            Assert.That(CreditCardMasker.IsValidLuhn("0"), Is.True, "0 is valid Luhn");
            Assert.That(CreditCardMasker.IsValidLuhn("1"), Is.False, "1 is invalid Luhn");
            Assert.That(CreditCardMasker.IsValidLuhn("5"), Is.False, "5 is invalid Luhn");
        }

        [Test]
        public void IsValidLuhn_WithTwoDigits_HandlesCorrectly()
        {
            Assert.That(CreditCardMasker.IsValidLuhn("18"), Is.True, "18 is valid Luhn (1*2=2, 2+8=10, 10%10=0)");
            Assert.That(CreditCardMasker.IsValidLuhn("26"), Is.True, "26 is valid Luhn (2*2=4, 4+6=10, 10%10=0)");
            Assert.That(CreditCardMasker.IsValidLuhn("10"), Is.False, "10 is invalid Luhn");
            Assert.That(CreditCardMasker.IsValidLuhn("11"), Is.False, "11 is invalid Luhn");
        }

        [Test]
        public void IsValidLuhn_WithDifferentLengths_HandlesCorrectly()
        {
            // Test various lengths to ensure algorithm works regardless of length
            Assert.That(CreditCardMasker.IsValidLuhn("4000000000006"), Is.True, "Valid 13-digit number");
            Assert.That(CreditCardMasker.IsValidLuhn("4000000000000002"), Is.True, "Valid 16-digit number");
            // Test that algorithm works with different lengths by testing both valid and invalid of different lengths
            Assert.That(CreditCardMasker.IsValidLuhn("123456789012345"), Is.False, "Invalid 15-digit number");
            Assert.That(CreditCardMasker.IsValidLuhn("12345678901234567890"), Is.False, "Invalid 20-digit number");
        }

        [Test]
        public void IsValidLuhn_WithLuhnAlgorithmEdgeCases_HandlesCorrectly()
        {
            // Test cases where doubling produces numbers > 9
            Assert.That(CreditCardMasker.IsValidLuhn("59"), Is.True, "Edge case: 5*2=10, 10-9=1, 1+9=10, 10%10=0");
            Assert.That(CreditCardMasker.IsValidLuhn("67"), Is.True, "Edge case: 6*2=12, 12-9=3, 3+7=10, 10%10=0");
        }

        #endregion

        #region Integration and Edge Case Tests

        [Test]
        public void MaskValidPANs_WithVeryLongText_PerformsWell()
        {
            // Create a long text with embedded PANs to test performance
            var longText = new System.Text.StringBuilder();
            for (int i = 0; i < 1000; i++)
            {
                longText.Append($"Transaction {i}: Some description text here. ");
                if (i % 100 == 0)
                {
                    longText.Append("4111111111111111 "); // Add valid PAN occasionally
                }
            }

            string input = longText.ToString();
            string result = CreditCardMasker.MaskValidPANs(input);

            // Should contain masked PANs
            Assert.That(result, Does.Contain("************1111"));
            // Should not contain unmasked valid PANs
            Assert.That(result, Does.Not.Contain("4111111111111111"));
        }

        [Test]
        public void MaskValidPANs_WithSpecialCharactersAroundNumbers_HandlesCorrectly()
        {
            string input = "(4111111111111111) and [5555555555554444] and {378282246310005}";
            string result = CreditCardMasker.MaskValidPANs(input);
            Assert.That(result, Is.EqualTo("(************1111) and [************4444] and {***********0005}"));
        }

        [Test]
        public void MaskValidPANs_WithNumbersInDifferentContexts_HandlesCorrectly()
        {
            string input = @"
            Phone: 555-123-4567
            Account: 1234567890
            Credit Card: 4111111111111111
            Tracking: 1Z999AA1234567890
            Valid 16-digit: 5555555555554444
            Invalid 16-digit: 1234567890123456
            ";

            string result = CreditCardMasker.MaskValidPANs(input);

            // Should mask valid PANs
            Assert.That(result, Does.Contain("************1111"));
            Assert.That(result, Does.Contain("************4444"));

            // Should not mask other numbers
            Assert.That(result, Does.Contain("555-123-4567")); // Phone (has non-digits)
            Assert.That(result, Does.Contain("1234567890")); // Account (10 digits, too short)
            Assert.That(result, Does.Contain("1Z999AA1234567890")); // Tracking (has letters)
            Assert.That(result, Does.Contain("1234567890123456")); // Invalid Luhn
        }

        #endregion
    }
}
