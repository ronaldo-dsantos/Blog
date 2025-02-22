using Blog.Data;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace Blog.Controllers
{
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly BlogDataContext _context;


        public PostController(BlogDataContext context)
        {
            _context = context;
        }

        [HttpGet("v1/posts")]
        public async Task<IActionResult> GetAsync([FromQuery] int page = 0, [FromQuery] int pageSize = 25)
        {
            try
            {
                var count = await _context.Posts.CountAsync();

                var posts = await _context.Posts
                                    .AsNoTracking()
                                    .Include(x => x.Category)
                                    .Include(x => x.Author)
                                    .Select(x => new ListPostsViewModel
                                    {
                                        Id = x.Id,
                                        Title = x.Title,
                                        Slug = x.Slug,
                                        LastUpdateDate = x.LastUpdateDate,
                                        Category = x.Category.Name,
                                        Author = $"{x.Author.Name} ({x.Author.Email})"
                                    })
                                    .Skip(page * pageSize)
                                    .Take(pageSize)
                                    .OrderByDescending(x => x.LastUpdateDate)
                                    .ToListAsync();

                return Ok(new ResultViewModel<dynamic>(new
                {
                    total = count,
                    page,
                    pageSize,
                    posts
                }));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Post>>("05x04 - Falha interna do servidor"));

            }
        }
    }
}
