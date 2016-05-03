using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoapIntegration
{
    public class TestProxy
    {
        /// <summary>
        /// Main test method for "Web reference" and "Service reference"
        /// </summary>
        /// <param name="distribusjonskanal">Brukersesjon.distribusjonskanal</param>
        /// <param name="us">UserSession</param>
        public static void WebReferenceRequest(UserSession us)
        {            
            // The Brukersesjon object (User Session) is created with "Service Reference".
            Brukersesjon.Brukersesjon bs = UserSession.LoginnTM(us);
            if (bs != null)
            {
                Console.WriteLine("Do you want to run HentAntallInnbyggere(), against DSF1_4 [Y/N]?");
                var input = Console.ReadLine();
                if (input.ToUpper().Equals("Y"))
                {
                    Brukersesjon.Brukersesjon newBs = UserSession.MakeBrukersesjon(bs.sesjonsid, us);
                    Dsf4AntallInnbyggere(newBs);
                }

                Console.WriteLine("Do you want to run HenDetaljer(), against DSF1_4 [Y/N]?");
                input = Console.ReadLine();
                if (input.ToUpper().Equals("Y"))
                {
                    string dsfversion = string.Empty;
                    Brukersesjon.Brukersesjon newBs = UserSession.MakeBrukersesjon(bs.sesjonsid, us);
                    Console.WriteLine("Do you want to run the latest version (2015_08_10), against DSF1_4 [Y/N]?");
                    input = Console.ReadLine();                    
                    if (input.ToUpper().Equals("Y")) dsfversion = "2015_08_10";                   
                    Dsf4Detaljer(newBs,dsfversion);
                }
            }
            
        }



        private static void Dsf4AntallInnbyggere(Brukersesjon.Brukersesjon bs)
        {
            string serachExpressions = "0018";
            if (!string.IsNullOrEmpty(bs.sesjonsid))
                TestDSF4AntallInnbyggere((Dsf4.Brukersesjon)UserSession.CopyUserSession(bs, new Dsf4.Brukersesjon()), serachExpressions);
        }

        private static void Dsf4Detaljer(Brukersesjon.Brukersesjon bs, string version)
        {
            string serachExpressions = "15047049145$B-GJENGEN$ITALIASKURK"; //SKATT official test data
            if (!string.IsNullOrEmpty(bs.sesjonsid))
            {
                if (version == "2015_08_10")
                {
                    TestDSF4Detaljer_2015_08_10((Dsf4_v_2015_08_10.Brukersesjon)UserSession.CopyUserSession(bs, new Dsf4_v_2015_08_10.Brukersesjon()), serachExpressions);
                }
                else
                {
                    TestDSF4Detaljer((Dsf4.Brukersesjon)UserSession.CopyUserSession(bs, new Dsf4.Brukersesjon()), serachExpressions);
                }
            }
        }

        




        //Autorisasjon: tjeneste:Folkeregisteret produkt:Treffliste
        private static void TestDSF4AntallInnbyggere(Dsf4.Brukersesjon bs, string serachExpression)
        {
            // The Dsf4 object is created with "Web Reference".
            Dsf4.DSFService dsf = new Dsf4.DSFService();
            dsf.BrukersesjonValue = bs;

            Dsf4.hentAntallInnbyggere antallInnbyggereParams = new Dsf4.hentAntallInnbyggere();
            string[] expressions;
            expressions = serachExpression.Split('$');
            antallInnbyggereParams.kommunenr = expressions[0];

            Dsf4.AntallInnbyggere svar;
            try
            {
                svar = dsf.hentAntallInnbyggere(antallInnbyggereParams);
                Console.WriteLine("SVAR DSF AntallInnbyggere: " + svar.RESULT.ANTALL);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Feil " + GetExceptionDetail.GetException(ex));
            }
        }

        //Autorisasjon: tjeneste:Folkeregisteret produkt:Treffliste
        private static void TestDSF4Detaljer(Dsf4.Brukersesjon bs, string serachExpression)
        {
            // The Dsf4 object is created with "Web Reference".
            Dsf4.DSFService dsf = new Dsf4.DSFService();
            dsf.BrukersesjonValue = bs;

            Dsf4.personsoek detaljerParams = new Dsf4.personsoek();
            string[] expressions;
            expressions = serachExpression.Split('$');
            detaljerParams.foedselsnr = expressions[0];
            detaljerParams.fornavn = expressions[1];
            detaljerParams.etternavn = expressions[2];

            Dsf4.Detaljer svar;
            try
            {
                svar = dsf.hentDetaljer(detaljerParams);
                const string RESP = "INGEN TREFF";
                string navn = string.Empty;
                if (svar.RESULT != null && svar.RESULT.HOV != null)
                    navn = "Treff på detaljer (her navn) for denne peronen: " + svar.RESULT.HOV.NAVN ?? RESP;
                else
                    navn = RESP;

                Console.WriteLine("SVAR DSF Detaljer: " + navn);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Feil " + GetExceptionDetail.GetException(ex));
            }
        }

        //Autorisasjon: tjeneste:Folkeregisteret produkt:Treffliste
        private static void TestDSF4Detaljer_2015_08_10(Dsf4_v_2015_08_10.Brukersesjon bs, string serachExpression)
        {
            // The Dsf4 object is created with "Web Reference".
            Dsf4_v_2015_08_10.DSFService dsf = new Dsf4_v_2015_08_10.DSFService();
            //Dsf4.DSFService dsf = new Dsf4.DSFService();
            dsf.BrukersesjonValue = bs;

            Dsf4_v_2015_08_10.personsoek detaljerParams = new Dsf4_v_2015_08_10.personsoek();
            string[] expressions;
            expressions = serachExpression.Split('$');
            detaljerParams.foedselsnr = expressions[0];
            detaljerParams.fornavn = expressions[1];
            detaljerParams.etternavn = expressions[2];

            Dsf4_v_2015_08_10.Detaljer svar;
            try
            {
                svar = dsf.hentDetaljer(detaljerParams);
                const string RESP = "INGEN TREFF";
                string navn = string.Empty;
                if (svar.RESULT != null && svar.RESULT.HOV != null)
                    navn = "Treff på detaljer (her navn) for denne peronen: " + svar.RESULT.HOV.NAVN ?? RESP;
                else
                    navn = RESP;

                Console.WriteLine("SVAR DSF Detaljer: " + navn);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Feil " + GetExceptionDetail.GetException(ex));
            }
        }
    }
}
