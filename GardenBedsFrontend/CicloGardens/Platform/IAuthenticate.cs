using System.Threading.Tasks;

namespace CicloGardens.Platform
{
    public interface IAuthenticate
    {
        Task<bool> AuthenticateAsync();
        Task<bool> Logout();
    }
}