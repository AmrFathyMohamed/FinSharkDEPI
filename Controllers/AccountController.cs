using FinShark.Dtos.Account;
using FinShark.Data;
using FinShark.Interfaces;
using FinShark.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FinShark.Controllers
{
    // Identity: الـ Controller الخاص بالتسجيل والدخول — الاتنين بيرجّعوا JWT
    // [controller] = اسم الكلاس من غير "Controller" → /api/account
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        // UserManager: خدمة جاهزة من Identity (متسجّلة بـ AddIdentity) — بتعمل Create / Find / CheckPassword / Roles
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        // POST /api/account/register — يعمل user جديد ويديله Role = User ويرجّع token
        [HttpPost("register")]
        [ProducesResponseType(typeof(NewUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            // Trim للـ Email والـ UserName بس عشان المسافات الزيادة متعملش user مختلف
            var appUser = new AppUser()
            {
                Email = registerDto.Email.Trim(),
                UserName = registerDto.Username.Trim()
            };

            // لازم نبعت الباسورد — من غيره الـ user بيتعمل من غير PasswordHash ومش هيعرف يعمل login أبداً
            // CreateAsync بيعمل Hash للباسورد وبيطبّق قواعد Program.cs (الطول، الأرقام، Unique Email...)
            var createResult = await _userManager.CreateAsync(appUser, registerDto.Password);
            if (!createResult.Succeeded)
            {
                // أخطاء Identity زي DuplicateUserName أو PasswordTooShort → 400
                return BadRequest(createResult.Errors);
            }

            // كل user جديد بياخد Role = User (الـ Admin والـ SuperAdmin بيتعملوا بطريقة تانية)
            var roleResult = await _userManager.AddToRoleAsync(appUser, AppRoles.User);
            if (!roleResult.Succeeded)
            {
                // منسيبش user من غير role
                await _userManager.DeleteAsync(appUser);
                return BadRequest(roleResult.Errors);
            }

            _logger.LogInformation("User {UserName} Registered", appUser.UserName);
            // NewUserDto مش AppUser — الـ AppUser فيه PasswordHash و SecurityStamp مينفعش يرجعوا للـ client
            return Ok(await ToNewUserDto(appUser));
        }

        // POST /api/account/login — يتحقق من الـ UserName والباسورد ويرجّع token
        [HttpPost("login")]
        [ProducesResponseType(typeof(NewUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // FindByNameAsync بيدوّر بالـ NormalizedUserName فمش فارق كابيتال ولا سمول
            var user = await _userManager.FindByNameAsync(loginDto.Username.Trim());

            // الباسورد مبيتعملهوش Trim — المسافة ممكن تكون جزء منه
            // نفس الرسالة لو الـ user مش موجود أو الباسورد غلط، عشان محدش يعرف أنهي فيهم الغلط
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                _logger.LogWarning("Failed login attempt For {UserName}", loginDto.Username);
                return Unauthorized("UserName or Password is incorrect");
            }
            return Ok(await ToNewUserDto(user));
        }

        // Helper: بيجيب الـ Roles ويعمل الـ token ويرجّع الـ DTO — مستخدم في Register و Login
        private async Task<NewUserDto> ToNewUserDto(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return new NewUserDto
            {
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = roles,
                Token = _tokenService.CreateToken(user, roles)
            };
        }


    }
}
