namespace AccessControl.Infraestructure.Dto
{
    /// <summary>
    /// DTO for User Login data returned by sp_UserLogin
    /// Success and Message are now handled via OUTPUT parameters
    /// </summary>
    public class UserLoginDataDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public int EstablishmentId { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string IdentityDocument { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool MustChangePassword { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        
        // Establishment data
        public string EstablishmentName { get; set; } = string.Empty;
        public int? MaxCapacity { get; set; }
    }
}
