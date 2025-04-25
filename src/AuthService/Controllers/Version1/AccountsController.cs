using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Asp.Versioning;
using AuthService.Data;
using AuthService.DataTransferObjects.Requests;
using AuthService.DataTransferObjects.Results;
using AuthService.Models;
using BuildingBlocks.Auth.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Controllers.Version1;

/// <summary>
/// Account API - v1
/// </summary>
[ApiController]
[Authorize(Policy = nameof(Policies.AdminOrUser))]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
[ApiVersion("1.0")]
public class AccountsController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ApplicationDbContext db,
    IPasswordHasher<ApplicationUser> passwordHasher,
    ILogger<AccountsController> logger,
    IConfiguration configuration)
    : ControllerBase
{

    #region Login

    /// <summary>
    /// Login.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Login user.
    ///```
    /// Input:
    /// userName: "user name"
    /// password: "pass123"
    /// ----------------------------
    /// Output:
    /// {
    ///     token: "token",
    ///     refreshToken: "refresh token"
    /// }
    /// ```
    /// </remarks>
    [ProducesResponseType(typeof(LoginResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Produces("application/json")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var user = await userManager.FindByNameAsync(model.UserName);
        if (user == null)
            return Unauthorized("Invalid credentials");

        var checkPassword = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!checkPassword.Succeeded)
            return Unauthorized("Invalid credentials");
        var roles = await userManager.GetRolesAsync(user);
       
        var token = GenerateJwtTokenAsync(user, roles);
        var refreshToken = await GenerateRefreshTokenAsync(user);

        return Ok(new LoginResult
        {
            Token = token,
            RefreshToken = refreshToken
        });
    }

    #endregion

    #region Logout

    /// <summary>
    /// Logout.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Logout user.
    ///```
    /// Input:
    /// refreshToken: "refresh token"
    /// ----------------------------
    /// Output:
    /// {
    ///     message: "message"
    /// }
    /// ```
    /// </remarks>
    [ProducesResponseType(typeof(MessageResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Consumes("application/json")]
    [Produces("application/json")]
    [HttpPost]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest logoutRequest)
    {
        if (string.IsNullOrWhiteSpace(logoutRequest.RefreshToken))
        {
            return BadRequest(new { message = "Refresh token is required." });
        }
        
        var storedRefreshToken = await db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == logoutRequest.RefreshToken);

        if (storedRefreshToken == null)
        {
            return Ok(new { message = "User logged out successfully." });
        }
        
        db.RefreshTokens.Remove(storedRefreshToken);
        
        await db.SaveChangesAsync();
        
        return Ok(new MessageResult { Message = "User logged out successfully." });
    }

    #endregion

    #region LogoutAll

    /// <summary>
    /// Logout All.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Logout All user sessions.
    ///```
    /// Input:
    /// ----------------------------
    /// Output:
    /// {
    ///     message: "message"
    /// }
    /// ```
    /// </remarks>
    [ProducesResponseType(typeof(MessageResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Consumes("application/json")]
    [Produces("application/json")]
    [HttpPost]
    public async Task<IActionResult> LogoutAll()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Unauthorized();
        }

        var tokens = db.RefreshTokens.Where(rt => rt.UserId == user.Id);
        db.RefreshTokens.RemoveRange(tokens);

        await db.SaveChangesAsync();

        return Ok(new MessageResult{ Message = "All sessions for this user have been logged out."});
    }

    #endregion

    #region Refresh

    /// <summary>
    /// Refresh.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Refresh token and get new one.
    ///```
    /// Input:
    /// refreshToken: "refresh token"
    /// ----------------------------
    /// Output:
    /// {
    ///     token: "token",
    ///     refreshToken: "refresh token"
    /// }
    /// ```
    /// </remarks>
    [ProducesResponseType(typeof(RefreshResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Consumes("application/json")]
    [Produces("application/json")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest model)
    {
        if (string.IsNullOrEmpty(model.RefreshToken))
            return BadRequest("No refresh token provided");
        
        var storedToken = await db.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == model.RefreshToken);

        if (storedToken == null
            || storedToken.IsRevoked
            || storedToken.ExpiresAt < DateTime.UtcNow)
        {
            return BadRequest("Invalid or expired refresh token");
        }
        
        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        db.RefreshTokens.Update(storedToken);
        await db.SaveChangesAsync();

        var roles = await userManager.GetRolesAsync(storedToken.User);
        
        var token = GenerateJwtTokenAsync(storedToken.User, roles);
        var newRefreshToken = await GenerateRefreshTokenAsync(storedToken.User);

        return Ok(new RefreshResult{ Token = token, RefreshToken = newRefreshToken });
    }

    #endregion

    #region GetUser

    /// <summary>
    /// Get User.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="404">The server cannot find the requested resource. The URL may be incorrect or the resource may not exist.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Get user data.
    ///```
    /// Input:
    /// ----------------------------
    /// Output:
    /// {
    ///     id: "7F095688-51BB-4E86-8308-69F4BC7BBBA0",
    ///     UserName: "user name",
    ///     email: "email@gmail.com"
    /// }
    /// ```
    /// </remarks>
    [ProducesResponseType(typeof(RefreshResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),404)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Consumes("application/json")]
    [Produces("application/json")]
    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized("User ID not found in token");
        }

        var user = await userManager.FindByIdAsync(userId);
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

    #region ChangePassword

    /// <summary>
    /// Change Password.
    /// </summary>
    /// <response code="200">Request was successful, and the server has returned the requested data.</response>
    /// <response code="400">The server could not understand the request due to invalid syntax.</response>
    /// <response code="401">Authentication is required, or the provided credentials are invalid.</response>
    /// <response code="500">The server encountered an unexpected condition that prevented it from fulfilling the request.</response>
    /// <remarks>
    /// Change a user password.
    ///```
    /// Input:
    /// oldPassword: "old password",
    /// newPassword: "new password",
    /// confirmPassword: "confirm password"
    /// ----------------------------
    /// Output:
    /// {
    ///     message: "message"
    /// }
    /// ```
    /// </remarks>
    [ProducesResponseType(typeof(MessageResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails),400)]
    [ProducesResponseType(typeof(ProblemDetails),401)]
    [ProducesResponseType(typeof(ProblemDetails),500)]
    [Consumes("application/json")]
    [Produces("application/json")]
    [HttpPost]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest("New password must not be empty.");
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                return BadRequest("New password and confirmation do not match.");
            }
            
            if (!IsPasswordComplexEnough(request.NewPassword))
            {
                return BadRequest("New password does not meet complexity requirements.");
            }
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("User is not properly authenticated.");
            }
            
            var isOldPasswordValid = await ValidateUserPasswordAsync(userId, request.OldPassword);
            if (!isOldPasswordValid)
            {
                return BadRequest("Old password is incorrect.");
            }
            
            var updatedSuccessfully = await UpdateUserPasswordAsync(userId, request.NewPassword);
            if (!updatedSuccessfully)
            {
                return StatusCode(500, "Error updating password. Please try again.");
            }
            
            logger.LogInformation("User {UserId} changed their password successfully.", userId);
            
            return Ok(new MessageResult{Message = "Password changed successfully."});
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error changing password.");
            return StatusCode(500, "Internal server error while changing password.");
        }
    }

    #endregion

    #region HelperFunctions

    private bool IsPasswordComplexEnough(string password)
    {
        var hasMinimumLength = password.Length >= 8;
        var hasUpperCase = false;
        var hasLowerCase = false;
        var hasDigit = false;
        var hasSpecialChar = false;

        foreach (var c in password)
        {
            if (char.IsUpper(c)) hasUpperCase = true;
            if (char.IsLower(c)) hasLowerCase = true;
            if (char.IsDigit(c)) hasDigit = true;
            if (!char.IsLetterOrDigit(c)) hasSpecialChar = true;
        }

        return hasMinimumLength && hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
    }

    private async Task<bool> ValidateUserPasswordAsync(string userId, string suppliedPassword)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return false;
        
        var isPasswordValid = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, suppliedPassword);
        return isPasswordValid == PasswordVerificationResult.Success;
    }

    private async Task<bool> UpdateUserPasswordAsync(string userId, string newPassword)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return false;
        
        var newHashedPassword = passwordHasher.HashPassword(user, newPassword);
        user.PasswordHash = newHashedPassword;
        
        var result = await userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    private string GenerateJwtTokenAsync(ApplicationUser user, IList<string> roles)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = jwtSettings["Key"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expiresInMinutes = Convert.ToDouble(jwtSettings["ExpiresInMinutes"]);
        
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new("userName", user.UserName!)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
            signingCredentials: credentials);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> GenerateRefreshTokenAsync(ApplicationUser user)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var refreshTokenExpiresInDays = Convert.ToDouble(jwtSettings["RefreshTokenExpiresInDays"]);

        var randomNumber = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var refreshToken = Convert.ToBase64String(randomNumber);

        var newRefreshToken = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpiresInDays),
            IsRevoked = false
        };

        db.RefreshTokens.Add(newRefreshToken);
        await db.SaveChangesAsync();

        return refreshToken;
    }

    #endregion
    
}