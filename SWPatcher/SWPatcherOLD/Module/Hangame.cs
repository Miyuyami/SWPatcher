using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SWPatcher.Module
{
    class Hangame
    {
        static public string getLatestClientVersion(Ionic.Zip.ZipFile zipFile)
        {
            string result = "0.0.0.0";
                var asdasd = zipFile["ServerVer.ini"];
            using (var contentStream = asdasd.OpenReader())
            using (System.IO.StreamReader reader = new System.IO.StreamReader(contentStream))
            {
                Ini.IniFile theSWIni = new Ini.IniFile(reader);
                result = theSWIni.GetValue("client", "ver", "0.0.0.0");
                theSWIni.Close();
                theSWIni = null;
            }
            return result;
        }

        static public string getLatestClientVersion(string zipFilePath)
        {
            string result = "0.0.0.0";
            using (Ionic.Zip.ZipFile zipFile = Ionic.Zip.ZipFile.Read(zipFilePath))
            {
                var asdasd = zipFile["ServerVer.ini"];
                using (var contentStream = asdasd.OpenReader())
                using (System.IO.StreamReader reader = new System.IO.StreamReader(contentStream))
                {
                    Ini.IniFile theSWIni = new Ini.IniFile(reader);
                    result = theSWIni.GetValue("client", "ver", "0.0.0.0");
                    theSWIni.Close();
                    theSWIni = null;
                }
            }
            return result;
        }

        static public string getLatestClientVersion(Classes.Network.SWWebClient theSWWebClient)
        {
            string result = "0.0.0.0";
            string tmpfile = System.IO.Path.GetTempFileName();
            {
                theSWWebClient.BaseAddress = "http://down.hangame.co.jp/jp/purple/plii/j_sw/";
                try
                {
                    theSWWebClient.Headers.Add(System.Net.HttpRequestHeader.UserAgent, "purple");
                }
                catch
                { theSWWebClient.Headers.Set(System.Net.HttpRequestHeader.UserAgent, "purple"); }
                theSWWebClient.DownloadFile("ServerVer.ini.zip", tmpfile);
                using (Ionic.Zip.ZipFile zipFile = Ionic.Zip.ZipFile.Read(tmpfile))
                {
                    var asdasd = zipFile["ServerVer.ini"];
                    using (var contentStream = asdasd.OpenReader())
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(contentStream))
                    {
                        Ini.IniFile theSWIni = new Ini.IniFile(reader);
                        result = theSWIni.GetValue("client", "ver", "0.0.0.0");
                        theSWIni.Close();
                        theSWIni = null;
                    }
                }
                System.IO.File.Delete(tmpfile);
            }
            tmpfile = null;
            return result;
        }
    }
}
