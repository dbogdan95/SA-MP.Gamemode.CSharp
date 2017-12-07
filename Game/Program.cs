using System;
using SampSharp.Core;

namespace Game
{
    class Program
    {
        static void Main(string[] args)
        {
            new GameModeBuilder()
                .Use<Game>()
                .UseStartBehaviour(GameModeStartBehaviour.FakeGmx)
                .Run();
        }
    }
}
