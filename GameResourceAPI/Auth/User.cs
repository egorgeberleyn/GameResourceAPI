namespace GameResourceAPI.Auth
{
    public record UserDto(string UserName, string Password);
    
    public record UserModel
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}
