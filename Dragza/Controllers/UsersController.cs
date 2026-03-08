using Dragza.Application.Interface;
using Dragza.Domain.DTO;
using Dragza.Domain.Models;
using Dragza.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly IUserRepository _userRepository;
        public UsersController(IUserService userService , ILogger<UsersController> logger, IUserRepository userRepository)
        {
            _userService = userService;
            _logger = logger;
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] CreateUserDto createUserDto)
        {
            try
            {
                _logger.LogInformation("Register object received: {0}", JsonConvert.SerializeObject(createUserDto));

                // نسجل المستخدم
                var userResponse = await _userService.RegisterUserAsync(createUserDto);

                // نرجع response بتاع register مش login
                return Ok(userResponse);
            }
            catch (ApplicationException appEx)
            {
                _logger.LogWarning(appEx, "Application exception during registration");
                return BadRequest(new { message = appEx.Message });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error during registration");
                return StatusCode(500, new { message = "Database error: " + dbEx.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration");
                return StatusCode(500, new { message = "Unexpected error: " + ex.Message });
            }
        }



        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromForm] UpdateUserDto updateUserDto)
        {
            try
            {
                var result = await _userService.UpdateUserAsync(id, updateUserDto);

                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var result = await _userService.LoginAsync(loginDto);
            if (result.HasDetails == false)
            {
                return NotFound(

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

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUsers(Guid userId)
        {
            var user = await _userService.GetUser(userId);

            if (user == null)
                return NotFound("User not found");

            return Ok(user);
        }

        [HttpGet("by-role/{roleId}")]
        public async Task<IActionResult> GetUsersByRole(Guid roleId)
        {
            var users = await _userService.GetUsersByRoleAsync(roleId);
            return Ok(users);
        }


        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
        {
            var result = await _userService.DeleteUser(userId);
            return Ok(result);
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
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message,
                    inner = ex.InnerException?.Message
                });
            }
        }
    }
}
