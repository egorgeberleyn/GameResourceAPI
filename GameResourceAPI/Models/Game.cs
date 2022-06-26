namespace GameResourceAPI.Models
{   
    public class Game
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Genre Genre { get; set; }
        public DateTime PublicationDate { get; set; }


        public virtual CompanyDev CompanyDev { get; set; }
    }

    public enum Genre
    {
        None = 0,
        Fighting = 1,
        Shooter = 2,
        Strategy = 3,
        MOBA = 4,
        Platformer = 5,
        RPG = 6,
        Survival = 7,
        Arcade = 8,
        Simulator = 9,
        Racing = 10,
    }
}
