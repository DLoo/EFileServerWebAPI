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

namespace EFileServerWebAPI.Controllers
{
    public class FormPostController : ApiController
    {
        string AppRootPath = ConfigurationManager.AppSettings["AppRootPath"] ?? @"C:\EFileServerData";
        I_EFileRepository resp = new EFileRepository();

        [HttpPost]
        public EFileInfo EFileFromEForm() {
            EFileInfo eFile = null;

            // Check if the request contains multipart/form-data. 
            if (!Request.Content.IsMimeMultipartContent()) {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }



            // HttpResponseMessage result = null;
            Dictionary<string, string> dictParams = new Dictionary<string, string>();
            HttpPostedFileBase media = null;
            var httpRequest = HttpContext.Current.Request;
            foreach (string item in httpRequest.Form) {
                dictParams.Add(item, httpRequest.Form[item]);
            }

            if (httpRequest.Files.Count == 1) {
                media = new HttpPostedFileWrapper(httpRequest.Files[0]);
                //foreach (string file in httpRequest.Files) {
                //  var postedFile = httpRequest.Files[file];
                //var filePath = HttpContext.Current.Server.MapPath("~/" + postedFile.FileName);
                //postedFile.SaveAs(filePath);
                //}
            }

            MemoryStream mStream = new MemoryStream();
            media.InputStream.CopyTo(mStream);
            UploadFileInfo fMedia = new UploadFileInfo {
                Name = "media",
                FileName = media.FileName,
                ByteArray = mStream.ToArray(),
            };

            try {
                eFile = resp.Upload_File(AppRootPath, Convert.ToInt32(dictParams["year"]),
                    Convert.ToInt32(dictParams["month"]), dictParams["location"], dictParams["brand"],
                    dictParams["department"], dictParams["type"], dictParams["generateFrom"],
                    Convert.ToInt32(dictParams["expiryDuration"]), fMedia);
            } catch (InvalidOperationException ex) {
                throw new NotImplementedException(ex.Message, new Exception(ex.InnerException.ToString()));
            }
            return eFile;

        }

    }
}
