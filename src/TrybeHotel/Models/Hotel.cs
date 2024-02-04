namespace TrybeHotel.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// 1. Implemente as models  da aplicação
public class Hotel
{
  [Key]
  public int HotelId { get; set; }
  public string? Name { get; set; }
  public string? Address { get; set; }

  [ForeignKey("City")]  // Corrigido para usar o nome da propriedade de navegação
  public int CityId { get; set; }
  public virtual City? City { get; set; }  // Adicionado para representar a relação de navegação
  public virtual ICollection<Room>? Rooms { get; set; }
}