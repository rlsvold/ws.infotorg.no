using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

using System.Net;
using System.Net.Security;
using System.IO;
using System.Xml;
using System.Threading;
using System.Security.Cryptography.X509Certificates;


namespace SoapIntegration
{
    public class TMmanually
    {
        // Protocols used in endpoints and namespaces.
        private const string TM_PROTOCOL_ENDPOINT = "https://";
        private const string TM_PROTOCOL_SCHEMA = "http://";

        // Main adress to environment
        private const string TM_TEST = "ws-test.infotorg.no"; // Testing against EVRYS test portal
        private const string TM_PROD = "ws.infotorg.no"; // Setting up code against EVRYS production portal
        private const string TM_LOCAL = "tmlocal:8001"; //typical your own local environment.       

        //Part of endpoint, depending on the environment.
        private const string ENDPOINT_DSF = "/ws/ErgoGroup/DetSentraleFolkeregister1_4.pl";
        //Part of endpoint, depending on the environment.
        private const string ENDPOINT_LOGIN = "/ws/Admin/Brukersesjon.pl";

        private TMmanually.Environment environment;

        public string Endpoint_dsf { get; set; }
        public string EndpointLogin { get; set; }
        public string SoapAction { get; set; }
        public bool useCert { get; set; }

        public string TmSchemaUrl { get; set; }

        public TMmanually(Environment env)
        {
            //Builds environment
            switch (env)
            {
                case Environment.Local:
                    Endpoint_dsf = TM_PROTOCOL_SCHEMA + TM_LOCAL + ENDPOINT_DSF;// The Endpoint i usually not https in local TM environment.
                    EndpointLogin = TM_PROTOCOL_SCHEMA + TM_LOCAL + ENDPOINT_LOGIN;// The Endpoint i usually not https in local TM environment.
                    break;
                case Environment.Test:
                    Endpoint_dsf = TM_PROTOCOL_ENDPOINT + TM_TEST + ENDPOINT_DSF;
                    EndpointLogin = TM_PROTOCOL_ENDPOINT + TM_TEST + ENDPOINT_LOGIN;
                    break;
                case Environment.Prod:
                    Endpoint_dsf = TM_PROTOCOL_ENDPOINT + TM_PROD + ENDPOINT_DSF;
                    EndpointLogin = TM_PROTOCOL_ENDPOINT + TM_PROD + ENDPOINT_LOGIN;
                    break;
            }

            TmSchemaUrl = TM_PROTOCOL_SCHEMA + TM_PROD; //Must use prod url in schema ns declaration  
        }

        //Setting environment for your integration.
        public enum Environment { Local = 0, Test = 1, Prod = 2 }

