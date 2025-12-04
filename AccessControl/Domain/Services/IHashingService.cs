
namespace AccessControl.Domain.Services
{
    public interface IHashingService
    {
        string GetHash(string source);
        bool Check(string origin, string target);
    }
}
