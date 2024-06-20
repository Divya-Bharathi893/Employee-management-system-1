using DGHCM.Models;
using DGHCM.ViewModel;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Deployment.Internal;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Xml.Schema;

namespace DGHCM.Controllers
{
    public class PayRollMasterController : Controller
    {
        // GET: PayRollMaster

        CommonClass common = new CommonClass();
        HumanCapitalManagementEntities context = new HumanCapitalManagementEntities();

        [HttpGet]
        public ActionResult PayRollMasterIndex()
        {
            var listofdata = context.PayRollHeaders.ToList();

            return View(listofdata);
        }

        [HttpGet]
        public ActionResult PayRollMasterCreate(int Id = 0)
        {

            var join = (from a in context.PayRollDetails

                        join b in context.EmployeeDetails on a.EmployeeId equals b.EmployeeId
                        join d in context.Emp_SalaryDetails on a.EmployeeId equals d.EmployeeId
                        where a.PayRollHeaderId == Id
                        select new PayrollVM
                        {

                            EmployeeName = b.FirstName + "" + b.LastName,
                            EmployeeId = a.EmployeeId,
                            Salary = d.Amount,
                            Allowance = a.TotalAllowance,
                            Deduction = a.TotalDeduction,
                            NetPay = a.NetSalary,
                            PayRollHeaderId = a.PayRollHeaderId,
                            PayRollId = a.PayRollId,
                        }).ToList();

            if (Id != 0 /*&& Id != null*/)
            {
                ViewBag.HeaderId = Id;
            }

            return View(join);
        }


        [HttpPost]
        public ActionResult PayRollMasterCreate(FormCollection frm)
        {
            //--modal as a instancename-------

            PayRollHeader model = new PayRollHeader();
            var companyId = common.CompanyId();
            model.CompanyId = Guid.Parse(companyId);
            var date = Convert.ToDateTime(frm["trip-start"]);
            model.PayRollDate = date.Day;



            //-----------for header allowance-------

            var TotalAllowance = context.EmployeeAllowanceDetails.Where(x => x.AllowanceOrDeduction == "a").ToList();
            if (TotalAllowance.Count > 0)
            {
                model.TotalAllowance = TotalAllowance.Sum(x => x.Amount);
            }
            //-----------for header deduction------

            var TotalDeductions = context.EmployeeAllowanceDetails.Where(x => x.AllowanceOrDeduction == "d").ToList();

            if (TotalDeductions.Count > 0)
            {
                model.TotalDeduction = TotalDeductions.Sum(x => x.Amount);
            }

            //-----------header salary---------

            var salary = context.Emp_SalaryDetails.ToList();
            if (salary.Count > 0)
            {

                var empsalary = salary.Sum(x => x.Amount);
                model.GrossPay = empsalary + model.TotalAllowance;
            }

            model.CreatedBy = 0;
            model.CreatedDate = DateTime.Now;
            model.CreatedBy = 0;
            model.UpdatedDate = DateTime.Now;

            //---------document number----------

            int year = DateTime.Now.Year;

            int lastDocumentNumber = GetLastDocumentNumberForYear(year);

            int newDocumentNumber = lastDocumentNumber + 1;

            string formattedDocumentNumber = newDocumentNumber.ToString("00000");

            string documentNumber = $"PR/{year}/{formattedDocumentNumber}"; //string interpolation;

            model.DocumentNumber = documentNumber;

            //------------

            context.PayRollHeaders.Add(model);
            context.SaveChanges();
            var a = Guid.Parse(companyId);
            var sd = context.PayRollHeaders.ToList();
            var data = sd.Where(x => x.CompanyId == a).Select(x => x.PayRollHeaderId).LastOrDefault();
            var datas = context.EmployeeDetails.ToList();

            foreach (var b in datas)
            {
                //------payrolldetetail as a instancename------------//

                PayRollDetail payRollDetail = new PayRollDetail();
                payRollDetail.CompanyId = b.CompanyId;
                payRollDetail.EmployeeId = b.EmployeeId;
                payRollDetail.PayRollHeaderId = data;

                //------------for getting allowance------

                var e = context.EmployeeAllowanceDetails.ToList();

                var TotalAllowances = context.EmployeeAllowanceDetails
                        .Where(x => x.EmployeeId == b.EmployeeId && x.AllowanceOrDeduction == "a").ToList();

                decimal Allowance = 0;

                if (TotalAllowances.Count > 0)
                {
                    Allowance = TotalAllowances.Sum(x => x.Amount);
                }

                //----------for getting deduction-----

                var h = context.EmployeeAllowanceDetails.ToList();

                var ToTalDeduction = context.EmployeeAllowanceDetails
                   .Where(x => x.EmployeeId == b.EmployeeId && x.AllowanceOrDeduction == "d").ToList();

                decimal Deduction = 0;

                if (ToTalDeduction.Count > 0)
                {
                    Deduction = ToTalDeduction.Sum(x => x.Amount);
                }

                payRollDetail.TotalAllowance = Allowance;
                payRollDetail.TotalDeduction = Deduction;

                //-----------for salary-----

                var g = context.Emp_SalaryDetails.ToList();
                decimal amnt = 0;
                var Netpay = context.Emp_SalaryDetails.Where(x => x.EmployeeId == b.EmployeeId).ToList();
                if (Netpay.Count > 0)
                {
                    amnt = Netpay.Sum(x => x.Amount);
                }
                payRollDetail.NetSalary = (amnt + Allowance) - Deduction;

                payRollDetail.PaymentDate = date;
                payRollDetail.Month = date.Month;
                payRollDetail.Year = date.Year;
                payRollDetail.IsActive = frm["isactive"] == "on" ? true : false;
                payRollDetail.CreatedBy = 0;
                payRollDetail.CreatedDate = DateTime.Now;
                payRollDetail.UpdatedBy = 0;
                payRollDetail.UpdatedaDate = DateTime.Now;

                context.PayRollDetails.Add(payRollDetail);

                context.SaveChanges();
            }
            return RedirectToAction("PayRollMasterCreate", new { Id = data });
        }

