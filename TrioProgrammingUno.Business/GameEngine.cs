namespace TrioProgrammingUno
{
    public class GameEngine
    {
        public int PlayerAmount { get; set; }
        public List<Player> ListOfPlayers { get; set; } = new List<Player>();
        public void DisplayMenu(MenuOptions choice)
        {
            switch (choice)
            {
                case MenuOptions.AmountOfPlayers:
                    AskForPlayers();
                    MakePlayers();
                    break;

                default:
                    break;
            }
        }

        //TODO: Dus, refactor = guard steken in property setter.
        private void AskForPlayers()
        {

            int players = 0;
            bool validInput = false;
            Console.WriteLine("Kies 1 tot 4 spelers");
            validInput = int.TryParse(Console.ReadLine(), out players);

            if (players <= 0 || players > 4 && validInput == false)
            {
                Console.WriteLine("Please enter a valid input");
                AskForPlayers();
            }
            PlayerAmount = players;
        }

        private void MakePlayers()
        {
            for (int i = 0; i < PlayerAmount; i++)
            {
                Player player = new(Console.ReadLine());
                ListOfPlayers.Add(player);
            }
        }
    }
}