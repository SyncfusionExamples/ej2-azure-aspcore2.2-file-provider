using Syncfusion.EJ2.FileManager.AzureFileProvider;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using Syncfusion.EJ2.FileManager.Base;
using System;

namespace EJ2APIServices.Controllers
{

    [Route("api/[controller]")]
    [EnableCors("AllowAllOrigins")]
    public class AzureFileManagerController : Controller
    {
        public AzureFileProvider operation;
        public AzureFileManagerController(IHostingEnvironment hostingEnvironment)
        {
            this.operation = new AzureFileProvider();
            this.operation.RegisterAzure("<--accountName-->", "<--accountKey-->", "<--blobName-->");
            this.operation.SetBlobContainer("<--blobPath-->", "<--filePath-->");
            //----------
            //for example 
            //this.operation.RegisterAzure("azure_service_account", "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx", "files");
            //this.operation.setBlobContainer("https://azure_service_account.blob.core.windows.net/files/", "https://azure_service_account.blob.core.windows.net/files/Files");
            //---------

        }
        [Route("AzureFileoperations")]
        public object AzureFileoperations([FromBody] FileManagerDirectoryParams args)
        {
            if (args.Path != "")
            {

                string startPath = "<--blobPath-->";
                string originalPath = ("<--filePath-->").Replace(startPath, "");
                //-----------------
                // for example
                //string startPath = "https://azure_service_account.blob.core.windows.net/files/";
                //string originalPath = ("https://azure_service_account.blob.core.windows.net/files/Files").Replace(startPath, "");
                //-------------------

                args.Path = (originalPath + args.Path).Replace("//", "/");
                args.TargetPath = (originalPath + args.TargetPath).Replace("//", "/");
            }
            switch (args.Action)
            {
                case "read":
                    // reads the file(s) or folder(s) from the given path.
                    return Json(this.ToCamelCase(this.operation.GetFiles(args.Path, args.Data)));
                case "delete":
                    // deletes the selected file(s) or folder(s) from the given path.
                    return this.ToCamelCase(this.operation.Delete(args.Path, args.Names, args.Data));
                case "details":
                    // gets the details of the selected file(s) or folder(s).
                    return this.ToCamelCase(this.operation.Details(args.Path, args.Names, args.Data));
                case "create":
                    // creates a new folder in a given path.
                    return this.ToCamelCase(this.operation.Create(args.Path, args.Name));
                case "search":
                    // gets the list of file(s) or folder(s) from a given path based on the searched key string.
                    return this.ToCamelCase(this.operation.Search(args.Path, args.SearchString, args.ShowHiddenItems, args.CaseSensitive, args.Data));
                case "rename":
                    // renames a file or folder.
                    return this.ToCamelCase(this.operation.Rename(args.Path, args.Name, args.NewName, false, args.Data));
                case "copy":
                    // copies the selected file(s) or folder(s) from a path and then pastes them into a given target path.
                    return this.ToCamelCase(this.operation.Copy(args.Path, args.TargetPath, args.Names, args.RenameFiles, args.TargetData, args.Data));
                case "move":
                    // cuts the selected file(s) or folder(s) from a path and then pastes them into a given target path.
                    return this.ToCamelCase(this.operation.Move(args.Path, args.TargetPath, args.Names, args.RenameFiles, args.TargetData, args.Data));

            }
            return null;
        }
        public string ToCamelCase(object userData)
        {
            return JsonConvert.SerializeObject(userData, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });
        }

        // uploads the file(s) into a specified path
        [Route("AzureUpload")]
        public ActionResult AzureUpload(FileManagerDirectoryParams args)
        {

            if (args.Path != "")
            {
                string startPath = "<--blobPath-->";
                string originalPath = ("<--filePath-->").Replace(startPath, "");
                args.Path = (originalPath + args.Path).Replace("//", "/");
                //----------------------
                //for example
                //string startPath = "https://azure_service_account.blob.core.windows.net/files/";
                //string originalPath = ("https://azure_service_account.blob.core.windows.net/files/Files").Replace(startPath, "");
                //args.Path = (originalPath + args.Path).Replace("//", "/");
                //----------------------
            }

            operation.Upload(args.Path, args.UploadFiles, args.Action, args.Data);
            return Json("");

        }

        // downloads the selected file(s) and folder(s)
        [Route("AzureDownload")]
        public object AzureDownload(string downloadInput)
        {
            FileManagerDirectoryParams args = JsonConvert.DeserializeObject<FileManagerDirectoryParams>(downloadInput);
            return operation.Download(args.Path, args.Names, args.Data);
        }

        // gets the image(s) from the given path
        [Route("AzureGetImage")]
        public IActionResult AzureGetImage(FileManagerDirectoryParams args)
        {
            return this.operation.GetImage(args.Path, args.Id, true, null, args.Data);
        }
    }


    public class FileManagerDirectoryParams
    {
        public string Path { get; set; }
        public string Action { get; set; }
        public string NewName { get; set; }
        public string[] Names { get; set; }
        public string TargetPath { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public string PreviousName { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DateCreated { get; set; }
        public bool HasChild { get; set; }
        public bool IsFile { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
        public string FilterPath { get; set; }
        public string FilterId { get; set; }
        public IList<IFormFile> UploadFiles { get; set; }
        public bool CaseSensitive { get; set; }
        public string SearchString { get; set; }
        public bool ShowHiddenItems { get; set; }

        public string IconClass { get; set; }

        public string NodeId { get; set; }

        public string ParentID { get; set; }

        public bool Selected { get; set; }

        public string Icon { get; set; }

        public string[] RenameFiles { get; set; }
        public FileManagerDirectoryContent[] Data { get; set; }

        public FileManagerDirectoryContent TargetData { get; set; }
    }

}

