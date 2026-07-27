namespace JRM.Domain.Entities
{
    public class UserScreenPermissionOverrides : BaseEntity
    {
        public long UserId { get; set; }
        public long ScreenId { get; set; }
        public bool? CanView { get; set; }
        public bool? CanCreate { get; set; }
        public bool? CanEdit { get; set; }
        public bool? CanDelete { get; set; }
        public bool? CanApprove { get; set; }
        public bool? CanExport { get; set; }

        public virtual Users? User { get; set; }
        public virtual SystemScreens? Screen { get; set; }
    }
}