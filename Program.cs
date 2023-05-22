using System;

namespace EE
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Game1())
            {
                game.InitializeGraphics();
                game.Run();
            }
        }
    }
}
