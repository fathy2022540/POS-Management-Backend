namespace POS.Domain.Enums
{
    public enum UserType
    {
        Administrator = 1,
        Manager = 2,
        Cashier = 3
    }
    public enum UserStatus
    {
        Pending = 1,
        Active = 2,
        Inactive = 3,
        Locked = 4,
        Deleted = 5
    }
}
