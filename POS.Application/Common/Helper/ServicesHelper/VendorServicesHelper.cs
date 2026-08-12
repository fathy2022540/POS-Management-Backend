using System.Text.RegularExpressions;

namespace POS.Application.Common.Helper
{
    /// <summary>
    /// Helper class for vendor-related operations
    /// </summary>
    public static class VendorCodeGenerator
    {
        private const string VendorCodePrefix = "VND";
        private const int CodePadLength = 3;

        /// <summary>
        /// Generates the next vendor code based on the last vendor code
        /// Example: VND001 -> VND002, VND999 -> VND1000
        /// </summary>
        /// <param name="lastVendorCode">The last vendor code (e.g., "VND001")</param>
        /// <returns>The next vendor code</returns>
        public static string GenerateNextCode(string lastVendorCode)
        {
            if (string.IsNullOrEmpty(lastVendorCode))
                return $"{VendorCodePrefix}001";

            // Extract numeric part from the code (e.g., "001" from "VEN001")
            var numericPart = Regex.Replace(lastVendorCode, @"[^0-9]", "");

            if (!int.TryParse(numericPart, out var currentNumber))
                return $"{VendorCodePrefix}001";

            // Increment and format with padding
            var nextNumber = currentNumber + 1;
            return $"{VendorCodePrefix}{nextNumber.ToString().PadLeft(CodePadLength, '0')}";
        }

        /// <summary>
        /// Validates if a vendor code follows the correct format
        /// </summary>
        /// <param name="vendorCode">The vendor code to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValidVendorCode(string vendorCode)
        {
            return !string.IsNullOrEmpty(vendorCode) &&
                   vendorCode.StartsWith(VendorCodePrefix) &&
                   Regex.IsMatch(vendorCode, @"^VND\d+$");
        }
    }
}
