using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ConsoleUI
{
    public class Request
    {
        private string _sid;

        public List<string> NipNumbers { get; set; } = new List<string>();

        public void Login()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@"https://wyszukiwarkaregontest.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc");
            webRequest.ContentType = "application/soap+xml";
            webRequest.Method = "POST";

            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
                <soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:ns=""http://CIS/BIR/PUBL/2014/07"">
                    <soap:Header xmlns:wsa=""http://www.w3.org/2005/08/addressing"">
                        <wsa:Action>http://CIS/BIR/PUBL/2014/07/IUslugaBIRzewnPubl/Zaloguj</wsa:Action>
                        <wsa:To>https://wyszukiwarkaregontest.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc</wsa:To>
                    </soap:Header>
                    <soap:Body>
                        <ns:Zaloguj>
                            <ns:pKluczUzytkownika>abcde12345abcde12345</ns:pKluczUzytkownika>
                        </ns:Zaloguj>
                    </soap:Body>
                </soap:Envelope>
            ");

            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

            using (WebResponse response = webRequest.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    var stringResult = rd.ReadToEnd();
                    _sid = stringResult.Substring(stringResult.IndexOf("<ZalogujResult>") + "<ZalogujResult>".Length, 20);
                }
            }
        }

        public void FindCompanies()
        {
            string nipNumbers = ConcatNipNumbers();

            HttpWebRequest request = CreateWebRequest();
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8""?>
                <soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" xmlns:ns=""http://CIS/BIR/PUBL/2014/07"" xmlns:dat=""http://CIS/BIR/PUBL/2014/07/DataContract""> 
                    <soap:Header xmlns:wsa=""http://www.w3.org/2005/08/addressing"">
                        <wsa:To>https://wyszukiwarkaregontest.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc</wsa:To> 
                        <wsa:Action>http://CIS/BIR/PUBL/2014/07/IUslugaBIRzewnPubl/DaneSzukajPodmioty</wsa:Action> 
                    </soap:Header>
                    <soap:Body>
                        <ns:DaneSzukajPodmioty>
                            <ns:pParametryWyszukiwania>
                                <dat:Nipy>" +
                                nipNumbers
                                + @"</dat:Nipy>
                            </ns:pParametryWyszukiwania>
                        </ns:DaneSzukajPodmioty>
                    </soap:Body>
                </soap:Envelope>");

            using (Stream stream = request.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                {
                    
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(ResponseFileProcessor.Process(rd).ToString());

                    foreach (XmlNode xmlNode in xmlDoc.FirstChild.ChildNodes)
                    {
                        foreach (XmlNode node in xmlNode)
                        {
                            Console.WriteLine(node.Name + ": " + node.InnerText);
                        }
                    }
                }
            }
        }

        private HttpWebRequest CreateWebRequest()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@"https://wyszukiwarkaregontest.stat.gov.pl/wsBIR/UslugaBIRzewnPubl.svc");
            webRequest.Headers.Add($"sid:{ _sid }");
            webRequest.ContentType = "application/soap+xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private string ConcatNipNumbers()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (var nip in NipNumbers)
            {
                stringBuilder.Append(nip);
                stringBuilder.Append(",");
            }

            stringBuilder.Remove(stringBuilder.Length - 1, 1);

            return stringBuilder.ToString();
        }
    }
}
