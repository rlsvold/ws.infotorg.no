using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Schema;

namespace SoapIntegration
{
    public class Util
    {
        /// <summary>
        /// Getting XML properties
        /// </summary>
        /// <param name="xmlDoc"></param>
        public static string GetXmlProps(string xmlDoc)
        {

            int ws = 0;
            int pi = 0;
            int dc = 0;
            int cc = 0;
            int ac = 0;
            int et = 0;
            int el = 0;
            int xd = 0;
            // Read a document
            XmlTextReader textReader = new XmlTextReader( new System.IO.StringReader(xmlDoc));

            // Read until end of file
            while (textReader.Read())
            {

                XmlNodeType nType = textReader.NodeType;

                // If node type us a declaration
                if (nType == XmlNodeType.XmlDeclaration)
                {
                    Console.WriteLine("Declaration:" + textReader.Name.ToString());
                    xd = xd + 1;
                }
                // if node type is a comment
                if (nType == XmlNodeType.Comment)
                {
                    Console.WriteLine("Comment:" + textReader.Name.ToString());
                    cc = cc + 1;
                }
                // if node type us an attribute
                if (nType == XmlNodeType.Attribute)
                {
                    Console.WriteLine("Attribute:" + textReader.Name.ToString());
                    ac = ac + 1;
                }
                // if node type is an element
                if (nType == XmlNodeType.Element)
                {
                    Console.WriteLine("Element:" + textReader.Name.ToString());
                    el = el + 1;
                }
                // if node type is an entity\
                if (nType == XmlNodeType.Entity)
                {
                    Console.WriteLine("Entity:" + textReader.Name.ToString());
                    et = et + 1;
                }
                // if node type is a Process Instruction
                if (nType == XmlNodeType.Entity)
                {
                    Console.WriteLine("Entity:" + textReader.Name.ToString());
                    pi = pi + 1;
                }
                // if node type a document
                if (nType == XmlNodeType.DocumentType)
                {
                    Console.WriteLine("Document:" + textReader.Name.ToString());
                    dc = dc + 1;
                }
                // if node type is white space
                if (nType == XmlNodeType.Whitespace)
                {
                    Console.WriteLine("WhiteSpace:" + textReader.Name.ToString());
                    ws = ws + 1;
                }
            }
            // Write the summary
            return "\n =============== XML PROPERTIES ===============\nTotal Comments: + " + cc.ToString() + 
           "\nTotal Attributes:" + ac.ToString() + 
           "\nTotal Elements:" + el.ToString() + 
           "\nTotal Entity:" + et.ToString() + 
           "\nTotal Process Instructions:" + pi.ToString() + 
           "\nTotal Declaration:" + xd.ToString() + 
           "\nTotal DocumentType:" + dc.ToString() + 
           "\nTotal WhiteSpaces:" + ws.ToString();
        }

        /// <summary>
        /// Finner element i XML
        /// </summary>
        /// <param name="xmlStr"></param>
        /// <param name="namespaces"></param>
        public static string RunXPath(string xmlStr, Hashtable namespaces)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc = CreateDoc(xmlStr, false);
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
            System.Collections.IDictionaryEnumerator paramvalue = namespaces.GetEnumerator();
            while (paramvalue.MoveNext())
            {
                nsMgr.AddNamespace(paramvalue.Key.ToString(), paramvalue.Value.ToString());
            }
            //nsMgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");

            XmlNodeList nodes = xmlDoc.SelectNodes("/soap:Envelope/soap:Header/brukersesjon:Brukersesjon/sesjonsid", nsMgr);
            foreach (XmlNode node in nodes)
            {
                // Do anything with node
                return node.InnerText;// OuterXml;
            }

            return string.Empty;
        }

        /// <summary>
        /// Validerer XML mot XSD
        /// </summary>
        /// <param name="xml">raw xml string</param>
        /// <param name="xsd">raw xsd string</param>
        /// <returns>bool validert resultat</returns>
        static bool ValidateXml(string xml, string xsd)
        {
            try
            {
                // bygger XSD schema                
                StringReader _XsdStream;
                _XsdStream = new StringReader(xsd);
                XmlSchema _XmlSchema;
                _XmlSchema = XmlSchema.Read(_XsdStream, null);
                // bygger settings (this replaces XmlValidatingReader)        
                XmlReaderSettings _XmlReaderSettings;
                _XmlReaderSettings = new XmlReaderSettings()
                {
                    ValidationType = ValidationType.Schema
                };
                _XmlReaderSettings.Schemas.Add(_XmlSchema);
                // bygger XML reader        
                StringReader _XmlStream;
                _XmlStream = new StringReader(xml);
                XmlReader _XmlReader;
                _XmlReader = XmlReader.Create(_XmlStream, _XmlReaderSettings);
                // validerer        
                using (_XmlReader)
                {
                    while (_XmlReader.Read()) ;
                }
                // validering succeeded        
                return true;
            }
            catch
            {
                // validering failed       
                return false;
            }
        }

        /// <summary>
        ///  Bygger opp xmldokument fra en string
        /// </summary>
        /// <param name="xml">raw xml string</param>
        /// <returns></returns>
        public static XmlDocument CreateDoc(string xml, bool medDeklarasjon)
        {
            XmlUrlResolver resolver = new XmlUrlResolver();
            resolver.Credentials = CredentialCache.DefaultCredentials;

            XmlDocument doc = new XmlDocument();
            if (medDeklarasjon)
            {
                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
                doc.InsertBefore(xmlDeclaration, null);
            }
            doc.XmlResolver = resolver;
            try
            {
                StringReader sr = new StringReader(xml);
                XmlTextReader reader = new XmlTextReader(sr);
                doc.Load(reader);
                //doc.Load(new XmlTextReader(new StringReader(xml)));
                reader.Close();
                return doc;
            }
            catch (Exception e) { throw e; }
        }
    }
}
