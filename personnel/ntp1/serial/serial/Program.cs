using System.Text.Json;

namespace serial
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Character> characters = new List<Character>();
            Character jd = new Character { FirstName = "Jean", LastName = "Dupont", Description = "Enseignant de programmation, c'est le plus gros consomateur d'endive au miel de l'école" };
            File.WriteAllText("characters/jean.json", JsonSerializer.Serialize(jd));

            foreach (string f in Directory.GetFiles("characters"))
            {
                string json = File.ReadAllText($"{f}");
                Character c = JsonSerializer.Deserialize<Character>(json);
                characters.Add(c);
            }

            characters.ForEach(c => Console.WriteLine(c));
        }
    }

    public class Character
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public Actor PlayedBy { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName}: {Description}";
        }
    }

    public class Actor
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Country { get; set; }
        public bool IsAlive { get; set; }
    }
}
