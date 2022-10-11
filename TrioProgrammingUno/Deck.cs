using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrioProgrammingUno
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
    }
    
}
