namespace TrioProgrammingUno.Business
{
    public class Player
    {
        public string Name { get; set; }
        public IList<Card> Hand { get; set; } = new List<Card>();

        public Player(string name)
        {
            Name = name;
        }
    }
}