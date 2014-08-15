using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Configuration;
using Ames.Abstract;
using Ames.Domain;
using Ames.Entities;
using Ames.Infrastructue;


namespace EFileServerWebAPI.Controllers
{
    public class EFileController : ApiController
    {
        string AppRootPath = ConfigurationManager.AppSettings["AppRootPath"] ?? @"C:\EFileServerData";
        I_EFileRepository resp = new EFileRepository();

        [HttpGet, ActionName("Test1")]
        [ApiProfileAction]
        public string GetEFile() {
            return "Hello me!";
        }

        [HttpGet, ActionName("Test2")]
        [ApiProfileAction]
        public EFileInfo GetEFileInfoByFileGuid(string fileGuid) {
            EFileInfo eF;
            if (String.IsNullOrEmpty(fileGuid)) {
                eF = new EFileInfo {
                    EFileName = "abc.txt",
                    EFileID = 100,
                    FileGUID = Guid.NewGuid(),
                    GeneratedFrom = "Testing",
                    Department = "Outlet",
                    ExpiryDate = DateTime.Now,
                    Brand = "YN",
                    Year = 2014,
                    DirectoryPath = "\\",
                    Location = "SG",
                    Month = 6,
                    Type = "DSR"
                };
            } else {
                eF = resp.Get_EFileByGUID(fileGuid);
            }

            return eF;
        }

        //[HttpPost, ActionName("efile")]
        [HttpPost]
        [ApiProfileAction]
        public EFileInfo PosteFile(WebApiParameters wParams) {
            EFileInfo eFileResult = null;
            var httpRequest = HttpContext.Current.Request;
            
            try {
                eFileResult = resp.Upload_File(AppRootPath, Convert.ToInt32(wParams.Parameters["year"]), 
                    Convert.ToInt32(wParams.Parameters["month"]), wParams.Parameters["location"], 
                    wParams.Parameters["brand"], wParams.Parameters["department"], wParams.Parameters["type"],
                    wParams.Parameters["generateFrom"], Convert.ToInt32(wParams.Parameters["expiryDuration"]), 
                    wParams.UploadFiles.First());
            } catch (InvalidOperationException ex) {
                throw new NotImplementedException(ex.Message, new Exception(ex.InnerException.ToString()));
            }
            return eFileResult;
        }



        
        
    }
}
