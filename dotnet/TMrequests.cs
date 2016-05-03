using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SoapIntegration
{
    public static class TMrequests
    {
        //Part of namespaces, depending on the environment.
        private const string XMLNS_ENVELOPE = "\"http://schemas.xmlsoap.org/soap/envelope/\"";
        private const string XMLNS_USERSESSION = "/xml/Admin/Brukersesjon/2006-07-07/Brukersesjon.xsd";
        private const string XMLNS_TRANSACTION = "/xml/Admin/Transaksjon/2006-07-07/Transaksjon.xsd";
        private const string XMLNS_DSF = "/xml/ErgoGroup/DetSentraleFolkeregister1_4/2011-09-26/DetSentraleFolkeregister1_4.xsd";
        private const string quote = "\"";

        public static StringBuilder GetAntallInnbyggereRequest(string tmUrl, UserSession us, string Kommnr)
        {
            StringBuilder xmlReq = new StringBuilder();
            xmlReq.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xmlReq.Append("<soap:Envelope xmlns:soap=" + XMLNS_ENVELOPE + " xmlns:brukersesjon=" + quote + tmUrl + XMLNS_USERSESSION + quote + " xmlns:transaksjon=" + quote + tmUrl + XMLNS_TRANSACTION + quote + " xmlns:dsf=" + quote + tmUrl + XMLNS_DSF + quote + ">");
            xmlReq.Append(GetBrukersesjonRequest(us));
            xmlReq.Append("<soap:Body>");
            xmlReq.Append("<dsf:hentAntallInnbyggere>");
            xmlReq.Append("<saksref/>");
            xmlReq.Append("<kommunenr>" + Kommnr + "</kommunenr>");
            xmlReq.Append("</dsf:hentAntallInnbyggere>");
            xmlReq.Append("</soap:Body>");
            xmlReq.Append("</soap:Envelope>");
            return xmlReq;
        }

        public static StringBuilder GetDetaljerRequest(string tmUrl, UserSession us, string fnr, string fNname, string eName)
        {
            StringBuilder xmlReq = new StringBuilder();
            xmlReq.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xmlReq.Append("<soap:Envelope xmlns:soap=" + XMLNS_ENVELOPE + " xmlns:brukersesjon=" + quote + tmUrl + XMLNS_USERSESSION + quote + " xmlns:transaksjon=" + quote + tmUrl + XMLNS_TRANSACTION + quote + " xmlns:dsf=" + quote + tmUrl + XMLNS_DSF + quote + ">");
            xmlReq.Append(GetBrukersesjonRequest(us));
            xmlReq.Append("<soap:Body>");
            xmlReq.Append("<dsf:hentDetaljer>");
            xmlReq.Append("<saksref/>");
            if (!string.IsNullOrEmpty(fnr)) xmlReq.Append("<foedselsnr>" + fnr + "</foedselsnr>");
            if (!string.IsNullOrEmpty(eName)) xmlReq.Append("<etternavn>" + eName + "</etternavn>");
            if (!string.IsNullOrEmpty(fNname)) xmlReq.Append("<fornavn>" + fNname + "</fornavn>");
            xmlReq.Append("<fornavnnr>B</fornavnnr>");
            xmlReq.Append("<fonetiskNavn>J</fonetiskNavn>");
            xmlReq.Append("<fonetiskAdresse>J</fonetiskAdresse>");
            xmlReq.Append("<ikkeDoed>J</ikkeDoed>");
            xmlReq.Append("<foedselsaarTil></foedselsaarTil>");
            xmlReq.Append("<alderFra></alderFra>");
            xmlReq.Append("<alderTil></alderTil>");
            xmlReq.Append("</dsf:hentDetaljer>");
            xmlReq.Append("</soap:Body>");
            xmlReq.Append("</soap:Envelope>");
            return xmlReq;
        }

        public static string GetBrukersesjonRequest(UserSession us)
        {
            if (!string.IsNullOrEmpty(us.sessionId))
            {
                return GetBrukersesjonRequestSessionId(us);
            }
            StringBuilder xmlReq = new StringBuilder();
            xmlReq.Append("<soap:Header>");
            xmlReq.Append("<brukersesjon:Brukersesjon>");
            xmlReq.Append("<distribusjonskanal>" + us.distribusjonskanal + "</distribusjonskanal>");
            xmlReq.Append("<systemnavn>" + us.systemnavn + "</systemnavn>");
            xmlReq.Append("<brukernavn>" + us.brukernavn + "</brukernavn>");
            xmlReq.Append("<passord>" + us.passord + "</passord>");
            xmlReq.Append("</brukersesjon:Brukersesjon>");
            xmlReq.Append("</soap:Header>");
            return xmlReq.ToString();
        }

        public static string GetBrukersesjonRequestSessionId(UserSession us)
        {
            StringBuilder xmlReq = new StringBuilder();
            xmlReq.Append("<soap:Header>");
            xmlReq.Append("<brukersesjon:Brukersesjon>");
            //xmlReq.Append("<distribusjonskanal>" + us.distribusjonskanal + "</distribusjonskanal>");
            xmlReq.Append("<sesjonsid>" + us.sessionId + "</sesjonsid>");
            xmlReq.Append("</brukersesjon:Brukersesjon>");
            xmlReq.Append("</soap:Header>");
            return xmlReq.ToString();
        }

        public static StringBuilder GetLogInRequest(string tmUrl, UserSession us)
        {
            
            StringBuilder xmlReq = new StringBuilder();
            xmlReq.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xmlReq.Append("<soap:Envelope xmlns:soap=" + XMLNS_ENVELOPE + " xmlns:transaksjon=" + quote + tmUrl + XMLNS_TRANSACTION + quote + ">");
            xmlReq.Append("<soap:Body>");
            xmlReq.Append("<brukersesjon:loggInn xmlns:brukersesjon=" + quote + tmUrl + XMLNS_USERSESSION + quote + ">");
            xmlReq.Append("<distribusjonskanal>" + us.distribusjonskanal.ToString() + "</distribusjonskanal>");
            xmlReq.Append("<systemnavn>" + us.systemnavn + "</systemnavn>");
            xmlReq.Append("<brukernavn>" + us.brukernavn + "</brukernavn>");
            xmlReq.Append("<passord>" + us.passord + "</passord>");
            //xmlReq.Append("<!--<delegertBrukernavn></delegertBrukernavn>-->");
            xmlReq.Append("</brukersesjon:loggInn>");
            xmlReq.Append("</soap:Body>");
            xmlReq.Append("</soap:Envelope>");
            return xmlReq;
        }

        public static StringBuilder GetLogOutRequest(string tmUrl, UserSession us)
        {            
            StringBuilder xmlReq = new StringBuilder();
            xmlReq.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            xmlReq.Append("<soap:Envelope xmlns:soap=" + XMLNS_ENVELOPE + " xmlns:brukersesjon=" + quote + tmUrl + XMLNS_USERSESSION + quote + " xmlns:transaksjon=" + quote + tmUrl + XMLNS_TRANSACTION + quote + ">");
            xmlReq.Append(GetBrukersesjonRequest(us));
            xmlReq.Append("<soap:Body>");
            xmlReq.Append("<brukersesjon:loggUt/>");
            xmlReq.Append("</soap:Body>");
            xmlReq.Append("</soap:Envelope>");
            return xmlReq;          
        }

        public static Hashtable GetNameSpaces(string tmUrl)
        {
            Hashtable namespaces = new Hashtable();
            namespaces.Add("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            namespaces.Add("brukersesjon", tmUrl + XMLNS_USERSESSION);
            namespaces.Add("Autorisasjon", tmUrl + XMLNS_USERSESSION);
            return namespaces;
        }
    }
}
