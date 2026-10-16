
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay_.Api.Data;
using RaceDay_.Api.Dtos;
using RaceDay_.Api.Filters;
using RaceDay_.Api.Models;

namespace RaceDay_.Api.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly RaceDayDbContext _context;

    public EventsController(RaceDayDbContext context)
    {
        _context = context;
    }

    // Both authenticated roles can view all events.
    [HttpGet]
    [SessionAuthorize]
    public async Task<IActionResult> GetEvents()
    {
        var events = await _context.Events
            .AsNoTracking()
            .OrderBy(e => e.EventDate)
            .Select(e => new
            {
                e.EventId,
                e.OrganiserId,
                e.EventName,
                e.Description,
                e.EventType,
                e.DistanceKm,
                e.EventDate,
                e.Venue,
                e.City,
                e.Province,
                e.RegistrationOpenDate,
                e.RegistrationCloseDate,
                e.MaxParticipants,
                e.CreatedAt
            })
            .ToListAsync();

        return Ok(events);
    }

    // Both authenticated roles can view an individual event.
    [HttpGet("{id:int}")]
    [SessionAuthorize]
    public async Task<IActionResult> GetEvent(int id)
    {
        var eventItem = await _context.Events
            .AsNoTracking()
            .Where(e => e.EventId == id)
            .Select(e => new
            {
                e.EventId,
                e.OrganiserId,
                e.EventName,
                e.Description,
                e.EventType,
                e.DistanceKm,
                e.EventDate,
                e.Venue,
                e.City,
                e.Province,
                e.RegistrationOpenDate,
                e.RegistrationCloseDate,
                e.MaxParticipants,
                e.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (eventItem == null)
        {
            return NotFound(new
            {
                message = "Event not found."
            });
        }

        return Ok(eventItem);
    }

    // Only an Organiser can create an event.
    [HttpPost]
    [SessionAuthorize("Organiser")]
    public async Task<IActionResult> CreateEvent(
        [FromBody] EventRequest request)
    {
        if (request.RegistrationOpenDate >= request.RegistrationCloseDate ||
            request.RegistrationCloseDate > request.EventDate)
        {
            return BadRequest(new
            {
                message = "Registration must open before it closes, and registration must close on or before the event date."
            });
        }

        var organiserId = HttpContext.Session.GetInt32("UserId");

        if (organiserId == null)
        {
            return Unauthorized(new
            {
                message = "Authentication is required."
            });
        }

        var eventItem = new Event
        {
            OrganiserId = organiserId.Value,
            EventName = request.EventName.Trim(),
            Description = request.Description?.Trim(),
            EventType = request.EventType,
            DistanceKm = request.DistanceKm,
            EventDate = request.EventDate,
            Venue = request.Venue.Trim(),
            City = request.City.Trim(),
            Province = request.Province.Trim(),
            RegistrationOpenDate = request.RegistrationOpenDate,
            RegistrationCloseDate = request.RegistrationCloseDate,
            MaxParticipants = request.MaxParticipants,
            CreatedAt = DateTime.UtcNow
        };

        _context.Events.Add(eventItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetEvent),
            new { id = eventItem.EventId },
            new
            {
                eventItem.EventId,
                eventItem.OrganiserId,
                eventItem.EventName,
                eventItem.Description,
                eventItem.EventType,
                eventItem.DistanceKm,
                eventItem.EventDate,
                eventItem.Venue,
                eventItem.City,
                eventItem.Province,
                eventItem.RegistrationOpenDate,
                eventItem.RegistrationCloseDate,
                eventItem.MaxParticipants,
                eventItem.CreatedAt
            });
    }

    // Only the Organiser who owns the event can update it.
    [HttpPut("{id:int}")]
    [SessionAuthorize("Organiser")]
    public async Task<IActionResult> UpdateEvent(
        int id,
        [FromBody] EventRequest request)
    {
        var organiserId = HttpContext.Session.GetInt32("UserId");

        var eventItem = await _context.Events
            .FirstOrDefaultAsync(e =>
                e.EventId == id &&
                e.OrganiserId == organiserId);

        if (eventItem == null)
        {
            return NotFound(new
            {
                message = "Event not found or you do not own this event."
            });
        }

        if (request.RegistrationOpenDate >= request.RegistrationCloseDate ||
            request.RegistrationCloseDate > request.EventDate)
        {
            return BadRequest(new
            {
                message = "Registration must open before it closes, and registration must close on or before the event date."
            });
        }

        eventItem.EventName = request.EventName.Trim();
        eventItem.Description = request.Description?.Trim();
        eventItem.EventType = request.EventType;
        eventItem.DistanceKm = request.DistanceKm;
        eventItem.EventDate = request.EventDate;
        eventItem.Venue = request.Venue.Trim();
        eventItem.City = request.City.Trim();
        eventItem.Province = request.Province.Trim();
        eventItem.RegistrationOpenDate = request.RegistrationOpenDate;
        eventItem.RegistrationCloseDate = request.RegistrationCloseDate;
        eventItem.MaxParticipants = request.MaxParticipants;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Event updated successfully."
        });
    }

    // Only the Organiser who owns the event can delete it.
    [HttpDelete("{id:int}")]
    [SessionAuthorize("Organiser")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var organiserId = HttpContext.Session.GetInt32("UserId");

        var eventItem = await _context.Events
            .FirstOrDefaultAsync(e =>
                e.EventId == id &&
                e.OrganiserId == organiserId);

        if (eventItem == null)
        {
            return NotFound(new
            {
                message = "Event not found or you do not own this event."
            });
        }

        _context.Events.Remove(eventItem);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                message = "This event cannot be deleted because related records exist. Remove or manage those records first."
            });
        }

        return NoContent();
    }
}
