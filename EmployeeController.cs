using DGHCM.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.ComponentModel;
using Antlr.Runtime.Misc;
using static System.Collections.Specialized.BitVector32;
using DGHCM.ViewModel;
using System.IO;
using Microsoft.Ajax.Utilities;
using System.Web.UI.WebControls;
using System.Data.Entity.Validation;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Services;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Migrations;
using System.Web.Services.Description;
using System.Runtime.Remoting.Contexts;
using System.Drawing.Printing;

namespace DGHCM.Controllers
{

    public class EmployeeController : Controller
    {

        HumanCapitalManagementEntities context = new HumanCapitalManagementEntities();
        CommonClass common = new CommonClass();

        public ActionResult EmployeeDetailsIndex()
        {

            var ListOfData = context.EmployeeDetails.ToList();
            return View(ListOfData);

        }
        [HttpGet]
        public ActionResult EmployeeDetailsCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EmployeeDetailsCreate(EmployeeDetail model, FormCollection frm)

        {
            var CompanyId = common.CompanyId();
            model.CompanyId = Guid.Parse(CompanyId);

            model.FirstName = frm["firstName"];
            model.LastName = frm["lastName"];
            model.Designation = frm["designation"];

            model.Gender = frm["gender"] == "Male" ? 1 : 2;
            model.AccountNo = frm["accountNo"];
            model.Nationality = frm["nationality"];
            model.DateOfBirth = Convert.ToDateTime(frm["dateOfBirth"]);
            model.DateOfJoining = Convert.ToDateTime(frm["dateOfJoining"]);
            model.EmailId = frm["emailId"];
            model.EmployeeAddress = frm["employeeAddress"];
            //model.MaritalStatus =frm["maritalStatus"];
            model.MaritalStatus = Convert.ToString(frm["maritalStatus"] == "Married" ? "Married" : "Unmarried");
            model.IsActive = frm["isactive"] == "on" ? true : false;
            model.CreatedBy = 0;
            model.CreateDate = DateTime.Now;
            model.UpdatedBy = 0;
            model.UpdateDate = DateTime.Now;
            model.PhoneNo = frm["phoneNo"];
            model.EmployeeShift = frm["Shift"];
            var data = context.EmployeeDetails.Add(model);
            context.SaveChanges();
            return View(data);
        }



        //Edit
        [HttpGet]
        public ActionResult EmployeeDetailsEdit(int Id)
        {
            var data = context.EmployeeDetails.Where(x => x.EmployeeId == Id).First();
            return View(data);
        }

        [HttpPost]
        public ActionResult EmployeeDetailsEdit(EmployeeDetail model, FormCollection frm)
        {

            var data = context.EmployeeDetails.FirstOrDefault(x => x.EmployeeId == model.EmployeeId);
            if (data != null)
            {
             
                data.FirstName = frm["firstName"];
                data.LastName = frm["lastName"];
                data.Designation = frm["designation"];
                data.Gender = frm["gender"] == "Male" ? 1 : 2;
                data.AccountNo = frm["accountNo"];
                data.Nationality = frm["nationality"];
                data.DateOfBirth = Convert.ToDateTime(frm["dateOfBirth"]);
                data.DateOfJoining = Convert.ToDateTime(frm["dateOfJoining"]);
                data.EmailId = frm["emailId"];
                data.EmployeeAddress = frm["employeeAddress"];
                data.MaritalStatus = Convert.ToString(frm["maritalStatus"] == "Married" ? "Married" : "Unmarried");
                data.IsActive = frm["isactive"] == "on" ? true : false;
                data.PhoneNo = frm["phoneNo"];
                data.EmployeeShift = Convert.ToString(frm["Shift"]== "RegularShift" ? "RegularShift" : "NightShift");
                context.SaveChanges();
                
                var CompanyId = common.CompanyId();
                model.CompanyId = Guid.Parse(CompanyId);

                data.CompanyId = model.CompanyId;
                data.FirstName = model.FirstName;
                data.LastName = model.LastName;
                data.Designation = model.Designation;
                data.Gender = model.Gender;
                data.AccountNo = model.AccountNo;
                data.Nationality = model.Nationality;
                data.DateOfBirth = model.DateOfBirth;
                data.DateOfJoining = model.DateOfJoining;
                data.EmailId = model.EmailId;
                data.EmployeeAddress = model.EmployeeAddress;
                data.MaritalStatus = model.MaritalStatus;
                data.IsActive = model.IsActive;
                data.PhoneNo = model.PhoneNo;
                data.EmployeeShift = model.EmployeeShift;
                context.SaveChanges();

            }
            return RedirectToAction("EmployeeDetailsIndex");
        }


        //Details
        //public ActionResult EmployeeDetailsDetail(int Id)
        //{
        //    var data = context.EmployeeDetails.Where(x => x.EmployeeId == Id).FirstOrDefault();
        //    return View(data);
        //}

