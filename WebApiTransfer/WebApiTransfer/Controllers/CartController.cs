using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApiTransfer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController(ICartService cartService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetListAsync()
        {
            var cartItems = await cartService.GetListAsync();
            return Ok(cartItems);
        }
        [HttpPost]
        public async Task<IActionResult> AddUpdateAsync([FromBody] CartAddUpdateModel model)
        {
            await cartService.AddUpdateAsync(model);
            return Ok();
        }

    }
}
