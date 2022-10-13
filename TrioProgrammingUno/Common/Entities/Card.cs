using TrioProgrammingUno.Common.Enums;

namespace TrioProgrammingUno.Business
{
    public class Card
    {
        public Card(string symbol, Color color)
        {
            CardSymbol = symbol;
            CardColor = color;
        }

        public Card(string symbol)
        {
            CardSymbol = symbol;
        }

        public string CardSymbol { get; set; }
        public Color CardColor { get; set; }

        public override string ToString()
        {
            return $"Type: {CardSymbol} Color: {CardColor}";
        }
    }
}