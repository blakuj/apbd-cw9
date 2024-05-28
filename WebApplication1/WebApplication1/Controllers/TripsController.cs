using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models.DTO;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TripsController : ControllerBase
{
    private readonly ApbdContext _context;
    public TripsController(ApbdContext context)
    {
        _context = context;
    }


    [HttpGet]
    public async Task<IActionResult> GetTrips(int page=1, int pageSize =10)
    {
        
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;
        
        var totalTrips = await _context.Trips.CountAsync();
        var allPages = (int) Math.Ceiling(totalTrips / (double)pageSize);
        
        
        var trips = await _context.Trips
            .OrderByDescending(e => e.DateFrom)
            
            //ile elementow ma skipnac, elementem jest caly obiekt RESPONSE
            .Skip((page-1)*pageSize)
            
            //pobiera pierwsze n elementow juz przefiltrowanych przez skipa
            .Take(pageSize)
            
            .Select(e => new
        {
            Name = e.Name,
            Description = e.Description,
            DateFrom = e.DateFrom,
            DateTo = e.DateTo,
            MaxPeaople = e.MaxPeople,
            
            Countries =  e.IdCountries.Select(c =>new {Name = c.Name}),
            
            Clients = e.ClientTrips.Select(ct=> new {FirstName = ct.IdClientNavigation.FirstName, LastName = ct.IdClientNavigation.LastName})
           

        })
            .ToListAsync();

        var response = new
        {
            pageNum = page,
            pageSize = pageSize,
            allPages = allPages,
            trips = trips
        };
        
        
        
        return Ok(response);
    }


    [HttpPost]
    [Route("{idTrip}/Clients")]

    public async Task<IActionResult> AssingClientToTrip(ClientsDTO clientsDto)
    {


        return Ok();

    }
}