using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
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

    public async Task<IActionResult> AssingOrAddClientToTrip(int idTrip,NewDataDTO newDataDto)
    {
        //payment i nameTrip do tripa
        var now = DateTime.Now;
        var actualClient = await _context.Clients.FirstOrDefaultAsync(c => c.Pesel == newDataDto.Pesel);

       

        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.IdTrip == idTrip);

        //czy wycieczka istnieje
        if (trip == null)
        {
            return NotFound("Nie ma takiej wycieczki w bazie.");
        }
        
        //czy data wycieczki jest w przeszlosci? jesli nie to error
        if (trip.DateFrom < DateTime.Today)
        {
            return BadRequest("Wycieczka która chcesz odbyć już miała miejsce.");
        }
        
        //Czy klient o danym numerze PESEL istnieje. Jeśli nie, dodajemy go do bazy danych.
        //czlowiek z tym peselem istnieje
        if (actualClient == null )
        {
            //dodajemy do bazy skoro nie ma goscia
            var newClient = new Client
            {
                FirstName = newDataDto.FirstName,
                LastName = newDataDto.LastName,
                Email = newDataDto.Email,
                Telephone = newDataDto.Telephone,
                Pesel = newDataDto.Pesel,
                
            };

            await _context.Clients.AddAsync(newClient);
            await _context.SaveChangesAsync();

            actualClient = newClient;
        }
        
        

        var acutalClientTrip = await _context.ClientTrips
            .FirstOrDefaultAsync(ct => ct.IdClient == actualClient.IdClient && ct.IdTrip == idTrip);
        
        //jesli klient ma juz przypisana wycieczke to wyrzucamy blad
        if (acutalClientTrip != null)
        {
            return BadRequest("Klient z tym numerem pesel już jest zapisany na wycieczke.");
        }
        //nie jest zapisany na wycieczke wiec mu ja dodajemy
        var clientTrip = new ClientTrip
        {
            IdClient = actualClient.IdClient,
            IdTrip = idTrip,
            RegisteredAt = now ,
            PaymentDate = newDataDto.PaymentDate,
        };
        await _context.ClientTrips.AddAsync(clientTrip);
        await _context.SaveChangesAsync();
        
        return Ok("Klient został przypisany do wycieczki");

    }
}