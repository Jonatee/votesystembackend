using System.Threading.Tasks;
using votesystembackend.Application.DTOs;
using votesystembackend.Application.Responses;

namespace votesystembackend.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request);
    }
}
