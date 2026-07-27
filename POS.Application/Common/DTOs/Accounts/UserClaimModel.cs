namespace JRM.Application.Common.DTOs
{
    public class UserClaimModel
    {
        #region Properties

        public long? Id { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? entityTypeNameEn { get; set; }

        public string? CategoryCode { get; set; }

        public string? entityNameEn { get; set; }

        public string? phone_no { get; set; }

        public static UserClaimModel CreateUserClaimModel(UserModel userModel)
        {
            return new UserClaimModel
            {
                Id = userModel.Id,
                UserName = userModel.UserName,
                Email = userModel.Email

            };
        }

        #endregion
    }
}