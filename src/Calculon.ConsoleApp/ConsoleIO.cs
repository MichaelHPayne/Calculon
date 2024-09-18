using Calculon.Core.Interfaces;

namespace Calculon.ConsoleApp
{
    public class ConsoleIO : IConsoleIO
    {
        public string ReadLine() => Console.ReadLine();
        public void WriteLine(string message) => Console.WriteLine(message);
        public void Write(string message) => Console.Write(message);
    }
}
