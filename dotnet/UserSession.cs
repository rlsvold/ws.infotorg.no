using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Xml.Serialization;

namespace SoapIntegration
{
    public class UserSession
    {

        [SoapElement("distribusjonskanal", DataType = "Brukersesjon.distribusjonskanal")]
        public Brukersesjon.distribusjonskanal distribusjonskanal;

        [SoapElement("systemnavn", DataType = "string")]
        public string systemnavn;

        [SoapElement("brukernavn", DataType = "string")]
        public string brukernavn;

        [SoapElement("passord", DataType = "string")]
        public string passord;

        [SoapElement("sessionId", DataType = "string")]
        public string sessionId;

        //Autorisasjon: tjeneste:WebServices produkt:Tilgang
        public static Brukersesjon.Brukersesjon LoginnTM(UserSession us)
        {
            Brukersesjon.loggInn logInnParam = new Brukersesjon.loggInn();
            logInnParam.distribusjonskanal = us.distribusjonskanal;
            logInnParam.systemnavn = us.systemnavn;
            logInnParam.ItemsElementName = new Brukersesjon.ItemsChoiceType1[2];
            logInnParam.ItemsElementName[0] = Brukersesjon.ItemsChoiceType1.brukernavn;
            logInnParam.ItemsElementName[1] = Brukersesjon.ItemsChoiceType1.passord;
            logInnParam.Items = new String[2];
            logInnParam.Items.SetValue(us.brukernavn, 0);
            logInnParam.Items.SetValue(us.passord, 1);
            //logInnParam.delegertBrukernavn = delegertBruker;

            try
            {
                SoapIntegration.Brukersesjon.brukersesjonPortTypeClient bptc = new Brukersesjon.brukersesjonPortTypeClient("brukersesjonPort");
                Console.WriteLine("Logging in");
                SoapIntegration.Brukersesjon.Transaksjon t = new Brukersesjon.Transaksjon();
                SoapIntegration.Brukersesjon.Autorisasjon auto = new Brukersesjon.Autorisasjon();
                SoapIntegration.Brukersesjon.Transaksjonsinfo ti = new Brukersesjon.Transaksjonsinfo();
                SoapIntegration.Brukersesjon.Brukersesjon bsl = bptc.loggInn(t, logInnParam, out ti, out auto);
                Console.WriteLine("Logged in");

                SoapIntegration.Brukersesjon.TransaksjonsinfoSammendrag sammendrag = ti.sammendrag;
                string sesjonsid = bsl.sesjonsid;
                return bsl;
            }
            catch (FaultException<Brukersesjon.feil> e)
            {
                Console.WriteLine("Feil " + GetExceptionDetail.GetException(e));
                return null;
            }
            catch (FaultException unknownFault)
            {
                Console.WriteLine("An unknown exception was received. " + unknownFault.Message);
                return null;
            }
            catch (CommunicationException commProblem)
            {
                Console.WriteLine("Feil " + GetExceptionDetail.GetException(commProblem));
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Feil " + GetExceptionDetail.GetException(ex));
                return null;
            }
        }

        public static Brukersesjon.Brukersesjon MakeBrukersesjon(string sessionId, UserSession us)
        {
            Brukersesjon.Brukersesjon brukersesjon = new Brukersesjon.Brukersesjon();
            brukersesjon.distribusjonskanal = us.distribusjonskanal;
            brukersesjon.systemnavn = us.systemnavn;
            brukersesjon.ItemsElementName = new Brukersesjon.ItemsChoiceType[2];
            brukersesjon.ItemsElementName[0] = Brukersesjon.ItemsChoiceType.brukernavn;
            brukersesjon.ItemsElementName[1] = Brukersesjon.ItemsChoiceType.passord;
            brukersesjon.Items = new String[2];
            brukersesjon.Items.SetValue(us.brukernavn, 0);
            brukersesjon.Items.SetValue(us.passord, 1);
            brukersesjon.sesjonsid = sessionId;
            return brukersesjon;
        }


        public static Object CopyUserSession(Brukersesjon.Brukersesjon source, Object target)
        {
            System.Reflection.PropertyInfo[] props = source.GetType().GetProperties();
            foreach (System.Reflection.PropertyInfo prop in props)
            {
                //Hvis prop er en klasse skal prop til klassen hentes. NB kun ett nivå!
                if (prop.PropertyType.IsClass)
                {
                    Object value = prop.GetValue(source, null);
                    String propertyName = prop.Name;
                    System.Reflection.PropertyInfo propertyTarget = target.GetType().GetProperty(propertyName);
                    if (propertyTarget != null)
                    {
                        propertyTarget.SetValue(target, value, null);
                    }
                }
            }
            return target;
        }
    }
}