        //------------method for document number-----------
        private int GetLastDocumentNumberForYear(int year)
        {

            var lastDocumentNumber = context.PayRollHeaders
                .Where(x => x.CreatedDate.Year == year)
                .Select(x => x.DocumentNumber)
                .ToList()
                .Select(documentNumber =>
                {

                    string[] parts = documentNumber.Split('/');
                    if (parts.Length == 3 && parts[1] == year.ToString())
                    {
                        int lastDocNumber;
                        if (int.TryParse(parts[2], out lastDocNumber))// trypase-parse a string representation into an integer//
                        {
                            return lastDocNumber;
                        }
                    }
                    return 0;
                })
                .DefaultIfEmpty()
                .Max();

            return lastDocumentNumber;
        }

        //-------------edit--------//

        [HttpGet]
        public JsonResult PayRollMasterEdit(int EmployeeId, int PayRollId)
        {



            var joins = (from bb in context.Emp_SalaryDetails
                         join dd in context.SalaryTypeMasters on new { bb.CompanyId, bb.SalaryTypeId } equals new { dd.CompanyId, dd.SalaryTypeId }

                         join ee in context.EmployeeAllowanceDetails on new { bb.CompanyId, bb.EmployeeId } equals new { ee.CompanyId, ee.EmployeeId }

                         join ff in context.MiscellaneousDetails on new { bb.CompanyId, bb.EmployeeId } equals new { ff.CompanyId, ff.EmployeeId } 

                         join pay in context.PayRollDetails on new { bb.CompanyId, bb.EmployeeId } equals new { pay.CompanyId, pay.EmployeeId }

                         join emp in context.EmployeeDetails on new { pay.CompanyId, pay.EmployeeId } equals new { emp.CompanyId, emp.EmployeeId }


                         where pay.EmployeeId == EmployeeId && pay.PayRollId == PayRollId
                         select new PayrollVM
                         {
                             SalaryTypeId = dd.SalaryTypeId,
                             SalaryTypeName = dd.SalaryTypeName,
                             Amount = bb.Amount,
                             AllowanceorDeduction = ee.AllowanceOrDeduction,
                             EmployeeId = emp.EmployeeId,
                             PayRollId = pay.PayRollId,
                             EmployeeName = emp.FirstName + " " + emp.LastName,
                             AllowanceAmount=ee.Amount,

                             //date = pay.PaymentDate,
                             //AllowanceorDeduction = (ee != null) ? ee.AllowanceOrDeduction : ff.AllowanceorDeduction,
                             //EmployeeId = emp == null ? 0 : emp.EmployeeId,
                             //PayRollId = pay == null ? 0 : pay.PayRollId,
                             //EmployeeName = emp == null ? null : emp.FirstName + " " + emp.LastName,

                             SalaryId = bb.SalaryId,
                             AllowanceName = ee.AllowanceName,
                             PayRollHeaderId = pay.PayRollHeaderId
                         }).ToList();

            var groupby = joins.GroupBy(ff => ff.EmployeeId); // make an groupby for one employeeid with no.of.salary
            PayrollVM Myvm = new PayrollVM();

            foreach (var item in groupby)
            {
                Myvm.EmployeeId = item.Key; //Key defines id 

                List<decimal> Amount = new List<decimal>();
                List<string> SalaryTypeName = new List<string>();
                List<int> SalaryId = new List<int>();
                List<string> AllowanceName = new List<string>();
                List<string> AllowanceorDeduction = new List<string>();
                List<decimal> AllowanceAmount = new List<decimal>();

                List<string>DeductionName = new List<string>();//new
                List<decimal> DeductionAmount = new List<decimal>();//new


                List<decimal>TotalAllowance= new List<decimal>();//new
                List<decimal>NetPay=new List<decimal>();//new

               foreach (var a in item)
                {
                    Myvm.EmployeeName = a.EmployeeName;
                    Myvm.AllowanceName=a.AllowanceName;
                    Myvm.AllowanceAmount=a.AllowanceAmount;
                    Amount.Add(a.Amount);
                    SalaryTypeName.Add(a.SalaryTypeName);
                    SalaryId.Add(a.SalaryId);
                    AllowanceorDeduction.Add(a.AllowanceorDeduction);
                    TotalAllowance.Add(a.AllowanceAmount);
                    NetPay.Add(a.Salary);

                    Myvm.DeductionName=a.DeductionName;//new
                    Myvm.DeductionAmount=a.DeductionAmount;


                    
                }
                Myvm.arr_Amounts = Amount.ToArray();
                Myvm.arr_SalaryId = SalaryId.ToArray();
                Myvm.arr_SalaryTypeName = SalaryTypeName.ToArray();
                Myvm.arr_AllowanceorDeduction = AllowanceorDeduction.ToArray();
            }
            return Json(Myvm, JsonRequestBehavior.AllowGet);

        }

    }

}