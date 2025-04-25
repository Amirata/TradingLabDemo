using BuildingBlocks.Auth.Enums;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Pagination;
using TradingJournal.Application.TradingPlans.Commands.CreateTradingPlan;
using TradingJournal.Application.TradingPlans.Commands.DeleteTradingPlan;
using TradingJournal.Application.TradingPlans.Commands.UpdateTradingPlan;
using TradingJournal.Application.TradingPlans.Queries.GetTradingPlanById;
using TradingJournal.Application.TradingPlans.Queries.GetTradingPlanByName;
using TradingJournal.Application.TradingPlans.Queries.GetTradingPlans;

namespace TradingJournal.Api.Controllers.Version1;

/// <summary>
/// Trading Plan API - v1
/// </summary>
[Authorize(Policy = nameof(Policies.AdminOrUser))]
[ApiController]
[Route("api/v{version:apiVersion}/trading-plan")]
[ApiVersion("1.0")]
public class TradingPlanController(ISender sender) : ControllerBase
{
    #region GetTradingPlans
    
    /// <summary>
    /// Get Trading Plans.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Get a list of all trading plans.
    /// ```
    /// Input:
    /// id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD1"
    /// pageNumber: 1
    /// pageSize: 10
    /// sorts:[
    ///     {
    ///         sortBy: "Name",
    ///         sortOrder: "asc",
    ///         order: 1
    ///     },
    /// ]
    /// search: "Name"
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
    ///             id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD1",
    ///             name: "plan name",
    ///             fromTime: "10:23:00",
    ///             toTime: "23:12:00",
    ///             selectedDays: [
    ///                 "Monday",
    ///                 ...
    ///             ],
    ///             technics: [
    ///                 {
    ///                     id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD1",
    ///                     name: "technic name",
    ///                     description: "description",
    ///                     images: [
    ///                        "pic1",
    ///                        ...
    ///                     ]
    ///                 },
    ///                 ...
    ///             ]
    ///         },
    ///         ...
    ///     ]
    /// }
    /// ```
    /// </remarks>
    [ProducesResponseType(typeof(PaginatedResult<GetTradingPlansResult>), 200)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),403)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Produces("application/json")]
    [HttpGet(Name = "GetTradingPlans")]
    public async Task<IActionResult> GetTradingPlans([FromQuery] PaginationRequest request)
    {
        var result = await sender.Send(new GetTradingPlansQuery
        {
            PaginationRequest = request
        });
        
        return Ok(result);
    }
    
    #endregion
    
    #region GetTradingPlanById
    
    /// <summary>
    /// Get Trading Plan By Id.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="404">The server cannot find the requested resource. The URL may be incorrect or the resource may not exist.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Get a single trading plan by Id.
    /// ```
    /// Input:
    /// id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD1"
    /// ------------------------------------------
    /// Output:
    /// {
    ///     id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD1",
    ///     name: "plan name",
    ///     fromTime: "10:23:00",
    ///     toTime: "23:12:00",
    ///     selectedDays: [
    ///         "Monday",
    ///         ...
    ///     ],
    ///     technics: [
    ///         {
    ///             id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD1",
    ///             name: "technic name",
    ///             description: "description",
    ///             images: [
    ///                "pic1",
    ///                ...
    ///             ]
    ///         },
    ///         ...
    ///     ]
    /// },
    /// </remarks>
    [ProducesResponseType(typeof(GetTradingPlanByIdResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),403)]
    [ProducesResponseType(typeof(ProblemDetails),404)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Produces("application/json")]
    [HttpGet("{id:guid}", Name = "GetTradingPlanById")]
    public async Task<IActionResult> GetTradingPlanById(Guid id)
    {
        var queryResponse = await sender.Send(new GetTradingPlanByIdQuery
        {
            TradingPlanId = id
        });

        return Ok(queryResponse);
    }
    
    #endregion
    
    #region GetTradingPlanByName
    
    /// <summary>
    /// Get Trading Plan By Name.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Get trading plans by name.
    /// ```
    /// Input:
    /// name: "plan name"
    /// ------------------------------------------
    /// Output:
    /// [
    ///     {
    ///         id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD1",
    ///         name: "plan name",
    ///         fromTime: "10:23:00",
    ///         toTime: "23:12:00",
    ///         selectedDays: [
    ///             "Monday",
    ///             ...
    ///         ],
    ///         technics: [
    ///             {
    ///                 id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD1",
    ///                 name: "technic name",
    ///                 description: "description",
    ///                 images: [
    ///                    "pic1",
    ///                    ...
    ///                 ]
    ///             },
    ///             ...
    ///         ]
    ///     },
    ///     ...
    /// ]
    /// </remarks>
    [ProducesResponseType(typeof(ICollection<GetTradingPlanByNameResult>), 200)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),403)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Produces("application/json")]
    [HttpGet("{name}", Name = "GetTradingPlanByName")]
    public async Task<IActionResult> GetTradingPlanByName(string name)
    {
        var queryResponse = await sender.Send(new GetTradingPlanByNameQuery
        {
            Name = name
        });

        return Ok(queryResponse);
    }
    
    #endregion

    #region CreateTradingPlan
    
    /// <summary>
    /// Create Trading Plan.
    /// </summary>
    /// <response code="201">The trading plan has been created.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Create a new trading plan record.
    ///```
    /// Input:
    /// name: "plan name"
    /// fromTime: "10:23:00"
    /// toTime: "22:23:00"
    /// selectedDays: [
    ///     "Monday",
    ///     ...
    /// ],
    /// technics:[
    ///     "F4C5D951-7129-4CAD-8B04-79BB1C954AD1",
    ///     ...
    /// ]
    /// ----------------------------
    /// Output:
    /// {
    ///     id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD4"
    /// }
    /// ```
    /// The selectedDays value is either an empty array or
    /// contains only these specific values.
    /// ```
    /// "selectedDays" = [
    ///     "Sunday",
    ///     "Monday",
    ///     "Tuesday",
    ///     "Wednesday",
    ///     "Thursday",
    ///     "Friday",
    ///     "Saturday"
    /// ]
    /// ```
    /// </remarks>
    [ProducesResponseType(typeof(CreateTradingPlanResult), 201)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),403)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Consumes("application/json")]
    [Produces("application/json")]
    [HttpPost(Name = "CreateTradingPlan")]
    public async Task<IActionResult> CreateTradingPlan([FromBody] CreateTradingPlanCommand request)
    {
        var response = await sender.Send(request);
        
        return CreatedAtRoute("GetTradingPlanById", new { response.Id }, response);
    }
    
    #endregion
    
    #region UpdateTradingPlan
    
    /// <summary>
    /// Update Trading Plan.
    /// </summary>
    /// <response code="204">The trading plan record has been successfully updated.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="404">The server cannot find the requested resource. The URL may be incorrect or the resource may not exist.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    ///<remarks>
    /// Update an existing trading plan.
    ///```
    /// Input:
    /// id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD4"
    /// name: "plan name"
    /// fromTime: "10:23:00"
    /// toTime: "22:23:00"
    /// selectedDays: [
    ///     "Monday",
    ///     ...
    /// ]
    /// technics:[
    ///     "F4C5D951-7129-4CAD-8B04-79BB1C954AD1",
    ///     ...
    /// ]
    /// removedTechnics:[
    ///     "F4C5D951-7129-4CAD-8B04-79BB1C954AD0",
    ///     ...
    /// ]
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
    [HttpPut("{id:guid}", Name = "UpdateTradingPlan")]
    public async Task<IActionResult> UpdateTradingPlan(Guid id, UpdateTradingPlanCommand request)
    {
        if (id != request.Id)
        {
            throw new BadRequestException("The request Id does not match");
        }
        
        await sender.Send(request);

        return NoContent();
    }
    
    #endregion
    
    #region DeleteTradingPlan
    
    /// <summary>
    /// Delete Trading Plan.
    /// </summary>
    /// <response code="204">The trading plan record has been successfully deleted.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="404">The server cannot find the requested resource. The URL may be incorrect or the resource may not exist.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    ///<remarks>
    /// Delete an existing trading plan record.
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
    [HttpDelete("{id:guid}", Name = "DeleteTradingPlan")]
    public async Task<ActionResult> DeleteTradingPlan(Guid id)
    {
        await sender.Send(new DeleteTradingPlanCommand
        {
            TradingPlanId = id
        });
        
        return NoContent();
    }
    
    #endregion
}