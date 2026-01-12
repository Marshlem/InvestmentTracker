using InvestmentTracker.API.Data;
using InvestmentTracker.API.Models;
using InvestmentTracker.API.DTOs.AssetCategories;
using InvestmentTracker.API.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvestmentTracker.API.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public CategoriesController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<AssetCategory>>> GetAll()
    {
        var userId = UserContext.GetUserId(User);

        return await _db.AssetCategories
            .AsNoTracking()
            .Where(c => c.UserId == userId && c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<AssetCategory>> Create([FromBody] CreateCategoryRequest request)
    {
        var userId = UserContext.GetUserId(User);

        var cat = new AssetCategory
        {
            Name = request.Name,
            IsActive = true,
            UserId = userId
        };

        _db.AssetCategories.Add(cat);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = cat.Id }, cat);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AssetCategory>> Rename(int id, RenameCategoryRequest request)
    {
        var userId = UserContext.GetUserId(User);

        var cat = await _db.AssetCategories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (cat == null)
            return NotFound();

        cat.Name = request.Name;
        await _db.SaveChangesAsync();

        return Ok(cat);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = UserContext.GetUserId(User);

        var cat = await _db.AssetCategories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (cat == null)
            return NotFound();

        cat.IsActive = false;
        await _db.SaveChangesAsync();

        return NoContent();
    }
}
