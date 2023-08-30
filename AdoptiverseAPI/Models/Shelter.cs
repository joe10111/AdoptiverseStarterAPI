namespace AdoptiverseAPI.Models
{
    public class Shelter
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool FosterProgram { get; set; }
        public int Rank { get; set; }
        public string City { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public List<Pet> Pets { get; set; } = new List<Pet>();
    }
}
