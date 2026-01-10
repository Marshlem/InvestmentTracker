using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly DashboardSummaryQuery _query;

    public DashboardController(DashboardSummaryQuery query)
    {
        _query = query;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> Summary(
        [FromQuery] DateTime date)
    {
        return Ok(await _query.ExecuteAsync(date));
    }

    [HttpPost("chart")]
    public async Task<ActionResult<DashboardChartResponse>> Chart(
        [FromBody] DashboardChartRequest request)
    {
        return Ok(await _query.GetChart(request));
    }

    [HttpGet("investment-table")]
    public async Task<ActionResult<DashboardInvestmentTableResponse>> GetInvestmentTable()
    {
        return Ok(await _query.GetInvestmentTable());
    }
}
