using CoreAppStructure.Features.Parameters.Interfaces;
using CoreAppStructure.Features.Parameters.Models;
using Microsoft.AspNetCore.Mvc;

namespace CoreAppStructure.Features.Parameters.Controllers
{
    //[Authorize(Roles = "Admin")]
    [ApiController]
    [Route("/api/parameter")]
    public class ParameterController : ControllerBase
    {
        private readonly IParameterService _parameterService;

        public ParameterController(IParameterService parameterService)
        {
            _parameterService = parameterService;
        }

        [HttpGet]
        public async Task<ActionResult> FindAll(string? name, string? sort, int page = 1)
        {
            var result = await _parameterService.FindAllAsync(name, sort, page);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> FindById(int id)
        {
            var result = await _parameterService.FindByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> Save(ParameterViewModel model)
        {
            var result = await _parameterService.SaveAsync(model);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, ParameterViewModel model)
        {
            var result = await _parameterService.UpdateAsync(id, model);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _parameterService.DeleteAsync(id);
            return Ok(result);
        }
    }
}
