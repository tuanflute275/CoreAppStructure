using CoreAppStructure.Core.WebSocket;

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
       
        [HttpGet("signin-google")]
        public async Task<ActionResult> GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleCallback", "Auth");
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        public async Task<ActionResult> GoogleCallback()
        {
            var result = await _authService.GoogleCallbackAsync(HttpContext);

            if (result.status != 200)
            {
                return Unauthorized(new { message = result.message });
            }
            return Ok(result);
        }

        [HttpGet("signin-facebook")]
        public async Task<ActionResult> FacebookLogin()
        {
            var redirectUrl = Url.Action("FacebookCallback", "Auth");
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, FacebookDefaults.AuthenticationScheme);
        }

        [HttpGet("facebook-callback")]
        public async Task<ActionResult> FacebookCallback()
        {
            var result = await _authService.FacebookCallbackAsync(HttpContext);

            if (result.status != 200)
            {
                return Unauthorized(new { message = result.message });
            }
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            var result = await _authService.LoginAsync(model);
            return Ok(result);

        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> refreshToken(string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);
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
