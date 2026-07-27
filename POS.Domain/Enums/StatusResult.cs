namespace JRM.Domain.Enums
{

    public enum StatusResult
    {
        // --- 2xx Success Responses ---
        Success = 200,
        UserCreated = 201,
        Authenticated = 202, // Kept 202, but standard HTTP 202 means "Accepted/Processing"
        ForceReset = 203,
        LoginNameUsed = 204,
        OtpExpired = 205,
        ForgetPasswordNotAvailableToOwners = 206,
        RegistrationFilesCorrupted = 207,
        EmailUsed = 208,
        InvalidOtp = 209,

        // --- 4xx Client Errors ---
        BadRequest = 400,
        Unauthorized = 401,
        PaymentRequired = 402, // Avoid using this for general "Failed"
        Forbidden = 403,       // Standard HTTP: Refused access
        NotFound = 404,        // Standard HTTP: Not Exists / No Data Found
        MethodNotAllowed = 405,
        RegistrationFailed = 406,
        EditProfileFailed = 407,

        // --- 5xx Server Errors ---
        InternalServerError = 500,
        ModelNotValid = 502,

        // --- Active Directory / LDAP Specific Codes ---
        PleaseContactQfiuAdministrator = 1,
        InvalidCredential = 49,
        AccountLocked = 53,

        // --- Custom Application Business Codes (1000+) ---
        RequestUnderProcess = 1001,
        NotConsented = 3001,
        NotFoundInActiveDirectory = 3002,
        NotFoundUserAndPasswordAuthentication = 3003,
        InvalidRequest = 3004,
        AccountExpired = 3005,
        InvalidPassword = 3006, // 👈 Added here

        // --- Custom Security/Validation Codes (5000+) ---
        UnauthorizedClient = 5001,
        InvalidClient = 5002,
        LockedOut = 5003,
        NotAllowedDelete = 5004,
        Infected = 5005,
        DataAlreadyExist = 5006
    }

    public static class EnumExtension
    {
        public static int Code(this StatusResult statusResult) => ((int)statusResult);
    }
}


