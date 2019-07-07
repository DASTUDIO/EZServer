using System;
using System.Text;
using System.Collections.Generic;

namespace  ezserver.Tools
{
    public class FileLogBlackBoard
    {
        public static bool BlackBoardIsLocked = false;

        protected static List<string> logBuffer = new List<string>();

        protected static StringBuilder sb = new StringBuilder();

        public static void WriteBlackBoard(string msg)
        {
            logBuffer.Add(msg);
        }
    
        public static string ReadAndWipeBlackBoard()
        {
			BlackBoardIsLocked = true;

            sb.Clear();

            for (int i = 0; i < logBuffer.Count;i++)
            {
                sb.Append(logBuffer[i]);
            }

            logBuffer.Clear();

            BlackBoardIsLocked = false;

            return sb.ToString();

        }

    }
}
