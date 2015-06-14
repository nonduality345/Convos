using System;

namespace Convos.Utilities
{
    /// <summary>
    /// A very basic logging class to illustrate the architecture
    /// </summary>
    public class Logger : ILogger
    {
        public void LogMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}