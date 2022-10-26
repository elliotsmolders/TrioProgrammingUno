using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrioProgrammingUno.Business
{
    internal class Logger
    {
        // TODO Decouple from Console
        public void WriteLine(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.Cyan : color;
            Console.WriteLine(message);
        }
        public void Write(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color == ConsoleColor.Black ? ConsoleColor.Cyan : color;
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
