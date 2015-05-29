using System;
using SevenZip;
using System.Configuration;
using System.Web;
using log4net;
using System.IO;
using System.Diagnostics;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;

namespace PonsUtil
{
    public class ZipUtil
    {
        private string _senvenZipPath;
        private string _ZipConfigName;
        private readonly string _defaultZipPath = "~/Lib/7z.dll";
        private readonly ILog _ilogger = LogManager.GetLogger(typeof(ZipUtil));


        private readonly SevenZipCompressor _zipCompressor;

        /// <summary>
        /// 取得配置7z Dll的配置名称
        /// </summary>
        public string ZipSoftDllConfigPath
        {
            get
            {
                return _ZipConfigName;
            }
        }

        /// <summary>
        /// SenvenPath Real Path
        /// </summary>
        public string SenvenZipPath
        {
            get
            {
                return _senvenZipPath;
            }
        }

        public ZipUtil()
            : this("SenvenZipPath")
        {

        }

        public ZipUtil(string zipConfigName)
        {
            _ZipConfigName = zipConfigName;
            Init();
            SetZipSoftPath();
            _zipCompressor = new SevenZipCompressor();
        }

        private void Init()
        {
            if (ConfigurationManager.AppSettings[_ZipConfigName] != null)
            {
                _senvenZipPath = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings[_ZipConfigName].ToString());
                return;
            }
            _senvenZipPath = HttpContext.Current.Server.MapPath(_defaultZipPath);
        }

        private void SetZipSoftPath()
        {
            SevenZipCompressor.SetLibraryPath(_senvenZipPath);
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="strOrigFileName">原始文件</param>
        /// <param name="strZipFileName">压缩文件</param>
        /// <param name="strMsg">错误消息</param>
        /// <returns></returns>
        public bool CompressFiles(string strOrigFileName, string strZipFileName, out string strMsg)
        {
            try
            {
                SetZipFormat(strZipFileName);
                _zipCompressor.CompressFiles(strZipFileName, strOrigFileName);
            }
            catch (Exception e)
            {
                strMsg = e.Message;
                _ilogger.ErrorFormat("压缩文件出错:原始文件名\t{0},压缩文件名\t{1}\t 原异常数据:{2}"
                    , strOrigFileName
                    , strZipFileName
                    , e.Message);
                return false;
            }
            strMsg = "";
            return true;
        }

        /// <summary>
        /// 设置压缩格式
        /// </summary>
        /// <param name="strZipFileName"></param>
        private void SetZipFormat(string strZipFileName)
        {
            string fileExt = Path.GetExtension(strZipFileName).ToLower();
            _zipCompressor.ArchiveFormat = fileExt.Equals(".zip") ? OutArchiveFormat.Zip : OutArchiveFormat.SevenZip;
        }


        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="strOrigDirecPath">原始文件夹路径</param>
        /// <param name="strZipFileName">压缩文件名</param>
        /// <param name="strMsg"></param>
        /// <returns></returns>
        public bool CompressDirectory(string strOrigDirecPath, string strZipFileName, out string strMsg)
        {
            try
            {
                SetZipFormat(strZipFileName);
                _zipCompressor.CompressDirectory(strOrigDirecPath, strZipFileName);
            }
            catch (Exception e)
            {
                strMsg = e.Message;
                _ilogger.ErrorFormat("压缩文件夹出错:原始文件夹\t{0},压缩文件名\t{1}\t 原异常数据:{2}"
                    , strOrigDirecPath
                    , strZipFileName
                    , e.Message);
                return false;
            }
            strMsg = "";
            return true;
        }

        /// <summary>
        /// 解压file到当前目录
        /// </summary>
        /// <param name="strZipFileName"></param>
        /// <param name="strMsg"></param>
        /// <returns></returns>
        public bool ExtractFiles(string strZipFileName, out string strMsg)
        {
            try
            {
                SevenZipExtractor zipExtractor = new SevenZipExtractor(strZipFileName);
                zipExtractor.ExtractArchive(Path.GetFullPath(strZipFileName));
            }
            catch (Exception e)
            {
                strMsg = e.Message;
                _ilogger.ErrorFormat("解压文件夹出错:原始压缩文件\t{0},解压后文件夹名\t{1}\t 原异常数据:{2}"
                    , strZipFileName
                    , Path.GetFullPath(strZipFileName)
                    , e.Message);
                return false;
            }
            strMsg = "";
            return true;
        }

        /// <summary>
        /// 解压到指定目录
        /// </summary>
        /// <param name="strZipFileName"></param>
        /// <param name="strExtractFileName">指定的解压路径</param>
        /// <param name="strMsg"></param>
        /// <returns></returns>
        public bool ExtractFiles(string strZipFileName, string strExtractFileName, out string strMsg)
        {
            if (string.IsNullOrEmpty(strExtractFileName))
            {
                return ExtractFiles(strZipFileName, out strMsg);
            }
            try
            {
                SevenZipExtractor zipExtractor = new SevenZipExtractor(strZipFileName);
                zipExtractor.ExtractArchive(strExtractFileName);
            }
            catch (Exception e)
            {
                strMsg = e.Message;
                _ilogger.ErrorFormat("解压文件夹出错:原始压缩文件\t{0},解压后文件夹名\t{1}\t 原异常数据:{2}"
                    , strZipFileName
                    , strExtractFileName
                    , e.Message);
                return false;
            }
            strMsg = "";
            return true;
        }

    }

