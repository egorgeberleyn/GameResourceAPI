namespace GameResourceAPI.Models
{    
    public class CompanyDev
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Grade Grade { get; set; }
        public string Location { get; set; }
    }

    public enum Grade
    {
        None = 0,
        Indie = 1,
        AAA = 2,
        Mobile = 3
    }
}
