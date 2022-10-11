// See https://aka.ms/new-console-template for more information
using TrioProgrammingUno;

Deck deck1 = new();
foreach (var card in deck1.CardDeck)
{
    Console.WriteLine($"Color:{card.CardColor} Type:{card.CardSymbol}");
}