using InvestmentTracker.API.Models;
using InvestmentTracker.API.Requests.Assets;
using InvestmentTracker.API.Services;
using InvestmentTracker.API.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class AssetsController : ControllerBase
{
    private readonly IAssetService _service;

    public AssetsController(IAssetService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<Asset>>> GetAll()
    {
        var userId = UserContext.GetUserId(User);
        return Ok(await _service.GetAllAsync(userId));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Asset>> Get(int id)
    {
        var userId = UserContext.GetUserId(User);
        var item = await _service.GetByIdAsync(userId, id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Asset>> Create(CreateAssetRequest req)
    {
        var userId = UserContext.GetUserId(User);

        var asset = new Asset
        {
            Name = req.Name,
            CategoryId = req.CategoryId,
            Status = req.Status,
            ValueChangeCurrency = req.ValueChangeCurrency,
            DividendCurrency = req.DividendCurrency,
            UserId = userId 
        };

        var created = await _service.CreateAsync(userId, asset);

        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Asset>> Update(int id, UpdateAssetRequest req)
    {
        var userId = UserContext.GetUserId(User);
        var existing = await _service.GetByIdAsync(userId, id);
        if (existing == null) return NotFound();

        if (req.Name != null) existing.Name = req.Name;
        existing.CategoryId = req.CategoryId;
        existing.Status = req.Status;
        if (req.ValueChangeCurrency != null) existing.ValueChangeCurrency = req.ValueChangeCurrency;
        if (req.DividendCurrency != null) existing.DividendCurrency = req.DividendCurrency;

        await _service.UpdateAsync(userId, id, existing);
        return Ok(existing);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = UserContext.GetUserId(User);
        var ok = await _service.DeleteAsync(userId, id);
        return ok ? NoContent() : NotFound();
    }

    [HttpGet("with-stats")]
    public async Task<IActionResult> GetWithStats()
    {
        var userId = UserContext.GetUserId(User);
        return Ok(await _service.GetAssetsWithStatsAsync(userId));
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] DateTime date)
    {
        var userId = UserContext.GetUserId(User);
        return Ok(await _service.GetAssetSummaryAsync(userId, date));
    }
}