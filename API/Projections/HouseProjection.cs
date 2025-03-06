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
        return new House(@event.Id, @event.Data.HouseName , @event.Data.Address, @event.Data.NumberOfRooms, null);
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

    public async Task<House> Apply(UserRate @event, House house, IQuerySession session)
    {
        var events = await session.Events.FetchStreamAsync(house.Id);

        var stars = events
            .Where(e => e.EventType == typeof(UserRate))
            .Select(e => Convert.ToDecimal(((UserRate)e.Data).Rate))
            .Union([Convert.ToDecimal(@event.Rate)]);
        
        return house with
        {
            Stars = stars.Average(),
        };
    }
   
}