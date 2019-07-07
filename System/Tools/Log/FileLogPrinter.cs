using System;
using System.Text;
using System.Threading;
using System.IO;

namespace  ezserver.Tools
{
    public class FileLogPrinter
    {
        int sleepTime = 1000;

        FileStream fs = null;

        string path;

        public FileLogPrinter(string path)
        {
            this.path = path;
        }

        public void run()
        {
            while (true)
            {
                string msgs = FileLogBlackBoard.ReadAndWipeBlackBoard();

                byte[] bytes = Encoding.UTF8.GetBytes(msgs);

                if(msgs == "")
                {
                    Thread.Sleep(sleepTime);

                    continue;
                }

                try
                {
                    fs = File.OpenWrite(this.path);

                    fs.Position = fs.Length;

                    fs.Write(bytes, 0, bytes.Length);

                    fs.Close();

                }
                catch(Exception e)
                {
                     Logger.LogError(e.Message + "\n" + e.StackTrace);
                }

                Thread.Sleep(sleepTime);
            }
        }
    }
}
