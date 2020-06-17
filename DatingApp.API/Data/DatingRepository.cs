using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
  public class DatingRepository : IDatingRepository
  {
    private readonly DataContext _context;

    public DatingRepository(DataContext context)
    {
      _context = context;
    }

    public async Task<Photo> GetMainPhotoForUser(int userId)
    {
      return await _context.Photos.Where(u => u.UserId == userId)
        .FirstOrDefaultAsync(p => p.IsMain);
    }

    public async Task<Photo> GetPhoto(int id)
    {
      return await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
    }

    void IDatingRepository.Add<T>(T entity)
    {
      _context.Add(entity);
    }

    void IDatingRepository.Delete<T>(T entity)
    {
      _context.Remove(entity);
    }

    async Task<User> IDatingRepository.GetUser(int id)
    {
      var user = await _context.Users.Include(u => u.Photos).FirstOrDefaultAsync(u => u.Id == id);
      return user;
    }

    async Task<IEnumerable<User>> IDatingRepository.GetUsers()
    {
      var users = await _context.Users.Include(p => p.Photos).ToListAsync();
      return users;
    }

    async Task<bool> IDatingRepository.SaveAll()
    {
      return await _context.SaveChangesAsync() > 0;
    }
  }
}