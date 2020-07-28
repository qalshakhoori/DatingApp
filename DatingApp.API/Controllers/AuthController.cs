using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [AllowAnonymous]
  public class AuthController : ControllerBase
  {
    public IAuthRepository _repo { get; }
    private readonly IConfiguration _config;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    private readonly IMapper _mapper;
    public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper,
    UserManager<User> userManager, SignInManager<User> signInManager)
    {
      _signInManager = signInManager;
      _userManager = userManager;
      _config = config;
      _repo = repo;
      _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
    {
      // validate request
      var username = userForRegisterDto.Username.ToLower();

      if (await _repo.UserExists(username))
        return BadRequest("Username already exists");

      var userToCreate = _mapper.Map<User>(userForRegisterDto);

      var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

      var userToReturn = _mapper.Map<UserForDetailedDto>(createdUser);

      return CreatedAtRoute("GetUser", new { controller = "Users", id = createdUser.Id }, userToReturn);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserForLoginDto userForLogin)
    {
      var user = await _userManager.FindByNameAsync(userForLogin.Username);

      var result = await _signInManager.CheckPasswordSignInAsync(user, userForLogin.Password, false);

      if (result.Succeeded)
      {
        var appUser = _mapper.Map<UserForListDto>(user);

        return Ok(new
        {
          token = GenerateJwtToken(user),
          user = appUser
        });
      }

      return Unauthorized();
    }

    private string GenerateJwtToken(User user)
    {
      var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.Now.AddDays(1),
        SigningCredentials = creds
      };

      var tokenHandler = new JwtSecurityTokenHandler();

      var token = tokenHandler.CreateToken(tokenDescriptor);

      return tokenHandler.WriteToken(token);
    }
  }
}