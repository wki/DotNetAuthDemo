namespace Auth3Demo
{
    public interface IUserRepository
    {
        int LoadUser(string username, string password);
    }
}