        [HttpPost]

        public JsonResult EmployeeDetailsDelete(int Id)
        {
            var a = Convert.ToInt32(Id);
            var data = context.EmployeeDetails.Where(x => x.EmployeeId == Id).FirstOrDefault();
            context.EmployeeDetails.Remove(data);
            context.SaveChanges();
            ViewBag.Messsage = "Record Delete Successfully";
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        //***********************************************************************************************************************************
        public ActionResult EmpSalaryIndex( string searching)
        {
            var listofData = context.Emp_SalaryDetails.ToList();

            var innerJoin = (from ES in context.Emp_SalaryDetails
                             join ST in context.SalaryTypeMasters on ES.SalaryTypeId equals ST.SalaryTypeId
                             join ED in context.EmployeeDetails on ES.EmployeeId equals ED.EmployeeId
                             select new SalaryVM
                             {
                                 EmployeeId = ED.EmployeeId,
                                 SalaryId = ES.SalaryId,
                                 EmployeeName = ED.FirstName + "" + ED.LastName,
                                 SalaryTypeId = ST.SalaryTypeId,
                                 SalaryTypeName = ST.SalaryTypeName,
                                 Amount = ES.Amount,
                                 IsActive = ES.IsActive,
                             }).ToList();
            var listitems = context.EmployeeDetails.ToList();
            var names = listitems.Select(e => new { e.EmployeeId, FullName = e.FirstName + " " + e.LastName });
            ViewBag.EmployeeList = new SelectList(names, "EmployeeId", "FullName");
            var EmpList = context.SalaryTypeMasters.ToList();
            ViewBag.SalaryType = new SelectList(EmpList, "SalaryTypeId", "SalaryTypeName");
            return View(innerJoin);
        }

        [HttpGet]
        public JsonResult SalaryList()
        {
            List<SalaryVM> innerJoin = (from ES in context.Emp_SalaryDetails
                                        join ST in context.SalaryTypeMasters on ES.SalaryTypeId equals ST.SalaryTypeId
                                        join ED in context.EmployeeDetails on ES.EmployeeId equals ED.EmployeeId
                                        select new SalaryVM
                                        {
                                            EmployeeId = ED.EmployeeId,
                                            SalaryId = ES.SalaryId,
                                            EmployeeName = ED.FirstName + "" + ED.LastName,
                                            SalaryTypeId = ST.SalaryTypeId,
                                            SalaryTypeName = ST.SalaryTypeName,
                                            Amount = ES.Amount,
                                            IsActive = ES.IsActive,
                                        }).ToList();
            return Json(innerJoin, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddSalary(Emp_SalaryDetails objData)
        {
            if (objData != null)
            {
                var CompanyId = common.CompanyId(); 
                if (!string.IsNullOrEmpty(CompanyId))
                {
                    objData.CompanyId = Guid.Parse(CompanyId);
                    objData.CreateDate = DateTime.Now;
                    objData.UpdateBy = 6; 
                    objData.UpdateDate = DateTime.Now;
                    context.Emp_SalaryDetails.Add(objData);
                    context.SaveChanges();

                    return Json(new { success = true, message = "Data Saved Successfully" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false, message = "Validation failed" });
        }

        [HttpGet]
        public JsonResult EditSalary(int id)
        {
            var salary = context.Emp_SalaryDetails.FirstOrDefault(x => x.SalaryId == id);
            return Json(salary, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateSalary(Emp_SalaryDetails frm)
        {
            var cnvrtedId = frm.SalaryId;
            var data = context.Emp_SalaryDetails.Where(x => x.SalaryId == cnvrtedId).FirstOrDefault();
            if (data != null)
            {

                data.EmployeeId = frm.EmployeeId;
                data.SalaryTypeId = frm.SalaryTypeId;
                data.IsActive = frm.IsActive;
                data.Amount = frm.Amount;
                data.UpdateBy = 6;
                data.UpdateDate = DateTime.Now;
                context.Emp_SalaryDetails.AddOrUpdate(data);
                context.SaveChanges();
                return Json(new { success = true, message = "Data Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Validation failed" });
        }
        //SalaryDelete
        public JsonResult EmpSalaryDelete(int SalaryId)
        {
            var a = Convert.ToInt32(SalaryId);
            var data = context.Emp_SalaryDetails.Where(x => x.SalaryId == a).FirstOrDefault();
            context.Emp_SalaryDetails.Remove(data);
            context.SaveChanges();
            ViewBag.Messsage = "Record Delete Successfully";
            return Json(new { success = true, message = "Data Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }
        //***********************************************************************************************************************************

        //SalaryCreate
        [HttpGet]
        public ActionResult EmpSalaryCreate()
        {
            var listitems = context.EmployeeDetails.ToList();
            var names = listitems.Select(e => new { e.EmployeeId, FullName = e.FirstName + " " + e.LastName });
            ViewBag.EmployeeList = new SelectList(names, "EmployeeId", "FullName");
            var EmpList = context.SalaryTypeMasters.ToList();
            ViewBag.SalaryType = new SelectList(EmpList, "SalaryTypeId", "SalaryTypeName");
            return View();
        }
        //SalaryCreate
        [HttpPost]
        public ActionResult EmpSalaryCreate(FormCollection frm)
        {
            Emp_SalaryDetails model = new Emp_SalaryDetails();
            var CompanyId = common.CompanyId();
            model.CompanyId = Guid.Parse(CompanyId);
            // model.SalaryId = Convert.ToInt32(frm["SalaryId"]);
            model.SalaryTypeId = Convert.ToInt32(frm["SalaryType"]);
            model.Amount = Convert.ToInt32(frm["Amount"]);
            model.EmployeeId = Convert.ToInt32(frm["EmployeeList"]);
            model.CreatedBy = 4;
            model.CreateDate = DateTime.Now;
            model.UpdateBy = 6;
            model.UpdateDate = DateTime.Now;
            model.IsActive = frm["IsActive"] == "on" ? true : false;
            var data = context.Emp_SalaryDetails.Add(model);
            ViewBag.Message = "Data Insert Successfully";
            context.SaveChanges();
            return RedirectToAction("EmpSalaryIndex");
        }


        //SalaryUpdate
        [HttpGet]
        public ActionResult EmpSalaryUpdate(int Id)
        {
            var datas = context.Emp_SalaryDetails.Where(x => x.SalaryId == Id).FirstOrDefault();
            var listitems = context.EmployeeDetails.ToList();
            var names = listitems.Select(e => new { e.EmployeeId, FullName = e.FirstName + " " + e.LastName });
            ViewBag.EmployeeList = new SelectList(names, "EmployeeId", "FullName", datas.EmployeeId);
            var EmpList = context.SalaryTypeMasters.ToList();
            ViewBag.SalaryType = new SelectList(EmpList, "SalaryTypeId", "SalaryTypeName", datas.SalaryTypeId);
            return View(datas);
        }

        //SalaryUpdate
        [HttpPost]
        public ActionResult EmpSalaryUpdate(FormCollection frm)
        {
            var cnvrtedId = Convert.ToInt32(frm["SalaryId"]);
            Emp_SalaryDetails Model = new Emp_SalaryDetails();
            var data = context.Emp_SalaryDetails.Where(x => x.SalaryId == cnvrtedId).FirstOrDefault();
            if (data != null)
            {

                data.EmployeeId = Convert.ToInt32(frm["EmployeeList"]);
                data.SalaryId = Convert.ToInt32(frm["SalaryId"]);
                data.Amount = Convert.ToDecimal(frm["Amount"]);
                data.IsActive = frm["IsActive"] == "on" ? true : false;
                data.CreatedBy = 4;
                data.CreateDate = DateTime.Now;
                data.UpdateBy = 8;
                data.UpdateDate = DateTime.Now;
                context.SaveChanges();
                return RedirectToAction("EmpSalaryIndex");
            }
            return RedirectToAction("EmpSalaryIndex");
        }
        //***********************************************************************************************************************************


        //SalaryTypeIndex
        public ActionResult EmpSalaryTypeIndex()
        {
            var listofData = context.SalaryTypeMasters.ToList();
            return View(listofData);

        }
        [HttpGet]
        public JsonResult SalaryTypeList()
        {
            var listofData = context.SalaryTypeMasters.ToList();
            return Json(listofData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddSalaryType(SalaryTypeMaster objData)
        {
            if (objData != null)
            {
                var CompanyId = common.CompanyId();
                objData.CompanyId = Guid.Parse(CompanyId);
                objData.CreateDate = DateTime.Now;
                objData.UpdateBy = 6;
                objData.UpdateDate = DateTime.Now;

                // Check if the SalaryTypeName already exists in the database
                var existingSalaryType = context.SalaryTypeMasters.FirstOrDefault(s => s.SalaryTypeName == objData.SalaryTypeName);
                if (existingSalaryType != null)
                {
                    return Json(new { success = false, message = "Salary type name already exists" }, JsonRequestBehavior.AllowGet);
                }

                // Save the data to the database
                context.SalaryTypeMasters.Add(objData);
                context.SaveChanges();
                return Json(new { success = true, message = "Data Saved Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Validation failed" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult EditSalaryType(int id)
        {
            var salarytype = context.SalaryTypeMasters.FirstOrDefault(x => x.SalaryTypeId == id);
            return Json(salarytype, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateSalaryType(SalaryTypeMaster frm)
        {
            var cnvrtedId = frm.SalaryTypeId;
            var data = context.SalaryTypeMasters.Where(x => x.SalaryTypeId == cnvrtedId).FirstOrDefault();
            if (data != null)
            {
                data.SalaryTypeId = frm.SalaryTypeId;
                data.IsActive = frm.IsActive;
                data.AllowanceAndDeduction = frm.AllowanceAndDeduction;
                data.IsFixedPay = frm.IsFixedPay;
                data.SalaryTypeName = frm.SalaryTypeName;
                data.UpdateBy = 6;
                data.UpdateDate = DateTime.Now;
                var existingSalaryType = context.SalaryTypeMasters.FirstOrDefault(s => s.SalaryTypeName == data.SalaryTypeName);
                if (existingSalaryType != null)
                {
                    return Json(new { success = false, message = "Salary type name already exists" }, JsonRequestBehavior.AllowGet);
                }


                // Save the data to the database
                context.SalaryTypeMasters.AddOrUpdate(data);
                context.SaveChanges();
                return Json(new { success = true, message = "Data Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, message = "Validation failed" }, JsonRequestBehavior.AllowGet);
        }
        //SalaryTypeDelete
        /*[HttpPost]
        public JsonResult SalaryTypeDelete(string SalaryTypeId)
        {

            var a = Convert.ToInt32(SalaryTypeId);
            var data = context.SalaryTypeMasters.Where(x => x.SalaryTypeId == a).FirstOrDefault();
            context.SalaryTypeMasters.Remove(data);
            context.SaveChanges();
            ViewBag.Messsage = "Record Delete Successfully";
            return Json(true, JsonRequestBehavior.AllowGet);
        }*/
        [HttpPost]
        public JsonResult DeleteUnusedSalaryTypes(int SalaryTypeId)
        {
            var a = Convert.ToInt32(SalaryTypeId);
            var data = context.Emp_SalaryDetails.Where(x => x.SalaryTypeId == a).ToList();
            if (data.Count > 0)
            {
                return Json(new {success=false}, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data1 = context.SalaryTypeMasters.Where(x => x.SalaryTypeId == a).FirstOrDefault();
                context.SalaryTypeMasters.Remove(data1);
                context.SaveChanges();
                return Json(new { success = true, message = "Data Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }


        /*try
        {
            // Get all distinct SalaryTypeIds from Emp_SalaryDetails
            var usedSalaryTypeIds = context.Emp_SalaryDetails.Select(es => es.SalaryTypeId).Distinct().ToList();

            // Get all SalaryTypeMasters that are not in usedSalaryTypeIds
            var unusedSalaryTypes = context.SalaryTypeMasters.Where(st => !usedSalaryTypeIds.Contains(st.SalaryTypeId)).ToList();

            // Delete unused salary types
            context.SalaryTypeMasters.RemoveRange(unusedSalaryTypes);
            context.SaveChanges();

            return Json(new { success = true, message = "Unused salary types deleted successfully" });
        }
        catch (Exception ex)
        {
            return Json(new { error = "An error occurred while deleting unused salary types: " + ex.Message });
        }*/



        //***********************************************************************************************************************************
        //SalaryTypeCreate
        [HttpGet]
        public ActionResult EmpSalaryTypeCreate()
        {
            return View();

        }
        //SalaryTypeCreate
        [HttpPost]
        public ActionResult EmpSalaryTypeCreate(FormCollection frm)
        {

            SalaryTypeMaster model = new SalaryTypeMaster();
            var CompanyId = common.CompanyId();
            model.CompanyId = Guid.Parse(CompanyId);
            model.SalaryTypeName = frm["SalaryTypeName"];
            model.AllowanceAndDeduction = frm["AllowanceOrDeduction"] == "on" ? true : false;
            model.IsFixedPay = frm["IsFixedPay"] == "on" ? true : false;
            model.CreatedBy = 5;
            model.CreateDate = DateTime.Now;
            model.UpdateBy = 8;
            model.UpdateDate = DateTime.Now;
            model.IsActive = frm["IsActive"] == "on" ? true : false;
            var data = context.SalaryTypeMasters.Add(model);
            ViewBag.Message = "Data Insert Successfully";
            context.SaveChanges();
            return RedirectToAction("EmpSalaryTypeIndex");
        }



        //SalaryTypeUpdate
        [HttpGet]
        public ActionResult EmpSalaryTypeUpdate(int id)
        {
            var data = context.SalaryTypeMasters.Where(x => x.SalaryTypeId == id).FirstOrDefault();
            return View(data);
        }

        //SalaryTypeUpdate
        [HttpPost]
        public ActionResult EmpSalaryTypeUpdate(FormCollection frm)
        {
            var convertId = Convert.ToInt32(frm["SalaryTypeId"]);
            SalaryTypeMaster Model = new SalaryTypeMaster();
            var data = context.SalaryTypeMasters.Where(x => x.SalaryTypeId == convertId).FirstOrDefault();
            if (data != null)
            {
                data.SalaryTypeName = frm["SalaryTypeName"];
                data.SalaryTypeId = Convert.ToInt32(frm["SalaryTypeId"]);
                data.AllowanceAndDeduction = frm["AllowanceOrDeduction"] == "on" ? true : false;
                data.IsFixedPay = frm["IsFixedPay"] == "on" ? true : false;
                data.IsActive = frm["IsActive"] == "on" ? true : false;
                data.UpdateBy = 8;
                data.UpdateDate = DateTime.Now;
                context.SaveChanges();
                return RedirectToAction("EmpSalaryTypeIndex");
            }

            return RedirectToAction("EmpSalaryTypeIndex");
        }

        
        //***********************************************************************************************************************************
        //DocumentTypeIndex
        public ActionResult DocumentTypeIndex()
        {
            var listofdata = context.DocumentTypeMasters.ToList();
            return View(listofdata);
        }
        public JsonResult DocumentTypeList()
        {
            var listofData = context.DocumentTypeMasters.ToList();
            return Json(listofData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddDocumentType(DocumentTypeMaster objData)
        {
            if (objData != null)
            {
                var CompanyId = common.CompanyId();
                objData.CompanyId = Guid.Parse(CompanyId);
                objData.CreateDate = DateTime.Now;
                objData.UpdateBy = 6;
                objData.UpdateDate = DateTime.Now;
                // Check if the DocumentTypeName already exists in the database
                var existingSalaryType = context.DocumentTypeMasters.FirstOrDefault(s => s.DocumentName == objData.DocumentName);
                if (existingSalaryType != null)
                {
                    return Json(new { success = false, message = "Document type name already exists" }, JsonRequestBehavior.AllowGet);
                }
                // Save the data to the database
                context.DocumentTypeMasters.Add(objData);
                context.SaveChanges();
                return Json(new { success = true, message = "Data Saved Successfully" }, JsonRequestBehavior.AllowGet);

            }
            return Json("validation failed");
        }
        [HttpGet]
        public JsonResult EditDocumentType(int id)
        {
            var documenttype = context.DocumentTypeMasters.FirstOrDefault(x => x.DocumentTypeId == id);
            return Json(documenttype, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateDocumentType(DocumentTypeMaster frm)
        {
            var cnvrtedId = frm.DocumentTypeId;
            var data = context.DocumentTypeMasters.Where(x => x.DocumentTypeId == cnvrtedId).FirstOrDefault();
            if (data != null)
            {
                data.DocumentTypeId = frm.DocumentTypeId;
                data.IsActive = frm.IsActive;
                data.DocumentName = frm.DocumentName;
                data.UpdateBy = 6;
                data.UpdateDate = DateTime.Now;
                // Check if the DocumentTypeName already exists in the database
                var existingSalaryType = context.DocumentTypeMasters.FirstOrDefault(s => s.DocumentName == data.DocumentName);
                if (existingSalaryType != null)
                {
                    return Json(new { success = false, message = "Document type name already exists" }, JsonRequestBehavior.AllowGet);
                }

                // Save the data to the database
                context.DocumentTypeMasters.AddOrUpdate(data);
                context.SaveChanges();
                return Json(new { success = true, message = "Data Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json("Failed");
        }
        //DocumentTypeDelete
        [HttpPost]
        public JsonResult DeleteUnusedDocumentTypes(int DocumentTypeId)
        {
            var a = Convert.ToInt32(DocumentTypeId);
            var data = context.Emp_DocumentDetails.Where(x => x.DocumentTypeId == a).ToList();
            if (data.Count > 0)
            {
                return Json(new { error = "Document type is in use and cannot be deleted" });
            }
            else
            {
                var data1 = context.DocumentTypeMasters.Where(x => x.DocumentTypeId == a).FirstOrDefault();
                context.DocumentTypeMasters.Remove(data1);
                context.SaveChanges();
                return Json(new { success = true });
            }
        }
        //***********************************************************************************************************************************
        //DocumentTypeCreate
        [HttpGet]
        public ActionResult DocumentTypeCreate()
        {
            return View();
        }
        //DocumentTypeCreate
        [HttpPost]
        public ActionResult DocumentTypeCreate(FormCollection frm)
        {
            DocumentTypeMaster model = new DocumentTypeMaster();
            var CompanyId = common.CompanyId();
            model.CompanyId = Guid.Parse(CompanyId);
            model.DocumentName = frm["DocumentTypeName"];
            model.DocumentTypeId = Convert.ToInt32(frm["DocumentTypeId"]);
            model.IsActive = frm["IsActive"] == "on" ? true : false;
            model.UpdateBy = 2;
            model.UpdateDate = DateTime.Now;
            model.CreatedBy = 3;
            model.CreateDate = DateTime.Now;
            var datas = context.DocumentTypeMasters.Add(model);
            context.SaveChanges();
            ViewBag.Message = "Data Inserted Sucessfully";
            return RedirectToAction("DocumentTypeIndex");
        }


        //DocumentTypeUpdate
        [HttpGet]
        public ActionResult DocumentTypeUpdate(int id)
        {
            var data = context.DocumentTypeMasters.Where(x => x.DocumentTypeId == id).FirstOrDefault();
            return View(data);
        }
        //DocumentTypeUpdate
        [HttpPost]
        public ActionResult DocumentTypeUpdate(FormCollection frm)
        {

            var convertId = Convert.ToInt32(frm["DocumentTypeId"]);
            DocumentTypeMaster model = new DocumentTypeMaster();
            var data = context.DocumentTypeMasters.Where(x => x.DocumentTypeId == convertId).FirstOrDefault();
            if (data != null)
            {
                data.DocumentName = frm["DocumentTypeName"];
                data.IsActive = frm["IsActive"] == "on" ? true : false;
                data.CreatedBy = 3;
                data.CreateDate = DateTime.Now;
                data.UpdateBy = 2;
                data.UpdateDate = DateTime.Now;
                data.RowVersion = model.RowVersion;
                context.SaveChanges();
                ViewBag.Message = "Data Update Sucessfully ";
                return RedirectToAction("DocumentTypeIndex");
            }
            return RedirectToAction("DocumentTypeIndex");
        }
        //DocumentDetails
        public ActionResult DocumentDetailsIndex()
        {
            var data = context.Emp_DocumentDetails.ToList();
            var details = (from ED in context.Emp_DocumentDetails
                           join DT in context.DocumentTypeMasters on ED.DocumentTypeId equals DT.DocumentTypeId
                           join EMD in context.EmployeeDetails on ED.EmployeeId equals EMD.EmployeeId
                           select new DocumentVM
                           {
                               DocumentName = DT.DocumentName, 
                               DocumentTypeId = DT.DocumentTypeId,
                               DocumentId = ED.DocumentId,
                               DocumentPath = ED.DocumentPath,
                               Description = ED.Description,
                               EmployeeId = ED.EmployeeId,
                               EmployeeName = EMD.FirstName + "" + EMD.LastName,
                               IsActive = ED.IsActive,
                           }).ToList();

            return View(details);
        }
        //DocumentDetailsCreate
        [HttpGet]
        public ActionResult DocumentDetailsCreate()
        {
            var listitems = context.EmployeeDetails.ToList();
            var names = listitems.Select(e => new { e.EmployeeId, FullName = e.FirstName + " " + e.LastName });
            ViewBag.EmployeeList = new SelectList(names, "EmployeeId", "FullName");
            var DocTypeitems = context.DocumentTypeMasters.ToList();
            ViewBag.DocumentType = new SelectList(DocTypeitems, "DocumentTypeId", "DocumentName");
            return View();
        }
        [HttpPost]
        public ActionResult DocumentDetailsCreate(FormCollection frm, HttpPostedFileBase file)
        {
            Emp_DocumentDetails model = new Emp_DocumentDetails();
            var CompanyId = common.CompanyId();
            model.CompanyId = Guid.Parse(CompanyId);
            model.DocumentId = Convert.ToInt32(frm["DocumentId"]);
            model.EmployeeId = Convert.ToInt32(frm["EmployeeList"]);
            model.DocumentTypeId = Convert.ToInt32(frm["DocumentType"]);
            model.Description = frm["Description"];
            model.IsActive = frm["IsActive"] == "on" ? true : false;
            model.CreatedBy = 2;
            model.CreateDate = DateTime.Now;
            model.UpdateBy = 8;
            model.UpdateDate = DateTime.Now;
            if (file != null && file.ContentLength > 0)
            {
                string fileExtension = Path.GetExtension(file.FileName);
                string[] allowedExtensions = { ".pdf", ".doc", ".docx" };
                if (allowedExtensions.Contains(fileExtension))
                {
                    var maxFileSize = 5 * 1024 * 1024; // 5MB
                    if (file.ContentLength > maxFileSize)
                    {
                        ViewBag.Message = "File size exceeds the maximum limit of 5MB.";
                        return View();
                    }
                    // Save the file to the server or perform any other necessary operations
                    string _FileName = Path.GetFileName(file.FileName);
                    string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
                    model.DocumentPath = _path;
                    file.SaveAs(_path);
                    ViewBag.Message = "File uploaded successfully!";
                }
                else
                {
                    ViewBag.Message = "Only pdf, doc and docx files are allowed.";

                }
            }
            var data = context.Emp_DocumentDetails.Add(model);
            context.SaveChanges();
            return RedirectToAction("DocumentDetailsIndex");

        }
       /*public JsonResult getElementById(int DocumentId)
        {
            var data = context.Emp_DocumentDetails.Where(x => x.DocumentId == DocumentId).FirstOrDefault();
            Var DocumentPath = GetDocumentPath();

            return Json();
        }

        private string GetDocumentPath(string DocumentId)
        {
            return "path/to/your/document.pdf";
        }*/

        //DocumentDetailsUpdate
        [HttpGet]
        public ActionResult DocumentDetailsUpdate(int id )
        {
            var data = context.Emp_DocumentDetails.Where(x => x.DocumentId == id).FirstOrDefault();
            var listitems = context.EmployeeDetails.ToList();
            var names = listitems.Select(e => new { e.EmployeeId, FullName = e.FirstName + " " + e.LastName });
            ViewBag.EmployeeList = new SelectList(names, "EmployeeId", "FullName", data.EmployeeId);
            var DocTypeitems = context.DocumentTypeMasters.ToList();
            ViewBag.DocumentType = new SelectList(DocTypeitems, "DocumentTypeId", "DocumentName", data.DocumentTypeId);
            return View(data);
        }

        [HttpPost]
        public ActionResult DocumentDetailsUpdate(FormCollection frm, HttpPostedFileBase file)
        {
            var cnvrtedId = Convert.ToInt32(frm["DocumentId"]);
            Emp_DocumentDetails model = new Emp_DocumentDetails();
            var data = context.Emp_DocumentDetails.Where(x => x.DocumentId == cnvrtedId).FirstOrDefault();

            if (data != null)
            {

                // Update properties with the values from the form
                data.EmployeeId = Convert.ToInt32(frm["EmployeeList"]);
                data.DocumentTypeId = Convert.ToInt32(frm["DocumentType"]);
                data.Description = frm["Description"];
                data.IsActive = frm["IsActive"] == "on" ? true : false;
                data.UpdateBy = 8;
                data.UpdateDate = DateTime.Now;
                if (file != null && file.ContentLength > 0)
                {
                    string fileExtension = Path.GetExtension(file.FileName);
                    string[] allowedExtensions = { ".pdf", ".doc", ".docx" };
                    if (allowedExtensions.Contains(fileExtension))
                    {
                        var maxFileSize = 5 * 1024 * 1024; // 5MB
                        if (file.ContentLength > maxFileSize)
                        {
                            ViewBag.Message = "File size exceeds the maximum limit of 5MB.";
                            return View(data); 
                        }

                        string _FileName = Path.GetFileName(file.FileName);
                        string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
                        data.DocumentPath = _path;
                        file.SaveAs(_path);
                        ViewBag.Message = "File uploaded successfully!";
                    }
                    else
                    {
                        ViewBag.Message = "Only pdf, doc and docx files are allowed.";
                        return View(data); 
                    }
                }
                context.SaveChanges();
                return RedirectToAction("DocumentDetailsIndex");
            }
            else
            {
                ViewBag.Message = "Document not found.";
                return View(model); // Return the view with model data
            }
        }

        [HttpPost]
        //DocumentDetailsDelete
        public JsonResult DocumentDetailsDelete(string DocumentId)
        {
            var a = Convert.ToInt32(DocumentId);
            var data = context.Emp_DocumentDetails.Where(x => x.DocumentId == a).FirstOrDefault();
            context.Emp_DocumentDetails.Remove(data);
            context.SaveChanges();
            ViewBag.Messsage = "Record Delete Successfully";
            return Json(true,JsonRequestBehavior.AllowGet);
        }
        public ActionResult EmployeeEducationIndex()
        {
            var ListOfData = context.EmployeeEducations.ToList();


            var result = (from a in context.EmployeeEducations
                          join b in context.EmployeeDetails
                          on a.EmployeeId equals b.EmployeeId
                          select new EducationVM
                          {
                              EmployeeName = b.FirstName + " " + b.LastName,
                              EmployeeId = a.EmployeeId,
                              EducationId = a.EducationId,
                              CourseName = a.CourseName,
                              IsActive = a.IsActive,
                              Institutions = a.Institutions,
                              Document = a.Document

                          }).ToList();
            return View(ListOfData);
        }

        [HttpGet]
        public ActionResult EmployeeEducationCreate(/*int Id*/)
        {
            /*ViewBag.ID = Id;*/

            var listitems = context.EmployeeDetails.ToList();
            // Concatenate first name and last name to form the display name
            /* var names = listitems.Select(e => new { Id = e.EmployeeId, FullName = e.FirstName + " " + e.LastName });
             ViewBag.EmployeeList = new SelectList(names, "EmployeeId", "FullName"*//*,Id*//*);*/

            var DocTypeId = context.DocumentTypeMasters.ToList();
            var Doc = DocTypeId.Select(d => new { DocumentTypeId = d.DocumentTypeId, DocName = d.DocumentName });
            ViewBag.DocumentList = new SelectList(DocTypeId, "DocumentTypeId", "DocumentName");

            /*var Educationlist = context.EmployeeEducations.ToList();
            var EmployeesEducation = Educationlist.Where(x => x.EmployeeId == Id).ToList(); 
*/
            return View();
        }
        [HttpPost]
        public ActionResult EmployeeEducationCreate(FormCollection frm, EmployeeEducation model, HttpPostedFileBase file)
        {
            var a = frm["percentage"];
            var CompanyId = common.CompanyId();
            model.CompanyId = Guid.Parse(CompanyId);
            model.CourseName = frm["course"];
            model.Institutions = frm["institution"];
            model.Comments = frm["Comments"];
            model.Percentages = Convert.ToDecimal(frm["percentage"]);
            model.Experience = (frm["experience"] == "Fresher") ? 0 : Convert.ToInt32(frm["experienceDetails"]);
            model.IsActive = frm["IsActive"] == "on" ? true : false;
            model.CreatedBy = 0;
            model.CreatedDate = DateTime.Now;
            model.UpdatedBy = 0;
            model.UpdatedDated = DateTime.Now;
            model.EmployeeId = Convert.ToInt32(frm["ID"]);

            //doc class
            Emp_DocumentDetails emp_DocumentDetails = new Emp_DocumentDetails();
            emp_DocumentDetails.EmployeeId = Convert.ToInt32(frm["EmployeeList"]);
            emp_DocumentDetails.DocumentTypeId = Convert.ToInt32(frm["DocumentList"]);
            emp_DocumentDetails.IsActive = true;
            emp_DocumentDetails.UpdateDate = DateTime.Now;
            emp_DocumentDetails.CreateDate = DateTime.Now;
            emp_DocumentDetails.UpdateBy = 0;
            emp_DocumentDetails.CreatedBy = 0;
            emp_DocumentDetails.CompanyId = Guid.Parse(CompanyId);
            //to change
            emp_DocumentDetails.Description = "";

            //file
            if (file != null && file.ContentLength > 0)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string uploadDirectory = Server.MapPath("~/UploadedFiles");
                string filePath = Path.Combine(uploadDirectory, filename);
                file.SaveAs(filePath);
                var DocPath = "~/UploadedFiles/" + filename;

                model.Document = DocPath;
                emp_DocumentDetails.DocumentPath = DocPath;
            }

            var data = context.EmployeeEducations.Add(model);
            var dataforDocument = context.Emp_DocumentDetails.Add(emp_DocumentDetails);

            context.SaveChanges();

            // user can click add more for more education details
            if (frm["action"] == "Addmore")
            {
                return RedirectToAction("EmployeeEducationCreate");
            }
            return RedirectToAction("EmployeeEducationIndex");
        }

        [HttpGet]
        public ActionResult EmployeeEducationEdit(int Id)
        {
            var data = context.EmployeeEducations.Where(x => x.EducationId == Id).FirstOrDefault();

            var DocTypeId = context.DocumentTypeMasters.ToList();
            var Doc = DocTypeId.Select(d => new { DocumentTypeId = d.DocumentTypeId, DocName = d.DocumentName });
            ViewBag.DocumentList = new SelectList(DocTypeId, "DocumentTypeId", "DocumentName");
            return View(data);
        }

        [HttpPost]
        public ActionResult EmployeeEducationEdit(EmployeeEducation model, HttpPostedFileBase file, string action)
        {
            if (ModelState.IsValid)
            {
                var existingEducation = context.EmployeeEducations.FirstOrDefault(x => x.EducationId == model.EducationId);
                if (existingEducation != null)
                {
                    existingEducation.CourseName = model.CourseName;
                    existingEducation.Institutions = model.Institutions;
                    existingEducation.Percentages = model.Percentages;
                    existingEducation.Experience = model.Experience;
                    existingEducation.Comments = model.Comments;
                    existingEducation.IsActive = model.IsActive;


                    if (file != null && file.ContentLength > 0)
                    {
                        string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string uploadDirectory = Server.MapPath("~/UploadedFiles");
                        string filePath = Path.Combine(uploadDirectory, filename);
                        file.SaveAs(filePath);
                        model.Document = filePath;
                    }
                    existingEducation.UpdatedDated = DateTime.Now;

                    // Save changes to the database
                    context.SaveChanges();

                    if (action == "Addmore")
                    {
                        return RedirectToAction("EmployeeEducationCreate");
                    }
                    return RedirectToAction("EmployeeEducationIndex");
                }
            }
            return View(model);
        }

        public JsonResult EducationDelete(string EducationId)
        {
            var a = Convert.ToInt32(EducationId);
            var data = context.EmployeeEducations.Where(x => x.EducationId == a).FirstOrDefault();
            context.EmployeeEducations.Remove(data);
            context.SaveChanges();
            ViewBag.Messsage = "Record Delete Successfully";
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}


