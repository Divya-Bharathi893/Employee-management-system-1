using DGHCM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DGHCM.Controllers
{
    public class AllowanceMasterController : Controller
    {
        HumanCapitalManagementEntities context = new HumanCapitalManagementEntities();
        CommonClass common = new CommonClass();

        // GET: AllowanceMaster
        public ActionResult AllowanceMasterIndex()
        {
            var ListOfData = context.AllowanceMasters.ToList();
            return View(ListOfData);

        }
        [HttpGet]
        public ActionResult AllowanceMasterCreate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AllowanceMasterCreate(AllowanceMaster model, FormCollection frm)

        {
            var CompanyId = common.CompanyId();
            model.CompanyId = Guid.Parse(CompanyId);
         
            model.AllowanceName = frm["AllowanceName"];
            model.IsActive = frm["isactive"] == "on" ? true : false;
            model.CreatedBy = 0;
            model.CreatedDate = DateTime.Now;
            model.UpdatedBy = 0;
            model.UpdatedDate = DateTime.Now;
            var data = context.AllowanceMasters.Add(model);
            context.SaveChanges();
            return View(data);
        }
        [HttpGet]
        public ActionResult AllowanceMasterEdit(int Id)
        {
            var data = context.AllowanceMasters.Where(x => x.AllowanceMasterId == Id).First();
            return View(data);
        }

        [HttpPost]
        public ActionResult AllowanceMasterEdit(AllowanceMaster model, FormCollection frm)
        {

            var data = context.AllowanceMasters.FirstOrDefault(x => x.AllowanceMasterId == model.AllowanceMasterId);
            if (data != null)
            {

                data.AllowanceName = frm["AllowanceName"];
                data.IsActive = frm["isactive"] == "on" ? true : false;
                context.SaveChanges();
                return RedirectToAction("AllowanceMasterIndex");
            }
            return RedirectToAction("AllowanceMasterIndex");
        }


        public ActionResult AllowanceMasterDelete(int Id)
        {
           var data = context.AllowanceMasters.Where(x => x.AllowanceMasterId == Id).FirstOrDefault();
           context.AllowanceMasters.Remove(data);
           context.SaveChanges();
           return RedirectToAction("AllowanceMasterIndex");
        }

    }
}