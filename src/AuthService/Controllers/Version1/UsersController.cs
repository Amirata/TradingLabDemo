using System.Collections.Immutable;
using System.Linq.Expressions;
using Asp.Versioning;
using AuthService.Data;
using AuthService.DataTransferObjects.Requests;
using AuthService.DataTransferObjects.Results;
using AuthService.Models;
using BuildingBlocks.Auth.Enums;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Extensions;
using BuildingBlocks.Pagination;
using BuildingBlocks.Utilities;
using Contracts.User;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controllers.Version1;

/// <summary>
/// User API - v1
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
[ApiVersion("1.0")]
public class UsersController(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ApplicationDbContext db,
    IPublishEndpoint publishEndpoint,
    ILogger<UsersController> logger)
    : ControllerBase
{

    #region GetUsers

    /// <summary>
    /// Get users.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Get a list of all user.
    /// ```
    /// Input:
    /// pageNumber: 1
    /// pageSize: 10
    /// sorts:[
    ///     {
    ///         sortBy: "UserName",
    ///         sortOrder: "asc",
    ///         order: 1
    ///     },
    /// ]
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
    ///             userName: "user name",
    ///             email: "email@gmail.com",
    ///         },
    ///         ...
    ///     ]
    /// }
    /// ```
    /// </remarks>
    [ProducesResponseType(typeof(PaginatedResult<GetUserResult>), 200)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),403)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Produces("application/json")]
    [Authorize(Policy = nameof(Policies.AdminOrUser))]
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] PaginationRequest request)
    {
        
        var query = db.Users
            .AsTracking()
            .AsQueryable()
            .Where(u=>u.UserName != "admin");

        
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(s => EF.Functions.Like(s.UserName, $"%{search}%"));
           
        }

        if (request.Sorts?.Any() == true)
        {
            var sortingExpressions = request.Sorts.Select(sort => (
                    sort.SortBy switch
                    {
                        nameof(ApplicationUser.UserName) => (Expression<Func<ApplicationUser, object>>)(p => p.UserName!),
                        _ => null
                    },
                    sort.SortOrder != null && sort.SortOrder.Equals("asc", StringComparison.OrdinalIgnoreCase))
                )
                .Where(t => t.Item1 != null)
                .ToList();

            query = query.OrderByDynamic(sortingExpressions.ToImmutableArray());
        }

        var model =  await query.Select(u=>new GetUserResult
        {
            Id = u.Id,
            UserName = u.UserName!,
            Email = u.Email!,
        }).ToPaginatedListAsync(request.PageNumber,
            request.PageSize, CancellationToken.None);
        
        return Ok(model);
    }

    #endregion
    
    #region GetUserById

    /// <summary>
    /// Get User By Id.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="404">The server cannot find the requested resource. The URL may be incorrect or the resource may not exist.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Get a single user by Id.
    /// ```
    /// Input:
    /// id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD1"
    /// ------------------------------------------
    /// Output:
    /// {
    ///     id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD1",
    ///     userName: "user name",
    ///     email: "email@gmail.com",
    /// }
    /// </remarks>
    [ProducesResponseType(typeof(GetUserResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),403)]
    [ProducesResponseType(typeof(ProblemDetails),404)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Produces("application/json")]
    [Authorize(Policy = nameof(Policies.AdminOrUser))]
    [HttpGet("{id:guid}", Name="GetUserById")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound("User not found");
        }

        return Ok(new GetUserResult
        {
            Id = user.Id,
            UserName = user.UserName!,
            Email = user.Email!
        });
    }

    #endregion

    #region CreateUser

    /// <summary>
    /// Create User.
    /// </summary>
    /// <response code="201">The user has been created.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Create a new user.
    ///```
    /// Input:
    /// userName: "user name"
    /// email: "admin@gmail.com"
    /// password: "pass123"
    /// role: 1 //only 1
    /// ----------------------------
    /// Output:
    /// {
    ///     id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD4"
    /// }
    /// ```
    /// </remarks>
    [ProducesResponseType(typeof(CreateResultId<string>), 201)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),403)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Produces("application/json")]
    [Authorize(Policy = nameof(Policies.AdminOnly))]
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] RegisterUserModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        if (model.Role == Roles.Admin)
        {
            return BadRequest("Admin already exists and cannot be created");
        }

        if (!await roleManager.RoleExistsAsync(model.Role.ToString()))
        {
            var role = new IdentityRole(model.Role.ToString());
            var roleResult = await roleManager.CreateAsync(role);
            if (roleResult.Succeeded == false)
            {
                return BadRequest(roleResult.Errors.Select(s => s.Description));
            }
        }

        var existingUser = await userManager.FindByNameAsync(model.UserName);
        if (existingUser != null)
        {
            return BadRequest("User already exists");
        }

        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        await userManager.AddToRoleAsync(user, model.Role.ToString());
        await publishEndpoint.Publish(new UserCreated
        {
            UserName = user.UserName,
            Id = Guid.Parse(user.Id)
        });
        await db.SaveChangesAsync();
        
        var response = new CreateResultId<string>
        {
            Id = user.Id,
        };
        
        return CreatedAtRoute("GetUserById", new { response.Id }, response);
    }

    #endregion
    
    #region UpdateUser

    /// <summary>
    /// Update User.
    /// </summary>
    /// <response code="204">The user record has been successfully updated.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="404">The server cannot find the requested resource. The URL may be incorrect or the resource may not exist.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    ///<remarks>
    /// Update an existing user.
    ///```
    /// Input:
    /// id: "F4C5D951-7129-4CAD-8B04-79BB1C954AD4"
    /// userName: "user name"
    /// email: "admin@gmail.com"
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
    [Authorize(Policy = nameof(Policies.AdminOnly))]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id,[FromBody] UpdateUserModel model)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound("User not found");
        }
        
        if (user.UserName == "admin")
        {
            return BadRequest("Admin can not be updated");
        }
        
        user.Email = model.Email;
        user.UserName = model.UserName;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        await publishEndpoint.Publish(new UserUpdated
        {
            Id = Guid.Parse(user.Id),
            UserName = user.UserName!
        });
        await db.SaveChangesAsync();
        
        return NoContent();
    }

    #endregion

    #region DeleteUser

    /// <summary>
    /// Delete User.
    /// </summary>
    /// <response code="204">The user record has been successfully deleted.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="403">The server understands the request, but the client does not have permission to access the resource.</response>
    /// <response code="404">The server cannot find the requested resource. The URL may be incorrect or the resource may not exist.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    ///<remarks>
    /// Delete an existing user record.
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
    [Authorize(Policy = nameof(Policies.AdminOnly))]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound("User not found");
        }
        
        if (user.UserName == "admin")
        {
            return BadRequest("Admin can not be deleted");
        }

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            return BadRequest(result.Errors);
        await publishEndpoint.Publish(new UserDeleted
        {
            Id = Guid.Parse(user.Id)
        });
        await db.SaveChangesAsync();
        
        return NoContent();
    }

    #endregion
}