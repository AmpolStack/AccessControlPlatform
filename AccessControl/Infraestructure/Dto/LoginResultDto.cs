using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessControl.Infraestructure.Dto
{
    public class LoginResultDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }

        public int Id { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }

        public int EstablishmentId { get; set; }
        public string? Role { get; set; }
        public bool IsActive { get; set; }
        public string? IdentityDocument { get; set; }
        public string? PhoneNumber { get; set; }
        public bool MustChangePassword { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        // Sp returns this
        public string? EstablishmentName { get; set; }
        public int? MaxCapacity { get; set; }
    }

}
