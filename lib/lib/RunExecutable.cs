using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace fp.lib
{
    public class RunExecutable
    {
        public RunExecutable()
        {
            workingDirectory = Directory.GetCurrentDirectory();
        }

        public string workingDirectory;
        public string outputFile;

        public void Run(string exe, string args = null,
            ProcessWindowStyle windowStyle = ProcessWindowStyle.Hidden, int timeout = -1)
        {
            if (timeout == -1)
                timeout = 1000 * 60 * 5;// Wait up to five minutes.
            Process process = new Process();
            process.StartInfo.FileName = exe.Contains("\\") ? exe : (workingDirectory + "\\" + exe);
            process.StartInfo.Arguments = args;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.ErrorDialog = true;
            process.StartInfo.WindowStyle = windowStyle;
            if(outputFile != null)
                process.StartInfo.RedirectStandardOutput = true;
            
            process.Start();

            if (outputFile != null)
            {
                using (StreamWriter writer = new StreamWriter(outputFile))
                {
                    char[] buffer = new char[1005];
                    while (!process.StandardOutput.EndOfStream)
                    {
                        int bytes = process.StandardOutput.Read(buffer, 0, 1000);
                        if (bytes > 0)
                        {
                            writer.Write(buffer, 0, bytes);
                        }
                    }
                }
            }

            process.WaitForExit(timeout);    

        }

    }
}
