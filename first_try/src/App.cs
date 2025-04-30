namespace zpg
{
    public class App
    {
        public static void Main(string[] args)
        {
            // try loading level from args, or default to predefined
            string levelPath = "./Levels/lvl00.txt";
            // string levelPath = "./Levels/lvlSimon01.txt.txt"; // this is just too much to handle
            if (args.Length > 0)
            {
                levelPath = args[0];
            }
            // start the game duh
            using (var game = new Window(800, 600, "ZPG", levelPath))
            {
                game.Run();
            }
        }
    }
}
