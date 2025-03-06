using API.Events;
using API.Models;
using Marten.Events;
using Marten.Events.Aggregation;
using Marten.Events.Projections;

namespace API.Projections;

public class HouseProjection : SingleStreamProjection<House>
{
    public House Create(IEvent<NewHouseEnroled> @event)
    {
        return new House(@event.Id, @event.Data.HouseName , @event.Data.Address, @event.Data.NumberOfRooms, 0);
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
    
    public House Apply(UserRate @event, House house) => 
        house with
        {
            Stars = @event.Rate,
        };
}