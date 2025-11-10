using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using Dragza.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace Dragza.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService , ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]

        public async Task<IActionResult> Register([FromForm] CreateUserDto createUserDto)
        {
            _logger.LogInformation("Register object :: {0}" ,JsonConvert.SerializeObject(createUserDto));

            await _userService.RegisterUserAsync(createUserDto);
            var loginDto = new LoginDto
            {
                UsernameOrEmail = createUserDto.UserName,
                Password = createUserDto.Password
            };
            var result = await _userService.LoginAsync(loginDto);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromForm] UpdateUserDto updateUserDto)
        {

            var result = await _userService.UpdateUserAsync(id, updateUserDto);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _userService.LoginAsync(loginDto);
            if (result.HasDetails == false)
            {
                return Ok(

                    new
                    {
                        message= "user does not exist",
                        statsus=HttpStatusCode.NotFound
                    }
                    );
            }
            return Ok(result);
        }

        [HttpGet("users")]
       // [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("user")]
        //[Authorize]
        public async Task<IActionResult> GetUsers(Guid userid)
        {
            var users = await _userService.GetUser(userid);
            return Ok(users);
        }

        [HttpPost("delete-user")]
        public async Task<IActionResult> DeleteUser([FromBody] Guid Id)
        {
            var result = await _userService.DeleteUser(Id
                );
            return Ok(result);
        }

        [HttpGet("by-role/{roleId}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersByRoleWithPharmacy(Guid roleId)
        {
            try
            {
                var users = await _userService.GetUsersByRoleWithPharmacyAsync(roleId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("deActive-user")]
        public async Task<IActionResult> DeActivatUser([FromBody] Guid Id)
        {
            var result = await _userService.DeActivateUser(Id);
            return Ok(result);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePassword model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userService.ChangePasswordAsync(model);
                    if (user) {

                        return Ok(new
                        {
                            message="تم تغير كلمه المرور",
                            status= HttpStatusCode.OK
                        });
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            message = "كلمه المرور الحاليه غير متطابقه",
                            status = HttpStatusCode.BadRequest
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest();
                throw;
            }
            return Ok();
        }

    }
}
