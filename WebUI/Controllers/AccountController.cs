using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using BLL.Interfaces;
using CaptchaMvc.HtmlHelpers;
using CaptchaMvc.Interface;
using WebUI.Infrastructure;
using WebUI.Models;
using WebUI.Providers;

namespace WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService service;

        public AccountController(IUserService service)
        {
            this.service = service;
        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn(AccountViewModel viewModel, string returnUrl)
        {
            if (ModelState.IsValidField("Name") && ModelState.IsValidField("Password"))
            {
                if (Membership.ValidateUser(viewModel.Name, viewModel.Password))
                {
                    FormsAuthentication.SetAuthCookie(viewModel.Name, viewModel.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect login or password");
                }
            }
            return View("Index",viewModel);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("LogIn", "Account");
        }

        [HttpGet]
        public ActionResult Register()
        {
            ViewBag.Redirect = true;
            return View("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(AccountViewModel viewModel)
        {
            ViewBag.Redirect = true;

            if (viewModel.Captcha != (string)Session[CaptchaImage.CaptchaValueKey])
            {
                ModelState.AddModelError("Captcha", "Incorrect input");
                return View("Index", viewModel);
            }

            if (!service.IsFreeLogin(viewModel.Name))
            {
                ModelState.AddModelError("", "User with this username already registered.");
                return View("Index", viewModel);
            }

            if (!service.IsFreeEmail(viewModel.Email))
            {
                ModelState.AddModelError("", "User with this e-mail address already registered.");
                return View("Index", viewModel);
            }

            if (ModelState.IsValid)
            {
                ModelState.Clear();

                var isUserCreated = ((CustomMembershipProvider)Membership.Provider)
                    .CreateUser(viewModel.Name, viewModel.Email, viewModel.Password);

                if (isUserCreated)
                {
                    FormsAuthentication.SetAuthCookie(viewModel.Email, false);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Error registration.");
            }
            return View("Index", viewModel);
        }

        //В сессии создаем случайное число от 1111 до 9999.
        //Создаем в ci объект CatchaImage
        //Очищаем поток вывода
        //Задаем header для mime-типа этого http-ответа будет "image/jpeg" т.е. картинка формата jpeg.
        //Сохраняем bitmap в выходной поток с форматом ImageFormat.Jpeg
        //Освобождаем ресурсы Bitmap
        //Возвращаем null, так как основная информация уже передана в поток вывод
        public ActionResult Captcha()
        {
            Session[CaptchaImage.CaptchaValueKey] =
                new Random(DateTime.Now.Millisecond).Next(1111, 9999).ToString(CultureInfo.InvariantCulture);
            var ci = new CaptchaImage(Session[CaptchaImage.CaptchaValueKey].ToString(), 211, 50, "Helvetica");

            // Change the response headers to output a JPEG image.
            Response.Clear();
            Response.ContentType = "image/jpeg";

            // Write the image to the response stream in JPEG format.
            ci.Image.Save(this.Response.OutputStream, ImageFormat.Jpeg);

            // Dispose of the CAPTCHA image object.
            ci.Dispose();
            return null;
        }
    }
}