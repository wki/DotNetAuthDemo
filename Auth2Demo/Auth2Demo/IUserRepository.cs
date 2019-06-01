namespace Auth2Demo
{
    public interface IUserRepository
    {
        int LoadUser(string name, string password);
    }
}