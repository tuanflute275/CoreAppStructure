namespace CoreAppStructure.Features.Roles.Controllers
{
    [ApiController]
    [Route("/api/role")]
    public class RoleController :ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult> FindAll(string? name, string? sort, int page = 1)
        {
            var result = await _roleService.FindAllAsync(name, sort, page);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<ActionResult> FindListAll()
        {
            var result = await _roleService.FindListAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> FindById(int id)
        {
            var result = await _roleService.FindByIdAsync(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Save(RoleViewModel model)
        {
            var result = await _roleService.SaveAsync(model);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, RoleViewModel model)
        {
            var result = await _roleService.UpdateAsync(id, model);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _roleService.DeleteAsync(id);
            return Ok(result);
        }
    }
}
