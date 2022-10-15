using System.Threading;
using System.Threading.Tasks;
using Server.ItSelf;

namespace InterviewDemo;

public record User(string Name, string Surname, string Login);

public class UsersController : IController
{
    public async Task<User[]>  Index()
    {
        await Task.Delay(100);
        
        return new[]
        {
            new User("Madiyar", "Ussabekov", "Mad"),
            new User("Dan", "Ussabekov", "Dan")
        };
    }
    
}