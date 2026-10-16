using System.ComponentModel.DataAnnotations;

namespace RaceDay_.Api.Dtos;

public class EventRequest
{
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string EventName { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required]
    [RegularExpression("^(Running|Walking|Cycling)$",
        ErrorMessage = "EventType must be Running, Walking, or Cycling.")]
    public string EventType { get; set; } = string.Empty;

    [Required]
    public DateTime EventDate { get; set; }

    [Required]
    [StringLength(200)]
    public string Venue { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Province { get; set; } = string.Empty;

    [Required]
    public DateTime RegistrationOpenDate { get; set; }

    [Required]
    public DateTime RegistrationCloseDate { get; set; }

    [Range(1, 1000000)]
    public int? MaxParticipants { get; set; }
}