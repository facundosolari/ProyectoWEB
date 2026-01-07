using Application.Interfaces;
using Application.Models.Helpers;
using Application.Models.Request;
using Domain.Entities;
using Domain.Enum;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.ThirdServices
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthenticationServiceOptions _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGoogleTokenValidator _googleTokenValidator;

        public AuthenticationService(
            IUserRepository userRepository,
            IOptions<AuthenticationServiceOptions> options,
            IHttpContextAccessor httpContextAccessor,
            IGoogleTokenValidator googleTokenValidator)
        {
            _userRepository = userRepository;
            _options = options.Value;
            _httpContextAccessor = httpContextAccessor;
            _googleTokenValidator = googleTokenValidator;
        }

        // -------------------------------
        // LOGIN LOCAL
        // -------------------------------
        private User? ValidateUser(AuthenticationRequest request)
        {
            var user = _userRepository.GetAllUsers()
                .FirstOrDefault(u => u.Usuario == request.Usuario);

            if (user == null) return null;

            // ✅ Solo falla si no tiene LOCAL vinculado
            if (!user.Provider.HasFlag(AuthProvider.Local))
                throw new UnauthorizedAccessException("Este usuario no tiene contraseña configurada. Iniciá sesión con Google.");

            if (user.Contraseña != request.Contraseña)
                return null;

            if (!user.Habilitado)
                throw new UnauthorizedAccessException("El usuario ha sido desactivado.");

            return user;
        }

        public string Authenticate(AuthenticationRequest request)
        {
            var user = ValidateUser(request)
                ?? throw new UnauthorizedAccessException("Usuario o contraseña incorrectos");

            return GenerateJwt(user);
        }

        // -------------------------------
        // LOGIN GOOGLE
        // -------------------------------
        public async Task<string> AuthenticateWithGoogle(string googleToken)
        {
            var payload = await _googleTokenValidator.ValidateAsync(googleToken);

            // Buscar usuario por email
            var user = _userRepository.GetAllUsers()
                .FirstOrDefault(u => u.Email == payload.Email);

            if (user != null)
            {
                if (!user.Provider.HasFlag(AuthProvider.Google))
                {
                    // Account linking automático
                    user.Provider |= AuthProvider.Google;
                    _userRepository.UpdateUser(user);
                }
            }

            else
            {
                // Crear usuario nuevo con provider Google
                user = new User
                {
                    Email = payload.Email,
                    Usuario = payload.Email,
                    Provider = AuthProvider.Google,
                    Contraseña = null,
                    Rol = Rol.User
                };

                _userRepository.AddUser(user);
            }

            if (!user.Habilitado)
                throw new UnauthorizedAccessException("El usuario ha sido desactivado.");

            return GenerateJwt(user);
        }

        // -------------------------------
        // ACCOUNT LINKING GOOGLE
        // -------------------------------
        public async Task LinkGoogleAccount(int userId, string googleToken)
        {
            var payload = await _googleTokenValidator.ValidateAsync(googleToken);

            var user = _userRepository.GetUserById(userId)
                ?? throw new UnauthorizedAccessException("Usuario no encontrado");

            // Validar que el email coincida
            if (user.Email != payload.Email)
                throw new UnauthorizedAccessException(
                    "El email de Google no coincide con la cuenta"
                );

            // Ya vinculado
            if (user.Provider.HasFlag(AuthProvider.Google))
                return;

            // Vincular
            user.Provider |= AuthProvider.Google;
            _userRepository.UpdateUser(user);
        }

        // -------------------------------
        // GENERACIÓN DE JWT + COOKIE
        // -------------------------------
        private string GenerateJwt(User user)
        {
            int tiempoMinutos = 15;

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_options.SecretForKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Rol", user.Rol.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(tiempoMinutos),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Guardar cookie HttpOnly
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(
                "AuthCookie",
                tokenString,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(tiempoMinutos)
                });

            return tokenString;
        }
    }
}