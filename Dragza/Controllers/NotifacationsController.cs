using Dragza.Application.Interface;
using Dragza.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dragza.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotifacationsController : ControllerBase
    {
        private readonly IRepository<Notifacation> _notifacationRepo;
        public NotifacationsController(IRepository<Notifacation> notifacationRepo)
        {
            _notifacationRepo = notifacationRepo;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAllNotifacations(Guid userId)
        {
            var notifacations = await _notifacationRepo.FindAsync(a=>a.UserId==userId);
            return Ok(notifacations);
        }

    }
}
