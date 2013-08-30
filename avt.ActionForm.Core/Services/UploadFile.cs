using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System;
using avt.ActionForm.Core.FileStorage;
using avt.ActionForm.Utils;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Modules;
using avt.ActionForm.Core.Form;

namespace avt.ActionForm.Core.Services
{
    public class FilesStatus
    {
        public const string HandlerPath = "/Upload/";

        public string group { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int size { get; set; }
        public string progress { get; set; }
        public string url { get; set; }
        public string relativeurl { get; set; }
        public string thumbnail_url { get; set; }
        public string delete_url { get; set; }
        public string delete_type { get; set; }
        public string error { get; set; }

        public FilesStatus() { }

        public FilesStatus(FileInfo fileInfo) { SetValues(fileInfo.Name, (int)fileInfo.Length, fileInfo.FullName); }

        public FilesStatus(string fileName, int fileLength, string fullPath) { SetValues(fileName, fileLength, fullPath); }

        private void SetValues(string fileName, int fileLength, string fullPath)
        {
            name = fileName;
            type = "image/png";
            size = fileLength;
            progress = "1.0";
            relativeurl = Path.GetFileName(fullPath);
            url = HandlerPath + "UploadHandler.ashx?f=" + relativeurl;
            delete_url = HandlerPath + "UploadHandler.ashx?f=" + relativeurl;
            delete_type = "DELETE";

            var ext = Path.GetExtension(fullPath);

            var fileSize = ConvertBytesToMegabytes(size);
            //if (fileSize > 3 || !IsImage(ext)) 
            thumbnail_url = "/Content/img/generalFile.png";
            //else thumbnail_url = @"data:image/png;base64," + EncodeFile(fullPath);
        }

        private bool IsImage(string ext)
        {
            return ext == ".gif" || ext == ".jpg" || ext == ".png";
        }

        private string EncodeFile(string fileName)
        {
            return Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName));
        }

