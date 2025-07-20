using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models.DTOs;
using NZWalks.API.Repositories.V1;

namespace NZWalks.API.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ITokenRepository _tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager, 
            SignInManager<IdentityUser> signInManager,
            ITokenRepository tokenRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenRepository = tokenRepository;
        }

        [HttpPost("Register")]
        [ValidateModel]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.UserName,
                Email = registerRequestDto.EmailAddress
            };

            var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if (identityResult.Succeeded) 
            {
                if (registerRequestDto.Roles is not null && registerRequestDto.Roles.Any())
                {
                    identityResult = await _userManager.AddToRolesAsync(identityUser, registerRequestDto.Roles);
                    if (identityResult.Succeeded)
                        return Ok("Registered successfully!");
                }
                    
            }
            return BadRequest("Something went wrong while registering.");
        }

        [HttpPost("Login")]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = new EmailAddressAttribute().IsValid(loginRequestDto.LoginId)
                ? await _userManager.FindByEmailAsync(loginRequestDto.LoginId) :
                  await _userManager.FindByNameAsync(loginRequestDto.LoginId);

            if (user is null)
                return Unauthorized("Invalid user name or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequestDto.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
                return Unauthorized("Invalid user name or password");      
            
            var roles = await _userManager.GetRolesAsync(user);
            if (roles is null)
                return BadRequest("Something went wrong while logging.");

            var jwtToken = _tokenRepository.CreateJwtToken(user, roles.ToList());

            // Create Token
            return Ok(new LoginResponseDto { JwtToken = jwtToken, Message= "Logged in successfully."});
        }
    }
}
