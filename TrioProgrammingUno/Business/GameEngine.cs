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

        public Player CurrentPlayer { get; set; }

        public int PlayerAmount { get; set; }
        public List<Player> ListOfPlayers { get; set; } = new();
        public List<Card> DiscardPile { get; set; } = new();
        private Deck deck { get; set; }
        public Color CurrentColor { get; set; }
        public string CurrentSymbol { get; set; }
        public bool PlayDirectionClockwise { get; set; }
        public bool SkipTurn { get; set; }
        public string specialEffectMessage { get; set; }

        public void Init(MenuOptions choice)
        {
            PlayDirectionClockwise = true;
            DisplayMenu(choice);
            deck.ShuffleDeck();
            foreach (Player player in ListOfPlayers)
            {
                DrawCards(deck.CardDeck, player, AmountOfInitialCards);
            }
            DiscardPile.Add(deck.CardDeck[0]);
            CurrentColor = deck.CardDeck[0].CardColor;
            CurrentSymbol = deck.CardDeck[0].CardSymbol;
            deck.CardDeck.RemoveAt(0);
            CurrentPlayer = ListOfPlayers[0]; // kan refactored zodat je startstpeler kan kiezen door prop

            //Debugtime();
        }

        public void Run()
        {
            int indexOfCurrentPlayer = ListOfPlayers.IndexOf(CurrentPlayer); //startingplayer
            //refactor so game can continue after winner (winnerslist...)remove winner,  while condition wordt dan spelerlijst.count >1 etc
            while (!CheckForEmptyHand())
            {
                PlayerTurn(ListOfPlayers[indexOfCurrentPlayer]);
                indexOfCurrentPlayer = GetNextPlayerIndex();
                

            }
            Console.WriteLine($"{CurrentPlayer.Name} has won the game");
        }

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

        private int GetNextPlayerIndex()
        {
            int indexOfPlayer = ListOfPlayers.IndexOf(CurrentPlayer);
            if (PlayDirectionClockwise)
            {
                if (indexOfPlayer < (ListOfPlayers.Count - 1))
                {
                    indexOfPlayer++;
                }
                else
                {
                    indexOfPlayer = 0;
                }
            }
            else
            {
                if (indexOfPlayer != 0)
                {
                    indexOfPlayer--;
                }
                else
                {
                    indexOfPlayer = ListOfPlayers.Count() - 1;
                }
            }
            return indexOfPlayer;
        }

        //TODO: Dus, refactor = guard steken in property setter.
        private int AskForPlayers()
        {
            int players = 0;
            bool validInput = false;
            Console.WriteLine("Choose 2 to 4 players");
            validInput = int.TryParse(Console.ReadLine(), out players);

            if (players <= 1 || players > 4 && validInput == false)
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
                Console.WriteLine($"Enter player {i + 1}'s name:");
                Player player = new(Console.ReadLine());
                ListOfPlayers.Add(player);
            }
        }

        private void PlayerTurn(Player currentPlayer)
        {
            //kan in apparte functie
            if(!(specialEffectMessage == ""))
            {
                Console.WriteLine(specialEffectMessage);
                specialEffectMessage = "";
            }
            CurrentPlayer = currentPlayer;
            if (SkipTurn)
            {
                Console.WriteLine($"{CurrentPlayer.Name} has been skipped");
                SkipTurn = false;
                return;
            }
            
            ShowGameState();
            ShowHand();
            if (CheckForPlayableCard())
            {
                SelectCard();
            }
            else
            {
                handleDrawCard();
            }
            Console.Clear();
            //kan in apparte functie
            if (currentPlayer.Hand.Count()==1)
            {
                Console.WriteLine($"{CurrentPlayer.Name}: UNO!");
            }
        }

        private void ShowGameState()
        {
            Console.WriteLine($"It's {CurrentPlayer.Name} turn:");
            Console.WriteLine($"Current card on the table: {Environment.NewLine} =*{CurrentSymbol}|{CurrentColor}*=");
        }

        private void ShowHand()
        {
            foreach (Card card in CurrentPlayer.Hand)
            {
                Console.Write(card.ToString() + ", ");
            }
        }

        public bool CheckForPlayableCard()
        {
            List<Card> playableCards = CurrentPlayer.Hand.Where(x => IsCardPlayable(x)).ToList();
            if (playableCards.Count() != 0)
            {
                Console.WriteLine("Your playable cards are:");
                foreach (Card card in playableCards)
                {
                    Console.WriteLine(card.ToString());
                }
                return true;
            }
            Console.WriteLine("No playable card in your hand!");
            return false;
        }

        private void SelectCard()
        {
            Console.WriteLine($"Choose a card from 1 to {CurrentPlayer.Hand.Count()} to play, or draw a card by pressing 0");
            int choice;
            bool validInput;
            do
            {
                validInput = int.TryParse(Console.ReadLine(), out choice);
            } while (!validInput || choice < 0 || choice > CurrentPlayer.Hand.Count());

            if (choice == 0)
            {
                handleDrawCard();
                return;
            }
            else
            {
                if (IsCardPlayable(CurrentPlayer.Hand[choice - 1]))
                {
                    PlayCard(choice - 1);
                    return;
                }
                SelectCard();
                Console.Clear();
                ShowGameState();
                ShowHand();
                return;
            }
        }

        private void PlayCard(int indexOfCard)
        {
            HandleSpecial(CurrentPlayer.Hand[indexOfCard]);
            Console.WriteLine($" {CurrentPlayer.Name} has played {CurrentPlayer.Hand[indexOfCard]}");
            CurrentSymbol = CurrentPlayer.Hand[indexOfCard].CardSymbol;
            //card effect?
            if (CurrentPlayer.Hand[indexOfCard].CardColor == Color.Black)
            {
                ChangeColorToPlayerChoice();
            }
            else
            {
                CurrentColor = CurrentPlayer.Hand[indexOfCard].CardColor;
            }
            DiscardPile.Add(CurrentPlayer.Hand[indexOfCard]);
            CurrentPlayer.Hand.RemoveAt(indexOfCard);
        }

        private void HandleSpecial(Card card)
        {
            if (card.CardSymbol == Enum.GetName(Specials.Add2))
            {
                DrawCards(deck.CardDeck, ListOfPlayers[GetNextPlayerIndex()], 2);
                specialEffectMessage = $"{ListOfPlayers[GetNextPlayerIndex()].Name} draws 2 cards";
            }
            if (card.CardSymbol == Enum.GetName(BlackSpecials.Add4))
            {
                SkipTurn = true;
                DrawCards(deck.CardDeck, ListOfPlayers[GetNextPlayerIndex()], 4);
                specialEffectMessage = $"{ListOfPlayers[GetNextPlayerIndex()].Name} draws 4 cards";
            }
            if (card.CardSymbol == Enum.GetName(Specials.Stop))
            {
                SkipTurn = true;
            }
            if (card.CardSymbol == Enum.GetName(Specials.SwitchDirection))
            {
                specialEffectMessage = "Direction has switched!";
                PlayDirectionClockwise ^= true;
            }
        }

        //need to format cw
        public void ChangeColorToPlayerChoice()
        {
            int i = 1;
            Console.WriteLine("Choose a color:");
            foreach (Color color in Enum.GetValues(typeof(Color)))
            {
                if (color != Color.Black)
                {
                    Console.Write($"{i}: {color}");
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

        private void handleDrawCard()
        {
            DrawCards(deck.CardDeck, CurrentPlayer, 1);
            Console.WriteLine($"Your drawn card is {CurrentPlayer.Hand[CurrentPlayer.Hand.Count() - 1]}");
            if (IsCardPlayable(CurrentPlayer.Hand[CurrentPlayer.Hand.Count() - 1]))
            {
                string answer = "";

                while (answer?.ToUpper() != "Y" && answer?.ToUpper() != "Y")
                {
                    Console.WriteLine("Deze kaart is speelbaar, wil je hem spelen? Y ? N");
                    answer = Console.ReadLine();
                }

                if (answer?.ToUpper() == "Y")
                {
                    PlayCard(CurrentPlayer.Hand.Count() - 1);
                }
            }
        }

        public bool IsCardPlayable(Card card)
        {
            if (card.CardSymbol == CurrentSymbol || card.CardColor == CurrentColor || card.CardColor == Color.Black)
            {
                return true;
            }
            return false;
        }


        public void DrawCards(List<Card> cards, Player player, int amountOfCards)
        {
            for (int i = 0; i < amountOfCards; i++)
            {
                player.Hand.Add(cards[0]);
                cards.RemoveAt(0);
            }
        }

        private bool CheckForEmptyHand() => CurrentPlayer.Hand.Count == 0;

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
    }
}
// todo: switchdirection, uitspelen, black card on start, init 1 per 1 uitdelen, indexing hand, whitespace/formatting console., bart:kleurtjes geven, ascii art...