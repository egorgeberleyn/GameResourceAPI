namespace GameResourceAPI.Auth
{
    public class UserRepository : IUserRepository
    {
        private readonly List<UserDto> _users = new()
        {
            new UserDto("John", "123"),
            new UserDto("Larry", "qwerty"),
            new UserDto("Fiona", "111")
        };
        
        public UserDto GetUser(UserModel model) =>         
            _users.FirstOrDefault(user=> 
                string.Equals(user.UserName, model.UserName) &&
                string.Equals(user.Password, model.Password)) ??
                throw new Exception("Not found");        
    }
}
