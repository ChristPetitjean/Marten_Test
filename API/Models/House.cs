namespace API.Models;

public record House(Guid Id, string Name, string Address, int NumberOfRooms, decimal? Stars);