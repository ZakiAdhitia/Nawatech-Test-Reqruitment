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
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("create-product")]
        [Authorize]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreate dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            bool exists = await _context.Products.AnyAsync(p =>
                p.Name.ToLower() == dto.Name.ToLower()
            );
            if (exists)
            {
                return BadRequest(new { message = "Produk sudah ada." });
            }

            string? savedFileName = null;

            if (dto.Image != null && dto.Image.Length > 0)
            {
                var uploadsFolder = Path.Combine("Data", "Static", "Images");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName =
                    Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                savedFileName = uniqueFileName;
            }

            var product = new Product
            {
                UserId = Guid.Parse(userId),
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Image = savedFileName,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product created", product });
        }

        [Authorize]
        [HttpGet("get-all-product")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context
                .Products.Where(p => p.DeletedAt == null)
                .Include(p => p.ProductCategory)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Description = p.Description,
                    Image = p.Image,
                    Category =
                        p.ProductCategory == null
                            ? null
                            : new CategoryDto
                            {
                                Id = p.ProductCategory.Id,
                                Name = p.ProductCategory.Name,
                            },
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("get-product/{id}")]
        [Authorize]
        public async Task<IActionResult> GetProductById(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null)
                return Unauthorized();

            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            var product = await _context.Products.FirstOrDefaultAsync(p =>
                p.Id == id && p.UserId == userId && p.DeletedAt == null
            );

            if (product == null)
                return NotFound(new { message = "Product not found" });

            return Ok(product);
        }

        [HttpPut("update-product/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpdate dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var product = await _context.Products.FirstOrDefaultAsync(p =>
                p.Id == id && p.UserId.ToString() == userId && p.DeletedAt == null
            );

            if (product == null)
                return NotFound(new { message = "Product not found" });

            if (dto.CategoryId.HasValue)
                product.CategoryId = dto.CategoryId.Value;
            if (!string.IsNullOrWhiteSpace(dto.Name))
                product.Name = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.Description))
                product.Description = dto.Description;
            if (dto.Price.HasValue)
                product.Price = dto.Price.Value;
            if (dto.Image != null && dto.Image.Length > 0)
            {
                var uploadsFolder = Path.Combine("Data", "Static", "Images");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName =
                    Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }
                if (!string.IsNullOrEmpty(product.Image))
                {
                    var oldFilePath = Path.Combine("Data", "Static", "Images", product.Image);
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }
                product.Image = uniqueFileName;
            }

            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Product updated", product });
        }

        [HttpDelete("delete-product/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var product = await _context.Products.FirstOrDefaultAsync(p =>
                p.Id == id && p.DeletedAt == null
            );

            if (product == null)
                return NotFound(new { message = "Product not found" });

            product.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product deleted" });
        }
    }
}
