using System.IO;

namespace  Z.Tools
{
    public class FileWriter
    {
        /// <summary>
        /// 写入一个文件
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <param name="content">写入内容.</param>
        public static void WriteFile(string filePath, string content)
        {
            FileWriter handler = new FileWriter(filePath);

            handler.Write(content, false);

            handler.Finished();

            handler = null;
        }

        #region Interior Stuff

        FileStream fs;

        StreamWriter sw;

        public FileWriter(string filePath)
        {
            fs = new FileStream(filePath, FileMode.OpenOrCreate);

            sw = new StreamWriter(fs);

        }

        public void Write(string content, bool isLine)
        {
            if (isLine)
            {
                sw.WriteLine(content);
            }
            else
            {
                sw.Write(content);
            }

            sw.Flush();

        }

        public void Finished()
        {
            sw.Close();

            fs.Close();

        }

        #endregion

    }
}
