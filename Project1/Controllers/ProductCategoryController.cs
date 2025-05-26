using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Data;
using Project1.Data.Dtos;
using Project1.Models;

namespace Project1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductCategoryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductCategoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("create-category")]
        [Authorize]
        public async Task<IActionResult> CreateCategory([FromBody] ProductCategoryCreate dto)
        {

            bool exists = await _context.ProductCategories.AnyAsync(c =>
                c.Name.ToLower() == dto.Name.ToLower()
            );
            if (exists)
            {
                return BadRequest(new { message = "Category name already exists." });
            }

            var category = new ProductCategory { Name = dto.Name, CreatedAt = DateTime.UtcNow };

            _context.ProductCategories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category created", category });
        }

        [HttpGet("get-all-category")]
        [Authorize]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context
                .ProductCategories.Where(c => c.DeletedAt == null)
                .ToListAsync();

            return Ok(categories);
        }

        [HttpGet("get-category/{id}")]
        [Authorize]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.ProductCategories.FirstOrDefaultAsync(c =>
                c.Id == id && c.DeletedAt == null
            );

            if (category == null)
                return NotFound(new { message = "Category not found" });

            return Ok(category);
        }

        [HttpPut("update-category/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory(
            int id,
            [FromBody] ProductCategoryUpdate dto
        )
        {
            var category = await _context.ProductCategories.FirstOrDefaultAsync(c =>
                c.Id == id && c.DeletedAt == null
            );

            if (category == null)
                return NotFound(new { message = "Category not found" });

            if (!string.IsNullOrWhiteSpace(dto.Name))
                category.Name = dto.Name;

            category.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category updated", category });
        }

        [HttpDelete("delete-category/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {

            var category = await _context.ProductCategories.FirstOrDefaultAsync(c =>
                c.Id == id && c.DeletedAt == null
            );

            if (category == null)
                return NotFound(new { message = "Category not found" });

            bool hasProducts = await _context.Products.AnyAsync(p =>
                p.CategoryId == id && p.DeletedAt == null
            );
            if (hasProducts)
            {
                return BadRequest(
                    new
                    {
                        message = "Tidak dapat melakukan delete dikarenakan masih ada produk dengan id tersebut.",
                    }
                );
            }

            category.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category deleted" });
        }
    }
}
