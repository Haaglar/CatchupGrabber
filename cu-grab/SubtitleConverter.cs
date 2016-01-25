using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SubCSharp
{
    
    public class SubtitleConverter
    {
        //private byte[] DFXPHead = System.Text.Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n");
        //Internal sub format to allow easy conversion
        private class SubtitleCU
        {
            public List<DateTime> startTime;
            public List<DateTime> endTime;
            public List<String> content;
            public SubtitleCU()
            {
                startTime = new List<DateTime>();
                endTime = new List<DateTime>();
                content = new List<String>();
            }
            /// <summary>
            /// Adds a specific entry to the subtitle
            /// </summary>
            /// <param name="sTime">The start time of the entry</param>
            /// <param name="eTime">The end time of the entry</param>
            /// <param name="text">The content of the entry</param>
            public void AddEntry(DateTime sTime, DateTime eTime, String text)
            {
                startTime.Add(sTime);
                endTime.Add(eTime);
                content.Add(text);
            }
            public int getLength()
            {
                return content.Count;
            }
        }

        SubtitleCU subTitleLocal;
        public SubtitleConverter()
        {
            subTitleLocal = new SubtitleCU();
        }
        //-------------------------------------------------------------------------Read Formats---------------//
        /// <summary>
        /// Converts a dfxp subtitle into the Catchup Grabbers subtitle format
        /// </summary>
        /// <param name="path">The path to the dfxp to convert</param>
        private void ReadDFXP(String path)
        {
            using(XmlTextReader reader = new XmlTextReader(path))
            {
                reader.Namespaces = false;// Namespaces are annoying, screw them.
                while (reader.ReadToFollowing("p")) //Read all p nodes
                {
                    DateTime beginTime;
                    DateTime endTime;
                    String begin = reader.GetAttribute("begin");
                    DateTime.TryParse(begin, out beginTime);
                    String end = reader.GetAttribute("end");
                    DateTime.TryParse(end, out endTime);
                    String text = reader.ReadInnerXml();
                    text = text.Replace("<br /><br />", "\n").Replace("<br/><br/>", "\n").Replace("<br />", "\n").Replace("<br/>", "\n"); //Depends on the format remove all
                    subTitleLocal.AddEntry(beginTime, endTime, text);

                }
            }
        }

        //-------------------------------------------------------------------------Write Formats---------------//
        private void WriteDFXP(String path)
        {
            FileStream fStream = new FileStream(path + "s", FileMode.Create, FileAccess.Write);
            fStream.Flush();
            XmlTextWriter writer = new XmlTextWriter(fStream, System.Text.Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;

            writer.WriteStartDocument();
            writer.WriteStartElement("tt", "http://www.w3.org/ns/ttml");
            writer.WriteStartElement("body");
            writer.WriteStartElement("div");
            writer.WriteAttributeString("xml","lang",null, "en");
            int length = subTitleLocal.getLength();
            for (int i = 0; i < length; i++)
            {
                String sTime = subTitleLocal.startTime[i].ToString("HH:mm:ss.ff");
                String eTime = subTitleLocal.endTime[i].ToString("HH:mm:ss.ff");
                String content =  subTitleLocal.content[i].Replace("\n","<br/>");
                writer.WriteStartElement("p");
                writer.WriteAttributeString("begin", sTime);
                writer.WriteAttributeString("end", eTime);
                writer.WriteAttributeString("xml", "id", null, "caption " + (i+1));

                writer.WriteRaw(content);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();//div
            writer.WriteEndElement();//Body
            writer.WriteEndElement();//tt
            writer.Flush();
            //Write to file
            fStream.Close();            
        }
        /// <summary>
        /// Converts the local format to Subrip format
        /// </summary>
        /// <param name="path">The path containing the path to the location and name of the original file</param>
        private void WriteSRT(String path)
        {
            String subExport = "";
            int length = subTitleLocal.getLength();
            for (int i = 0; i < length; i++ )
            {
                String sTime = subTitleLocal.startTime[i].ToString("HH:mm:ss,fff");
                String eTime = subTitleLocal.endTime[i].ToString("HH:mm:ss,fff");
                subExport = subExport + (i + 1) + "\n" + sTime + " ---> " + eTime + "\n" + subTitleLocal.content[i] + "\n" + "\n";
            }
            System.IO.File.WriteAllText(path,subExport);
        }
        /// <summary>
        /// Converts a dfxp sub to srt
        /// </summary>
        /// <param name="path">The path containing the path to the dfxp file</param>
        public void DfxpToSrt(String path)
        {
            ReadDFXP(path);
            WriteSRT(System.IO.Path.ChangeExtension(path, "srt"));
        }
    }
}