        static double ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
    }

    /// <summary>
    /// Summary description for UploadHandler
    /// </summary>
    public class UploadHandler : IHttpHandler
    {
        private readonly JavaScriptSerializer js;

        //private string StorageRoot
        //{
        //    get { 
        //        //return Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/Files/")); 
        //        var azureLocalResource = RoleEnvironment.GetLocalResource("LocalFileStorage");
        //        var filepath = Path.Combine(azureLocalResource.RootPath, "Files");
        //        if (!Directory.Exists(filepath))
        //            Directory.CreateDirectory(filepath);
        //        return filepath;
        //    }
        //}

        ModuleInfo _Module;
        IFileStorage _Storage;
        string _UploadFolder;
        eHandleDuplicates _HandleDuplicates;
        string _FilePattern = "";
        FormData _Data;


        public UploadHandler()
        {
            js = new JavaScriptSerializer();
            js.MaxJsonLength = 41943040;
            _Storage = App.Instance.Container.ResolveService<IFileStorage>();
        }

        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.AddHeader("Pragma", "no-cache");
            context.Response.AddHeader("Cache-Control", "private, no-cache");
            //System.Threading.Thread.Sleep(3000);

            // TODO: through config
            int mid;
            if (!int.TryParse(context.Request.QueryString["mid"], out mid))
                throw new ArgumentException("Invalid module");
            _Module = new ModuleController().GetModule(mid);

            int fieldId;
            if (!int.TryParse(context.Request.QueryString["fieldid"], out fieldId))
                throw new ArgumentException("Invalid field id");

            ActionFormSettings settings = new ActionFormSettings(mid);
            var field = settings.Fields.FirstOrDefault(x=>x.FormFieldId == fieldId);
            if (field == null)
                throw new ArgumentException("Invalid field");

            var portal = new PortalController().GetPortal(_Module.PortalID);
            _UploadFolder = portal.HomeDirectory + "/" + field.Parameters.GetValue("Folder", "").Trim('/');
            _HandleDuplicates =  field.Parameters.GetValue<eHandleDuplicates>("Duplicates", eHandleDuplicates.Rename);
            _FilePattern = field.Parameters.GetValue("Pattern", "");
            _Data = new FormData(settings, context);

            HandleMethod(context);
        }

        // Handle request based on method
        private void HandleMethod(HttpContext context)
        {
            switch (context.Request.HttpMethod) {
                case "HEAD":
                case "GET":
                    if (GivenFilename(context))
                        DeliverFile(context);
                    break;

                case "POST":
                case "PUT":
                    UploadFile(context);
                    break;

                case "DELETE":
                    DeleteFile(context);
                    break;

                case "OPTIONS":
                    ReturnOptions(context);
                    break;

                default:
                    context.Response.ClearHeaders();
                    context.Response.StatusCode = 405;
                    break;
            }
        }

        private static void ReturnOptions(HttpContext context)
        {
            context.Response.AddHeader("Allow", "DELETE,GET,HEAD,POST,PUT,OPTIONS");
            context.Response.StatusCode = 200;
        }

        // Delete file from the server
        private void DeleteFile(HttpContext context)
        {
            _Storage.Delete(_UploadFolder, context.Request["f"]);
        }

        // Upload file to the server
        private void UploadFile(HttpContext context)
        {
            var statuses = new List<FilesStatus>();
            var headers = context.Request.Headers;

            if (string.IsNullOrEmpty(headers["X-File-Name"])) {
                UploadWholeFile(context, statuses);
            } else {
                UploadPartialFile(headers["X-File-Name"], context, statuses);
            }

            WriteJsonIframeSafe(context, statuses);
        }

        // Upload partial file
        private void UploadPartialFile(string fileName, HttpContext context, List<FilesStatus> statuses)
        {
            if (context.Request.Files.Count != 1) throw new HttpRequestValidationException("Attempt to upload chunked file containing more than one fragment per request");
            var inputStream = context.Request.Files[0].InputStream;
            var storageFileName = string.IsNullOrEmpty(_FilePattern) ? Path.GetFileName(fileName)
                : _Data.ApplyAllTokens(_FilePattern, _Module);
            var fileInfo = _Storage.Upload(_UploadFolder, storageFileName, null, inputStream, FileMode.Append, _HandleDuplicates);

            statuses.Add(new FilesStatus(fileInfo));
        }

        // Upload entire file
        private void UploadWholeFile(HttpContext context, List<FilesStatus> statuses)
        {
            for (int i = 0; i < context.Request.Files.Count; i++) {
                var file = context.Request.Files[i];

                //var fullPath = StorageRoot + Path.GetFileName(file.FileName);
                var storageFileName = string.IsNullOrEmpty(_FilePattern) ? Path.GetFileName(file.FileName)
                    : _Data.ApplyAllTokens(_FilePattern, _Module) + Path.GetExtension(file.FileName);

                var fileInfo = _Storage.Upload(_UploadFolder, storageFileName, null, file.InputStream, FileMode.Create, _HandleDuplicates);

                statuses.Add(new FilesStatus(fileInfo));
            }
        }

        private void WriteJsonIframeSafe(HttpContext context, List<FilesStatus> statuses)
        {
            context.Response.AddHeader("Vary", "Accept");
            try {
                if (context.Request["HTTP_ACCEPT"].Contains("application/json"))
                    context.Response.ContentType = "application/json";
                else
                    context.Response.ContentType = "text/plain";
            } catch {
                context.Response.ContentType = "text/plain";
            }

            var jsonObj = js.Serialize(statuses.ToArray());
            context.Response.Write(jsonObj);
        }

        private static bool GivenFilename(HttpContext context)
        {
            return !string.IsNullOrEmpty(context.Request["f"]);
        }

        private void DeliverFile(HttpContext context)
        {
            var filename = context.Request["f"];
            var filePath = _Storage.GetFilePath(_UploadFolder, filename);// Path.Combine(StorageRoot, filename);

            if (File.Exists(filePath)) {
                context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + filename + "\"");
                context.Response.ContentType = "application/octet-stream";
                context.Response.ClearContent();
                context.Response.WriteFile(filePath);
            } else
                context.Response.StatusCode = 404;
        }

        //private void ListCurrentFiles(HttpContext context)
        //{
        //    var files =
        //        new DirectoryInfo(StorageRoot)
        //            .GetFiles("*", SearchOption.TopDirectoryOnly)
        //            .Where(f => !f.Attributes.HasFlag(FileAttributes.Hidden))
        //            .Select(f => new FilesStatus(f))
        //            .ToArray();

        //    string jsonObj = js.Serialize(files);
        //    context.Response.AddHeader("Content-Disposition", "inline; filename=\"files.json\"");
        //    context.Response.Write(jsonObj);
        //    context.Response.ContentType = "application/json";
        //}

    }
}