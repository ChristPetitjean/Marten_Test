using API.Events;
using API.Models;
using Marten;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Marten.Services;

namespace API.Projections;

public class HouseProjection : SingleStreamProjection<House>
{
    public House Create(IEvent<NewHouseEnrolled> @event)
    {
        return new House(@event.Id, @event.Data.HouseName , @event.Data.Address, @event.Data.NumberOfRooms, null, 0);
    }

    public House Apply(HouseRenamed @event, House house) => 
        house with
        {
            Name = @event.Name,
        };
    
    public House Apply(AddressChanged @event, House house) => 
        house with
        {
            Address = @event.Address,
        };
    
    public House Apply(RoomsClosed @event, House house) => 
        house with
        {
            NumberOfRooms = house.NumberOfRooms - @event.Number,
        };
    
    public House Apply(RoomsOpened @event, House house) => 
        house with
        {
            NumberOfRooms = house.NumberOfRooms + @event.Number,
        };

    public House Apply(UserRate @event, House house)
    {
        var average = ((house.Rate ?? 0) * house.NumberOfRates + @event.Rate) / (house.NumberOfRates + 1.0m);
        return house with
        {
            Rate = average,
            NumberOfRates = house.NumberOfRates + 1
        };
    }
   
}