using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;

namespace avt.ActionForm.Core.FileStorage
{
    public class LocalFileStorage : IFileStorage
    {
        public string ComponentId { get; set; }

        public LocalFileStorage()
        {
            
        }

        string GetFolderPathAndCreate(string uploadsFolder)
        {
            string fullPath = Path.Combine(HttpRuntime.AppDomainAppPath, uploadsFolder.Trim('/', '\\'));
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);
            return fullPath;
        }

        public Uri BuildUrl(string uploadsFolder, string relativeUrl, DateTime timestamp)
        {
            //return new Uri(string.Format("{0}://{1}{2}/{3}/{4}", 
            //    HttpContext.Current.Request.Url.Scheme, 
            //    HttpContext.Current.Request.Url.Host,
            //    HttpContext.Current.Request.Url.IsDefaultPort ? "" : ":" + HttpContext.Current.Request.Url.Port,
            //    UploadsFolder.Trim('/'),
            //    relativeUrl.Trim('/')));

            return new Uri(string.Format("{0}/{1}/{2}", HttpRuntime.AppDomainAppVirtualPath.TrimEnd('/'), uploadsFolder, relativeUrl), UriKind.Relative);
        }

        public string GetFilePath(string uploadsFolder, string relativeUrl)
        {
            return Path.Combine(Path.Combine(HttpRuntime.AppDomainAppPath, uploadsFolder), relativeUrl);
        }


        public void Move(string uploadsFolder, string relativeUrl, string contentType, IFileStorage toStorage, eHandleDuplicates handleDuplicates)
        {
            var t = DateTime.Now;
            if (BuildUrl(uploadsFolder, relativeUrl, t).Equals(toStorage.BuildUrl(uploadsFolder, relativeUrl, t)))
                return; // can't move to same location

            var filePath = Path.Combine(GetFolderPathAndCreate(uploadsFolder), relativeUrl);
            if (!File.Exists(filePath))
                return;

            toStorage.Upload(uploadsFolder,filePath, contentType, handleDuplicates);
            File.Delete(filePath);
        }

        public void Delete(string uploadsFolder, string relativeUrl)
        {
            var filePath = Path.Combine(GetFolderPathAndCreate(uploadsFolder), relativeUrl);
            if (File.Exists(filePath))
                File.Delete(filePath);
        }



        public void Download(string uploadsFolder, string relativeUrl, Stream output)
        {
            using (var fs = File.OpenRead(GetFilePath(uploadsFolder, relativeUrl))) {
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, (int)fs.Length);
                output.Write(bytes, 0, (int)fs.Length);
            }
        }

        public Stream Download(string uploadsFolder, string relativeUrl)
        {
            return File.OpenRead(GetFilePath(uploadsFolder, relativeUrl));
        }


        public FileInfo Upload(string uploadsFolder, string filePath, string contentType, eHandleDuplicates handleDuplicates)
        {
            string fileName = Path.GetFileName(filePath);
            string localFilepath = Path.Combine(Path.Combine(HttpRuntime.AppDomainAppPath, uploadsFolder), fileName);

            if (filePath.Equals(localFilepath, StringComparison.OrdinalIgnoreCase))
                return new FileInfo(localFilepath);

            if (File.Exists(localFilepath)) {
                switch (handleDuplicates) {
                    case eHandleDuplicates.Ovewrite:
                        File.Delete(localFilepath);
                        break;
                    case eHandleDuplicates.Rename:
                        localFilepath = Path.GetDirectoryName(localFilepath) + Path.GetFileNameWithoutExtension(localFilepath) +
                            "-" + Guid.NewGuid().GetHashCode() + Path.GetExtension(localFilepath);
                        break;
                    //case eHandleDuplicates.ThrowError:
                    //    throw new Exception(string.Format("File {0} already exists", localFilepath));
                }
            }

            File.Copy(filePath, localFilepath);
            return new FileInfo(localFilepath);
        }

        public FileInfo Upload(string uploadsFolder, string fileName, string contentType, Stream stream, FileMode mode, eHandleDuplicates handleDuplicates)
        {
            //throw new NotImplementedException();
            string filePath = Path.Combine(GetFolderPathAndCreate(uploadsFolder), fileName);

            if (File.Exists(filePath)) {
                switch (handleDuplicates) {
                    case eHandleDuplicates.Ovewrite:
                        File.Delete(filePath);
                        break;
                    case eHandleDuplicates.Rename:
                        filePath = Path.GetDirectoryName(filePath) + '\\' + Path.GetFileNameWithoutExtension(filePath) +
                            "-" + Math.Abs(Guid.NewGuid().GetHashCode()) + Path.GetExtension(filePath);
                        break;
                    //case eHandleDuplicates.ThrowError:
                    //    throw new Exception(string.Format("File {0} already exists", filePath));
                }
            }

            FileStream file = new FileStream(filePath, mode, System.IO.FileAccess.Write);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);
            file.Write(bytes, 0, bytes.Length);
            file.Close();

            return new FileInfo(filePath);
        }
    }
}
