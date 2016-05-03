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
        private const string Brukernavn = "xxxxxx"; //Add your username
        private const string Passord = "xxxxxx";      //Add your password
        private const Brukersesjon.distribusjonskanal Distribusjonskanal = Brukersesjon.distribusjonskanal.PTP;
        private const string Systemnavn = "RSKJTEST";//Add your sysname
        private const TMmanually.Environment environment = TMmanually.Environment.Test; // environment you want to test against

        static void Main(string[] args)
        {
            UserSession us = new UserSession();
            us.brukernavn = Brukernavn;
            us.passord = Passord;
            us.systemnavn = Systemnavn;
            us.distribusjonskanal = Distribusjonskanal;

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
    }
}
