using DGHCM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;
using System.Drawing.Printing;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.Remoting.Messaging;
using Microsoft.Ajax.Utilities;
using System.Xml.Linq;
using System.IO;
using System.Net;
using System.Net.Mail;




namespace DGHCM.Controllers
{ 

    public class LoginController : Controller

    {
        CommonClass common = new CommonClass();
        HumanCapitalManagementEntities context = new HumanCapitalManagementEntities();

        // GET: Login
        [HttpGet]
        public ActionResult UserIndex()
        {
            var listofdata = context.UserDetails.ToList();
            return View(listofdata);
        }
        [HttpGet]
        public ActionResult UserCreate()
        {

            return View();
        }
        [HttpPost]
        public ActionResult UserCreate(FormCollection frm)
        {

            UserDetail model = new UserDetail();
            var companyId = common.CompanyId();
            model.CompanyId = Guid.Parse(companyId);

            model.UserName = frm["Username"];
            model.Userpassword = frm["password"];
            model.Email = frm["Email"];
            model.IsActive = frm["isactive"] == "on" ? true : false;
            model.CreatedBy = 0;
            model.CreatedDate = DateTime.Now;
            model.UpdatedBy = 0;
            model.UpdatedDate = DateTime.Now;
            var data = context.UserDetails.Add(model);
            context.SaveChanges();
            ViewBag.Message = "Data Insert Successfully";
            //return View(data);

            return RedirectToAction("UserIndex");
        }

        [HttpGet]
        public ActionResult UserEdit(int id)
        {
            var data = context.UserDetails.Where(x => x.UserId == id).FirstOrDefault();
            return View(data);
        }
        [HttpPost]
        public ActionResult UserEdit(UserDetail model, FormCollection frm)
        {
            var data = context.UserDetails.Where(x => x.UserId == model.UserId).FirstOrDefault();
            if (data != null)
            {

                data.UserName = model.UserName;
                data.Userpassword = model.Userpassword;
                data.Email = model.Email;
                data.IsActive = model.IsActive;
                model.UserId = Convert.ToInt32(frm["UserId"]);
                model.UpdatedBy = 0;
                data.UpdatedDate = DateTime.Now;
                context.SaveChanges();
                ViewBag.Message = "Data Insert Successfully";
                //return View(data);
            }
            return RedirectToAction("UserIndex");
        }

        [HttpGet]
        public ActionResult UserDelete(int id)
        {
            var data = context.UserDetails.Where(x => x.UserId == id).FirstOrDefault();
            context.UserDetails.Remove(data);
            context.SaveChanges();
            return RedirectToAction("UserIndex");
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]

        // method for login page----
        public ActionResult Login(UserDetail model)
        {
            //condition for the lowecase letter--

            string lowercaseUsername;
            var data = context.UserDetails.ToList();
            var getDatas = data.Where(x => x.UserName == model.UserName && x.Userpassword == model.Userpassword).FirstOrDefault();
           
            lowercaseUsername = model.UserName.ToLower();

            if (getDatas != null)

            {
                ViewBag.Message = "Password is correct";
                sendEmail(getDatas.Email, "Login in Successfully", "We are pleasure to inform you that your login in digifrontal site ");


                return RedirectToAction("HomeIndex", "Home");

            }
            else
            {
                ViewBag.Mess = "Invalid User Name or password";
                return View();
            }
        }

        //mail sending----
        public string sendEmail(string mail, string subject = "" , string body = "")
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("digifrontalsolutions@gmail.com");
                message.To.Add(new MailAddress(mail));
                message.Subject = subject;
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = body;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host  
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential("digifrontalsolutions@gmail.com", "huqv ifcs xptk khkj ");
                smtp.Credentials = NetworkCred;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("digifrontalsolutions@gmail.com", " huqv ifcs xptk khkj");
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                return "success";
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
   
}


