using JRM.Domain.Enums;

namespace JRM.Application.Common.DTOs
{

    public class ApiResponses<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string Message { get; }
        public StatusResult Status { get; }

        private ApiResponses(bool isSuccess, T? value, StatusResult status, string message)
        {
            IsSuccess = isSuccess;
            Value = value;
            Status = status;
            Message = message;
        }

        // Success Factory Methods
        public static ApiResponses<T> Success(T value, string message = "Success")
            => new(true, value, StatusResult.Success, message);

        // Failure Factory Methods with explicit statuses
        public static ApiResponses<T> Failure(StatusResult status, string message)
            => new(false, default, status, message);
    }
}
