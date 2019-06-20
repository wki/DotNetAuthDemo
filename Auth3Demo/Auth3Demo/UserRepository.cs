using Microsoft.Extensions.Logging;

namespace Auth3Demo
{
    public class UserRepository: IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ILogger<UserRepository> logger)
        {
            _logger = logger;
        }

        public int LoadUser(string username, string password)
        {
            using (_logger.BeginScope($"Authenticating User {username}"))
            {
                return 42;
            }
        }
    }
}