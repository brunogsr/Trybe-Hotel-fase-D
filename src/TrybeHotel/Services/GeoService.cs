using System.Net.Http;
using TrybeHotel.Dto;
using TrybeHotel.Repository;

namespace TrybeHotel.Services
{
    public class GeoService : IGeoService
    {
        private readonly HttpClient _client;
        public GeoService(HttpClient client)
        {
            _client = client;
        }

        // 11. Desenvolva o endpoint GET /geo/status
        public async Task<object> GetGeoStatus()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://nominatim.openstreetmap.org/status.php?format=json/");
            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.Headers.Add("User-Agent", "aspnet-user-agent");
            var response = await _client.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<object>();
                return content!;
            }
            return null!;
        }

        // 12. Desenvolva o endpoint GET /geo/address
        public async Task<GeoDtoResponse> GetGeoLocation(GeoDto geoDto)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://nominatim.openstreetmap.org/search?street={geoDto.Address}&city={geoDto.City}s&country=Brazil&state={geoDto.State}&format=json&limit=1");

            var response = await _client.SendAsync(request);
            response.Headers.Add("Accept", "application/json");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<List<GeoDtoResponse>>();
                if (content == null)
                {
                    return null!;
                }

                return new GeoDtoResponse
                {
                    lat = content[0].lat,
                    lon = content[0].lon
                };
            }
            return null!;
        }

        // 12. Desenvolva o endpoint GET /geo/address
        public async Task<List<GeoDtoHotelResponse>> GetHotelsByGeo(GeoDto geoDto, IHotelRepository repository)
        {
            var geoLocation = await GetGeoLocation(geoDto);
            if (geoLocation == null)
            {
                return new List<GeoDtoHotelResponse>();
            }

            var hotels = repository.GetHotels();
            if (hotels == null)
            {
                return new List<GeoDtoHotelResponse>();
            }

            var hotelsByGeo = new List<GeoDtoHotelResponse>();

            foreach (var hotel in hotels)
            {
                try
                {
                    var distance = await GetGeoLocation(new GeoDto { Address = hotel.Address, City = hotel.CityName, State = hotel.State });
                    if (distance == null)
                    {
                        // Se a distância não puder ser calculada, continue para o próximo hotel
                        continue;
                    }

                    hotelsByGeo.Add(new GeoDtoHotelResponse
                    {
                        HotelId = hotel.HotelId,
                        Name = hotel.Name,
                        Address = hotel.Address,
                        CityName = hotel.CityName,
                        State = hotel.State,
                        Distance = CalculateDistance(geoLocation!.lat!, geoLocation!.lon!, distance!.lat!, distance.lon!)
                    });
                }
                catch (Exception)
                {
                    return null!;
                }
            }

            var hotelsByGeoOrdered = hotelsByGeo.OrderBy(h => h.Distance).ToList();
            return hotelsByGeoOrdered;
        }


        public int CalculateDistance(string latitudeOrigin, string longitudeOrigin, string latitudeDestiny, string longitudeDestiny)
        {
            double latOrigin = double.Parse(latitudeOrigin.Replace('.', ','));
            double lonOrigin = double.Parse(longitudeOrigin.Replace('.', ','));
            double latDestiny = double.Parse(latitudeDestiny.Replace('.', ','));
            double lonDestiny = double.Parse(longitudeDestiny.Replace('.', ','));
            double R = 6371;
            double dLat = radiano(latDestiny - latOrigin);
            double dLon = radiano(lonDestiny - lonOrigin);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(radiano(latOrigin)) * Math.Cos(radiano(latDestiny)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;
            return int.Parse(Math.Round(distance, 0).ToString());
        }

        public double radiano(double degree)
        {
            return degree * Math.PI / 180;
        }

    }
}