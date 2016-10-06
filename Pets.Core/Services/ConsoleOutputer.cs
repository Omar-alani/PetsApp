using Pets.Core.Infrastructure;
using System;

namespace Pets.Core.Services
{
    public class ConsoleOutputer : IOutputer
    {
        public void Output(string message)
        {
            Console.WriteLine(message);
        }
    }
}
