using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly BlogDataContext _context;
        private readonly IMemoryCache _cache;

        public CategoryController(BlogDataContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var categories = await _cache.GetOrCreateAsync("CategoriesCache", async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return await GetCategories();
                });

                return Ok(new ResultViewModel<List<Category>>(categories));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05x01 - Falha interna no servidor"));
            }
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                    return NotFound(new ResultViewModel<Category>("Categoria não encontrada"));

                return Ok(new ResultViewModel<Category>(category));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Category>("05x02 - Falha interna no servidor"));
            }
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync([FromBody] EditorCategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

            try
            {
                var category = new Category
                {
                    Name = model.Name ?? string.Empty,
                    Slug = model.Slug?.ToLower() ?? string.Empty,
                };

                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                _cache.Remove("CategoriesCache");

                return Created($"/v1/categories/{category.Id}", new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Category>("05xe3 - Não foi possível incluir a categoria"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Category>("05x03 - Falha interna no servidor"));
            }
        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync([FromRoute] int id, [FromBody] EditorCategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));

            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                    return NotFound(new ResultViewModel<Category>("Categoria não encontrada"));

                category.Name = model.Name ?? string.Empty;
                category.Slug = model.Slug?.ToLower() ?? string.Empty;

                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                _cache.Remove("CategoriesCache"); 

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Category>("05xe4 - Não foi possível alterar a categoria"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Category>("05x04 - Falha interna no servidor"));
            }
        }
              
        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null)
                    return NotFound(new ResultViewModel<Category>("Categoria não encontrada"));

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                _cache.Remove("CategoriesCache");

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Category>("05xe5 - Não foi possível excluir a categoria"));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<Category>("05x05 - Falha interna no servidor"));
            }
        }

        private async Task<List<Category>> GetCategories()
        {
            return await _context.Categories
                .AsNoTracking()
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug
                })
                .ToListAsync();
        }
    }
}