using TrioProgrammingUno.Common.Enums;

namespace TrioProgrammingUno.Business
{
    public class Deck
    {
        private int numberOfSpecials = 4;

        public Deck()
        {
            //add 0-9 every color
            foreach (Color color in Enum.GetValues(typeof(Color)))
            {
                if (color != Color.Black)
                {
                    for (int i = 0; i <= 9; i++)
                    {
                        CardDeck.Add(new Card(i.ToString(), color));
                    }
                }
            }
            // add specials with color
            foreach (Color color in Enum.GetValues(typeof(Color)))
            {
                foreach (Specials special in Enum.GetValues(typeof(Specials)))
                {
                    CardDeck.Add(new Card(special.ToString(), color));
                }
            }
            // add colourless specials
            for (int i = 0; i < numberOfSpecials; i++)
            {
                foreach (ColorlessSpecials colorlessSpecial in Enum.GetValues(typeof(ColorlessSpecials)))
                {
                    CardDeck.Add(new Card(colorlessSpecial.ToString()));
                }
            }
        }

        public List<Card> CardDeck { get; set; } = new List<Card>();

        private Random rng = new Random();

        public void ShuffleDeck()
        {
            List<Card> shuffledCards = CardDeck.OrderBy(c => rng.Next()).ToList();
            CardDeck = shuffledCards;
        }

        //public override string ToString()
        //{
        //    string cardListString = "";
        //    foreach (Card card in CardDeck)
        //    {
        //        cardListString += $"{card.CardSymbol} {card.CardColor} {Environment.NewLine}";
        //    }
        //    return cardListString;
        //}

        public void PrintDeck()
        {
            foreach (Card card in CardDeck)
            {
                Console.WriteLine(card.ToString());
            }
        }
    }
}