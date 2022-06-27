namespace GameResourceAPI.Auth
{
    public interface IUserRepository
    {
        UserDto GetUser(UserModel model);
    }
}
