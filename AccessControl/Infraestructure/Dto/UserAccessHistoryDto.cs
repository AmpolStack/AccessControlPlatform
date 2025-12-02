namespace AccessControl.Infraestructure.Dto
{
    public class UserAccessHistoryDto
    {
        public string FullName { get; set; } = string.Empty;
        public string IdentityDocument { get; set; } = string.Empty;
        public DateTime EntryDateTime { get; set; }
        public DateTime? ExitDateTime { get; set; }
        public int DurationMinutes { get; set; }
    }
}
