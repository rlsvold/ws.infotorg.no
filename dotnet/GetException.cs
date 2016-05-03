using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.ServiceModel;

namespace SoapIntegration
{
    public class GetExceptionDetail
    {
        private Exception exception;
        private String feilmelding;

        public GetExceptionDetail(Exception e)
        {
            this.exception = e;
        }

        public GetExceptionDetail(String f)
        {
            this.feilmelding = f;
        }


        public static String GetException(Exception e)
        {
            String errMsg = "";

            object theType = e.GetType();
            if (e.GetType() == typeof(System.Web.Services.Protocols.SoapException))
            {
                errMsg += "\r\n**** SOAP EXCEPTION START ****";
                errMsg += "\r\n" + ((System.Web.Services.Protocols.SoapException)e).Detail.InnerText;
                errMsg += "**** SOAP EXCEPTION END ****\r\n";
            }

            else if (e.GetType() == typeof(System.Net.WebException))
            {
                errMsg += "\r\n**** WebException EXCEPTION START ****";
                var response = ((System.Net.WebException)e).Response as System.Net.HttpWebResponse;
                if (response != null)
                {
                    errMsg += "\r\n" + ("HTTP Status Code: " + (int)response.StatusCode);
                    errMsg += "\r\n" + ("HTTP Status Description: " + response.StatusDescription) + "\r\n";
                }
                errMsg += "**** WebException EXCEPTION END ****\r\n";
            }
            else if (e.GetType() == typeof(System.ServiceModel.FaultException<Brukersesjon.feil>))
            {
                errMsg += "\r\n********************* FaultException<Brukersesjon.feil> EXCEPTION START ****";
                var response = ((FaultException<Brukersesjon.feil>)e).Detail as Brukersesjon.feil;
                if (response != null)
                {
                    errMsg += "\r\n" + ("Feilmelding: " + response.feilmelding);
                    errMsg += "\r\n" + ("Feilgruppekode: " + response.feilgruppekode);
                    errMsg += "\r\n" + ("Feilgruppetekst: " + response.feilgruppetekst);
                    errMsg += "\r\n" + ("Feilkode: " + response.feilkode);
                    errMsg += "\r\n" + ("Feiltekst: " + response.feiltekst);
                    errMsg += "\r\n" + ("XML Any: " + response.Any ?? string.Empty) + "\r\n";
                }
                errMsg += "********************* FaultException<Brukersesjon.feil> EXCEPTION END ****\r\n";
            }           
            
            errMsg += "\r\n**** EXCEPTION START ****";
            errMsg += "\r\nMESSAGE: " + e.Message;
            errMsg += "\r\nSOURCE: " + e.Source;
            errMsg += "\r\nTARGET: " + e.TargetSite;
            errMsg += "\r\nSTACK: " + e.StackTrace;
            errMsg += ("\r\n****  EXCEPTION END  ****");

            if (e.InnerException != null)
            {
                errMsg += "\r\n";
                errMsg += ("\r\n**** INNEREXCEPTION START ****");
                errMsg += ("\r\nINNEREXCEPTION MESSAGE: " + e.InnerException.Message);
                errMsg += ("\r\nINNEREXCEPTION SOURCE: " + e.InnerException.Source);
                errMsg += ("\r\nINNEREXCEPTION STACK: " + e.InnerException.StackTrace);
                errMsg += ("\r\nINNEREXCEPTION TARGETSITE: " + e.InnerException.TargetSite);
                errMsg += ("\r\n****  INNEREXCEPTION END  ****");
            }
            return errMsg;
        }

        public static String hentSqlException(SqlException e)
        {
            StringBuilder errMessage = new StringBuilder();
            for (int i = 0; i < e.Errors.Count; i++)
            {
                errMessage.Append("Feilindex #" + i + "\n" +
                    "Message" + e.Errors[i].Message + "\n" +
                    "LineNumber" + e.Errors[i].LineNumber + "\n" +
                    "Source" + e.Errors[i].Source + "\n" +
                    "Procedure" + e.Errors[i].Procedure + "\n");
            }
            return errMessage.ToString();
        }
    }
}
