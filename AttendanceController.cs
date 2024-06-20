using Antlr.Runtime.Misc;
using DGHCM.Models;
using DGHCM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;


namespace DGHCM.Controllers
{
    public class AttendanceController : Controller
    {

        HumanCapitalManagementEntities context = new HumanCapitalManagementEntities();
        CommonClass common = new CommonClass();
       
          


             public ActionResult AttendanceIndex()

          {        
            
        //               List<DateTime> dates = new List<DateTime>();

        //              DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

        ////                for (DateTime date = startDate; date <= DateTime.Now; date = date.AddDays(1))
        ////                {
        ////                    dates.Add(date);
        ////                }
        ////            context.SaveChanges();
                  return View();
                  }





        //        //    var ListOfData = context.EmployeeAttendanceDetails.ToList();
        //        //    var result = (from a in context.EmployeeAttendanceDetails
        //        //                  join b in context.EmployeeDetails
        //        //                    on a.EmployeeId equals b.EmployeeId
        //        //                  select new AttendanceVM
        //        //                  {
        //        //                      EmployeeName = b.FirstName + " " + b.LastName,
        //        //                      EmployeeId = b.EmployeeId,
        //        //                      AttendanceId = a.AttendanceId,
        //        //                      Day = a.Day,
        //        //                      Month = a.Month,
        //        //                      Year = a.Year,
        //        //                      AttendanceStatus = a.AttendanceStatus,
        //        //                      IsActive = a.IsActive,
        //        //                  }).ToList();

        //        //    return View(result);
        //        //}

        //        [HttpGet]
        //        public ActionResult AttendanceCreate()
        //        {
        //            var listitems = context.EmployeeDetails.ToList();
        //            var names = listitems.Select(e => new { e.EmployeeId, FullName = e.FirstName + " " + e.LastName });
        //            ViewBag.EmployeeList = new SelectList(names, "EmployeeId", "FullName");

        //            //List<Adstatus> Items = new List<Adstatus>()
        //            //{
        //            //    new Adstatus() {Id = "Present", Name="Present" },
        //            //    new Adstatus() {Id = "Absent", Name="Absent" },
        //            //    new Adstatus() {Id = "Permission", Name="Permission" },
        //            //};
        //            //ViewBag.Actionlists = new SelectList(Items, "Id", "Name");
        //            return View();
        //        }

        //        [HttpPost]
        //        public ActionResult AttendanceCreate(EmployeeAttendanceDetail model, FormCollection frm)
        //        {
        //            string strDate = (frm["attendancedate"]);

        //            string[] arrDate = strDate.Split('-');

        //            string Year = arrDate[0].ToString();
        //            string Month = arrDate[1].ToString();
        //            string Day = arrDate[2].ToString();

        //            var CompanyId = common.CompanyId();
        //            model.CompanyId = Guid.Parse(CompanyId);
        //            model.EmployeeId = Convert.ToInt32(frm["EmployeeList"]);
        //            model.Year = Convert.ToInt32(Year);
        //            model.Month = Convert.ToInt32(Month);
        //            model.Day = Convert.ToInt32(Day);
        //            string selectedStatus = frm["status"];
        //            model.AttendanceStatus = selectedStatus;
        //            model.PermissionHour = selectedStatus == "permission" ? frm["perhour"] : "0";
        //            model.IsActive = frm["isactive"] == "on" ? true : false;
        //            model.CreatedBy = 0;
        //            model.CreateDate = DateTime.Now;
        //            model.UpdatedBy = 0;
        //            model.UpdateDate = DateTime.Now;
        //            var data = context.EmployeeAttendanceDetails.Add(model);
        //            context.SaveChanges();
        //            return RedirectToAction("AttendanceIndex");
        //        }


        //        [HttpGet]
        //        public ActionResult AttendanceEdit(int Id)
        //        {
        //            var data = context.EmployeeAttendanceDetails.Where(x => x.AttendanceId == Id).First();
        //            AttendanceVM vm = new AttendanceVM();
        //            vm.EmployeeId = data.EmployeeId;
        //            vm.AttendanceId = data.AttendanceId;
        //            vm.Date = Convert.ToDateTime(data.Year + "/" + data.Month + "/" + data.Day);
        //            vm.AttendanceStatus = data.AttendanceStatus;
        //            vm.IsActive = data.IsActive;
        //            vm.PermissionHour = data.PermissionHour;

        //            var listitems = context.EmployeeDetails.ToList();
        //            var names = listitems.Select(e => new { e.EmployeeId, FullName = e.FirstName + " " + e.LastName });
        //            ViewBag.EmployeeList = new SelectList(names, "EmployeeId", "FullName", vm.EmployeeId);
        //            List<Adstatus> Items = new List<Adstatus>()
        //            {
        //                new Adstatus() {Id = "present", Name="present" },
        //                new Adstatus() {Id = "absent", Name="absent" },
        //                new Adstatus() {Id = "permission", Name="permission" },
        //            };
        //            ViewBag.Actionlists = new SelectList(Items, "Id", "Name", data.AttendanceStatus);
        //            ViewBag.PermissionHour= new SelectList(Items," " ,data.AttendanceStatus);

        //            return View(vm);
        //        }



        //        [HttpPost]
        //        public ActionResult AttendanceEdit(FormCollection frm)
        //        {

        //            string strDate = frm["attendancedate"];
        //            string[] arrDate = strDate.Split('-');
        //            var convertId = Convert.ToInt32(frm["AttendanceId"]);
        //            string Month = arrDate[1];
        //            string Day = arrDate[2];
        //            string Year = arrDate[0];

        //            var data = context.EmployeeAttendanceDetails.Where(x => x.AttendanceId == convertId).FirstOrDefault();
        //            if (data != null)
        //            {

        //                // data.AttendanceStatus = frm["Actionlists"];
        //                data.EmployeeId = Convert.ToInt32(frm["EmployeeList"]);
        //                data.Year = Convert.ToInt32(Year);
        //                data.Month = Convert.ToInt32(Month);
        //                data.Day = Convert.ToInt32(Day);
        //                var selectedStatus = frm["Actionlists"];
        //                data.AttendanceStatus = selectedStatus;
        //                data.PermissionHour = selectedStatus == "permission" ? frm["PermissionHour"] : "0";
        //                data.IsActive = frm["isactive"] == "on" ? true : false;
        //                context.SaveChanges();
        //            }
        //            return RedirectToAction("AttendanceIndex");
        //        }




        //        public ActionResult AttendanceDelete(int Id)
        //            {
        //                var data = context.EmployeeAttendanceDetails.Where(x => x.AttendanceId == Id).FirstOrDefault();
        //                context.EmployeeAttendanceDetails.Remove(data);
        //                context.SaveChanges();
        //                return RedirectToAction("AttendanceIndex");
        //            }

        //        }

        //public class Adstatus
        //    {
        //        public string Id { get; set; }
        //        public string Name { get; set; }

        //        //public string Permission { get; set; }
        //    }

    }
}     