    public class ZipHelper
    {
        private static string RaRFilePath
        {
            get
            {
                try
                {
                    return DataConvert.ToString(ConfigurationManager.AppSettings["RaRFilePath"]);
                }
                catch (Exception ex)
                {
                    return @"C:\Program Files\WinRAR\WinRAR.exe";
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileToZip">文件夹地址</param>
        /// <param name="ZipedFile">文件名称</param>
        /// <param name="CompressionLevel">压缩等级</param>
        /// <param name="BlockSize">缓存块大小</param>
        public static void ZipFile(string FileToZip, string ZipedFile, int CompressionLevel, int BlockSize)
        {
            //如果文件没有找到，则报错
            if (!System.IO.File.Exists(FileToZip))
            {
                throw new System.IO.FileNotFoundException("The specified file " + FileToZip + " could not be found. Zipping aborderd");
            }

            System.IO.FileStream StreamToZip = new System.IO.FileStream(FileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.FileStream ZipFile = System.IO.File.Create(ZipedFile);

            //zip  输出流
            ZipOutputStream ZipStream = new ZipOutputStream(ZipFile);
            ZipEntry ZipEntry = new ZipEntry("ZippedFile");
            ZipStream.PutNextEntry(ZipEntry);
            ZipStream.SetLevel(CompressionLevel);
            byte[] buffer = new byte[BlockSize];


            System.Int32 size = StreamToZip.Read(buffer, 0, buffer.Length);
            ZipStream.Write(buffer, 0, size);
            try
            {
                while (size < StreamToZip.Length)
                {
                    int sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                    ZipStream.Write(buffer, 0, sizeRead);
                    size += sizeRead;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            ZipStream.Finish();
            ZipStream.Close();
            StreamToZip.Close();
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="ZipPath">被解压的文件路径</param>
        /// <param name="Path">解压后文件的路径</param>
        public static void UnZip(string ZipPath, string Path, string password)
        {
            //这里通过File.OpenRead方法读取指定文件，并通过其返回的FileStream构造ZipInputStream对象；
            ZipInputStream s = new ZipInputStream(File.OpenRead(ZipPath));

            if (String.IsNullOrEmpty(password) == false)
            {
                s.Password = password;
            }

            //每个包含在Zip压缩包中的文件都被看成是ZipEntry对象，并通过ZipInputStream的GetNextEntry方法
            //依次遍历所有包含在压缩包中的文件。
            ZipEntry theEntry;
            try
            {
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string fileName = System.IO.Path.GetFileName(theEntry.Name);

                    //生成解压目录
                    Directory.CreateDirectory(Path);

                    if (fileName != String.Empty)
                    {
                        //解压文件
                        FileStream streamWriter = File.Create(Path + fileName);

                        int size = 2048;
                        byte[] data = new byte[2048];

                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {

                                streamWriter.Close();
                                streamWriter.Dispose();
                                break;
                            }
                        }

                        streamWriter.Close();
                        streamWriter.Dispose();
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                s.Close();
                s.Dispose();
            }
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="ZipPath">被解压的文件路径</param>
        /// <param name="Path">解压后文件的路径</param>
        public static void UnRaR(string filename, string localfolder)
        {
            String myRar;
            String myInfo;
            ProcessStartInfo myStartInfo;
            Process myProcess;

            //myReg = Registry.ClassesRoot.OpenSubKey(@"Applications/WinRAR.exe/shell/open/command");
            //myObj = myReg.GetValue("");
            myRar = RaRFilePath;
            //myReg.Close();

            //myRar = myRar.Substring(1, myRar.Length - 7);
            myInfo = " X " + filename + " " + localfolder;
            myStartInfo = new ProcessStartInfo();
            myStartInfo.FileName = myRar;
            myStartInfo.UseShellExecute = false;
            myStartInfo.Arguments = myInfo;
            myStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            myProcess = new Process();
            myProcess.StartInfo = myStartInfo;
            myProcess.Start();

            myProcess.WaitForExit();

            myProcess.Close();
            myProcess.Dispose();
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="fileName">要压缩的所有文件（完全路径)</param>
        /// <param name="name">压缩后文件路径</param>
        /// <param name="Level">压缩级别</param>
        public static void ZipFileMain(string[] filenames, string name, int Level)
        {
            const string strCode = "9B47DCA1-CC31-522A-E95E-42BBEE60C130";

            ZipOutputStream s = new ZipOutputStream(File.Create(name));
            Crc32 crc = new Crc32();
            //压缩级别
            s.SetLevel(Level); // 0 - store only to 9 - means best compression
            s.Password = strCode;
            try
            {
                foreach (string file in filenames)
                {
                    //打开压缩文件
                    FileStream fs = File.OpenRead(file);

                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);

                    //建立压缩实体
                    ZipEntry entry = new ZipEntry(System.IO.Path.GetFileName(file));

                    //时间
                    entry.DateTime = DateTime.Now;

                    entry.Size = fs.Length;
                    fs.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    s.PutNextEntry(entry);
                    s.Write(buffer, 0, buffer.Length);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                s.Finish();
                s.Close();
            }

        }
    }
}
