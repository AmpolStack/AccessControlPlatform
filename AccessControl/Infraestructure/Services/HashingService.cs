using AccessControl.Domain.Services;

namespace AccessControl.Infraestructure.Services
{
    public class HashingService : IHashingService
    {
        public bool Check(string origin, string target)
        {
            var hash = BCrypt.Net.BCrypt.Verify(target, origin);
            return hash;
        }

        public string GetHash(string source)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(source);
            return hash;
        }
    }
}
