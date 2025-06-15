using Core;

namespace Infrastructure.Repositories.UserRepository;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}