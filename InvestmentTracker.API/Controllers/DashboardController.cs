using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InvestmentTracker.API.Infrastructure;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public sealed class DashboardController : ControllerBase
{
    private readonly DashboardSummaryQuery _query;

    public DashboardController(DashboardSummaryQuery query)
    {
        _query = query;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> Summary([FromQuery] DateTime date)
    {
        var userId = UserContext.GetUserId(User);
        return Ok(await _query.ExecuteAsync(userId, date));
    }

    [HttpPost("chart")]
    public async Task<ActionResult<DashboardChartResponse>> Chart([FromBody] DashboardChartRequest request)
    {
        var userId = UserContext.GetUserId(User);
        return Ok(await _query.GetChart(userId, request));
    }

    [HttpGet("investment-table")]
    public async Task<ActionResult<DashboardInvestmentTableResponse>> GetInvestmentTable()
    {
        var userId = UserContext.GetUserId(User);
        return Ok(await _query.GetInvestmentTable(userId));
    }
}
