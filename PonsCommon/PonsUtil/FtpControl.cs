using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PonsUtil
{
    /// <summary>
    /// FTP操作类型
    /// </summary>
    public enum FtpFileAction
    {
        /// <summary>
        /// 上传文件,参数1:Stream stream,参数2:string fileName
        /// </summary>
        Upload,
        /// <summary>
        /// 删除文件
        /// </summary>
        Delete,
        /// <summary>
        /// 创建目录,参数1:string filePath
        /// </summary>
        Download,
        /// <summary>
        /// 上传文件并访问
        /// </summary>
        UploadAndAccess,
        /// <summary>
        /// 移动文件
        /// </summary>
        Move,
        /// <summary>
        /// 拷贝文件
        /// </summary>
        Copy,
        /// <summary>
        /// 无操作
        /// </summary>
        DoNothing
    }
    public class FtpFileOperationControl
    {
        private FtpFileAction _action = FtpFileAction.DoNothing;

        private FtpFileAction Action
        {
            get
            {
                return _action;
            }
            set
            {
                _action = value;
            }
        }

        /// <summary>
        /// 文件移动原始文件路径
        /// </summary>
        public string RemoteFilePath { get; set; }

        /// <summary>
        /// 文件移动目标路径
        /// </summary>
        public string MoveToFilePath { get; set; }

        /// <summary>
        /// 上传文件流,如要执行上传文件操作,请为此属性赋值
        /// </summary>
        public Stream UploadFileStream { get; set; }

        /// <summary>
        /// 上传文件名,如要执行上传文件操作,请为此属性赋值
        /// </summary>
        public string UploadFileName { get; set; }

        /// <summary>
        /// 下载文件本地路径,如要执行下载文件操作,请为此属性赋值
        /// </summary>
        public string DownloadLocalPath { get; set; }

        /// <summary>
        /// 下载文件名,如要下载或访问文件,请为此属性赋值
        /// </summary>
        public string DownLoadServerFileName { get; set; }
        /// <summary>
        /// 下载文件名,如要下载或访问文件,请为此属性赋值
        /// </summary>
        public string DownLoadServerFullFileName { get; set; }

        /// <summary>
        /// 上传文件服务器目录,如要上传或访问文件,请为此属性赋值
        /// </summary>
        public string MakeDirName { get; set; }

        public string DeleteFileName { get; set; }

        public delegate void FtpActionHanler(object sender, FtpOperateEventArgs e);

        /// <summary>
        /// 下载完成后事件
        /// </summary>
        public event FtpActionHanler AfterDownLoad;

        /// <summary>
        /// 上传完成后事件
        /// </summary>
        public event FtpActionHanler AfterUpload;

        /// <summary>
        /// 访问完成后事件
        /// </summary>
        public event FtpActionHanler AfterAccess;

        /// <summary>
        /// 上传之前事件
        /// </summary>
        public event FtpActionHanler BeforeUpload;

        /// <summary>
        /// 下载之前事件,如创建本地文件夹
        /// </summary>
        public event FtpActionHanler BeforeDownLoad;

        /// <summary>
        /// 访问之前事件
        /// </summary>
        public event FtpActionHanler BeforeAccess;

        /// <summary>
        /// 执行操作
        /// </summary>
        public void Do(FtpFileAction action)
        {
            this.Action = action;
            DoAction();
        }

        private void DoAction()
        {
            FtpOperateEventArgs args = new FtpOperateEventArgs(true, string.Empty);

            switch (this.Action)
            {
                case FtpFileAction.Delete:
                    {
                        if (DeleteFileName == "")
                        {
                            throw new Exception("FtpFileOperationControl的DeleteFileName属性不能为空.");
                        }

                        FtpFileHelper.Delete(DeleteFileName);

                        break;
                    }
                case FtpFileAction.UploadAndAccess:
                    if (this.BeforeAccess != null)
                        this.BeforeAccess(this, args);
                    Do(FtpFileAction.Upload);
                    Do(FtpFileAction.Download);
                    if (AfterAccess != null)
                    {
                        args = new FtpOperateEventArgs(this.DownloadLocalPath, string.Empty);
                        AfterAccess(this, args);
                    }
                    break;
                case FtpFileAction.Upload:
                    if (this.BeforeUpload != null)
                        BeforeUpload(this, args);
                    if (this.MakeDirName == null)
                        throw new Exception("FtpFileOperationControl的MakeDirName属性不能为空.");
                    FtpFileHelper.DirectoryExist(this.MakeDirName);
                    if (this.UploadFileStream == null)
                    {
                        throw new Exception("FtpFileOperationControl的UploadFileStream属性不能为空.");
                    }
                    if (this.UploadFileName == null)
                    {
                        throw new Exception("FtpFileOperationControl的UploadFileName属性不能为空.");
                    }
                    try
                    {
                        FtpFileHelper.Upload(this.UploadFileStream, this.UploadFileName);
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        if (AfterUpload != null)
                        {
                            args = new FtpOperateEventArgs(FtpFileHelper.GetUrl() + this.UploadFileName, string.Empty);
                            AfterUpload(this, args);
                        }
                    }
                    break;
                case FtpFileAction.Download:
                    if (this.DownloadLocalPath == null)
                        throw new Exception("FtpFileOperationControl的DownloadLocalPath属性不能为空.");
                    if (this.DownLoadServerFileName == null && this.DownLoadServerFullFileName == null)
                        throw new Exception("FtpFileOperationControl的DownLoadServerFileName属性不能为空.");
                    try
                    {
                        if (this.BeforeDownLoad != null)
                        {
                            args = new FtpOperateEventArgs(this.DownloadLocalPath.Replace(Path.GetFileName(this.DownloadLocalPath), ""), string.Empty);
                            this.BeforeDownLoad(this, args);
                        }
                        if (this.DownLoadServerFullFileName == null && this.DownLoadServerFileName != null)
                        {
                            FtpFileHelper.Download(this.DownloadLocalPath, this.DownLoadServerFileName);

                        }
                        if (this.DownLoadServerFileName == null && this.DownLoadServerFullFileName != null)
                        {
                            FtpFileHelper.DownloadftpFile(this.DownloadLocalPath, this.DownLoadServerFullFileName);
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        if (AfterDownLoad != null)
                        {
                            args = new FtpOperateEventArgs(this.DownloadLocalPath, string.Empty);
                            AfterDownLoad(this, args);
                        }
                    }
                    break;
                case FtpFileAction.Move:
                    if (this.RemoteFilePath == null || this.MoveToFilePath == null)
                    {
                        throw new Exception("为设置RemoteFilePath和MoveToFilePath");
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(this.MakeDirName))
                        {
                            FtpFileHelper.DirectoryExist(this.MakeDirName);
                        }
                        try
                        {
                            FtpFileHelper.Move(this.RemoteFilePath, this.MoveToFilePath);
                        }
                        catch
                        {
                            throw;
                        }
                    }
                    break;
                case FtpFileAction.DoNothing:
                    break;
                default:
                    break;
            }

        }

        public class FtpOperateEventArgs : EventArgs
        {
            /// <summary>
            /// 返回值
            /// </summary>
            public object Result { get; private set; }

            /// <summary>
            /// 错误消息
            /// </summary>
            public string Message { get; private set; }

            public FtpOperateEventArgs(object result, string msg)
            {
                this.Result = result;
                this.Message = msg;
            }

        }

        /// <summary>
        /// FTP操作帮助类
        /// </summary>
        private class FtpFileHelper
        {
            private static readonly string _serverUri = ConfigurationManager.AppSettings["FileServerDefaultFtpAddress"];

            private static readonly string _ftpUserName = ConfigurationManager.AppSettings["FileServerFtpUserName"];

            private static readonly string _ftpUserPwd = ConfigurationManager.AppSettings["FileServerFtpUserPwd"];

            public static string GetUrl()
            {
                return _serverUri;
            }

            /// <summary>
            /// 上传文件
            /// </summary>
            /// <param name="stream">文件流</param>
            /// <param name="fileName">保存的文件名</param>
            public static void Upload(Stream stream, string fileName)
            {
                string uri = _serverUri + fileName;
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.Credentials = new NetworkCredential(_ftpUserName, _ftpUserPwd);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                reqFTP.UseBinary = true;
                reqFTP.ContentLength = stream.Length;
                byte[] buff = new byte[stream.Length];
                int contentLen;
                try
                {
                    Stream strm = reqFTP.GetRequestStream();
                    contentLen = stream.Read(buff, 0, int.Parse(stream.Length.ToString()));
                    while (contentLen != 0)
                    {
                        strm.Write(buff, 0, contentLen);
                        contentLen = stream.Read(buff, 0, int.Parse(stream.Length.ToString()));
                    }
                    strm.Close();
                    stream.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// 下载
            /// </summary>
            /// <param name="localFilePath">本地保存路径</param>
            /// <param name="fileName">文件名</param>
            public static void Download(string localFilePath, string fileName)
            {
                FtpWebRequest reqFTP;
                try
                {
                    FileStream outputStream = new FileStream(localFilePath, FileMode.Create);

                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(_serverUri + fileName));
                    reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                    reqFTP.UseBinary = true;
                    reqFTP.Credentials = new NetworkCredential(_ftpUserName, _ftpUserPwd);
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    Stream ftpStream = response.GetResponseStream();
                    long cl = response.ContentLength;

                    int bufferSize = 2048;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }

                    ftpStream.Close();
                    outputStream.Close();
                    response.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// 下载
            /// </summary>
            /// <param name="localFilePath">本地保存路径</param>
            /// <param name="filepatchName">文件名</param>
            public static void DownloadftpFile(string localFilePath, string filepatchName)
            {
                FtpWebRequest reqFTP;
                try
                {
                    FileStream outputStream = new FileStream(localFilePath, FileMode.Create);

                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(filepatchName));
                    reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                    reqFTP.UseBinary = true;
                    reqFTP.Credentials = new NetworkCredential(_ftpUserName, _ftpUserPwd);
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    Stream ftpStream = response.GetResponseStream();
                    long cl = response.ContentLength;

                    int bufferSize = 2048;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }

                    ftpStream.Close();
                    outputStream.Close();
                    response.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// 删除文件(未测试,禁止使用)
            /// </summary>
            /// <param name="fileName"></param>
            public static void Delete(string fileName)
            {
                try
                {
                    string uri = _serverUri + fileName;
                    FtpWebRequest reqFTP;
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

                    reqFTP.Credentials = new NetworkCredential(_ftpUserName, _ftpUserPwd);
                    reqFTP.KeepAlive = false;
                    reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;

                    string result = String.Empty;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    long size = response.ContentLength;
                    Stream datastream = response.GetResponseStream();
                    StreamReader sr = new StreamReader(datastream);
                    result = sr.ReadToEnd();
                    sr.Close();
                    datastream.Close();
                    response.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// 获取指定目录下明细(包含文件和文件夹)
            /// </summary>
            /// <param name="tagDir">目录名</param>
            /// <returns></returns>
            public static string[] GetFilesDetailList(string tagDir)
            {
                try
                {
                    StringBuilder result = new StringBuilder();
                    FtpWebRequest ftp;
                    ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(_serverUri + "/" + tagDir));
                    ftp.Credentials = new NetworkCredential(_ftpUserName, _ftpUserPwd);

                    ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                    WebResponse response = ftp.GetResponse();
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        result.Append(line);
                        result.Append("\n");

                        line = reader.ReadLine();
                    }
                    if (result.ToString() != "")
                        result.Remove(result.ToString().LastIndexOf("\n"), 1);
                    reader.Close();
                    response.Close();
                    return result.ToString().Split('\n');
                }
                catch
                {
                    return null;
                }
            }

            /// <summary>
            /// 获取指定目录下所有的文件夹列表(仅文件夹)
            /// </summary>
            /// <param name="tagDir">目录名</param>
            /// <returns></returns>
            private static string[] GetDirectoryList(string tagDir)
            {
                string[] drectory = GetFilesDetailList(tagDir);
                string m = string.Empty;
                if (drectory != null)
                {
                    foreach (string str in drectory)
                    {
                        if (str != "")
                        {
                            if (str.Trim().Substring(0, 1).ToUpper() == "D")
                            {
                                m += str.Trim() + "\n";
                            }
                        }
                    }
                }
                char[] n = new char[] { '\n' };
                return m.Split(n);
            }

            /// <summary>
            /// 判断当前目录下指定的子目录是否存在,不存在自动创建目录
            /// </summary>
            /// <param name="RemoteDirectoryName">指定的目录名</param>
            public static void DirectoryExist(string RemoteDirectoryName)
            {
                if (RemoteDirectoryName.IndexOf('/') != 0)
                    RemoteDirectoryName = "/" + RemoteDirectoryName;
                if (RemoteDirectoryName.LastIndexOf('/') == RemoteDirectoryName.Length - 1)
                    RemoteDirectoryName = RemoteDirectoryName.Substring(0, RemoteDirectoryName.Length - 2);
                string[] dirs = RemoteDirectoryName.Split('/');
                if (dirs.Length == 2)
                {
                    if (!DrectoryExist(dirs[0], dirs[1]))
                        MakeDir(dirs[0], dirs[1]);

                }
                string currentDir = string.Empty;
                for (int i = 0; i < dirs.Length - 1; i++)
                {
                    if (dirs[i] != "")
                        currentDir += "/" + dirs[i];
                    if (DrectoryExist(currentDir, dirs[i + 1]))
                        continue;
                    else
                        MakeDir(currentDir, dirs[i + 1]);
                }
            }

            /// <summary>
            /// 判断当前文件夹下是否存在目标文件夹
            /// </summary>
            /// <param name="currentDir">当前目录名</param>
            /// <param name="nextDir">目标目录名</param>
            /// <returns></returns>
            private static bool DrectoryExist(string currentDir, string nextDir)
            {
                string[] dirList = GetDirectoryList(currentDir);
                foreach (string str in dirList)
                {
                    if (GetDirName(str.Trim()).Trim() == nextDir.Trim())
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// 解析ftp文件名
            /// </summary>
            /// <param name="originalName">原始文件名</param>
            /// <returns></returns>
            protected static string GetDirName(string originalName)
            {
                string[] s = originalName.Replace(" ", "$").Split('$');
                return s[s.Length - 1];
            }


            /// <summary>
            /// 创建文件夹
            /// </summary>
            /// <param name="pDirName">要创建新文件夹的目录</param>
            /// <param name="dirName">文件夹名称</param>
            public static void MakeDir(string pDirName, string dirName)
            {
                FtpWebRequest reqFTP;
                try
                {
                    if (pDirName != string.Empty && pDirName != null)
                        pDirName = pDirName + "/";
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(_serverUri + pDirName + dirName));
                    reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                    reqFTP.UseBinary = true;
                    reqFTP.Credentials = new NetworkCredential(_ftpUserName, _ftpUserPwd);
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    Stream ftpStream = response.GetResponseStream();

                    ftpStream.Close();
                    response.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// 移动文件
            /// </summary>
            /// <param name="remoteFile">文件路径</param>
            /// <param name="newDirectory">目标文件夹</param>
            public static bool Move(string remoteFile, string newFileName)
            {
                FtpWebRequest reqFTP;
                try
                {
                    if (string.IsNullOrEmpty(remoteFile) || string.IsNullOrEmpty(newFileName))
                        return false;
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(_serverUri + remoteFile));
                    reqFTP.Method = WebRequestMethods.Ftp.Rename;
                    reqFTP.UseBinary = true;
                    reqFTP.Credentials = new NetworkCredential(_ftpUserName, _ftpUserPwd);
                    reqFTP.RenameTo = newFileName;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    Stream ftpStream = response.GetResponseStream();

                    ftpStream.Close();
                    response.Close();

                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
    }
}
