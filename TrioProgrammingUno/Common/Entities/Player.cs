namespace TrioProgrammingUno.Business
{
    public class Player
    {
        public string Name { get; set; }
        public List<Card> Hand { get; set; } = new();

        public Player(string name)
        {
            Name = name;
        }
    }
}