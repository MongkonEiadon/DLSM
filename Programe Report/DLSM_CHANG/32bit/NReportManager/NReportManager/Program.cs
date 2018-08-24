using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Threading;

namespace NReportManager
{
    class Program
    {
        static string RootReport = "";
        static string RootXml = "";
        static string RootPdf = "";
        static List<String> workList = new List<string>();

        static void Main(string[] args)
        {
            try
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Clear();

                Console.WriteLine("{0}:{1}",DateTime.Now,"NReportManager Start");

                RootReport = ConfigurationManager.AppSettings["ROOT_REPORT"];
                RootXml = ConfigurationManager.AppSettings["ROOT_XML"];
                RootPdf = ConfigurationManager.AppSettings["ROOT_PDF"];

                if (RootReport == null) throw new Exception("Configuration Error: require ROOT_REPORT");
                if (RootXml == null) throw new Exception("Configuration Error: require ROOT_XML");
                if (RootPdf == null) throw new Exception("Configuration Error: require ROOT_PDF");

                if (!RootReport.EndsWith("\\")) RootReport += "\\";
                if (!RootXml.EndsWith("\\")) RootXml += "\\";
                if (!RootPdf.EndsWith("\\")) RootPdf += "\\";

                while (true)
                {
                    try
                    {
                        String [] files = Directory.GetFiles(RootXml,"*.xml"); // return full path
                        if (files.Length > 0)
                        {
                            Console.Write("                             \r"); // enter
                            foreach (string file in files)
                            {
                                //RunReport(file);
                                if(!workList.Contains(file))
                                    StartReportThread(file);
                            }
                        }
                        else
                        {
                            Console.Write("\r{0}:{1}", DateTime.Now, "No job in queue.");
                        }
                    }
                    catch(Exception exInner)
                    {
                        Console.WriteLine("{0}:{1}", DateTime.Now, exInner.Message);
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}:{1}", DateTime.Now, ex.Message);
                
            }
            finally
            {
                Console.WriteLine("{0}:{1}", DateTime.Now, "NReportManager being shutdown...");
                while (workList.Count > 0)
                    Thread.Sleep(100);
                Console.WriteLine("{0}:{1}", DateTime.Now, "NReportManager Terminated.");
            }

        }

        static void RunReport(Object objFileName)
        {
            workList.Add( objFileName.ToString());
            String fileName = objFileName.ToString();
            FileInfo finfo = new FileInfo(fileName);
            try
            {

                Console.WriteLine("{0}:{1}", DateTime.Now, "Process " + finfo.Name);

                XmlDocument xdoc = new XmlDocument();
                xdoc.Load(fileName);

                Console.WriteLine("{0}:{1}", DateTime.Now, "Report " + xdoc.DocumentElement.SelectSingleNode("Report").InnerText);
                string reportName = RootReport + xdoc.DocumentElement.SelectSingleNode("Report").InnerText;
                string outputName = RootPdf + xdoc.DocumentElement.SelectSingleNode("Output").InnerText;

                ReportDocument report = new ReportDocument();
                report.Load(reportName);
                Console.WriteLine("{0}:{1}", DateTime.Now, "Report loaded");

                string strServer = xdoc.DocumentElement.SelectSingleNode("Server").InnerText;
                string strDatabase = xdoc.DocumentElement.SelectSingleNode("Database").InnerText;
                string strUserID = xdoc.DocumentElement.SelectSingleNode("User").InnerText;
                string strPwd = xdoc.DocumentElement.SelectSingleNode("Password").InnerText;

                if (report.DataSourceConnections.Count > 0)
                {
                    report.DataSourceConnections[0].IntegratedSecurity = false;
                    report.DataSourceConnections[0].SetConnection(strServer, strDatabase, strUserID, strPwd);
                    Console.WriteLine("{0}:{1}", DateTime.Now, "Set connection " + strServer + "/" + strDatabase + "/" + strUserID);
                }

                XmlNodeList parameters = xdoc.DocumentElement.SelectSingleNode("Parameters").SelectNodes("Item");
                foreach (XmlNode item in parameters)
                {
                    string pname = item.Attributes["Name"].Value;
                    string ptype = item.Attributes["Type"].Value;
                    string pvalue = item.Attributes["Value"].Value;

                    if (ptype == "Integer")
                        report.SetParameterValue(pname, Convert.ToInt32(pvalue));
                    else if (ptype == "DateTime")
                        report.SetParameterValue(pname, Convert.ToDateTime(pvalue));
                    else if (ptype == "Double")
                        report.SetParameterValue(pname, Convert.ToDouble(pvalue));
                    else if (ptype == "Decimal")
                        report.SetParameterValue(pname, Convert.ToDecimal(pvalue));
                    else
                        report.SetParameterValue(pname, pvalue);

                    Console.WriteLine("{0}:{1}", DateTime.Now, "Parameter " + pname + "/" + ptype + "/" + pvalue);
                }
                if (outputName.ToLower().EndsWith("pdf"))
                {
                    report.ExportToDisk(ExportFormatType.PortableDocFormat, outputName);
                    Console.WriteLine("{0}:{1}", DateTime.Now, "PDF:" + xdoc.DocumentElement.SelectSingleNode("Output").InnerText);
                }
                else if (outputName.ToLower().EndsWith("rtf"))
                {
                    report.ExportToDisk(ExportFormatType.RichText, outputName);
                    Console.WriteLine("{0}:{1}", DateTime.Now, "RTF:" + xdoc.DocumentElement.SelectSingleNode("Output").InnerText);
                }
                else if (outputName.ToLower().EndsWith("doc"))
                {
                    report.ExportToDisk(ExportFormatType.WordForWindows, outputName);
                    Console.WriteLine("{0}:{1}", DateTime.Now, "WORD:" + xdoc.DocumentElement.SelectSingleNode("Output").InnerText);
                }

                finfo.Delete();
                Console.WriteLine("{0}:FINISH:{1}:{2}", DateTime.Now, finfo.Name, "Deleted");
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}:ERROR:{1}:{2}", DateTime.Now, finfo.Name, ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("{0}:ERROR:{1}:{2}", DateTime.Now, finfo.Name, ex.InnerException.Message);

                }

                try
                {
                    File.Move(fileName, fileName + ".err");
                }
                catch
                {

                }
            }
            finally
            {
                workList.Remove(objFileName.ToString());
            }
        } // end RunReport

        static Thread StartReportThread(String file)
        {
            Thread t = new Thread(new ParameterizedThreadStart(RunReport));
            t.Start(file);
            return t;
        } // end StartReportThread

    } // end class program
}
