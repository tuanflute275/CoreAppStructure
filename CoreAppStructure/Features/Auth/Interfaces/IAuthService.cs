﻿namespace CoreAppStructure.Features.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseObject> LoginAsync(LoginViewModel model);
        Task<ResponseObject> RegisterAsync(RegisterViewModel model);
        Task<ResponseObject> RefreshTokenAsync(string refreshToken);
    }
}
