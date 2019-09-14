using System.IO;
using System.Text;
using System.Collections.Generic;

// 读取格式 varName = varValue
// #行首为注释行
// 空格不限 变量名大小写不限 一律大写

namespace  Z.Tools
{
    public class FileReader
    {
        /// <summary>
        /// 读取一个配置文件 （varName=varValue格式 #开头是注释）
        /// </summary>
        /// <returns>参数列表.</returns>
        /// <param name="filePath">File path.</param>
        public static Dictionary<string, string> ReadFile(string filePath)
        {
            FileReader handler = new FileReader(filePath);

            Dictionary<string, string> result = handler.Read();

            handler = null;

            return result;
        }

        #region Interior Stuff

        StreamReader sr;

        public FileReader(string filePath) : this(filePath, EzServer.GlobalEncoding) { }

        public FileReader(string filePath, Encoding codingType)
        {
            sr = new StreamReader(filePath, codingType);
        }

        // return parameters
        public Dictionary<string, string> Read()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            string line;

            while ((line = sr.ReadLine()) != null)
            {

                if (line.Length > 0)
                {

                    if (line.Substring(0, 1) == @"#")
                    {
                        line = "";

                        continue;  // annotation line
                    }
                }
                else // empty line
                {
                    continue;
                }

                string[] strPair = line.Split('=');

                line = "";

                if (strPair.Length == 2)
                {
                    result.Add(
                        strPair[0].ToUpper().Replace("\"", "").Replace("\'", "").TrimStart().TrimEnd(),
                        strPair[1].Replace("\"", "").Replace("\'", "").TrimStart().TrimEnd());
                }
                else
                {
                    continue;
                }
            }

            sr.Close();

            return result;

        }

        #endregion

    }
}
