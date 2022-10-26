using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrioProgrammingUno.Business
{
    internal class PlayableCardList<I, C>
    {
        public PlayableCardList(I integer, C card)
        {
            Integer = integer;
            Card = card;
        }

        public I Integer { get; }
        public C Card { get; }
    }
}