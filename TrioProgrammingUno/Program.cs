// See https://aka.ms/new-console-template for more information
using TrioProgrammingUno.Business;

internal class Program
{
    private static void Main(string[] args)
    {
        Deck deck = new();
        GameEngine engine = new GameEngine(deck);
        engine.Init(0);
        engine.PlayGame();


    }
}