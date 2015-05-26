using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace PonsUtil
{
    public class SerializeHelper
    {
        #region Soap 序列化工具
        /// <summary>
        /// 序列化，可序列化对象数组
        /// </summary>
        public static string SoapSerialize(object obj)
        {
            string result = "";
            if (obj != null)
            {
                try
                {
                    SoapFormatter formatter = new SoapFormatter();
                    MemoryStream stream = new MemoryStream();
                    formatter.Serialize(stream, obj);
                    result = Encoding.Unicode.GetString(stream.ToArray());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return result;
        }

        /// <summary>
        /// Soap 反序列化
        /// </summary>
        /// <returns></returns>
        public static object SoapDeserialize(string str)
        {
            object result = null;
            if (str != "")
            {
                try
                {
                    SoapFormatter formatter = new SoapFormatter();
                    MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(str));
                    result = formatter.Deserialize(stream);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 将一个 DataSet 序列化，返回字符串
        /// </summary>
        public static string SerializeData(object obj)
        {
            StreamWriter sw = null;
            string serializeString = null;

            try
            {
                // 以下代码可能导致头晕，请选择观看
                XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
                MemoryStream memStream = new MemoryStream();
                sw = new StreamWriter(memStream);
                xmlSerializer.Serialize(sw, obj);
                memStream.Position = 0;
                serializeString = Encoding.UTF8.GetString(memStream.GetBuffer());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }

            return serializeString;
        }

        /// <summary>
        /// 反持续化，用于恢复 DataSet
        /// </summary>
        /// <param name="serializedString">通过 SerializeDataSet 生成的 string</param>
        /// <returns></returns>
        public static object DeserializeData(Type type, string serializedString)
        {
            //如果传入的是空串，直接返回
            if (serializedString.Trim().Equals(string.Empty))
            {
                throw new Exception("传入的序列化字符串为空");
            }

            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(type);
                StringReader stringReader = new StringReader(serializedString);
                object obj = xmlSerializer.Deserialize(stringReader);

                return obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 序列化DataTable
        /// </summary>
        /// <param name="dataTable">包含数据的DataTable</param>
        /// <returns>序列化的DataTable</returns>
        public static string GetXmlFormatDt(DataTable dataTable)
        {
            XmlSerializer ser = new XmlSerializer(dataTable.GetType());
            System.IO.MemoryStream mem = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(mem, System.Text.Encoding.Default);
            ser.Serialize(writer, dataTable);
            writer.Close();
            return System.Text.Encoding.Default.GetString(mem.ToArray());
        }

        /// <summary>
        /// 反序列化DataTable
        /// </summary>
        /// <param name="dt">序列化的DataTable</param>
        /// <returns>DataTable</returns>
        public static DataSet GetDtFormatXml(string dt)
        {
            XmlSerializer mySerializer = new XmlSerializer(typeof(DataSet));
            StreamReader mem = new StreamReader(new MemoryStream(System.Text.Encoding.Default.GetBytes(dt)), System.Text.Encoding.Default);
            return (DataSet)mySerializer.Deserialize(mem);
        }

        /// <summary>
        /// 序列化DataSet对象并压缩
        /// </summary>
        /// <param name="ds">数据源</param>
        /// <param name="FilePath">例如：D:/DataBak/TestData.dat</param>
        public static bool DataSetSerializer(DataSet ds, string FilePath)
        {
            bool _state = true;
            try
            {
                IFormatter formatter = new BinaryFormatter();//定义BinaryFormatter以序列化DataSet对象
                MemoryStream ms = new MemoryStream();//创建内存流对象
                formatter.Serialize(ms, ds);//把DataSet对象序列化到内存流
                byte[] buffer = ms.ToArray();//把内存流对象写入字节数组
                ms.Close();//关闭内存流对象
                ms.Dispose();//释放资源
                Stream _Stream = File.Open(FilePath, FileMode.Create);//创建文件
                GZipStream gzipStream = new GZipStream(_Stream, CompressionMode.Compress, true);//创建压缩对象
                gzipStream.Write(buffer, 0, buffer.Length);//把压缩后的数据写入文件
                gzipStream.Close();//关闭压缩流,这里要注意：一定要关闭，要不然解压缩的时候会出现小于4K的文件读取不到数据，大于4K的文件读取不完整
                gzipStream.Dispose();//释放对象
                _Stream.Flush();//释放内存
                _Stream.Close();//关闭流
                _Stream.Dispose();//释放对象
            }
            catch (Exception ex)
            {
                _state = false;
            }
            return _state;
        }

        /// <summary>
        /// 序列化DataSet对象并压缩
        /// </summary>
        /// <param name="ds">数据源</param>
        /// <param name="FilePath">例如：D:/DataBak/TestData.dat</param>
        public static bool DataSetSerializer1(DataSet ds, string FilePath)
        {
            bool _state = true;
            try
            {
                IFormatter formatter = new BinaryFormatter();//定义BinaryFormatter以序列化DataSet对象
                Stream _Stream = File.Open(FilePath, FileMode.Create);//创建文件
                formatter.Serialize(_Stream, ds);//把DataSet对象序列化到文件
                _Stream.Flush();//释放内存
                _Stream.Close();//关闭流
                _Stream.Dispose();//释放对象
            }
            catch (Exception ex)
            {
                _state = false;
            }
            return _state;
        }

        /// <summary>
        /// 反序列化压缩的DataSet 
        /// </summary>
        /// <param name="FilePath">例如：D:/DataBak/TestData.dat</param>
        /// <returns></returns>
        public static DataSet DataSetDeserialize(string FilePath)
        {
            Stream _Stream = File.Open(FilePath, FileMode.Open);//打开文件
            _Stream.Position = 0;//设置文件流的位置 
            GZipStream gzipStream = new GZipStream(_Stream, CompressionMode.Decompress);//创建解压对象
            byte[] buffer = new byte[4096];//定义数据缓冲
            int offset = 0;//定义读取位置 
            MemoryStream ms = new MemoryStream();//定义内存流 
            while ((offset = gzipStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                ms.Write(buffer, 0, offset);//解压后的数据写入内存流   
            }
            IFormatter formatter = new BinaryFormatter();//定义BinaryFormatter以反序列化DataSet对象
            ms.Position = 0;//设置内存流的位置
            DataSet ds;
            try
            {
                ds = (DataSet)formatter.Deserialize(ms);//反序列化   
            }
            catch
            {
                ds = null;
            }
            ms.Close();//关闭内存流   
            ms.Dispose();//释放资源 
            _Stream.Flush();//释放内存
            _Stream.Close();//关闭文件流   
            _Stream.Dispose();//释放资源   
            gzipStream.Close();//关闭解压缩流   
            gzipStream.Dispose();//释放资源   
            return ds;
        }

        /// <summary>
        /// 反序列化未压缩的DataSet   
        /// </summary>
        /// <param name="FilePath">例如：D:/DataBak/TestData.dat</param>
        /// <returns></returns>
        public static DataSet DataSetDeserialize1(string FilePath)
        {
            Stream _Stream = File.Open(FilePath, FileMode.Open);//打开文件 
            _Stream.Position = 0;//设置文件流的位置 
            IFormatter formatter = new BinaryFormatter();//定义BinaryFormatter以反序列化DataSet对象 
            DataSet ds;
            try
            {
                ds = (DataSet)formatter.Deserialize(_Stream);//反序列化   
            }
            catch
            {
                ds = null;
            }
            _Stream.Flush();//释放内存
            _Stream.Close();//关闭文件流   
            _Stream.Dispose();//释放资源   
            return ds;
        }

        /// <summary>
        /// 打印对象
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string PrintObject(object obj)
        {
            var objectStr = new StringBuilder();
            Type t = obj.GetType();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                object value = pi.GetValue(obj, null);
                string name = pi.Name;
                objectStr.AppendLine(string.Format("{0}:{1}", name, value));
            }
            return objectStr.ToString();
        }

        public static string BinarySerialize(object obj)
        {
            BinaryFormatter ser = new BinaryFormatter();
            MemoryStream mStream = new MemoryStream();
            ser.Serialize(mStream, obj);
            byte[] buf = mStream.ToArray();
            mStream.Close();
            return Convert.ToBase64String(buf);
        }

        public static object DeSerialize(string byteToStr)
        {
            byte[] binary = Convert.FromBase64String(byteToStr);
            BinaryFormatter ser = new BinaryFormatter();
            MemoryStream mStream = new MemoryStream(binary);
            object obj = (object)ser.Deserialize(mStream);
            mStream.Close();
            return obj;
        }
    }
}
