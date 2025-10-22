// See https://aka.ms/new-console-template for more information
using CreditCardMasking.CreditCardMasker;

Console.WriteLine("--- Credit Card Masking Demo ---");

// Example from a "SpecificDescription" or "PayeeName"
string validPANText = "Transaction to AMZN MKTPLACE 4111111111111111 for $19.99";

// An invalid PAN (fails Luhn) that should NOT be masked
string invalidPANText = "Payment to VENDOR 1234567890123456 (Ref: 98765)";

// A long number that is NOT a PAN and should NOT be masked
string longTrackingId = "Order confirmation 888777666555444333222 (20 digits)";

// A short number that should NOT be masked (based on old logic)
string oldLogicMatch = "REF 12345678, should be ignored by new logic.";

// A mix of all
string mixedText = @"
PayeeName: WALMART STORE #1234, 4242424242424242
Description: Purchase of $50. Reference ID 99988877766655. 
GeneralDescription: Payment for account 1234567890. 
PayeeName: TAXI 5555444433332222 (Valid PAN)
Description: Hold for 12345678. (Old logic match, now ignored)
PayeeName: VENDOR_ABC 1111222233334444 (Invalid PAN)
";

Console.WriteLine("\n--- Test 1: Valid PAN ---");
Console.WriteLine($"Original:  {validPANText}");
Console.WriteLine($"Masked:    {CreditCardMasker.MaskValidPANs(validPANText)}");

Console.WriteLine("\n--- Test 2: Invalid PAN (Fails Luhn) ---");
Console.WriteLine($"Original:  {invalidPANText}");
Console.WriteLine($"Masked:    {CreditCardMasker.MaskValidPANs(invalidPANText)}");

Console.WriteLine("\n--- Test 3: Long Number (Wrong Length) ---");
Console.WriteLine($"Original:  {longTrackingId}");
Console.WriteLine($"Masked:    {CreditCardMasker.MaskValidPANs(longTrackingId)}");

Console.WriteLine("\n--- Test 4: Short Number (Old Logic) ---");
Console.WriteLine($"Original:  {oldLogicMatch}");
Console.WriteLine($"Masked:    {CreditCardMasker.MaskValidPANs(oldLogicMatch)}");

Console.WriteLine("\n--- Test 5: Mixed Content Block ---");
Console.WriteLine("Original:");
Console.WriteLine(mixedText);
Console.WriteLine("Masked:");
Console.WriteLine(CreditCardMasker.MaskValidPANs(mixedText));

Console.WriteLine("\nPress any key to exit.");
Console.ReadKey();