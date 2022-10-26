using System.Drawing;
using TrioProgrammingUno.Business;
using TrioProgrammingUno.Common.Enums;

namespace TrioProgrammingUno.Common
{
    public class Messages
    {
        public void WonTheGame(string name)
        {
            Console.WriteLine($"{name} has won the game");
        }

        public int AskForPlayers()
        {
            int players = 0;
            bool validInput = false;
            Console.WriteLine("Choose 2 to 4 players");
            validInput = int.TryParse(Console.ReadLine(), out players);

            while (players <= 1 || players > 4 || !validInput)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please enter a valid input");
                Console.ForegroundColor = ConsoleColor.White;
                validInput = int.TryParse(Console.ReadLine(), out players);
            }
            return players;
        }

        private int tellerke = 0;

        public void WriteLine(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.Cyan : color;
            Console.WriteLine(message);
        }

        public void Write(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.Cyan : color;
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void ShowGameState(string name, string symbol, string color)
        {
            WriteLine($"It's {name} turn:", ConsoleColor.Cyan);
            WriteLine($"Current card on the table: {Environment.NewLine}", ConsoleColor.Gray);
            Write($"=* {symbol} |", (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color));
            Write($" {color} *=", (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color));
            Console.WriteLine($"{Environment.NewLine}");
        }

        public void ShowHand(IList<Card> hand, IList<Card> playableCards)
        {
            int i = 1;
            foreach (Card card in hand)
            {
                Write($"[{i}]", ConsoleColor.Magenta);
                Write($"  {card}", (ConsoleColor)Enum.Parse(typeof(ConsoleColor), card.CardColor.ToString()));
                if (playableCards.Contains(card))
                {
                    Write(" ==> Playable !", ConsoleColor.DarkYellow);
                }
                i++;
                Console.WriteLine();
            }
        }
    }
}