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
                Console.WriteLine($"Enter player {i+1}'s name:");
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

        private void PlayCard(int indexOfCard)
        {
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


        //need to format cw
        public void ChangeColorToPlayerChoice()
        {
            int i = 1;
            Console.WriteLine("Choose a color:");
            foreach (Color color in Enum.GetValues(typeof(Color)))
            {
                if(color != Color.Black)
                {
                    Console.Write($"{i}: {color}");
                    i++;
                }
            }
            int answer = 0;
            bool validInput = false;

            while (answer<=0 || answer> Enum.GetNames(typeof(Color)).Length || !validInput)
            {
                validInput = int.TryParse(Console.ReadLine(), out answer);
            }
            CurrentColor = (Color)answer;

        }

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
            while (true)
            {
                PlayerTurn(ListOfPlayers[indexOfCurrentPlayer]);
                if (PlayDirectionClockwise)
                {
                    if (!(indexOfCurrentPlayer < (ListOfPlayers.Count-1)))
                    {
                        indexOfCurrentPlayer++;
                    }
                    else
                    {
                        indexOfCurrentPlayer = 0;
                    }
                }
                else
                {
                    if(indexOfCurrentPlayer!=0){
                        indexOfCurrentPlayer--;

                    }
                    else
                    {
                        indexOfCurrentPlayer = ListOfPlayers.Count() - 1;
                    }
                }
            }

        }



        private void PlayerTurn(Player currentPlayer)
        {
            CurrentPlayer = currentPlayer;
            ShowGameState();
            ShowHand();
            if (CheckForPlayableCard())
            {
                SelectCard();
                CheckForEmptyHand();
            }
            else
            {
                handleDrawCard();
            }



        }

        private void handleDrawCard()
        {

            DrawCards(deck.CardDeck, CurrentPlayer, 1);
            Console.WriteLine($"Uw getrokken kaart is {CurrentPlayer.Hand[CurrentPlayer.Hand.Count() - 1]}");
            if (IsCardPlayable(CurrentPlayer.Hand[CurrentPlayer.Hand.Count() - 1]))
            {

                char answer = ' ';

                while (Char.ToUpper(answer) != 'Y' || Char.ToUpper(answer) != 'N')
                {
                    Console.WriteLine("Deze kaart is speelbaar, wil je hem spelen? Y ? N");
                    answer = (char)Console.Read();
                    
                }

                if (Char.ToUpper(answer) == 'Y')
                {

                }
               
            }
        }

        private void ShowGameState()
        {
            Console.WriteLine($"{CurrentPlayer} is aan de beurt:");
            Console.WriteLine($"Huidige kaart op tafel: {Environment.NewLine} --{DiscardPile[0]}--");
        }

        public bool IsCardPlayable(Card card)
        {
            if (card.CardSymbol == CurrentSymbol || card.CardColor == CurrentColor || card.CardColor == Color.Black)
            {
                return true;
            }
            return false;
        } 

        private void ShowHand()
        {
            foreach (Card card in CurrentPlayer.Hand)
            {
                Console.Write(card.ToString() + ", " );
            }
        }

        private bool CheckForEmptyHand() => CurrentPlayer.Hand.Count == 0;


        public bool CheckForPlayableCard()
        {

            List<Card> playableCards = CurrentPlayer.Hand.Where(x => IsCardPlayable(x)).ToList();
            if (playableCards.Count()!=0)
            {
                Console.WriteLine("uw speelbare kaarten zijn:");
                foreach (Card card in playableCards)
                {
                    Console.WriteLine(card.ToString());
                }
                return true;
            }
            return false;
        }

        // refactor: enkel kiezen uit playable card list
        // bugfix: kan alles inputten, geen foutmelding, ook juiste opties geen reactie.
        private void SelectCard()
        {
            Console.WriteLine($"Kies een kaart 1 tot {CurrentPlayer.Hand.Count()} om te spelen, of trek een kaart met 0");
            int choice;
            bool validInput;
            do
            {
            validInput = int.TryParse(Console.ReadLine(), out choice);
            } while (validInput||0<=choice||choice >CurrentPlayer.Hand.Count());

            if (choice == 0)
            {
              handleDrawCard();
            }
            else
            {
                PlayCard(choice - 1);
            }

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