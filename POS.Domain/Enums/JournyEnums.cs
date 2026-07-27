namespace JRM.Domain.Enums
{
    public enum JourneyType
    {
        Internal = 1,
        External = 2
    }

    public enum ReservationStatus
    {
        Pending = 1,
        Confirmed = 2,
        Cancelled = 3,
        Completed = 4
    }

    public enum RequestStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }
    public enum RoomType
    {
        Single = 1,     // 1 Person
        Double = 2,     // 2 Persons
        Triple = 3,     // 3 Persons
        Family = 4      // 4+ Persons (Optional)
    }
}
