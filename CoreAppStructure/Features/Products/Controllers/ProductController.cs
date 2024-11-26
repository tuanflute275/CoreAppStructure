using CoreAppStructure.Core.Exceptions;
using CoreAppStructure.Features.Categories.Models;
using CoreAppStructure.Features.Products.Interfaces;
using CoreAppStructure.Features.Products.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreAppStructure.Features.Products.Controllers
{
    [ApiController]
    [Route("/api/product")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseObject>> FindAll(string? name, string? sort, int page = 1)
        {
            var response = await _productService.FindAllAsync(name, sort, page);
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Category>> FindById(int id)
        {
            var result = await _productService.FindByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult<Product>> FindBySlug(string slug)
        {
            var result = await _productService.FindBySlugAsync(slug);
            return Ok(result);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Product>> Save([FromForm] ProductViewModel model)
        {
            var result = await _productService.SaveAsync(model);
            return Ok(result);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> Update(int id, [FromForm] ProductViewModel model)
        {
            var result = await _productService.UpdateAsync(id, model);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);
            return Ok(result);
        }
    }
}
