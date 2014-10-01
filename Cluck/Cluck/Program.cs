using System;

namespace Cluck
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Cluck game = new Cluck())
            {
                game.Run();
            }
        }
    }
#endif
}

