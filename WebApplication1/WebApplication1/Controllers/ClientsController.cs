using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly ApbdContext _context;
    public ClientsController(ApbdContext context)
    {
        _context = context;
    }

    [HttpDelete]
    [Route("{idClient}")]
    public async Task<IActionResult> DeleteClient(int idClient)
    {
        var allTrips = await _context.ClientTrips.CountAsync(ct => ct.IdClient == idClient);
        if (allTrips > 0)
        {
            return Conflict($"Client with choosen id: {idClient} has assigned trips");
        }

        var clientToRemove = await _context.Clients.FindAsync(idClient);

        if (clientToRemove != null)
        {
            _context.Clients.Remove(clientToRemove);
            await _context.SaveChangesAsync();
        }else
        {
            return NotFound($"Client with given id: {idClient} does not exist");
        }
        
        
        return NoContent();
    }
    



}