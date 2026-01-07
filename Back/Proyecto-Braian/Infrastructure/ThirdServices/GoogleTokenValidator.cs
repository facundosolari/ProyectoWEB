using Application.Interfaces;
using Application.Models.Helpers;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.ThirdServices
{
    public class GoogleTokenValidator : IGoogleTokenValidator
    {
        private readonly IConfiguration _configuration;

        public GoogleTokenValidator(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<GoogleUserPayload> ValidateAsync(string token)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[]
                {
            _configuration["Authentication:Google:ClientId"]
        }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

            return new GoogleUserPayload
            {
                Email = payload.Email,
                Name = payload.Name
            };
        }
    }
}