using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PonsUtil
{
    /// <summary>
    /// 文件I/O操作
    /// </summary>
    public class FileIO
    {
        /// <summary>
        /// 保存二进制数据流到文件
        /// </summary>
        /// <param name="bitArray">binary array</param>
        /// <param name="savePath">file directory</param>
        public static void SaveBytes(byte[] bitArray, string savePath)
        {
            FileStream fs = new FileStream(savePath, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(bitArray);
            fs.Close();
            fs.Dispose();
            bw.Close();
        }

        /// <summary>
        /// 保存文本文件
        /// </summary>
        /// <param name="fileName">file directory</param>
        /// <param name="txt">file content</param>
        /// <param name="encoding">encoding</param>
        public static void SaveTextFile(string fileName, string txt, Encoding encoding)
        {
            using (StreamWriter sw = new StreamWriter(fileName, false, encoding))
            {
                sw.Write(txt);
            }
        }
    }
}
