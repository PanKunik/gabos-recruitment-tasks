using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleUI
{
    public static class ResponseFileProcessor
    {
        public static StringBuilder Process(StreamReader stream)
        {
            StringBuilder soapResult = new StringBuilder();
            soapResult.Append(stream.ReadToEnd());
            soapResult.Replace("&lt;", "<");
            soapResult.Replace("&gt;", ">");
            soapResult.Replace("&#xD;", "");
            soapResult.Remove(0, soapResult.ToString().IndexOf("<DaneSzukajPodmiotyResult>") + "<DaneSzukajPodmiotyResult>".Length);
            soapResult.Remove(soapResult.ToString().IndexOf("</DaneSzukajPodmiotyResult>"), soapResult.Length - soapResult.ToString().IndexOf("</DaneSzukajPodmiotyResult>"));

            return soapResult;
        }
    }
}
