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
using BLL.DTO;
using BLL.Interfaces;
using BLL.Services;
using Newtonsoft.Json;
using WebUI.Models;
using static WebUI.Models.Mapper;

namespace WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IFolderService folderService;
        private readonly IUserService userService;
        private readonly IFileTypeService fileTypeService;
        private readonly IFileService fileService;

        private DtoUser user;

        public HomeController(IFolderService fodlerService, IUserService userService, IFileTypeService fileTypeService ,IFileService fileService)
        {
            this.folderService = fodlerService;
            this.userService = userService;
            this.fileService = fileService;
            this.fileTypeService = fileTypeService;

        }

        public ActionResult Index()
        {
            if (ReferenceEquals(user, null))
            {
                user = GetUser();
            }

            string jsonText = folderService.ToTreeJson(user.ID);

            return View(new FolderViewModel { FolderStructJson = jsonText });
        }

        public ActionResult AddFolder(string title,string id)
        {
            if (ReferenceEquals(user, null))
            {
                user = GetUser();
            }

            long ID;

            if (string.IsNullOrEmpty(id))
            {
                ID = user.Folders.Min(folder => folder.ID);
                folderService.AddFolder(folderService.GetById(ID), title);
                return Json(new {parentId = ID, data = folderService.ToTreeJson(user.ID)});
            }

            long.TryParse(id, out ID);

            DtoFolder newFolder = folderService.AddFolder(folderService.GetById(ID), title);

            return Json(new { parentId = id, id = newFolder.ID });
        }

        public ActionResult EditFolder(string title, long id)
        {
            if (!ReferenceEquals(title,null))
            {
                folderService.UpdateFolderTitle(title, id);
            }
            return Json("");
        }

        public ActionResult DeleteFolder(long id)
        {
            if (ModelState.IsValid)
            {
                folderService.DeleteFolder(folderService.GetById(id));
            }
            return Json("");
        }

        public ActionResult Configurate()
        {
            return Json(new { maxFileSize = 2147483647, container = "vaultObj", uploadUrl = Url.Action("LoadFiles","Home")}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> LoadFiles(HttpPostedFileBase file, string ID)
        {
            if (!ReferenceEquals(file, null) && !string.IsNullOrEmpty(ID)) 
            {
                var fileData = new MemoryStream();
                await file.InputStream.CopyToAsync(fileData);

                if (ReferenceEquals(user, null))
                {
                    user = GetUser();
                }

                long id;

                long.TryParse(ID, out id);

                string format = file.FileName.Split('.')[1];
                List<DtoFileType> fileTypes = fileTypeService.GetFileTypesByPredicate(fl => fl.Format.Equals(format)).ToList();

                if (!fileTypes.Any())
                {
                    fileTypes.Add(fileTypeService.CreateFileType(new DtoFileType
                    {
                        Format = format,
                        TypeName = file.ContentType
                    }));                    
                }

                DtoFile newFile = new DtoFile {DateTime = DateTime.Now, Data = fileData.ToArray(), Title = file.FileName, FolderID = id ,FileTypes = fileTypes};

                fileService.CreateFile(newFile);

                return Json(new {state = true, name = file.FileName, size = file.ContentLength});
            }

            return Json("");
        }

        public ActionResult LoadGrid(long id)
        {
            return Json(folderService.GetById(id).ToGridJson());
        }

        [Authorize(Roles = "admin")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [NonAction]
        private DtoUser GetUser() => 
            HttpContext.User.Identity.Name.Contains("@") ? userService.GetUserByPredicate(usr => usr.Email.Equals(HttpContext.User.Identity.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() :
                                                             userService.GetUserByPredicate(usr => usr.Login.Equals(HttpContext.User.Identity.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

    }
}