        /// <summary>
        /// Main method for integration with TM, DSF Services.
        /// </summary>
        /// <param name="env">Environment</param>
        /// <param name="passord">passord</param>
        /// <param name="brukernavn">brukernavn</param>
        /// <param name="systemNavn">systemNavn</param>
        /// <param name="produkt">produkt</param>
        /// <param name="serachExpression">serachExpression</param>
        public string ManuallyRequestTM(UserSession us, string produkt, string serachExpression)
        {
            try
            {
                string[] expressions;
                expressions = serachExpression.Split('$');
                string endpoint = string.Empty;   

                //Builds XML request
                StringBuilder xmlReq = new StringBuilder();
                if (produkt.ToUpper() == "LOGIN")
                {
                    xmlReq = TMrequests.GetLogInRequest(TmSchemaUrl, us);
                    endpoint = EndpointLogin;
                }
                if (produkt.ToUpper() == "LOGOUT")
                {
                    xmlReq = TMrequests.GetLogOutRequest(TmSchemaUrl, us);
                    endpoint = EndpointLogin;
                }
                else if (produkt.ToUpper() == "ANTALLINNBYGGERE")
                {
                    xmlReq = TMrequests.GetAntallInnbyggereRequest(TmSchemaUrl, us, expressions[0]);
                    endpoint = Endpoint_dsf;
                }
                else if (produkt.ToUpper() == "DETALJER")
                {
                    xmlReq = TMrequests.GetDetaljerRequest(TmSchemaUrl, us, expressions[0], expressions[1], expressions[2]);
                    endpoint = Endpoint_dsf;
                }              

                if (string.IsNullOrEmpty(xmlReq.ToString()))
                {
                    Console.WriteLine("ERROR: Product is not suported in this test!");
                    return string.Empty;
                }

                string request = xmlReq.ToString();

                //Print to console
                Console.WriteLine("=====================================");
                Console.WriteLine("XML REQUEST " + produkt + ":");
                Console.WriteLine(request);
                Console.WriteLine("===================================== \r\n");

                Console.WriteLine("Request will be sent, wait for response........");

                // Sending the request
                HttpWebResponse response = SendRequest(request, SoapAction, endpoint, useCert); 

                // Getting the respone stream
                Stream streamResponse = response.GetResponseStream();

                // Convert the response stream to string for printing
                StreamReader streamRead = new StreamReader(streamResponse);
                string responseString = streamRead.ReadToEnd();
                Console.WriteLine("=====================================");
                Console.WriteLine("XML RESPONSE " + produkt + ":");
                Console.WriteLine(responseString);
                Console.WriteLine(Util.GetXmlProps(responseString));
                Console.WriteLine("===================================== \r\n");


                // Close the stream object.
                streamResponse.Close();
                streamRead.Close();

                // Release the HttpWebResponse.
                response.Close();

                return responseString;
            }
            catch (WebException we)
            {
                Console.WriteLine("Feil " + GetExceptionDetail.GetException(we));
                return string.Empty;
            }
            catch (Exception e)
            {
                Console.WriteLine("Feil " + GetExceptionDetail.GetException(e));
                return string.Empty;
            }

        }


        private static bool AlwaysGoodCert(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors)
        {
            return true;
        }

        private static X509Certificate2Collection GetCerts()
        {
            //  certificate and password
            var certificate_str = ConfigurationManager.AppSettings["certificate"];
            var password = ConfigurationManager.AppSettings["password"];
            var certificates = new X509Certificate2Collection();
            certificates.Import(certificate_str, password, X509KeyStorageFlags.DefaultKeySet);

            return certificates;
        }

        private static X509Certificate2 GetCert()
        {
            //  certificate and password
            var certificate_str = ConfigurationManager.AppSettings["certificate"];
            var password = ConfigurationManager.AppSettings["password"];
            var certificates = new X509Certificate2Collection(new X509Certificate2(certificate_str, password));
            var certificate = new X509Certificate2(certificate_str, password);

            return certificate;
        }

        /// <summary>
        /// Sending the request
        /// </summary>
        /// <param name="soapmessage">The XML request</param>
        /// <param name="SOAPAction">SOAPAction in this case, no action</param>
        /// <param name="endpoint">endpoint</param>
        /// <returns>HttpWebResponse</returns>
        public static HttpWebResponse SendRequest(string soapmessage, string SOAPAction, string endpoint, bool useCert)
        {           
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(endpoint);
            //request.ClientCertificates.Add(certs[2]);
            if (useCert) request.ClientCertificates.Add(GetCert());
            request.Timeout = 1 * 60 * 1000;
            request.Method = "POST";
            request.ContentType = "text/xml;charset=\"UTF-8\"";//ISO-8859-1
            request.Headers.Add("SOAPAction", SOAPAction);

            if (useCert)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(AlwaysGoodCert);
            }


            StreamWriter myWriter = null;
            try
            {
                myWriter = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);
                myWriter.Write(soapmessage);
            }
            catch (Exception e)
            {

                throw new WebException(e.Message, e);
            }
            finally
            {

                if (myWriter != null)
                {
                    myWriter.Flush();
                    myWriter.Close();
                }
            }

            return (HttpWebResponse)request.GetResponse();

        }
    }
}
