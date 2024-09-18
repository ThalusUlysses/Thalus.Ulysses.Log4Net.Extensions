
using Thalus.Ulysses.Log4Net.Extensions;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config")]

namespace Thalus.Ulysses.Log4Net.TestApp
{
    internal class Program
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
#pragma warning disable CS0168 // Variable is declared but never used
            // This is to fix Microsoft idiocracy in not copying all dependencies to the output folder
            RemoteLogAppenderExample app;
#pragma warning restore CS0168 // Variable is declared but never used

            Console.WriteLine("Hello, World!");

            // Expect that the IPAddress is scrapped
            logger.Info("This is a low hanging Fruit with Log4Net making it to a structured like approach example 192.1.6.0 with a scrapped IP address for pi reasons", new Exception());
            // Expect that the url is scapped
            logger.Info("sdfsdfsdf http://las.com/asdfsadfsd/asfsdf sdfsidufhoisdfn sdfnosdifnsdf ");

            // Expect user password and url scrapped
            logger.Info("sdfsdfsdf http://las.com/asdfsadfsd/asfsdf sdfsidufhoisdfn password=123456789987654321321654987789945 user IamEagle");

            // Expect "InTextErrorIndication": true
            logger.Info("sdfsdfjkbsudfbsldfb Error");
        }
    }
}