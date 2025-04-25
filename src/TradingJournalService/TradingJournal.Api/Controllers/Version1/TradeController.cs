using BuildingBlocks.Auth.Enums;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Pagination;
using TradingJournal.Application.Trades.Commands.CreateTrade;
using TradingJournal.Application.Trades.Commands.DeleteTrade;
using TradingJournal.Application.Trades.Commands.UpdateTrade;
using TradingJournal.Application.Trades.Queries.GetTradeById;
using TradingJournal.Application.Trades.Queries.GetTrades;

namespace TradingJournal.Api.Controllers.Version1;

/// <summary>
/// Trade API - v1
/// </summary>
[Authorize(Policy = nameof(Policies.AdminOrUser))]
[ApiController]
[Route("api/v{version:apiVersion}/trade")]
[ApiVersion("1.0")]
public class TradeController(ISender sender) : ControllerBase
{
    #region GetTrades
    
    /// <summary>
    /// Get Trades.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Get a list of all trades.
    /// ```
    /// Input:
    /// id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD1"
    /// pageNumber: 1
    /// pageSize: 10
    /// sorts:[
    ///     {
    ///         sortBy: "CloseDateTime",
    ///         sortOrder: "asc",
    ///         order: 1
    ///     },
    /// ]
    /// search: "EntryDateTime"
    /// ------------------------------------------
    /// Output:
    /// {
    ///     currentPage: 1,
    ///     totalPages: 1,
    ///     totalCount: 2,
    ///     pageSize: 10,
    ///     hasPreviousPage: false,
    ///     hasNextPage: false,
    ///     data: [
    ///         {
    ///             id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD4",
    ///             symbol: 0,
    ///             positionType: 2,
    ///             volume: 50.0,
    ///             entryPrice: 2342.11,
    ///             closePrice: 2345.12,
    ///             stopLossPrice: 2330.00,
    ///             entryDateTime: "2025-12-01T12:25:32",
    ///             closeDateTime: "2025-12-01T15:00:00",
    ///             commission: -1.0,
    ///             swap: -2.0,
    ///             pips: 100.0,
    ///             netProfit: 150.0,
    ///             grossProfit: 160.0,
    ///             balance: 1200.0,
    ///             tradingPlanId: "F4C5D951-7129-4CAD-8B04-79BB1C954AD4"
    ///         },
    ///         ...
    ///     ]
    /// }
    /// ```
    /// </remarks>
    [ProducesResponseType(typeof(PaginatedResult<GetTradesResult>), 200)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),403)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Produces("application/json")]
    [HttpGet(Name = "GetTrades")]
    public async Task<IActionResult> GetTrades([FromQuery] PaginationRequestWithId<Guid> request)
    {
        var result = await sender.Send(new GetTradesQuery
        {
            PaginationRequestWithId = request
        });
        
        return Ok(result);
    }
    
    #endregion
    
    #region GetTradeById
    
    /// <summary>
    /// Get Trade By Id.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="404">The server cannot find the requested resource. The URL may be incorrect or the resource may not exist.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Get a single trade by Id.
    /// ```
    /// Input:
    /// id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD1"
    /// ------------------------------------------
    /// Output:
    /// {
    ///     id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD4",
    ///     symbol: 0,
    ///     positionType: 2,
    ///     volume: 50.0,
    ///     entryPrice: 2342.11,
    ///     closePrice: 2345.12,
    ///     stopLossPrice: 2330.00,
    ///     entryDateTime: "2025-12-01T12:25:32",
    ///     closeDateTime: "2025-12-01T15:00:00",
    ///     commission: -1.0,
    ///     swap: -2.0,
    ///     pips: 100.0,
    ///     netProfit: 150.0,
    ///     grossProfit: 160.0,
    ///     balance: 1200.0,
    ///     tradingPlanId: "F4C5D951-7129-4CAD-8B04-79BB1C954AD4"
    /// },
    /// </remarks>
    [ProducesResponseType(typeof(GetTradeByIdResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),403)]
    [ProducesResponseType(typeof(ProblemDetails),404)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Produces("application/json")]
    [HttpGet("{id:guid}", Name = "GetTradeById")]
    public async Task<IActionResult> GetTradeById(Guid id)
    {
        var queryResponse = await sender.Send(new GetTradeByIdQuery
        {
            TradeId = id
        });

        return Ok(queryResponse);
    }
    
    #endregion
    
    #region CreateTrade
    
    /// <summary>
    /// Create trade.
    /// </summary>
    /// <response code="201">The trade has been created.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Create a new trade record.
    ///```
    /// Input:
    /// TradingPlanId: "F4C5D951-7129-4CAD-8B04-79BB1C954AD4"
    /// Symbol: 1
    /// PositionType: 2
    /// Volume: 1.0
    /// EntryPrice: 2233.0
    /// ClosePrice: 2244.0
    /// StopLossPrice: 2222.0
    /// EntryDateTime: "2025-12-01T12:25:32"
    /// CloseDateTime: "2025-12-01T15:00:00"
    /// Commission: -3.8
    /// Swap: -1.2
    /// Pips: 200.0
    /// NetProfit: 150.0
    /// GrossProfit: 160.0
    /// Balance: 1500.0
    /// ----------------------------
    /// Output:
    /// {
    ///     id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD4"
    /// }
    ///```
    /// </remarks>
    [ProducesResponseType(typeof(CreateTradeResult), 201)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),403)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Consumes("application/json")]
    [Produces("application/json")]
    [HttpPost(Name = "CreateTrade")]
    public async Task<IActionResult> CreateTrade([FromBody] CreateTradeCommand request)
    {
        var response = await sender.Send(request);
        
        return CreatedAtRoute("GetTradeById", new { response.Id }, response);
    }
    
    #endregion
    
    #region UpdateTrade
    
    /// <summary>
    /// Update Trade.
    /// </summary>
    /// <response code="204">The trade record has been successfully updated.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="404">The server cannot find the requested resource. The URL may be incorrect or the resource may not exist.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    ///<remarks>
    /// Update an existing trade.
    ///```
    /// Input:
    /// id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD4"
    /// Symbol: 1
    /// PositionType: 2
    /// Volume: 1.0
    /// EntryPrice: 2233.0
    /// ClosePrice: 2244.0
    /// StopLossPrice: 2222.0
    /// EntryDateTime: "2025-12-01T12:25:32"
    /// CloseDateTime: "2025-12-01T15:00:00"
    /// Commission: -3.8
    /// Swap: -1.2
    /// Pips: 200.0
    /// NetProfit: 150.0
    /// GrossProfit: 160.0
    /// Balance: 1500.0
    /// ------------------------------------------
    /// Output:
    /// {
    /// }
    ///```
    /// </remarks>
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),403)]
    [ProducesResponseType(typeof(ProblemDetails),404)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Produces("application/json")]
    [HttpPut("{id:guid}", Name = "UpdateTrade")]
    public async Task<IActionResult> UpdateTrade(Guid id, UpdateTradeCommand request)
    {
        if (id != request.Id)
        {
            throw new BadRequestException("The request Id does not match");
        }
        
        await sender.Send(request);

        return NoContent();
    }
    
    #endregion
    
    #region DeleteTrade
    
    /// <summary>
    /// Delete Trade.
    /// </summary>
    /// <response code="204">The trade record has been successfully deleted.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="404">The server cannot find the requested resource. The URL may be incorrect or the resource may not exist.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    ///<remarks>
    /// Delete an existing trade record.
    ///```
    /// Input:
    /// id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD4"
    /// ------------------------------------------
    /// Output:
    /// {
    /// }
    ///```
    /// </remarks>
    
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),403)]
    [ProducesResponseType(typeof(ProblemDetails),404)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Produces("application/json")]
    [HttpDelete("{id:guid}", Name = "DeleteTrade")]
    public async Task<ActionResult> DeleteTrade(Guid id)
    {
        await sender.Send(new DeleteTradeCommand
        {
            TradeId = id
        });
        
        return NoContent();
    }
    
    #endregion
}