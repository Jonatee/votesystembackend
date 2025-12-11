using System.Threading.Tasks;
using votesystembackend.Application.DTOs;
using votesystembackend.Application.Interfaces;
using votesystembackend.Domain.Entities;
using votesystembackend.Infrastructure.Repositories;
using votesystembackend.Infrastructure.Services;
using votesystembackend.Application.Responses;

namespace votesystembackend.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IJwtProvider _jwt;

        public AuthService(IUserRepository userRepo, IJwtProvider jwt)
        {
            _userRepo = userRepo;
            _jwt = jwt;
        }

        public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest req)
        {
            // check existing email
            var existing = await _userRepo.GetByEmailAsync(req.Email);
            if (existing != null)
                return ServiceResult<AuthResponse>.Fail(409, "Email already registered.");

            var hashed = BCrypt.Net.BCrypt.HashPassword(req.Password);

            var user = new User
            {
                FirstName = req.FirstName,
                LastName = req.LastName,
                Email = req.Email,
                Username = req.Username,
                PasswordHash = hashed
            };

            await _userRepo.AddAsync(user);

            var response = new AuthResponse
            {
                Success = true,
                Token = _jwt.GenerateToken(user),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName
                }
            };

            return ServiceResult<AuthResponse>.Created(response, "Registered");
        }

        public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
        {
            var user = await _userRepo.GetByEmailAsync(request.Email);
            if (user == null)
                return ServiceResult<AuthResponse>.Fail(401, "Invalid credentials.");

            var valid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!valid)
                return ServiceResult<AuthResponse>.Fail(401, "Invalid credentials.");

            var response = new AuthResponse
            {
                Success = true,
                Token = _jwt.GenerateToken(user),
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName

                }
            };

            return ServiceResult<AuthResponse>.Ok(response, "Logged in");
        }
    }
}
