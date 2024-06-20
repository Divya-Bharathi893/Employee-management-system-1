using Antlr.Runtime.Misc;
using DGHCM.Models;
using DGHCM.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace DGHCM.Controllers
{
    public class Actions
    {
        public string Id { get; set; }
        public string Name { get; set; }

    }
    public class AllowanceController : Controller

    {
        
        HumanCapitalManagementEntities context = new HumanCapitalManagementEntities();
        CommonClass common = new CommonClass();

        // GET: Allowance
        public ActionResult AllowanceIndex()
        {
            var detail = context.EmployeeAllowanceDetails.ToList();
            var result = (from x in context.EmployeeAllowanceDetails
                          join y in context.EmployeeDetails on x.EmployeeId equals y.EmployeeId
                         // join z in context.AllowanceMasters on x.AllowanceId equals z.AllowanceMasterId
                          select new AllowanceVM
                          {
                              EmployeeId = y.EmployeeId,                      
                              EmployeeName = y.FirstName + "" + y.LastName,
                            //  AllowanceMasterId = z.AllowanceMasterId,
                              AllowanceId = x.AllowanceId,
                              ddlDeduction = x.AllowanceOrDeduction,
                              //AllowanceName = z.AllowanceName,
                              DateOfAllowance = x.DateOfAllowance,
                              Amount = x.Amount,
                              Comments = x.Comments,
                              IsActive = x.IsActive,
                          }).ToList();
            return View(result);
        }
        [HttpGet]
        public ActionResult AllowanceCreate()
        {
            var listitems = context.EmployeeDetails.ToList();
            var names = listitems.Select(x => x.EmployeeId);
            ViewBag.EmployeeList = new SelectList(names, "EmployeeId");
            var a = listitems.Select(e => new { e.EmployeeId, FullName = e.FirstName + " " + e.LastName });
            ViewBag.Employee = new SelectList(a, "EmployeeId", "FullName");

            var allowancename = context.AllowanceMasters.ToList();
            var allowance = allowancename.Select(x => x.AllowanceMasterId);
            ViewBag.Allowance = new SelectList(allowance, "AllowanceMasterId");
            var b = allowancename.Select(e => new { e.AllowanceMasterId, e.AllowanceName });
            ViewBag.AllowanceList = new SelectList(b, "AllowanceMasterId", "AllowanceName");

            List<Actions> deduction = new List<Actions>()
            {
                new Actions() {Id = "A", Name="A" },
                new Actions() {Id = "D", Name="D" },
            };
            ViewBag.ddlDeduction = new SelectList(deduction, "Id", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult AllowanceCreate( FormCollection frm)
        {
            EmployeeAllowanceDetail model = new EmployeeAllowanceDetail();
            var CompanyId = common.CompanyId();
            model.CompanyId = Guid.Parse(CompanyId);           
            model.EmployeeId = Convert.ToInt32(frm["Employee"]);
            model.AllowanceOrDeduction = frm["ddlDeduction"];
            model.AllowanceName = frm["AllowanceList"];
            model.DateOfAllowance = Convert.ToDateTime(frm["date"]);
            model.Amount = Convert.ToInt32(frm["amount"]);
            model.Comments = frm["Comments"];
            model.IsActive = frm["isactive"] == "on" ? true : false;
            model.CreatedBy = 0;
            model.CreateDate = DateTime.Now;
            model.UpdatedBy = 0;
            model.UpdateDate = DateTime.Now;
            var data = context.EmployeeAllowanceDetails.Add(model);
            context.SaveChanges();
            return RedirectToAction("AllowanceIndex");
        }
        
        [HttpGet]
        public ActionResult AllowanceEdit(int Id)
        {
            var data = context.EmployeeAllowanceDetails.Where(x => x.AllowanceId == Id).FirstOrDefault();
        
            var listitems = context.EmployeeDetails.ToList();
            var names = listitems.Select(e => new { e.EmployeeId, FullName = e.FirstName + " " + e.LastName });
            ViewBag.Employee = new SelectList(names, "EmployeeId", "FullName",data.EmployeeId);

            var allowancename = context.AllowanceMasters.ToList();
            var allowance = allowancename.Select(x => x.AllowanceMasterId);
            ViewBag.Allowance = new SelectList(allowance, "AllowanceMasterId");
            var b = allowancename.Select(e => new { e.AllowanceMasterId, e.AllowanceName });
            ViewBag.AllowanceList = new SelectList(allowancename, "AllowanceMasterId", "AllowanceName", data.AllowanceName);

            List<Actions> deduction = new List<Actions>()
            {
                new Actions() {Id = "A", Name="A" },
                new Actions() {Id = "D", Name="D" },
            };
            ViewBag.ddlDeduction = new SelectList(deduction, "Id", "Name",data.AllowanceOrDeduction);
           return View(data);
           
        }

        [HttpPost]
        public ActionResult AllowanceEdit( FormCollection frm)
        {
            var convertId = Convert.ToInt32(frm["AllowanceId"]);
            EmployeeAllowanceDetail model = new EmployeeAllowanceDetail();           
            var data = context.EmployeeAllowanceDetails.Where(x => x.AllowanceId == convertId).FirstOrDefault();
            if (data != null)
            {

                data.EmployeeId = Convert.ToInt32(frm["Employee"]);
                data.AllowanceOrDeduction = frm["ddlDeduction"];
                data.AllowanceName = Convert.ToString(frm["AllowanceList"]);
                data.DateOfAllowance = Convert.ToDateTime(frm["date"]);
                data.Amount = Convert.ToDecimal(frm["amount"]);
                data.Comments = frm["Comments"];
                data.IsActive = frm["isactive"] == "on" ? true : false;
                context.SaveChanges();
                return RedirectToAction("AllowanceIndex");
            }
            return RedirectToAction("AllowanceIndex");
        }
        
        [HttpPost]
       
        public JsonResult AllowanceDelete(int Id)
        {
            var a = Convert.ToInt32(Id);
            var data = context.EmployeeAllowanceDetails.Where(x => x.AllowanceId == Id).FirstOrDefault();
            context.EmployeeAllowanceDetails.Remove(data);
            context.SaveChanges();
            ViewBag.Messsage = "Record Delete Successfully";
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }

}
