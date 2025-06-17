using Newtonsoft.Json;

namespace DevHabit.Api.DTOs.Habits;

public sealed record HabitWithTagsDto : HabitDto
{
    [JsonProperty(Order = int.MaxValue)] //agar Tags dibawah
    public required string[] Tags { get; init; }
}
