using InvestmentTracker.API.Data;
using InvestmentTracker.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvestmentTracker.API.DTOs.AssetCategories;

namespace InvestmentTracker.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public CategoriesController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IEnumerable<AssetCategory>> GetAll()
    {
        return await _db.AssetCategories
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        var cat = new AssetCategory
        {
            Name = request.Name,
            IsActive = true
        };

        await _db.AssetCategories.AddAsync(cat);
        await _db.SaveChangesAsync();

        return Ok(cat);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Rename(int id, RenameCategoryRequest request)
    {
        var cat = await _db.AssetCategories.FindAsync(id);
        if (cat == null) return NotFound();

        cat.Name = request.Name;
        await _db.SaveChangesAsync();

        return Ok(cat);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cat = await _db.AssetCategories.FindAsync(id);
        if (cat == null) return NotFound();

        cat.IsActive = false;

        await _db.SaveChangesAsync();
        return NoContent();
    }
}
