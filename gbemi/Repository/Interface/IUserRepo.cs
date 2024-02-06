using gbemi.Models;

namespace gbemi.Repository.Interface
{
    public interface IUserRepo
    {
        Task<List<User>> GetAllAsync();
        Task<User> FindAsync(int UserId);
        Task<User> FindAsyncByNameAsync(string fname);
        Task<User> FindAsyncByLNameAsync(string lname);
        Task<User> FindAsyncByUNameAsync(string uname);
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task RemoveAsync(int UserId);
        Task<User> FindAsyncByUNameAndPasswordAsync(string uname, string password);
    }
}
