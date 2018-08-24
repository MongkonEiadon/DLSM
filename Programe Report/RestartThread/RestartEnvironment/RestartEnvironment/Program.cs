using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading;

namespace RestartEnvironment
{
    class Program
    {
        static void Main(string[] args)
        {
            List<ProgramFile> ProcessName = GetProcessName();
            while(true){
                foreach (ProgramFile i in ProcessName)
                {
                    try 
                    {
                        Process[] ProcessList = Process.GetProcessesByName(i.Name);
                        if (ProcessList.Length == 0)
                        {
                            Process.Start(i.PathFile);
                            Console.WriteLine("{0}:{1}", DateTime.Now, "NameStart: " + i.Name);
                        }
                    }
                    catch 
                    {
                        Console.Write("\r{0}:{1}", DateTime.Now, "  Error : Can not start process program");
                    }
                }
                Thread.Sleep(2000);
            }           
        }

        static List<ProgramFile> GetProcessName()
        {
            int count = Int32.Parse(ConfigurationManager.AppSettings["Number_ProcessName"]);
            List<ProgramFile> TempList = new List<ProgramFile>();
            for (int i = 1; i <= count;i++)
            {
                try
                {
                    string tempname = ConfigurationManager.AppSettings["Programe" + i];
                    string temppath = ConfigurationManager.AppSettings["PathFile" + i];
                    ProgramFile programfile = new ProgramFile();
                    programfile.Name = tempname;
                    programfile.PathFile = temppath;
                    TempList.Add(programfile);
                }
                catch
                {
                    Console.Write("\r{0}:{1}", DateTime.Now, "  Error : Can not found path files");
                }               
            }
            return TempList;
        }
    }
}
