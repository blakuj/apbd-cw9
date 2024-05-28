using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.DTO;

public class ClientsDTO
{
    [MaxLength(120)]
    public String FirstName { get; set; }
    [MaxLength(120)]
    public String LastName { get; set; }
    [MaxLength(120)]
    [EmailAddress]
    public String Email { get; set; }
    [MaxLength(12)]
    [Phone]
    public String Telephone { get; set; }
    [MaxLength(11)]
    public String Pesel { get; set; }
    [MaxLength(120)]
    public int idTrip { get; set; }
    [MaxLength(120)]
    public DateTime PaymentDate { get; set; }
    
}