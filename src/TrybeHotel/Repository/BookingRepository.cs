using TrybeHotel.Models;
using TrybeHotel.Dto;
using Microsoft.EntityFrameworkCore;

namespace TrybeHotel.Repository
{
    public class BookingRepository : IBookingRepository
    {
        protected readonly ITrybeHotelContext _context;
        public BookingRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 9. Refatore o endpoint POST /booking
        public BookingResponse Add(BookingDtoInsert booking, string email)
        {
            var room = GetRoomById(booking.RoomId);

            var bookingEntity = new Booking
            {
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                Room = room
            };

            _context.Bookings.Add(bookingEntity);
            _context.SaveChanges();
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            var hotel = _context.Hotels.FirstOrDefault(h => h.HotelId == room.HotelId);
            var city = _context.Cities.FirstOrDefault(c => c.CityId == hotel!.CityId);

            return new BookingResponse
            {
                BookingId = bookingEntity.BookingId,
                CheckIn = bookingEntity.CheckIn,
                CheckOut = bookingEntity.CheckOut,
                GuestQuant = bookingEntity.GuestQuant,
                Room = new RoomDto
                {
                    RoomId = room!.RoomId,
                    Name = room.Name,
                    Capacity = room.Capacity,
                    Image = room.Image,
                    Hotel = new HotelDto
                    {
                        HotelId = room.Hotel!.HotelId,
                        Name = room.Hotel.Name,
                        Address = room.Hotel.Address,
                        CityId = room.Hotel.CityId,
                        CityName = room.Hotel.City!.Name,
                        State = room.Hotel.City!.State
                    },
                }
            };
        }

        // 10. Refatore o endpoint GET /booking
        public BookingResponse GetBooking(int bookingId, string email)
        {
            var booking = _context.Bookings.Include(u => u.User).FirstOrDefault(b => b.BookingId == bookingId);
            if (booking!.User!.Email != email)
            {
                return null!;
            }
            var createdBooking = _context.Bookings.Include(r => r.Room)
            .ThenInclude(h => h!.Hotel)
            .ThenInclude(c => c!.City)
            .FirstOrDefault(b => b.BookingId == bookingId);
            var bookingResponse = new BookingResponse
            {
                BookingId = booking.BookingId,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                Room = new RoomDto
                {
                    RoomId = booking.RoomId,
                    Name = booking!.Room!.Name,
                    Capacity = booking.Room.Capacity,
                    Image = booking.Room.Image,
                    Hotel = new HotelDto
                    {
                        HotelId = booking.Room.Hotel!.HotelId,
                        Name = booking.Room.Hotel.Name,
                        Address = booking.Room.Hotel.Address,
                        CityId = booking.Room.Hotel.CityId,
                        CityName = booking.Room.Hotel.City!.Name,
                        State = booking.Room.Hotel.City!.State
                    }
                }
            };
            return bookingResponse;
        }

        public Room GetRoomById(int RoomId)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == RoomId);
            return room!;
        }

    }

}