using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;

namespace ClientTest
{
    public class Helper
    {
        private const string urlConst = "http://localhost:51389";

        public string GetInfo(string _keyName)
        {
            string url = urlConst + "/api/FileHandlingAPI/getFileInfo?Id=" + _keyName;
            var obj = GetFileInformation(url);
            return obj == null ? "" : obj.Filename;
        }
        public bool SetDownload(string _keyName, ref string _stt)
        {
            string url = urlConst + "/Uploads/";
            string downloadFileName = _keyName;
            string downloadPath = Application.StartupPath + @"\Downloads\";

            if (!Directory.Exists(downloadPath))
                Directory.CreateDirectory(downloadPath);

            Boolean isFileDownloaded = Download(url, downloadFileName, downloadPath);
            if (isFileDownloaded)
            {
                _stt = "File Downloaded";
            }
            else
            {
                _stt = "File Not Downloaded";
            }

            return isFileDownloaded;
        }

        public bool SetUpload(string _keyName, ref string _stt)
        {
            string url = urlConst + "/api/FileHandlingAPI";
            string filePath = @"\";
            Random rnd = new Random();
            string uploadFileName = "Imag" + rnd.Next(9999).ToString();

            var isFileUploaded = Upload(url, filePath, _keyName, uploadFileName);
            if (isFileUploaded)
            {
                _stt = "File Uploaded";
            }
            else
            {
                _stt = "File Not Uploaded";
            }

            return isFileUploaded;
        }

        private bool Download(string url, string downloadFileName, string downloadFilePath)
        {
            string downloadfile = downloadFilePath + downloadFileName;
            string httpPathWebResource = null;
            Boolean ifFileDownoadedchk = false;
            ifFileDownoadedchk = false;
            WebClient myWebClient = new WebClient();
            httpPathWebResource = url + downloadFileName;
            myWebClient.DownloadFile(httpPathWebResource, downloadfile);
            ifFileDownoadedchk = true;
            return ifFileDownoadedchk;
        }

        private bool Upload(string url, string filePath, string localFilename, string uploadFileName)
        {
            var isFileUploaded = false;
            try
            {
                var message = new HttpRequestMessage();
                var content = new MultipartFormDataContent();
                //
                var filestream = new FileStream(localFilename, FileMode.Open);
                var fileInfo = new FileInfo(localFilename);
                content.Headers.Add("filePath", filePath);
                content.Add(new StreamContent(filestream), "\"file\"", string.Format("\"{0}\"", uploadFileName + fileInfo.Extension));

                message.Method = HttpMethod.Post;
                message.Content = content;
                message.RequestUri = new Uri(urlConst + "/api/test/filesNoContentType");

                var client = new HttpClient();
                client.SendAsync(message).ContinueWith(task =>
                {
                    if (task.Result.IsSuccessStatusCode)
                    {
                        var result = task.Result;
                        Console.WriteLine(result);
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                isFileUploaded = false;
            }
            Console.ReadLine();
            return isFileUploaded;
        }

        /// <summary>
        /// var t = Task.Run(() => GetResponseFromURI(new Uri("http://www.google.com")));
        /// t.Wait();
        /// t.Result
        /// </summary>
        /// <param name="url"></param>
        private ServerFileInformation GetFileInformation(string url)
        {
            var filesinformation = new List<ServerFileInformation>();
            try
            {
                using (var client = new HttpClient())
                {
                    using (var response = client.GetAsync(url).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var fileJsonString = response.Content.ReadAsStringAsync().Result;
                            filesinformation = JsonConvert.DeserializeObject<ServerFileInformation[]>(fileJsonString).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return filesinformation[0];
        }
    }

    public class ServerFileInformation
    {
        public string Filename { get; set; } // Filename of the file 
        public string Path { get; set; } // Path of the file on the server 
        public long Length { get; set; } // Size of the file (bytes) 
        public bool IsDirectory { get; set; } // true if the filename is 
    }
    public class UploadFIle
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public long FileLength { get; set; }
    }
}
