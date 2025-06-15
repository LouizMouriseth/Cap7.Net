using Core.Enums;
using Core.SeedWork;

namespace Core;

public class User : GenericModel
{
    public User()
    {
        
    }
    
    public User(string username, string email, string password, UserRole role)
    {
        Username = username;
        Email = email;
        Password = password;
        Role = role;
    }

    public string Username { get; private set; }
    public string Email { get; private set; }
    public string Password { get; private set; }
    public UserRole Role { get; private set; }

    public void SetPassword(string password)
    {
        Password = password;
    }
}