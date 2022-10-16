using TrioProgrammingUno.Common;
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

        private Messages _message = new();

        public Player CurrentPlayer { get; set; }

        public int PlayerAmount { get; set; }
        public List<Player> ListOfPlayers { get; set; } = new();
        public List<Card> DiscardPile { get; set; } = new();
        private Deck deck { get; set; }
        public Color CurrentColor { get; set; }
        public string CurrentSymbol { get; set; }
        public bool PlayDirectionClockwise { get; set; }
        public bool SkipTurn { get; set; }
        public string SpecialEffectMessage { get; set; }

        public void Init(MenuOptions choice)
        {
            PlayDirectionClockwise = true;
            DisplayMenu(choice);
            deck.ShuffleDeck();

            foreach (Player player in ListOfPlayers)
            {
                DrawCards(player, AmountOfInitialCards);
            }

            Card topCard = deck.CardDeck.First();
            DiscardPile.Add(topCard);
            CurrentColor = topCard.CardColor;
            CurrentSymbol = topCard.CardSymbol;
            deck.CardDeck.Remove(topCard);

            CurrentPlayer = ListOfPlayers.First(); // kan refactored zodat je startstpeler kan kiezen door prop

            //Debugtime();
        }

        public void Run()
        {
            //startingplayer
            //refactor so game can continue after winner (winnerslist...)remove winner,  while condition wordt dan spelerlijst.count >1 etc
            int indexOfCurrentPlayer = ListOfPlayers.IndexOf(CurrentPlayer);
            bool hasCurrentPlayerCards = CurrentPlayer.Hand.Any();

            while (hasCurrentPlayerCards)
            {
                PlayerTurn(ListOfPlayers[indexOfCurrentPlayer]);
                indexOfCurrentPlayer = GetNextPlayerIndex();
            }
            _message.WonTheGame(CurrentPlayer.Name);
        }

        public void DisplayMenu(MenuOptions choice)
        {
            switch (choice)
            {
                case MenuOptions.AmountOfPlayers:
                    PlayerAmount = _message.AskForPlayers();
                    MakePlayers();
                    break;

                default:
                    break;
            }
        }

        private int GetNextPlayerIndex()
        {
            int currentPlayerIndex = ListOfPlayers.IndexOf(CurrentPlayer);
            int lastPlayerIndex = ListOfPlayers.Count - 1;

            if (PlayDirectionClockwise)
            {
                if (currentPlayerIndex == lastPlayerIndex)
                {
                    return 0;
                }
                return currentPlayerIndex + 1;
            }
            else
            {
                if (currentPlayerIndex == 0)
                {
                    return lastPlayerIndex;
                }
                return currentPlayerIndex - 1;
            }
        }

        private void MakePlayers()
        {
            for (int i = 0; i < PlayerAmount; i++)
            {
                _message.WriteLine($"Enter player {i + 1}'s name:");
                Player player = new(Console.ReadLine());
                ListOfPlayers.Add(player);
            }
        }

        private void PlayerTurn(Player currentPlayer)
        {
            //kan in apparte functie
            if (!(SpecialEffectMessage == null))
            {
                _message.WriteLine(SpecialEffectMessage);
                SpecialEffectMessage = null!;
            }
            CurrentPlayer = currentPlayer;
            if (SkipTurn)
            {
                _message.WriteLine($"{CurrentPlayer.Name} has been skipped");
                SkipTurn = false;
                return;
            }

            _message.ShowGameState(CurrentPlayer.Name, CurrentSymbol, CurrentColor.ToString());
            List<Card> playableCards = CurrentPlayer.Hand.Where(x => x.CardSymbol == CurrentSymbol || x.CardColor == CurrentColor || x.CardColor == Color.Black).ToList();
            _message.ShowHand(CurrentPlayer.Hand, playableCards);
            if (playableCards.Any())
            {
                SelectCard();
            }
            else
            {
                HandleDrawCard();
            }
            Console.Clear();
            //kan in apparte functie
            if (currentPlayer.Hand.Count() == (int)Specials.Uno)
            {
                _message.WriteLine($"{CurrentPlayer.Name}: UNO!");
            }
        }

        private void SelectCard()
        {
            _message.WriteLine($"Choose a card from 1 to {CurrentPlayer.Hand.Count()} to play, or draw a card by pressing 0");
            int choice;
            bool validInput;
            do
            {
                validInput = int.TryParse(Console.ReadLine(), out choice);
            } while (!validInput || choice < 0 || choice > CurrentPlayer.Hand.Count());

            if (choice == 0)
            {
                HandleDrawCard();
                return;
            }

            var card = CurrentPlayer.Hand[choice - 1];
            if (IsCardPlayable(card))
            {
                PlayCard(card);
                return;
            }

            SelectCard();
            Console.Clear();
        }

        private void PlayCard(Card card)
        {
            HandleSpecial(card);
            _message.WriteLine($" {CurrentPlayer.Name} has played {card}");
            CurrentSymbol = card.CardSymbol;
            //card effect?
            if (card.CardColor == Color.Black)
            {
                ChangeColorToPlayerChoice();
            }
            else
            {
                CurrentColor = card.CardColor;
            }
            DiscardPile.Add(card);
            CurrentPlayer.Hand.Remove(card);
        }

        private void HandleSpecial(Card card)
        {
            if (card.CardSymbol == Specials.Add2.ToString())
            {
                DrawCards(ListOfPlayers[GetNextPlayerIndex()], (int)Specials.Add2);
                SpecialEffectMessage = $"{ListOfPlayers[GetNextPlayerIndex()].Name} draws 2 cards";
            }
            if (card.CardSymbol == BlackSpecials.Add4.ToString())
            {
                SkipTurn = true;
                DrawCards(ListOfPlayers[GetNextPlayerIndex()], (int)BlackSpecials.Add4);
                SpecialEffectMessage = $"{ListOfPlayers[GetNextPlayerIndex()].Name} draws 4 cards";
            }
            if (card.CardSymbol == Specials.Stop.ToString())
            {
                SkipTurn = true;
            }
            if (card.CardSymbol == Specials.SwitchDirection.ToString())
            {
                SpecialEffectMessage = "Direction has switched!";
                PlayDirectionClockwise = !PlayDirectionClockwise;
            }
        }

        /// <summary>
        /// need to format cw
        /// </summary>
        public void ChangeColorToPlayerChoice()
        {
            int i = 1;
            _message.WriteLine("Choose a color:");
            foreach (Color color in Enum.GetValues(typeof(Color)))
            {
                if (color != Color.Black)
                {
                    _message.WriteLine($"{i}: {color}");
                    i++;
                }
            }
            int answer = 0;
            bool validInput = false;

            while (answer <= 0 || answer > Enum.GetNames(typeof(Color)).Length || !validInput)
            {
                validInput = int.TryParse(Console.ReadLine(), out answer);
            }
            CurrentColor = (Color)answer;
        }

        private void HandleDrawCard()
        {
            DrawCards(CurrentPlayer, 1);

            var card = CurrentPlayer.Hand[CurrentPlayer.Hand.Count() - 1];
            _message.WriteLine($"Your drawn card is {card}");
            if (IsCardPlayable(card))
            {
                string answer = null!;

                while (!string.Equals(answer, "Y", StringComparison.OrdinalIgnoreCase) && !string.Equals(answer, "N", StringComparison.OrdinalIgnoreCase))
                {
                    _message.WriteLine("This card is playable. Do you wish to play? Y / N");
                    answer = Console.ReadLine();
                }

                if (!string.Equals(answer, "Y", StringComparison.OrdinalIgnoreCase))
                {
                    PlayCard(card);
                }
            }
        }

        public bool IsCardPlayable(Card card)
        {
            return card.CardSymbol == CurrentSymbol || card.CardColor == CurrentColor || card.CardColor == Color.Black;
        }

        private void DrawCards(Player player, int amountOfCards)
        {
            for (var i = 0; i < amountOfCards; i++)
            {
                Card card = deck.CardDeck.First();
                player.Hand.Add(card);
                deck.CardDeck.Remove(card);
            }
        }

        public void Debugtime()
        {
            foreach (var player in ListOfPlayers)
            {
                _message.WriteLine(player.Name);
                foreach (Card card in player.Hand)
                {
                    _message.WriteLine($"{card.CardSymbol} {card.CardColor}");
                }
            }
        }
    }
}

// todo: switchdirection, uitspelen, black card on start, init 1 per 1 uitdelen, indexing hand, whitespace/formatting console., bart:kleurtjes geven, ascii art...