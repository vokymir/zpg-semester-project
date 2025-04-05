namespace zpg
{
    public class App
    {
        public static void Main(string[] args)
        {
            string levelPath = "./Levels/lvl01.txt";
            if (args.Length > 0)
            {
                levelPath = args[0];
            }
            using (var game = new Window(800, 600, "ZPG - epic DOOM-like almost game", levelPath))
            {
                game.Run();
            }
        }
    }
}
