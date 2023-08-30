namespace AdoptiverseAPI.Models
{
    public class Pet
    {
       public int Id { get; set; }
       public int ShelterId { get; set; }
       public DateTime CreatedAt { get; set; }
       public DateTime UpdatedAt { get; set; }
       public bool Adoptable { get; set; }
       public int Age { get; set; }
       public string Breed { get; set; } = string.Empty;
       public string Name { get; set; } = string.Empty;
    }
}
