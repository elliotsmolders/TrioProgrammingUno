namespace TrioProgrammingUno.Business
{
    public class Player
    {
        public string Name { get; set; }
        public List<Card> Hand { get; set; } = new();
        public int TurnCounter { get; set; }

        public Player(string name)
        {
            Name = name;
        }
    }
}