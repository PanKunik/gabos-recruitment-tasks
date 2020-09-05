using System;
using System.IO;
using System.Net;
using System.Xml;

namespace ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Request request = new Request();

            request.Login();

            request.NipNumbers.Add("6770065406");
            request.NipNumbers.Add("9492107026");

            request.FindCompanies();

            Console.ReadLine();
        }
    }
}
