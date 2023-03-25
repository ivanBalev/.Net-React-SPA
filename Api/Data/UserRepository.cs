using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

public interface IUserRepository
{
    Task<UserDto?> GetSingle(Expression<Func<UserEntity, bool>> predicate);

    Task<UserDto> Create(string username, string email, string passwordHash);

    public Task<bool> isEmailUnique(string email);

    public Task<bool> isUsernameUnique(string username);
}

public class UserRepository : IUserRepository
{
    private readonly HouseDbContext context;

    public UserRepository(HouseDbContext context)
    {
        this.context = context;
    }

    public async Task<UserDto?> GetSingle(Expression<Func<UserEntity, bool>> predicate)
    {
        var user = await this.context.Users.SingleOrDefaultAsync(predicate);

        if (user == null)
            return null;

        return new UserDto(user.Id, user.Username, user.Email, user.Password);
    }

    public async Task<UserDto> Create(string username, string email, string passwordHash)
    {
        var userEntity = new UserEntity
        {
            Id = Guid.NewGuid().ToString(),
            Username = username,
            Email = email,
            Password = passwordHash
        };

        context.Users.Add(userEntity);
        await context.SaveChangesAsync();

        return new UserDto(userEntity.Id, userEntity.Username, userEntity.Email, userEntity.Password);
    }

    public async Task<bool> isEmailUnique(string email)
    {
        var user = await this.context.Users.SingleOrDefaultAsync(u => u.Email == email);
        return user == null;
    }

    public async Task<bool> isUsernameUnique(string username)
    {
        var user = await this.context.Users.SingleOrDefaultAsync(u => u.Username == username);
        return user == null;
    }
}