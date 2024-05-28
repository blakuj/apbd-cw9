using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripController : ControllerBase
{
    private readonly ApbdContext _context;
    public TripController(ApbdContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<IActionResult> GetTrips()
    {
        var trips = await _context.Trips.Select(e => new
        {
            Name = e.Name,
            Description = e.Description,
            DateFrom = e.DateFrom,
            DateTo = e.DateTo,
            MaxPeaople = e.MaxPeople,
            
            Countries =  e.IdCountries.Select(c =>new {Name = c.Name}),
            
            Clients = e.ClientTrips.Select(ct=> new {FirstName = ct.IdClientNavigation.FirstName, LastName = ct.IdClientNavigation.LastName})
           

        }).ToListAsync();

        return Ok(trips);
    }
}