using LearningCache.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace LearningCache.Controllers;

[ApiController]
[Route("[controller]")]
public class ClientController(IMemoryCache cache) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Client>> GetClientWithoutCache(
        [FromRoute] string firstName
    )
    {
        return default;
    }
    
    [HttpGet]
    public async Task<ActionResult<Client>> GetClientWithCache(
        [FromRoute] string firstName
    )
    {
        return default;
    }
}