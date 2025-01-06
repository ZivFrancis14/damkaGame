namespace Ex02
{
    internal class Program
    {
        public static void Main()
        {
            playDamkaGame();
        }
        private static void playDamkaGame()
        {
            ConsoleGame damkaGame = new ConsoleGame();
            damkaGame.StartGame();
        }
    }
}
