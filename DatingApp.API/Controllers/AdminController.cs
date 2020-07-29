using System.Linq;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AdminController : ControllerBase
  {
    private readonly DataContext _context;
    public AdminController(DataContext context)
    {
      _context = context;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("usersWithRoles")]
    public async System.Threading.Tasks.Task<IActionResult> GetUsersWithRoleAsync()
    {
      var userList = await _context.Users.OrderBy(x => x.UserName)
          .Select(user => new
          {
            Id = user.Id,
            UserName = user.UserName,
            Roles = (from userRole in user.UserRoles
                     join role in _context.Roles
                       on userRole.RoleId
                       equals role.Id
                     select role.Name).ToList()
          }).ToListAsync();

      return Ok(userList);
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photosForModeration")]
    public IActionResult GetPhotoForModerator()
    {
      return Ok("Admin or moderators can see this");
    }

  }
}