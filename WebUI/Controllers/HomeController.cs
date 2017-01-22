using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using BLL.DTO;
using BLL.Interfaces;
using Newtonsoft.Json;
using WebUI.Models;
using static WebUI.Models.Mapper;

namespace WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IFolderService fodlerService;
        private readonly IUserService userService;

        private DtoUser user;

        public HomeController(IFolderService fodlerService, IUserService userService)
        {
            this.fodlerService = fodlerService;
            this.userService = userService;

        }

        public ActionResult Index()
        {
            if (ReferenceEquals(user, null))
            {
                user = GetUser();
            }

            string jsonText = fodlerService.ToJson(user.ID);

            return View(new FolderViewModel { FolderStructJson = jsonText});
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
                fodlerService.AddFolder(fodlerService.GetById(ID), title);
                return Json(new {parentId = ID, data = fodlerService.ToJson(user.ID), title = title});
            }

            long.TryParse(id, out ID);

            DtoFolder newFolder = fodlerService.AddFolder(fodlerService.GetById(ID), title);

            return Json(new { parentId = id, id = newFolder.ID, title = newFolder.Title });
        }

        public ActionResult EditFolder(string title, long id)
        {
            if (ReferenceEquals(user, null))
            {
                user = GetUser();
            }

            fodlerService.UpdateFolderTitle(title, id);

            return Json("");
        }

        public ActionResult DeleteFolder(long id)
        {
            if (ReferenceEquals(user, null))
            {
                user = GetUser();
            }

            fodlerService.DeleteFolder(fodlerService.GetById(id));

            return Json("");
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