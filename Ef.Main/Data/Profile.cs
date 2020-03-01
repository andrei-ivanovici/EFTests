namespace Ef.Main.Data
{
    public class Profile
    {
        public int Id { get; set; }
        public string Alias { get; set; } 
        public Author Author { get; set; }
    }
}