using BuildingBlocks.Auth.Enums;
using BuildingBlocks.FromToDate;
using TradingJournal.Application.TradeAnalyses.Queries.GetCalendarByYear;
using TradingJournal.Application.TradeAnalyses.Queries.GetChartBalance;
using TradingJournal.Application.TradeAnalyses.Queries.GetChartNetProfit;
using TradingJournal.Application.TradeAnalyses.Queries.GetGrossAndNetForEachSymbol;
using TradingJournal.Application.TradeAnalyses.Queries.GetGrossAndNetForEachSymbolForEachDayOfWeek;
using TradingJournal.Application.TradeAnalyses.Queries.GetTradeYears;

namespace TradingJournal.Api.Controllers.Version1;

/// <summary>
/// Analyse API - v1
/// </summary>
[Authorize(Policy = nameof(Policies.AdminOrUser))]
[ApiController]
[Route("api/v{version:apiVersion}/trade-analyse")]
[ApiVersion("1.0")]
public class TradeAnalyseController(ISender sender) : ControllerBase
{
    #region GetTradeYears

    /// <summary>
    /// Get Trade Years.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// To retrieve all trading years based on the plan. The output is an array of trading years from the most recent to the oldest.
    ///```
    /// Input:
    /// planId: "A2FF564B-740C-41EA-A47C-7DFEEADE3B14"
    /// ----------------------------------------------
    /// Output:
    /// [
    ///     2025,
    ///     2024,
    ///     ...,
    /// ]
    ///```
    /// </remarks>
    [ProducesResponseType(typeof(IEnumerable<int>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [Produces("application/json")]
    [HttpGet("{planId:guid}", Name = "GetTradeYears")]
    public async Task<IActionResult> GetTradeYears(Guid planId)
    {
        var result = await sender.Send(new GetTradeYearsQuery
        {
            PlanId = planId
        });

        return Ok(result);
    }

    #endregion
    
    #region GetCalendarByYear

    /// <summary>
    /// Get Calendar By Year.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// To retrieve the trading calendar based on the plan and year. The output is an array of objects including gross profit, net profit, number of losing trades, number of winning trades, total trades, win rate, average risk-to-reward ratio, and profit and loss calendar.
    /// ```
    /// Input:
    /// planId: "A2FF564B-740C-41EA-A47C-7DFEEADE3B14"
    /// year: 2025
    /// ----------------------------------------------
    /// Output:
    /// {
    ///     calendar: [
    ///         {
    ///             date: "2025-12-01",
    ///             count: 1,
    ///             level: 2
    ///         },
    ///         ...
    ///     ],
    ///     riskToRewardMean: 2,
    ///     winRate: 55,
    ///     totalTradeCount: 60,
    ///     totalWinTradeCount: 33,
    ///     totalLossTradeCount: 27,
    ///     netProfit: 150,
    ///     grossProfit: 160
    /// }
    /// ```
    /// </remarks>
    [ProducesResponseType(typeof(GetCalendarByYearResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [Produces("application/json")]
    [HttpGet("{planId:guid}/{year:int}", Name = "GetCalendarByYear")]
    public async Task<IActionResult> GetCalendarByYear(Guid planId, int year)
    {
        var queryResponse = await sender.Send(new GetCalendarByYearQuery
        {
            PlanId = planId,
            Year = year
        });

        return Ok(queryResponse);
    }

    #endregion

    #region GetChartBalance

    /// <summary>
    /// Get Chart Balance.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// To retrieve the balance based on the plan. The output is an array of balance from the oldest day to the most recent day.
    /// ```
    /// Input:
    /// planId: "A2FF564B-740C-41EA-A47C-7DFEEADE3B14"
    /// ----------------------------------------------
    /// Output:
    /// [
    ///     {
    ///         date: "2025-12-01",
    ///         balance: 150
    ///     },
    ///     ...
    /// ]
    /// ```
    /// </remarks>
    [ProducesResponseType(typeof(ICollection<GetChartBalanceResult>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [Produces("application/json")]
    [HttpGet("GetChartBalance/{planId:guid}", Name = "GetChartBalance")]
    public async Task<IActionResult> GetChartBalance(Guid planId)
    {
        var queryResponse = await sender.Send(new GetChartBalanceQuery
        {
            PlanId = planId,
        });

        return Ok(queryResponse);
    }

    #endregion
    
    #region GetChartNetProfit

    /// <summary>
    /// Get Chart NetProfit.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// To retrieve the total of daily profits based on the plan. The output is an array of total daily profits from the oldest day to the most recent day.
    /// ```
    /// Input:
    /// planId: "A2FF564B-740C-41EA-A47C-7DFEEADE3B14"
    /// ----------------------------------------------
    /// Output:
    /// [
    ///     {
    ///         date: "2025-12-01",
    ///         netProfit: 150
    ///     },
    ///     ...
    /// ]
    /// ```
    /// </remarks>
    [ProducesResponseType(typeof(ICollection<GetChartNetProfitResult>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [Produces("application/json")]
    [HttpGet("GetChartNetProfit/{planId:guid}", Name = "GetChartNetProfit")]
    public async Task<IActionResult> GetChartNetProfit(Guid planId)
    {
        var queryResponse = await sender.Send(new GetChartNetProfitQuery
        {
            PlanId = planId,
        });

        return Ok(queryResponse);
    }

    #endregion
    
    #region GetGrossAndNetForEachSymbol

    /// <summary>
    /// Get Gross And Net For Each Symbol.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// To calculate the total net profit and gross profit based on the plan for each symbol within a given date time range. The output should be an array of symbols along with their net profit and gross profit.
    ///```
    /// Input:
    /// planId: "A2FF564B-740C-41EA-A47C-7DFEEADE3B14"
    /// fromDate: "2020-12-24" or null
    /// toDate: "2020-12-24" or null
    /// ----------------------------------------------
    /// Output:
    /// [
    ///     {
    ///         symbol: 0,
    ///         netProfit: 150,
    ///         grossProfit: 160
    ///     },
    ///     ...
    /// ]
    ///```
    /// </remarks>
    [ProducesResponseType(typeof(ICollection<GetGrossAndNetForEachSymbolResult>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [Produces("application/json")]
    [HttpGet("GetGrossAndNetForEachSymbol/{planId:guid}", Name = "GetGrossAndNetForEachSymbol")]
    public async Task<IActionResult> GetGrossAndNetForEachSymbols(Guid planId, [FromQuery] FromToDateRequest request)
    {
        var queryResponse = await sender.Send(new GetGrossAndNetForEachSymbolQuery
        {
            PlanId = planId,
            FromDate = request.FromDate,
            ToDate = request.ToDate
        });

        return Ok(queryResponse);
    }

    #endregion

    #region GetGrossAndNetForEachSymbolForEachDayOfWeek

    /// <summary>
    /// Get Gross And Net For Each Symbol For Each Day Of Week.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// To calculate the total net profit and gross profit based on the plan for each day of the week within a given date time range or for a specific symbol or all symbols. The output should be an array of weekdays along with their net profit and gross profit.
    /// ```
    /// Input:
    /// planId: "A2FF564B-740C-41EA-A47C-7DFEEADE3B14"
    /// fromDate: "2020-12-24" or null
    /// toDate: "2020-12-24" or null
    /// symbol: 0 or null
    /// ----------------------------------------------
    /// Output:
    /// [
    ///     {
    ///         dayOfWeek: 0,
    ///         netProfit: 150,
    ///         grossProfit: 160
    ///     },
    ///     ...
    /// ]
    /// ```
    /// </remarks>
    [ProducesResponseType(typeof(ICollection<GetGrossAndNetForEachSymbolForEachDayOfWeekResult>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    [ProducesResponseType(typeof(ProblemDetails), 403)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    [Produces("application/json")]
    [HttpGet("GetGrossAndNetForEachSymbolForEachDayOfWeek/{planId:guid}",
        Name = "GetGrossAndNetForEachSymbolForEachDayOfWeek")]
    public async Task<IActionResult> GetGrossAndNetForEachSymbolForEachDayOfWeek(Guid planId,
        [FromQuery] GetGrossAndNetForEachSymbolForEachDayOfWeekFromQuery request)
    {
        var queryResponse = await sender.Send(new GetGrossAndNetForEachSymbolForEachDayOfWeekQuery
        {
            PlanId = planId,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            Symbol = request.Symbol
        });

        return Ok(queryResponse);
    }

    #endregion
}