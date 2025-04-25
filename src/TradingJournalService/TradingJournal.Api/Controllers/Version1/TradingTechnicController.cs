using BuildingBlocks.Auth.Enums;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Pagination;
using TradingJournal.Application.TradingTechnics.Commands.CreateTradingTechnic;
using TradingJournal.Application.TradingTechnics.Commands.DeleteTradingTechnic;
using TradingJournal.Application.TradingTechnics.Commands.UpdateTradingTechnic;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicById;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnicByName;
using TradingJournal.Application.TradingTechnics.Queries.GetTradingTechnics;

namespace TradingJournal.Api.Controllers.Version1;

/// <summary>
/// Trading Technic API - v1
/// </summary>
[Authorize(Policy = nameof(Policies.AdminOrUser))]
[ApiController]
[Route("api/v{version:apiVersion}/trading-technic")]
[ApiVersion("1.0")]
public class TradingTechnicController(ISender sender) : ControllerBase
{
    #region GetTradingTechnics
    
   /// <summary>
    /// Get Trading Technics.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Get a list of all trading technics.
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
    ///             description: "description",
    ///             images: [
    ///                 "pic1",
    ///                 ...
    ///             ],
    ///         },
    ///         ...
    ///     ]
    /// }
    /// ```
    /// </remarks>
    [ProducesResponseType(typeof(PaginatedResult<GetTradingTechnicsResult>), 200)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),403)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Produces("application/json")]
    [HttpGet(Name = "GetTradingTechnics")]
    public async Task<IActionResult> GetTradingTechnics([FromQuery] PaginationRequest request)
    {
        var result = await sender.Send(new GetTradingTechnicsQuery(request));
        
        return Ok(result);
    }
    
    #endregion

    #region GetTradingTechnicById
    
    /// <summary>
    /// Get Trading Technic By Id.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="404">The server cannot find the requested resource. The URL may be incorrect or the resource may not exist.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Get a single trading technic by id.
    /// ```
    /// Input:
    /// id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD1"
    /// ------------------------------------------
    /// Output:
    /// {
    ///     id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD1",
    ///     name: "plan name",
    ///     description: "description",
    ///     images: [
    ///         "pic1",
    ///         ...
    ///     ]
    /// },
    /// </remarks>
    [ProducesResponseType(typeof(GetTradingTechnicByIdResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),403)]
    [ProducesResponseType(typeof(ProblemDetails),404)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Produces("application/json")]
    [HttpGet("{id:guid}", Name = "GetTradingTechnicById")]
    public async Task<IActionResult> GetTradingTechnicById(Guid id)
    {
        var queryResponse = await sender.Send(new GetTradingTechnicByIdQuery
        {
            TradingTechnicId = id
        });

        return Ok(queryResponse);
    }
    
    #endregion
    
    #region GetTradingTechnicByName
    
    /// <summary>
    /// Get Trading Technic By Name.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Get trading technics by name.
    /// ```
    /// Input:
    /// name: "technic name"
    /// ------------------------------------------
    /// Output:
    /// [
    ///     {
    ///         id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD1",
    ///         name: "plan name",
    ///         description: "description",
    ///         images: [
    ///             "pic1",
    ///             ...
    ///         ]
    ///     },
    ///     ...
    /// ]
    /// </remarks>
    [ProducesResponseType(typeof(ICollection<GetTradingTechnicByNameResult>), 200)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),403)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Produces("application/json")]
    [HttpGet("{name}", Name = "GetTradingTechnicByName")]
    public async Task<IActionResult> GetTradingTechnicByName(string name)
    {
        var queryResponse = await sender.Send(new GetTradingTechnicByNameQuery
        {
            Name = name
        });

        return Ok(queryResponse);
    }
    
    #endregion

    #region CreateTradingTechnic
    
    /// <summary>
    /// Create trading technic.
    /// </summary>
    /// <response code="201">The trading technic has been created.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Create a new trading technic record.
    ///```
    /// Input:
    /// name: "plan name"
    /// description: "description"
    /// newImages: [
    ///     imageFile,
    ///     ...
    /// ]
    /// userId: "F4C5D951-7129-4CAD-8B04-79BB1C954AD1"
    /// ----------------------------
    /// Output:
    /// {
    ///     id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD4"
    /// }
    /// ```
    /// </remarks>
    [ProducesResponseType(typeof(CreateTradingTechnicResult), 201)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),403)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [HttpPost(Name = "CreateTradingTechnic")]
    public async Task<IActionResult> CreateTradingTechnic([FromForm] CreateTradingTechnicCommand request)
    {
        var response = await sender.Send(request);
        
        return CreatedAtRoute("GetTradingTechnicById", new { response.Id }, response);
    }
    
    #endregion
    
    #region UpdateTradingTechnic
    
    /// <summary>
    /// Update Trading Technic.
    /// </summary>
    /// <response code="204">The trading technic record has been successfully updated.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="404">The server cannot find the requested resource. The URL may be incorrect or the resource may not exist.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    ///<remarks>
    /// Update an existing trading technic.
    ///```
    /// Input:
    /// id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD4"
    /// name: "plan name"
    /// description: "description"
    /// newImages: [
    ///     imageFile,
    ///     ...
    /// ]
    /// images: [
    ///     "pic1",
    ///     ...
    /// ]
    /// removedImages: [
    ///     "pic2,
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
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [HttpPut("{id:guid}", Name = "UpdateTradingTechnic")]
    public async Task<IActionResult> UpdateTradingTechnic(Guid id, [FromForm] UpdateTradingTechnicCommand request)
    {
        if (id != request.Id)
        {
            throw new BadRequestException("The request Id does not match");
        }
        
        await sender.Send(request);

        return NoContent();
    }
    
    #endregion
    
    #region DeleteTradingTechnic
    
    /// <summary>
    /// Delete Trading Technic.
    /// </summary>
    /// <response code="204">The trading technic record has been successfully deleted.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="404">The server cannot find the requested resource. The URL may be incorrect or the resource may not exist.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    ///<remarks>
    /// Delete an existing trading technic record.
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
    [HttpDelete("{id:guid}", Name = "DeleteTradingTechnic")]
    public async Task<ActionResult> DeleteTradingTechnic(Guid id)
    {
        await sender.Send(new DeleteTradingTechnicCommand
        {
            TradingTechnicId = id
        });
        
        return NoContent();
    }
    
    #endregion
}