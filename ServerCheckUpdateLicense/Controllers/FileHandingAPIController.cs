using ServerCheckUpdateLicense.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ServerCheckUpdateLicense.Controllers
{
    public class FileHandingAPIController : ApiController
    {
        //Upload the File 
        [MimeMultipart]
        public async Task<UploadFIle> Post()
        {
            var uploadPath = HttpContext.Current.Server.MapPath("~/Uploads");

            if (Request.Content.IsMimeMultipartContent())
            {
                var filePath = Request.Headers.GetValues("filePath").ToList();
                string filepathfromclient = "";

                if (filePath != null)
                {
                    filepathfromclient = filePath[0];
                    uploadPath = uploadPath + filepathfromclient;
                }
            }

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var multipartFormDataStreamProvider = new UploadFileMultiparProvider(uploadPath);

            // Read the MIME multipart asynchronously 
            await Request.Content.ReadAsMultipartAsync(multipartFormDataStreamProvider);

            string _localFileName = multipartFormDataStreamProvider
                .FileData.Select(multiPartData => multiPartData.LocalFileName).FirstOrDefault();

            // Create response
            return new UploadFIle
            {
                FilePath = _localFileName,

                FileName = Path.GetFileName(_localFileName),

                FileLength = new FileInfo(_localFileName).Length
            };
        }

        //To Get File Details
        //api/ FileHandlingAPI/getFileInfo?Id=1
        [ActionName("get"), HttpGet]
        public IEnumerable<FilePath> getFileInfo(int Id)
        {
            List<FilePath> files = new List<FilePath>();
            var uploadPath = HttpContext.Current.Server.MapPath("~/Uploads");

            DirectoryInfo dirInfo = new DirectoryInfo(uploadPath);

            foreach (FileInfo fInfo in dirInfo.GetFiles())
            {
                files.Add(new FilePath() { Path = uploadPath, Filename = fInfo.Name, Length = fInfo.Length, IsDirectory = File.GetAttributes(uploadPath).HasFlag(FileAttributes.Directory) });
            }

            getAllSubfolderFiles(dirInfo, files);

            return files.ToList();
        }


        //Get all Folder Details
        public void getAllSubfolderFiles(DirectoryInfo dirInfo, List<FilePath> files)
        {
            foreach (DirectoryInfo dInfo in dirInfo.GetDirectories())
            {
                foreach (FileInfo fInfo in dInfo.GetFiles())
                {
                    files.Add(new FilePath() { Path = fInfo.DirectoryName, Filename = fInfo.Name, Length = fInfo.Length, IsDirectory = File.GetAttributes(fInfo.DirectoryName).HasFlag(FileAttributes.Directory) });
                    getAllSubfolderFiles(dInfo, files);
                }
            }
        }
    }
}
