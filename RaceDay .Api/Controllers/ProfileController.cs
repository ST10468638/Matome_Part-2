using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_.Api.Data;
using RaceDay_.Api.Dtos;
using RaceDay_.Api.Filters;

namespace RaceDay_.Api.Controllers;

[ApiController]
[Route("api/profile")]
[SessionAuthorize]
public class ProfileController : ControllerBase
{
    private readonly RaceDayDbContext _context;

    public ProfileController(RaceDayDbContext context)
    {
        _context = context;
    }

    // An authenticated user can view only their own profile.
    [HttpGet]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return Unauthorized(new
            {
                message = "Authentication is required."
            });
        }

        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.UserId == userId.Value)
            .Select(u => new
            {
                u.UserId,
                u.FirstName,
                u.LastName,
                u.Email,
                u.PhoneNumber,
                u.Role,
                u.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            HttpContext.Session.Clear();

            return Unauthorized(new
            {
                message = "The account associated with this session no longer exists."
            });
        }

        return Ok(user);
    }

    // Users may update their own details but not their role or password.
    [HttpPut]
    public async Task<IActionResult> UpdateMyProfile(
        [FromBody] UpdateProfileRequest request)
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            return Unauthorized(new
            {
                message = "Authentication is required."
            });
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId.Value);

        if (user == null)
        {
            HttpContext.Session.Clear();

            return Unauthorized(new
            {
                message = "The account associated with this session no longer exists."
            });
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var emailInUse = await _context.Users.AnyAsync(u =>
            u.UserId != userId.Value &&
            u.Email == normalizedEmail);

        if (emailInUse)
        {
            return Conflict(new
            {
                message = "That email address is already in use."
            });
        }

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        user.Email = normalizedEmail;
        user.PhoneNumber = request.PhoneNumber?.Trim();

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "The profile could not be updated. Check whether the email address is already in use."
            });
        }

        HttpContext.Session.SetString("Email", user.Email);

        return Ok(new
        {
            message = "Profile updated successfully.",
            user.UserId,
            user.FirstName,
            user.LastName,
            user.Email,
            user.PhoneNumber,
            user.Role,
            user.CreatedAt
        });
    }
}