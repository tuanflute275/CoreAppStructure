﻿using CoreAppStructure.Features.Auth.Interfaces;
using CoreAppStructure.Features.Auth.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoreAppStructure.Features.Auth.Controlles
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            var result = await _authService.LoginAsync(model);
            return Ok(result);

        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterViewModel model)
        {
            var result = await _authService.RegisterAsync(model);
            return Ok(result);
        }
    }
}