using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using BLL;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Services;
using Newtonsoft.Json;
using WebUI.Models;
using static WebUI.Models.Mapper;
using System.CodeDom.Compiler;

namespace WebUI.Controllers
{
    [HandleError()]
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IFolderService folderService;
        private readonly IUserService userService;
        private readonly IFileTypeService fileTypeService;
        private readonly IFileService fileService;
        private DtoUser _user;

        private DtoUser user => GetUser();

        public HomeController(IFolderService fodlerService, IUserService userService, IFileTypeService fileTypeService, IFileService fileService)
        {
            this.folderService = fodlerService;
            this.userService = userService;
            this.fileService = fileService;
            this.fileTypeService = fileTypeService;

            
        }

        public ActionResult Index()
        {
            var test = user.SharedFolders.ToTreeJson(folderService);

            return View(new FileExplorerViewModel { TreeStructJson = folderService.ToTreeJson(user.ID),
                                                    UserName = user.Login ,
                                                    SharedTreeStructJson = user.SharedFolders.ToTreeJson(folderService)});
        }

        public ActionResult AddFolder(string title, string id)
        {
            long ID;

            if (string.IsNullOrEmpty(id))
            {
                ID = user.Folders.Min(folder => folder.ID);
                folderService.AddFolder(folderService.GetById(ID), title);
                return Json(new { parentId = ID, data = folderService.ToTreeJson(user.ID) });
            }

            long.TryParse(id, out ID);

            DtoFolder newFolder = folderService.AddFolder(folderService.GetById(ID), title);

            return Json(new { parentId = id, id = newFolder.ID });
        }

        [HttpPost]
        public ActionResult EditFolder(string title, long id)
        {
            if (!ReferenceEquals(title, null))
            {
                folderService.UpdateFolderTitle(title, id);
            }

            return Json("");
        }

        public ActionResult ShareFolder(long folderId,string usersLogins)
        {
            DtoUser[] users = ParseUsers(usersLogins);

            if (users.Length != 0)
                folderService.ShareFolderToUsers(folderService.GetById(folderId), users);

            return Json("");
        }

        public ActionResult UnshareFolder(long folderId, string usersLogins)
        {
            DtoUser[] users = ParseUsers(usersLogins);

            if (users.Length != 0)
                folderService.RemoveAccessToFolderToUsers(folderService.GetById(folderId), users);

            return Json("");
        }

        [HttpPost]
        public ActionResult EditFile(string title, long id)
        {
            if (!ReferenceEquals(title, null))
            {
                DtoFile file = fileService.GetFileById(id);
                string format = file.FileTypes.First().Format;

                fileService.UpdateFileTitle(title.Split('.')[0] + $".{format}", id);
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult Search(string text)
        {

            if (!ReferenceEquals(text, null))
            {
                var folders = user.Folders.Where(folder => folder.Title.IndexOf(text,StringComparison.InvariantCultureIgnoreCase) >= 0);

                List<DtoFile> files = new List<DtoFile>();

                foreach(var folder in user.Folders)
                {
                    files.AddRange(folder.Files.Where(file => file.Title.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0));
                }

                List<IEntity> searchEnumeration = new List<IEntity>(folders);
                searchEnumeration.AddRange(files);

                return Json(searchEnumeration.ToGridJson(folderService));
            }

            return Json("");
        }

        [HttpPost]
        public ActionResult Delete(long id, string type)
        {
            if (type.Equals("Folder"))
            {
                var parentFolder = folderService.GetById(id);

                foreach (var folder in folderService.GetChildNodes(parentFolder).Reverse())
                {
                    foreach (var file in folder.Files)
                    {
                        fileService.DeleteFile(file);
                    }
                }

                folderService.DeleteFolder(parentFolder);

                return Json("");
            }
            if (type.Equals("File"))
            {
                var file = fileService.GetFileById(id);
                fileService.DeleteFile(file);

                return Json(folderService.ToGridJson(folderService.GetById(file.FolderID)));
            }

            return Json("");
        }

        public ActionResult Configurate()
            => Json(new { maxFileSize = 2147483647, container = "vaultObj", uploadUrl = Url.Action("UploadFiles", "Home")}, JsonRequestBehavior.AllowGet);
        
        [HttpPost]
        public async Task<ActionResult> UploadFiles(HttpPostedFileBase file, string ID, string mode)
        {
            
            if (!ReferenceEquals(file, null) && !string.IsNullOrEmpty(ID)) 
            {
                using (var fileData = new MemoryStream(new byte[file.InputStream.Length]))
                {
                    await file.InputStream.CopyToAsync(fileData);

                    long id;

                    long.TryParse(ID, out id);

                    string format = file.FileName.Split('.')[1];
                    DtoFileType fileType = fileTypeService.GetFileTypesByPredicate(fl => fl.Format.Equals(format)).FirstOrDefault();

                    if (ReferenceEquals(fileType, null))
                    {
                        fileType = fileTypeService.CreateFileType(new DtoFileType
                        {
                            Format = format,
                            TypeName = file.ContentType
                        });
                    }                  

                    /*var data = new byte[file.InputStream.Length];

                    fileData.Position = 0;
                    for (int totalBytesCopied = 0; totalBytesCopied < fileData.Length;)
                        totalBytesCopied += fileData.Read(data, totalBytesCopied, Convert.ToInt32(fileData.Length) - totalBytesCopied);
                    */

                    DtoFile newFile = new DtoFile { DateTime = DateTime.Now, Data = fileData.ToArray(), Title = file.FileName, FolderID = id, FileTypes = new[] { fileType } };

                    fileService.CreateFile(newFile);
                }

                return Json(new {state = true, name = file.FileName, size = file.ContentLength});
            }

            return Json("");
        }

        public ActionResult LoadGrid(long id)
        {
            if (id == 0)
                id = user.Folders.Min(folder => folder.ID);

            return Json(folderService.ToGridJson(folderService.GetById(id)));
        }

        public ActionResult LoadFile(long id)
        {
            var file = fileService.GetFileById(id);

            //Response.BufferOutput = false;
            return File(file.Data, file.FileTypes.First().TypeName, file.Title);
        }

        public ActionResult LoadUsers()
        {
            return Json("");
        }

        [Authorize(Roles = "admin")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [NonAction]
        private DtoUser[] ParseUsers(string ids)
            => ids.Split(';').Select(item => userService.GetUserByPredicate(user => user.Login.Equals(item.Trim(' '), StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault()).Where(item => !ReferenceEquals(item, null) && item.ID != user.ID).ToArray();

        [NonAction]
        private DtoUser GetUser()
        {
            if (ReferenceEquals(_user, null))
            {
                _user = HttpContext.User.Identity.Name.Contains("@") ? userService.GetUserByPredicate(usr => usr.Email.Equals(HttpContext.User.Identity.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() :
                                                             userService.GetUserByPredicate(usr => usr.Login.Equals(HttpContext.User.Identity.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
            }
            return _user;
        }
    }
}