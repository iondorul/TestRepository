using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;

namespace avt.ActionForm.Core.FileStorage
{
    public enum eHandleDuplicates
    {
        Ovewrite,
        Rename//,
        //ThrowError
    }

    public interface IFileStorage
    {
        string ComponentId { get; }
        Uri BuildUrl(string uploadsFolder, string relativeUrl, DateTime timestamp);
        string GetFilePath(string uploadsFolder, string relativeUrl);

        FileInfo Upload(string uploadsFolder, string filePath, string contentType, eHandleDuplicates handleDuplicates);
        FileInfo Upload(string uploadsFolder, string fileName, string contentType, Stream stream, FileMode mode, eHandleDuplicates handleDuplicates);

        void Download(string uploadsFolder, string relativeUrl, Stream output);
        Stream Download(string uploadsFolder, string relativeUrl);

        void Move(string uploadsFolder, string relativeUrl, string contentType, IFileStorage toStorage, eHandleDuplicates handleDuplicates);
        void Delete(string uploadsFolder, string relativeUrl);

    }
}
