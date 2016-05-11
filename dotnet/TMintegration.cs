using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;


namespace SoapIntegration
{
    /// <summary>
    /// Main class for testing TM integration
    /// </summary>
    public class TMintegration
    {
        // Remember to use a user with the necessary privileges 
        private const TMmanually.Environment environment = TMmanually.Environment.Test; // environment you want to test against

        static void Main(string[] args)
        {
            UserSession us = GetUserSession();

            // Testing manually integration with TM
            Console.WriteLine("\nDo you want to run a manually soap request against TM [Y/N]?");
            var input = Console.ReadLine();
            if (input.ToUpper().Equals("Y"))
            {
                TestManually.ManuallyRequest(us, environment);
            }

            // Testing reference integration with TM
            Console.WriteLine("\nDo you want to run a soap request through \"Web Reference\" against TM [Y/N]?");
            input = Console.ReadLine();
            if (input.ToUpper().Equals("Y"))
            {
                TestProxy.WebReferenceRequest(us);
            }

            //Dsf4DetaljerManually();

            Console.WriteLine("\n=============================================================\nDone testing, hit enter to quit!");
            Console.In.Read();
        }

        private static UserSession GetUserSession()
        {
            UserSession us = new UserSession();

            string username = System.Configuration.ConfigurationManager.AppSettings["username"];
            if (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("\nType in your username -> hit enter?");
                username = Console.ReadLine();
            }
            us.brukernavn = username.ToUpper();

            string password = System.Configuration.ConfigurationManager.AppSettings["password"];
            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("\nType in your password -> hit enter?");
                password = Console.ReadLine();
            }
            us.passord = password.ToUpper();

            string sysname = System.Configuration.ConfigurationManager.AppSettings["sysname"];
            if (string.IsNullOrEmpty(sysname))
            {
                Console.WriteLine("\nType in your system name(Max 11 chr) -> hit enter?");
                sysname = Console.ReadLine();
            }
            us.systemnavn = sysname;

            us.distribusjonskanal = Brukersesjon.distribusjonskanal.PTP;
            return us;
        }         
    }
}
