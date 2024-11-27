namespace CoreAppStructure.Features.Categories.Controllers
{
    [ApiController]
    [Route("/api/category")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult> FindAll(string? name, string? sort, int page = 1)
        {
            var result = await _categoryService.FindAllAsync(name, sort, page);
            return Ok(result);
        }
        [HttpGet("all")]
        public async Task<ActionResult> FindListAll()
        {
            var result = await _categoryService.FindListAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> FindById(int id)
        {
            var result = await _categoryService.FindByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult> FindBySlug(string slug)
        {
            var result = await _categoryService.FindBySlugAsync(slug);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> Save(CategoryViewModel model)
        {
            var result = await _categoryService.SaveAsync(model);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, CategoryViewModel model)
        {
            var result = await _categoryService.UpdateAsync(id, model);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteAsync(id);
            return Ok(result);
        }
    }
}
