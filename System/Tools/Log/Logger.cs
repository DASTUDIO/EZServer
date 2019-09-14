using System;
using System.Text;
using System.Threading;

namespace Z.Tools
{
    public class Logger
    {
        #region Elements

        /// <summary>
        /// 打印类型 控制台 or Unity
        /// </summary>
        public static LogType logType = LogType.Console;

        /// <summary>
        /// 是否开启打印
        /// </summary>
        public static bool DoLog = true;

        /// <summary>
        /// 是否开启Log文件输出
        /// </summary>
        public static bool FileLog = false;

        /// <summary>
        /// 自定义log string:msg
        /// </summary>
        public static event Action<string> LogDelegate;

        public static event Action<string> WarnDelegate;

        public static event Action<string> ErrorDelegate;

        #endregion

        /// <summary>
        /// 普通样式 Log 打印一条信息
        /// </summary>
        /// <returns>The log.</returns>
        /// <param name="msg">Message.</param>
        public static void Log(string msg)
        {
            msg = DateTime.Now.ToString() + " " + msg + "\n";

            if (FileLog)
            {

                #region Initialize FileLogger

                if (!FileLoggerIsRunning)
                {
                    new Thread(new ThreadStart(fp.run)).Start();

                    FileLoggerIsRunning = true;
                }

                #endregion

                #region Write BlackBoard

                if (FileLogBlackBoard.BlackBoardIsLocked)
                {
                    logBuffer.Append(msg);
                }
                else
                {
                    if (logBuffer.Length != 0)
                    {
                        FileLogBlackBoard.WriteBlackBoard(logBuffer.ToString() + msg);

                        logBuffer.Clear();
                    }
                    else
                    {
                        FileLogBlackBoard.WriteBlackBoard(msg);
                    }
                }

                #endregion

            }

            if (DoLog)
            {
                switch (logType)
                {
                    case LogType.Console:
                        Console.WriteLine(msg);
                        break;
                    case LogType.Custom:
                        if (LogDelegate != null)
                            LogDelegate(msg);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 警告样式 Log 打印一条信息
        /// </summary>
        /// <param name="msg">Message.</param>
        public static void LogWarnning(string msg)
        {
            msg = "[WARNING] " + DateTime.Now.ToString() + " " + msg + "\n";

            if (FileLog)
            {
                #region Initialize FileLogger

                if (!FileLoggerIsRunning)
                {
                    new Thread(new ThreadStart(fp.run)).Start();

                    FileLoggerIsRunning = true;
                }

                #endregion

                #region Write BlackBoard

                if (FileLogBlackBoard.BlackBoardIsLocked)
                {
                    logBuffer.Append(msg);
                }
                else
                {
                    if (logBuffer.Length != 0)
                    {
                        FileLogBlackBoard.WriteBlackBoard(logBuffer.ToString() + msg);

                        logBuffer.Clear();
                    }
                    else
                    {
                        FileLogBlackBoard.WriteBlackBoard(msg);
                    }
                }

                #endregion

            }

            if (DoLog)
            {
                switch (logType)
                {
                    case LogType.Console:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(msg);
                        Console.ResetColor();
                        break;
                    case LogType.Custom:
                        if (WarnDelegate != null)
                            WarnDelegate(msg);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 错误样式 Log 打印一条信息
        /// </summary>
        /// <param name="msg">Message.</param>
        public static void LogError(string msg)
        {
            msg = "[ERROR] " + DateTime.Now.ToString() + " " + msg + "\n";

            if (FileLog)
            {
                #region Initialize FileLogger

                if (!FileLoggerIsRunning)
                {
                    new Thread(new ThreadStart(fp.run)).Start();

                    FileLoggerIsRunning = true;
                }

                #endregion

                #region Write BlackBoard

                if (FileLogBlackBoard.BlackBoardIsLocked)
                {
                    logBuffer.Append(msg);
                }
                else
                {
                    if (logBuffer.Length != 0)
                    {
                        FileLogBlackBoard.WriteBlackBoard(logBuffer.ToString() + msg);

                        logBuffer.Clear();
                    }
                    else
                    {
                        FileLogBlackBoard.WriteBlackBoard(msg);
                    }
                }

                #endregion

            }

            if (DoLog)
            {
                switch (logType)
                {
                    case LogType.Console:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(msg);
                        Console.ResetColor();
                        break;
                    case LogType.Custom:
                        if (ErrorDelegate != null)
                            ErrorDelegate(msg);
                        break;
                    default:
                        break;
                }

            }
        }



        static bool FileLoggerIsRunning = false;

        static StringBuilder logBuffer = new StringBuilder();

        static FileLogPrinter fp = new FileLogPrinter("./Log_" +
                                                      DateTime.Now.Year + "-" +
                                                      DateTime.Now.Month + "-" +
                                                      DateTime.Now.Day +
                                                      ".Log");

        private Logger() { }

        public enum LogType : Byte
        {
            Console = 0,
            Custom = 1
        }


    }
}
