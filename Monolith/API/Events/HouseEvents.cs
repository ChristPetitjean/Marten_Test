namespace API.Events;

public sealed record NewHouseEnrolled(string HouseName, string Address, int NumberOfRooms);
public sealed record HouseRenamed(string Name);
public sealed record AddressChanged(string Address);
public sealed record RoomsClosed(int Number);
public sealed record RoomsOpened(int Number);
public sealed record UserRate(decimal Rate);