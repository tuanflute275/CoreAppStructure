using CoreAppStructure.Core.Helpers;
using CoreAppStructure.Features.Users.Interfaces;
using CoreAppStructure.Features.Users.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoreAppStructure.Features.Users.Controllers
{
    [ApiController]
    [Route("/api/user")]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseObject>> FindAll(string? name, string? sort, int page = 1)
        {
            var response = await _userService.FindAllAsync(name, sort, page);
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDTO>> FindById(int id)
        {
            var result = await _userService.FindByIdAsync(id);
            return Ok(result);
        }
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Save([FromForm] UserViewModel model)
        {
            var result = await _userService.SaveAsync(model, HttpContext.Request.Host.Value);
            return Ok(result);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromForm] UserViewModel model)
        {
            var result = await _userService.UpdateAsync(id, model, HttpContext.Request.Host.Value);
            return Ok(result);
        }

        //[Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _userService.DeleteAsync(id);
            return Ok(result);
        }
    }
}
