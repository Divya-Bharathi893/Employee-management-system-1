using DGHCM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;

namespace DGHCM.Controllers
{
    public class PayRollController : Controller
    { 
      
        HumanCapitalManagementEntities context = new HumanCapitalManagementEntities();
        CommonClass common = new CommonClass();

        // GET: PayRoll
        [HttpGet]
        public ActionResult PayRollIndex()
        {
            var listofdata = context.PayrollConfigurations.ToList();
            return View(listofdata);
        }
        public JsonResult PayRollList()
        {
            var listofData = context.PayrollConfigurations.ToList();
            return Json(listofData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddPayRollConfig(PayrollConfiguration objData)
        {
            if (objData != null)
            {
                var CompanyId = common.CompanyId();
                objData.companyId = Guid.Parse(CompanyId);
                objData.CreatedDate = DateTime.Now;
                objData.UpdatedBy = 6;
                objData.UpdatedDate = DateTime.Now;
                // Check if the CustomerName already exists in the database
                var existingpayRollDate = context.PayrollConfigurations.FirstOrDefault(s => s.PayRollDate != null);
                if (existingpayRollDate != null)
                {
                    return Json(new { success = false, message = "PayRoll Date already exists" }, JsonRequestBehavior.AllowGet);
                }
                // Save the data to the database
                context.PayrollConfigurations.Add(objData);
                context.SaveChanges();
                return Json(new { success = true, message = "Data Saved Successfully" }, JsonRequestBehavior.AllowGet);

            }
            return Json("validation failed");
        }
        //[HttpPost]
        //public JsonResult AddPayRollConfig(PayRollConfigModel model)
        //{
        //    var message = string.Empty;
        //    var isExisting = CheckIfPayrollConfigExists(model); // Implement this function as per your logic

        //    if (isExisting)
        //    {
        //        message = "Payroll configurations already exist!";
        //        return Json(new { success = false, message = message });
        //    }

        //    // Save payroll config logic here

        //    message = "Payroll configuration saved successfully!";
        //    return Json(new { success = true, message = message });
        //}

        [HttpGet]
        public JsonResult EditPayRollConfig(int id)
        {
            var payroll = context.PayrollConfigurations.FirstOrDefault(x => x.PayRollConfigId == id);
            return Json(payroll, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdatePayRollConfig(PayrollConfiguration frm)
        {
            var cnvrtedId = frm.PayRollConfigId;
            var data = context.PayrollConfigurations.Where(x => x.PayRollConfigId == cnvrtedId).FirstOrDefault();
            if (data != null)
            {
                data.PayRollConfigId = frm.PayRollConfigId;
                data.IsActive = frm.IsActive;
                data.PayRollDate = frm.PayRollDate;
                data.UpdatedBy = 6;
                data.UpdatedDate = DateTime.Now;
                // Save the data to the database
                context.PayrollConfigurations.AddOrUpdate(data);
                context.SaveChanges();
                return Json(new { success = true, message = "Data Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json("Failed");
        }
        public JsonResult PayRollConfigDelete(string PayRollConfigId)
        {
            var a = Convert.ToInt32(PayRollConfigId);
            var data = context.PayrollConfigurations.Where(x => x.PayRollConfigId == a).FirstOrDefault();
            context.PayrollConfigurations.Remove(data);
            context.SaveChanges();
            ViewBag.Messsage = "Record Delete Successfully";
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        //***********************************************************************************************************************************
        // create method--

        [HttpGet]
        public ActionResult PayRollCreate(string data )
        {
            if(data != null)
            {
                ViewBag.Message = data;
            }
            return View();
        }
        [HttpPost]
        public ActionResult PayRollCreate(PayrollConfiguration model, FormCollection frm, String button)
        {
            if (button == "save")
            {
                var data = context.PayrollConfigurations.Where(x => x.IsActive == true).Count();


                if (data != 0)
                {

                    var Message = "Payroll configurations already exist";
                    return RedirectToAction("PayRollCreate", new {data = Message});

                }
                else
                {
                    var companyId = common.CompanyId();
                    model.companyId = Guid.Parse(companyId);

                    model.PayRollDate = Convert.ToInt32(frm["date"]);
                    model.IsActive = frm["isactive"] == "on" ? true : false;
                    model.CreatedBy = 0;
                    model.CreatedDate = DateTime.Now;
                    model.UpdatedBy = 0;
                    model.UpdatedDate = DateTime.Now;
                    context.PayrollConfigurations.Add(model);
                    context.SaveChanges();
                }
                //return View();
            }
            return RedirectToAction("PayRollIndex");
        }


        //edit method----

        [HttpGet]
        public ActionResult PayRollEdit(int Id)
        {
            var data = context.PayrollConfigurations.Where(x => x.PayRollConfigId == Id).FirstOrDefault();
            return View(data);
        }
        [HttpPost]
        public ActionResult PayRollEdit(PayrollConfiguration model, FormCollection frm)
        {
            var data = context.PayrollConfigurations.Where(x => x.PayRollConfigId == model.PayRollConfigId).FirstOrDefault();

            if (data != null)

            {
                // its for isactive true or flase condition--

                var datas = context.PayrollConfigurations.Where(x => x.IsActive == true).Count();

                if (datas != 0)
                {
                    Console.WriteLine("Error: Payroll configurations already exist");
                }
                else
                {
                    data.PayRollDate = Convert.ToInt32(frm["PayRollDate"]);
                    data.IsActive = frm["isactive"] == "on" ? true : false;
                    data.UpdatedBy = 0;
                    data.UpdatedDate = DateTime.Now;

                    context.SaveChanges();
                }
            }
            return RedirectToAction("PayRollIndex");
        }

        //delete method----
        public ActionResult payrolldelete(int Id)
        {
            var data = context.PayrollConfigurations.Where(x => x.PayRollConfigId == Id).FirstOrDefault();
            context.PayrollConfigurations.Remove(data);
            context.SaveChanges();
            return RedirectToAction("PayRollIndex");



        }
    }
}