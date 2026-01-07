using Application.Models.Helpers;

namespace Application.Interfaces
{
    public interface IGoogleTokenValidator
    {
        Task<GoogleUserPayload> ValidateAsync(string token);
    }
}