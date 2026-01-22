using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApiTransfer.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class TransportationsController(ICartService cartService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var result = await cartService.GetAllTransportationsAsync();
        return Ok(result);
    }
} 