namespace Calculon.Core.Interfaces
{
    public interface IConsoleIO
    {
        string ReadLine();
        void WriteLine(string message);
        void Write(string message);
    }
}
