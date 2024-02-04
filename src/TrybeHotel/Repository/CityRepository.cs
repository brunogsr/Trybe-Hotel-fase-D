using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class CityRepository : ICityRepository
    {
        protected readonly ITrybeHotelContext _context;
        public CityRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 4. Refatore o endpoint GET /city
        public IEnumerable<CityDto> GetCities()
        {
            var citiesList = _context.Cities.ToList();
            var citiesDtoList = new List<CityDto>();
            foreach (var city in citiesList)
            {
                citiesDtoList.Add(new CityDto
                {
                    CityId = city.CityId,
                    Name = city.Name,
                    State = city.State
                });
            }
            return citiesDtoList;
        }

        // 2. Refatore o endpoint POST /city
        public CityDto AddCity(City city)
        {
            var newCity = _context.Cities.Add(city);
            _context.SaveChanges();
            return new CityDto
            {
                CityId = newCity.Entity.CityId,
                Name = newCity.Entity.Name,
                State = newCity.Entity.State
            };
        }

        // 3. Desenvolva o endpoint PUT /city
        public CityDto UpdateCity(City city)
        {
            var cityToUpdate = _context.Cities.Find(city.CityId);
            cityToUpdate!.Name = city.Name;
            cityToUpdate.State = city.State;
            _context.SaveChanges();
            return new CityDto
            {
                CityId = cityToUpdate.CityId,
                Name = cityToUpdate.Name,
                State = cityToUpdate.State
            };
        }

    }
}