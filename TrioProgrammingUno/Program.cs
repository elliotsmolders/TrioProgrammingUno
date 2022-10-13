// See https://aka.ms/new-console-template for more information
using TrioProgrammingUno.Business;
using TrioProgrammingUno.Common.Enums;

internal class Program
{
    private static void Main(string[] args)
    {
        Deck deck = new();
        GameEngine engine = new GameEngine(deck);
        engine.Init(0);
        engine.Run();
        //Card kaart1 = new Card("9", Color.Green);
        //Card kaart2 = new Card("5", Color.Green);
        //Card kaart3 = new Card("9", Color.Blue);
        //Card kaart4 = new Card("1", Color.Black);

        //Console.WriteLine(engine.CompareTwoCards(kaart1, kaart3)); 

    }
}