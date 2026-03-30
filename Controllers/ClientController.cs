using LearningCache.Data;
using LearningCache.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace LearningCache.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientController(DataContext context) : ControllerBase
{
    [HttpGet("nocache")]
    public async Task<ActionResult<Client>> GetClientWithoutCache(
        [FromRoute] string firstName
    )
    {
        var clientFromContext = await context
            .Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(clt => clt.Name == firstName);

        if (clientFromContext is not null)
            return Ok(clientFromContext);
        
        return NotFound(null);
    }
    
    [HttpGet("cache")]
    public async Task<ActionResult<Client>> GetClientWithCache(
        [FromServices] IMemoryCache cache,
        [FromRoute] string firstName
    )
    {
        var clientExists = cache
            .TryGetValue(firstName, out Client? client);

        if (clientExists && client is not null)
            return Ok(client);

        var clientFromContext = await context
            .Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(clt => clt.Name == firstName);

        if (clientFromContext is not null)
        {
            cache.Set(firstName, clientFromContext, TimeSpan.FromMinutes(5));
            return Ok(clientFromContext);
        }
        
        return NotFound(null);
    }
}