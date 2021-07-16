using System;
using myIT_Workplace_ConfigParser;

namespace myIT_Workplace_ConfigParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ConfigParser2 parser = new ConfigParser2();

            parser.parseConfig();
        }
    }
}
