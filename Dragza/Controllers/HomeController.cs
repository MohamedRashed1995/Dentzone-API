using Dragza.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dragza.API.Controllers.Mobile
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        [HttpGet("GetHome")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHome()
        {
            var result = await _homeService.GetMobileHomeAsync();
            return Ok(result);
        }
        [HttpGet("GetHomeProduct")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHomeProduct()
        {
            var result = await _homeService.GetMobileHomeProductsAsync();
            return Ok(result);
        }
    }
}