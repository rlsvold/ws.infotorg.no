using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoapIntegration
{
    public class TestManually
    {
        /// <summary>
        /// Test manually requests 
        /// </summary>
        /// <param name="passord">passord</param>
        /// <param name="brukernavn">brukernavn</param>
        /// <param name="systemnavn">systemnavn</param>
        public static void ManuallyRequest(UserSession us, TMmanually.Environment environment)
        {
            TMmanually tm = new TMmanually(environment);
            Console.WriteLine("\nDo you want to create an session against TM [Y/N]?");
            var input = Console.ReadLine();
            if (input.ToUpper().Equals("Y"))
            {
                string loginXml = tm.ManuallyRequestTM(us, "LOGIN", string.Empty);
                string sessionId = string.Empty;
                if (!string.IsNullOrEmpty(loginXml))
                {
                    sessionId = Util.RunXPath(loginXml, TMrequests.GetNameSpaces(tm.TmSchemaUrl));
                    if (!string.IsNullOrEmpty(sessionId))
                    {
                        us.sessionId = sessionId;
                    }
                }
            }
            Dsf4AntallInnbyggereManually(us, tm);
            Dsf4DetaljerManually(us, tm);
            
            tm.ManuallyRequestTM(us, "LOGOUT", string.Empty);
        }

        private static string Dsf4DetaljerManually(UserSession us, TMmanually tm)
        {
            Console.WriteLine("Do you want to run HenDetaljer(), against DSF1_4 [Y/N]?");
            var input = Console.ReadLine();
            if (input.ToUpper().Equals("Y"))
            {
                // Split search expressions with "$"
                string serachExpressions1 = "06069649236$$"; // Require access for given user to only search with fnr
                string serachExpressions2 = "06069649236$BARN$TEST";
                tm.ManuallyRequestTM(us, "DETALJER", serachExpressions1);
            }
            return input;
        }

        private static void Dsf4AntallInnbyggereManually(UserSession us, TMmanually tm)
        {
            Console.WriteLine("Do you want to run HentAntallInnbyggere(), against DSF1_4 [Y/N]?");
            var input = Console.ReadLine();
            if (input.ToUpper().Equals("Y"))
            {
                string serachExpressions = "0018";
                tm.ManuallyRequestTM(us, "ANTALLINNBYGGERE", serachExpressions);
            }
        }
    }
}
