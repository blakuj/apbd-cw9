using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.DTO;

public class NewDataDTO
{
    [MaxLength(120)]
    public String FirstName { get; set; }
    [MaxLength(120)]
    public String LastName { get; set; }
    [MaxLength(120)]
    [EmailAddress]
    public String Email { get; set; }
    
    [MaxLength(12)]
    public String Telephone { get; set; }
    [MaxLength(11)]
    public String Pesel { get; set; } 
    public DateTime? PaymentDate { get; set; }
    
}