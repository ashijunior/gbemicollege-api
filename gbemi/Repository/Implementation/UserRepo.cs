using gbemi.Contexts;
using gbemi.Models;
using Microsoft.EntityFrameworkCore;


using gbemi.Repository.Interface;
using System;

namespace gbemi.Repository.Implementation
{
    public class UserRepo:IUserRepo
    {
        private readonly AppDbContext _context;
        public UserRepo(AppDbContext context)
        {
            _context = context;

        }

        public async Task<User> FindAsync(int UserId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(s => s.Id == UserId);
            return user;
        }
        public async Task<User> FindAsyncByNameAsync(string name)
        {
            var user = await _context.Users.FirstOrDefaultAsync(s => s.FirstName == name);
            return user;
        }

        public async Task<User> FindAsyncByLNameAsync(string lname)
        {
            var user = await _context.Users.FirstOrDefaultAsync(s => s.LastName == lname);
            return user;
        }


        public async Task<User> FindAsyncByUNameAsync(string uname)
        {
            var user = await _context.Users.FirstOrDefaultAsync(s => s.UserName == uname);
            return user;
        }


        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task RemoveAsync(int userId)
        {
            var myUser = await _context.Users.FirstOrDefaultAsync(s => s.Id == userId);

            if (myUser != null)
            {
                _context.Users.Remove(myUser);
                await _context.SaveChangesAsync();
            }
        }


        public Task<List<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User> FindAsyncByUNameAndPasswordAsync(string uname, string password)
        {
            throw new NotImplementedException();
        }


    }
}
