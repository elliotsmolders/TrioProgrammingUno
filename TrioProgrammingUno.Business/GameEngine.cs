using TrioProgrammingUno.Common.Enums;

namespace TrioProgrammingUno.Business
{
    public class GameEngine
    {
        private const int AmountOfInitialCards = 7;
        private Logger _logger;
            
        public GameEngine(Deck deck)
        {
            this.deck = deck;
            _logger = new Logger();
        }

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

            Card topCard = DrawTopCard();
            CurrentColor = topCard.CardColor;
            CurrentSymbol = topCard.CardSymbol;

            CurrentPlayer = GetFirstPlayer(); // kan refactored zodat je startstpeler kan kiezen door prop

            //Debugtime();
        }

        private Player GetFirstPlayer()
        {
            return ListOfPlayers.First();
        }

        private Card DrawTopCard()
        {
            Card topCard = deck.CardDeck.First();
            DiscardPile.Add(topCard);
            deck.CardDeck.Remove(topCard);
            return topCard;
        }

        private bool PlayerHasCards() => CurrentPlayer.Hand.Any();

        public void PlayGame()
        {
            //startingplayer
            //refactor so game can continue after winner (winnerslist...)remove winner,  while condition wordt dan spelerlijst.count >1 etc
            int indexOfCurrentPlayer = ListOfPlayers.IndexOf(CurrentPlayer);

            while (PlayerHasCards())
            {
                PlayerTurn(ListOfPlayers[indexOfCurrentPlayer]);
                indexOfCurrentPlayer = GetNextPlayerIndex();
            }

            WonTheGame(CurrentPlayer.Name);
        }

        public void DisplayMenu(MenuOptions choice)
        {
            switch (choice)
            {
                case MenuOptions.AmountOfPlayers:
                    PlayerAmount = AskForPlayers();
                    MakePlayers();
                    break;
            }
        }

        public string WonTheGame(string name)
        {
            return $"{name} has won the game";
        }

        //TODO Refactor
        public int AskForPlayers()
        {
            int players = 0;
            Console.WriteLine("Choose 2 to 4 players");
            bool isValidInput = int.TryParse(Console.ReadLine(), out players);

            while (players <= 1 || players > 4 || !isValidInput)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please enter a valid input");
                Console.ForegroundColor = ConsoleColor.White;
                isValidInput = int.TryParse(Console.ReadLine(), out players);
            }
            return players;
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


        //TODO Refactor
        private void MakePlayers()
        {
            for (int i = 0; i < PlayerAmount; i++)
            {
                _logger.WriteLine($"Enter player {i + 1}'s name:");
                Player player = new(Console.ReadLine());
                ListOfPlayers.Add(player);
            }
        }

        private void PlayerTurn(Player currentPlayer)
        {
            //kan in apparte functie
            if (!(SpecialEffectMessage == null))
            {
                _logger.WriteLine(SpecialEffectMessage);
                SpecialEffectMessage = null!;
            }
            CurrentPlayer = currentPlayer;
            if (SkipTurn)
            {
                _logger.WriteLine($"{CurrentPlayer.Name} has been skipped");
                SkipTurn = false;
                return;
            }

            ShowGameState(CurrentPlayer.Name, CurrentSymbol, CurrentColor.ToString());
            
            IList<Card> playableCards = CurrentPlayer.Hand
                .Where(x => x.CardSymbol == CurrentSymbol 
                || x.CardColor == CurrentColor 
                || x.CardColor == Color.Black)
                .ToList();
            ShowHand(CurrentPlayer.Hand, playableCards);
            
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
                _logger.WriteLine($"{CurrentPlayer.Name}: UNO!");
            }
        }

        private void SelectCard()
        {
            _logger.WriteLine($"Choose a card from 1 to {CurrentPlayer.Hand.Count()} to play, or draw a card by pressing 0");
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
            _logger.WriteLine($" {CurrentPlayer.Name} has played {card}");
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
            _logger.WriteLine("Choose a color:");
            foreach (Color color in Enum.GetValues(typeof(Color)))
            {
                if (color != Color.Black)
                {
                    _logger.WriteLine($"{i}: {color}");
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

        public void ShowHand(IList<Card> hand, IList<Card> playableCards)
        {
            int i = 1;
            foreach (Card card in hand)
            {
                _logger.Write($"[{i}]", ConsoleColor.Magenta);
                _logger.Write($"  {card}", (ConsoleColor)Enum.Parse(typeof(ConsoleColor), card.CardColor.ToString()));
                if (playableCards.Contains(card))
                {
                    _logger.Write(" ==> Playable !", ConsoleColor.DarkYellow);
                }
                i++;
                Console.WriteLine();
            }
        }

        public void ShowGameState(string name, string symbol, string color)
        {
            _logger.WriteLine($"It's {name} turn:", ConsoleColor.Cyan);
            _logger.WriteLine($"Current card on the table: {Environment.NewLine}", ConsoleColor.Gray);
            _logger.Write($"=* {symbol} |", (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color));
            _logger.Write($" {color} *=", (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color));
            Console.WriteLine($"{Environment.NewLine}");
        }

        private void HandleDrawCard()
        {
            DrawCards(CurrentPlayer, 1);

            Card card = CurrentPlayer.Hand[CurrentPlayer.Hand.Count() - 1];
            _logger.WriteLine($"Your drawn card is {card}");
            
            if (IsCardPlayable(card))
            {
                string answer = null!;

                while (!string.Equals(answer, "Y", StringComparison.OrdinalIgnoreCase) && !string.Equals(answer, "N", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.WriteLine("This card is playable. Do you wish to play? Y / N");
                    answer = GetUserInput();
                }

                if (string.Equals(answer, "Y", StringComparison.OrdinalIgnoreCase))
                {
                    PlayCard(card);
                }
            }
        }

        private string GetUserInput()
        {
            return Console.ReadLine();
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
                _logger.WriteLine(player.Name);
                foreach (Card card in player.Hand)
                {
                    _logger.WriteLine($"{card.CardSymbol} {card.CardColor}");
                }
            }
        }
    }
}

// todo: switchdirection, uitspelen, black card on start, init 1 per 1 uitdelen, indexing hand, whitespace/formatting console., bart:kleurtjes geven, ascii art...