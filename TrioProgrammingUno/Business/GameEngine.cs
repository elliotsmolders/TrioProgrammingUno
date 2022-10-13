using TrioProgrammingUno.Common.Enums;

namespace TrioProgrammingUno.Business
{
    public class GameEngine
    {
        private const int AmountOfInitialCards = 7;

        public GameEngine(Deck deck)
        {
            this.deck = deck;
        }

        public int PlayerAmount { get; set; }
        public List<Player> ListOfPlayers { get; set; } = new();
        public List<Card> DiscardPile { get; set; } = new();
        private Deck deck { get; set; }

        public void DisplayMenu(MenuOptions choice)
        {
            switch (choice)
            {
                case MenuOptions.AmountOfPlayers:
                    PlayerAmount = AskForPlayers();
                    MakePlayers();
                    break;

                default:
                    break;
            }
        }

        //TODO: Dus, refactor = guard steken in property setter.
        private int AskForPlayers()
        {
            int players = 0;
            bool validInput = false;
            Console.WriteLine("Kies 1 tot 4 spelers");
            validInput = int.TryParse(Console.ReadLine(), out players);

            if (players <= 0 || players > 4 && validInput == false)
            {
                Console.WriteLine("Please enter a valid input");
                return AskForPlayers();
            }
            return players;
        }

        private void MakePlayers()
        {
            for (int i = 0; i < PlayerAmount; i++)
            {
                Player player = new(Console.ReadLine());
                ListOfPlayers.Add(player);
            }
        }

        public void Debugtime()
        {
            foreach (var player in ListOfPlayers)
            {
                Console.WriteLine(player.Name);
                foreach (Card card in player.Hand)
                {
                    Console.WriteLine(card.CardSymbol + "" + card.CardColor);
                }
            }
        }

        public void Init(MenuOptions choice)
        {
            DisplayMenu(choice);
            deck.ShuffleDeck();
            foreach (Player player in ListOfPlayers)
            {
                DrawCards(deck.CardDeck, player, AmountOfInitialCards);
            }
            DiscardPile.Add(deck.CardDeck[0]);
            deck.CardDeck.RemoveAt(0);
            //Debugtime();
        }

        public void Run()
        {
            Play();
            bool won = CheckWinCondition();
        }

        private void CheckWinCondition()
        {
            throw new NotImplementedException();
        }

        private void Play()
        {
            CheckForPlayableCard();
            SelectCard();
            CheckForEmptyHand();
        }

        private void CheckForEmptyHand()
        {
            throw new NotImplementedException();
        }

        private void CheckForPlayableCard()
        {
        }

        private void SelectCard()
        {
            throw new NotImplementedException();
        }

        public void DrawCards(List<Card> cards, Player player, int amountOfCards)
        {
            for (int i = 0; i < amountOfCards; i++)
            {
                player.Hand.Add(cards[0]);
                cards.RemoveAt(0);
            }
        }
    }
}