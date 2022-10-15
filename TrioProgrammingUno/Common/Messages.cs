using System.Drawing;
using TrioProgrammingUno.Business;
using TrioProgrammingUno.Common.Enums;

namespace TrioProgrammingUno.Common
{
    public class Messages
    {
        private const int countFromOne = 1;

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

            if (players <= 1 || players > 4 || validInput == false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please enter a valid input");
                Console.ForegroundColor = ConsoleColor.White;
                return AskForPlayers();
            }
            return players;
        }

        public void WriteMessage(string message)
        {
            Console.WriteLine(message);
        }

        public void WriteMessage(string message, ConsoleColor color)
        {
            if (color == ConsoleColor.Black)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else
            {
                Console.ForegroundColor = color;
            }
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void WriteMessage(string message, ConsoleColor color, bool asLine)
        {
            if (color == ConsoleColor.Black)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            else
            {
                Console.ForegroundColor = color;
            }
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void ShowGameState(string name, string symbol, string color)
        {
            WriteMessage($"It's {name} turn:", ConsoleColor.Cyan);
            WriteMessage($"Current card on the table: {Environment.NewLine}", ConsoleColor.Gray);
            WriteMessage($"=* {symbol} |", (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color), true);
            WriteMessage($" {color} *=", (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color), true);
            Console.WriteLine($"{Environment.NewLine}");
        }

        public void ShowHand(List<Card> hand, List<Card> playableCards)
        {
            int i = countFromOne;
            foreach (Card card in hand)
            {
                WriteMessage($"[{i}]", ConsoleColor.Magenta, true);
                WriteMessage($"  {card}", (ConsoleColor)Enum.Parse(typeof(ConsoleColor), card.CardColor.ToString()), true);
                if (playableCards.Contains(card))
                {
                    WriteMessage(" ==> Playable !", ConsoleColor.DarkYellow, true);
                }
                i++;
                Console.WriteLine();
            }
        }
    }
}