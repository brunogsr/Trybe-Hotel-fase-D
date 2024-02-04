using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class RoomRepository : IRoomRepository
    {
        protected readonly ITrybeHotelContext _context;
        public RoomRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 7. Refatore o endpoint GET /room
        public IEnumerable<RoomDto> GetRooms(int HotelId)
        {
            var rooms = from r in _context.Rooms
                        where r.HotelId == HotelId
                        select new RoomDto
                        {
                            RoomId = r.RoomId,
                            Name = r.Name,
                            Capacity = r.Capacity,
                            Image = r.Image,
                            Hotel = (from hotel in _context.Hotels
                                     where hotel.HotelId == HotelId
                                     select new HotelDto()
                                     {
                                         HotelId = hotel.HotelId,
                                         Name = hotel.Name,
                                         Address = hotel.Address,
                                         CityId = hotel.CityId,
                                         CityName = (from c in _context.Cities
                                                     where c.CityId == hotel.CityId
                                                     select c.Name).FirstOrDefault(),
                                         State = (from c in _context.Cities
                                                  where c.CityId == hotel.CityId
                                                  select c.State).FirstOrDefault()
                                     }).FirstOrDefault()
                        };
            return rooms;
        }

        // 8. Refatore o endpoint POST /room
        public RoomDto AddRoom(Room room)
        {
            _context.Rooms.Add(room);
            _context.SaveChanges();
            return new RoomDto
            {
                RoomId = room.RoomId,
                Name = room.Name,
                Capacity = room.Capacity,
                Image = room.Image,
                HotelId = room.HotelId,
                Hotel = (from hotel in _context.Hotels
                         where hotel.HotelId == room.HotelId
                         select new HotelDto()
                         {
                             HotelId = hotel.HotelId,
                             Name = hotel.Name,
                             Address = hotel.Address,
                             CityId = hotel.CityId,
                             CityName = (from c in _context.Cities
                                         where c.CityId == hotel.CityId
                                         select c.Name).FirstOrDefault(),
                             State = (from c in _context.Cities
                                      where c.CityId == hotel.CityId
                                      select c.State).FirstOrDefault()
                         }).FirstOrDefault()
            };
        }

        public void DeleteRoom(int RoomId)
        {
            var room = _context.Rooms.Find(RoomId);
            _context.Rooms.Remove(room!);
            _context.SaveChanges();
        }
    }
}