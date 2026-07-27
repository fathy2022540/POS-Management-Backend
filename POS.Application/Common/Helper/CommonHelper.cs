namespace JRM.Application.Common.Helper
{
    public static class CommonHelper
    {
        private static readonly Random _random = new Random();

        public static string GenerateVendorCode()
        {
            int code = _random.Next(0, 10000);

            return $"VND-{code:D4}";
        }
        public static (string FirstName, string LastName) SplitFullName(this string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return (string.Empty, string.Empty);
            }

            // Clean up accidental double spaces or trailing spaces from the frontend input
            string trimmedName = fullName.Trim();

            // Find the index of the very first space character
            int firstSpaceIndex = trimmedName.IndexOf(' ');

            // Scenario A: User only typed one name with no spaces
            if (firstSpaceIndex == -1)
            {
                return (trimmedName, string.Empty);
            }

            // Scenario B: Split securely using modern C# range indexing
            string firstName = trimmedName[..firstSpaceIndex];
            string lastName = trimmedName[(firstSpaceIndex + 1)..].Trim();

            return (firstName, lastName);
        }
    }
}
