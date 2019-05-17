using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerCheckUpdateLicense.Models
{
    public class FilePath
    {
        public string Filename { get; set; } // Filename of the file 
        public string Path { get; set; } // Path of the file on the server 
        public long Length { get; set; } // Size of the file (bytes) 
        public bool IsDirectory { get; set; } // true if the filename is 

    }
}