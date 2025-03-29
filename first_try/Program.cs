namespace zpg
{
    public class App
    {
        public static void Main(string[] args)
        {
            using (var game = new Window(800, 600, "ZPG - epic DOOM-like almost game"))
            {
                game.Run();
            }
        }
    }
}
