using Antlr.Runtime.Misc;
using DGHCM.Models;
using DGHCM.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace DGHCM.Controllers
{
    public class FinanceController : Controller
    {

        HumanCapitalManagementEntities context = new HumanCapitalManagementEntities();
        CommonClass common = new CommonClass();
        FinanceBankMaster bankMaster = new FinanceBankMaster();
        AccountMaster accountMaster = new AccountMaster();
        CustomerDetail customerDetail = new CustomerDetail();
        SupplyDetail supplyDetail = new SupplyDetail();
        PaymentDetail paymentDetail = new PaymentDetail();
        GeneralLedgerDetail generalLedgerDetail = new GeneralLedgerDetail();
        FinanceSubDetail FinanceSubDetail = new FinanceSubDetail();
        //BankMaster
        public ActionResult FinanceBankMasterIndex()
        {
            var ListOfData = context.FinanceBankMasters.ToList();
            return View(ListOfData);
        }
        public JsonResult BankNameList()
        {
            var listofData = context.FinanceBankMasters.ToList();
            return Json(listofData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddBank(FinanceBankMaster objData)
        {
            if (objData != null)
            {
                var CompanyId = common.CompanyId();
                objData.CompanyId = Guid.Parse(CompanyId);
                objData.CreatedDate = DateTime.Now;
                objData.UpdatedBy = 6;
                objData.UpdatedDate = DateTime.Now;
                // Check if the CustomerName already exists in the database
                var existingbankname = context.FinanceBankMasters.FirstOrDefault(s => s.BankName == objData.BankName);
                if (existingbankname != null)
                {
                    return Json(new { success = false, message = "Bank name already exists" }, JsonRequestBehavior.AllowGet);
                }
                // Save the data to the database
                context.FinanceBankMasters.Add(objData);
                context.SaveChanges();
                return Json(new { success = true, message = "Data Saved Successfully" }, JsonRequestBehavior.AllowGet);

            }
            return Json("validation failed");
        }
        [HttpGet]
        public JsonResult EditBank(int id)
        {
            var bank = context.FinanceBankMasters.FirstOrDefault(x => x.BankId == id);
            return Json(bank, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateBank(FinanceBankMaster frm)
        {
            var cnvrtedId = frm.BankId;
            var data = context.FinanceBankMasters.Where(x => x.BankId == cnvrtedId).FirstOrDefault();
            if (data != null)
            {
                data.BankId = frm.BankId;
                data.IsActive = frm.IsActive;
                data.BankName = frm.BankName;
                data.UpdatedBy = 6;
                data.UpdatedDate = DateTime.Now;
                // Save the data to the database
                context.FinanceBankMasters.AddOrUpdate(data);
                context.SaveChanges();
                return Json(new { success = true, message = "Data Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json("Failed");
        }
        public JsonResult BankMasterDelete(string BankId)
        {
            var a = Convert.ToInt32(BankId);
            var data = context.FinanceBankMasters.Where(x => x.BankId == a).FirstOrDefault();
            context.FinanceBankMasters.Remove(data);
            context.SaveChanges();
            ViewBag.Messsage = "Record Delete Successfully";
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //***********************************************************************************************************************************
        [HttpGet]
        public ActionResult FinanceBankMasterCreate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FinanceBankMasterCreate(FormCollection frm)
        {
            var CompanyId = common.CompanyId();
            bankMaster.CompanyId = Guid.Parse(CompanyId);
            bankMaster.BankName = frm["BankName"];
            bankMaster.IsActive = frm["IsActive"] == "on" ? true : false;
            bankMaster.CreatedBy = 0;
            bankMaster.CreatedDate = DateTime.Now;
            bankMaster.UpdatedBy = 0;
            bankMaster.UpdatedDate = DateTime.Now;

            var data = context.FinanceBankMasters.Add(bankMaster);
            context.SaveChanges();
            return RedirectToAction("FinanceBankMasterIndex");
        }

        [HttpGet]
        public ActionResult FinanceBankMasterEdit(int Id)
        {
            var data = context.FinanceBankMasters.FirstOrDefault(x => x.BankId == Id);
            return View(data);
        }
        [HttpPost]
        public ActionResult FinanceBankMasterEdit(FinanceBankMaster bankMaster)
        {
            var data = context.FinanceBankMasters.Where(x => x.BankId == bankMaster.BankId).FirstOrDefault();

            if (data != null)
            {
                data.BankName = bankMaster.BankName;
                data.IsActive = bankMaster.IsActive;
                data.UpdatedBy = 0;
                data.UpdatedDate = DateTime.Now;
                context.SaveChanges();
            }
            return RedirectToAction("FinanceBankMasterIndex");
        }

       

        //***********************************************************************************************************************************

        //Accont Master

        public ActionResult AccountMastersIndex()
        {
            var ListOfData = context.AccountMasters.ToList();
            return View(ListOfData);
        }
        public JsonResult AccountList()
        {
            var listofData = context.AccountMasters.ToList();
            return Json(listofData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddAccount(AccountMaster objData)
        {
            if (objData != null)
            {
                var CompanyId = common.CompanyId();
                objData.CompanyId = Guid.Parse(CompanyId);
                objData.AccountCode = common.AccountMasterCode();
                objData.CreatedDate = DateTime.Now;
                objData.UpdatedBy = 6;
                objData.UpdatedDate = DateTime.Now;
                // Check if the AccountName already exists in the database
                var existingAccount = context.AccountMasters.FirstOrDefault(s => s.AccountName == objData.AccountName);
                if (existingAccount != null)
                {
                    return Json(new { success = false, message = "Account name already exists" }, JsonRequestBehavior.AllowGet);
                }
                // Save the data to the database
                context.AccountMasters.Add(objData);
                context.SaveChanges();
                return Json(new { success = true, message = "Data Saved Successfully" }, JsonRequestBehavior.AllowGet);

            }
            return Json("validation failed");
        }
        [HttpGet]
        public JsonResult EditAccount(int id)
        {
            var account = context.AccountMasters.FirstOrDefault(x => x.AccountMasterId == id);
            return Json(account, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateAccount(AccountMaster frm)
        {
            var cnvrtedId = frm.AccountMasterId;
            var data = context.AccountMasters.Where(x => x.AccountMasterId == cnvrtedId).FirstOrDefault();
            if (data != null)
            {
                data.AccountMasterId = frm.AccountMasterId;
                data.IsActive = frm.IsActive;
                data.AccountName = frm.AccountName;
                data.OpeningBalance = frm.OpeningBalance;
                data.BankOrGL = frm.BankOrGL;
                data.UpdatedBy = 6;
                data.UpdatedDate = DateTime.Now;
                // Save the data to the database
                context.AccountMasters.AddOrUpdate(data);
                context.SaveChanges();
                return Json(new { success = true, message = "Data Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json("Failed");
        }
        public JsonResult AccountMasterDelete(string AccountMasterId)
        {
            var a = Convert.ToInt32(AccountMasterId);
            var data = context.AccountMasters.Where(x => x.AccountMasterId == a).FirstOrDefault();
            context.AccountMasters.Remove(data);
            context.SaveChanges();
            ViewBag.Messsage = "Record Delete Successfully";
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        //***********************************************************************************************************************************
        [HttpGet]
        public ActionResult AccountMastersCreate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AccountMastersCreate(FormCollection frm)
        {

            var CompanyId = common.CompanyId();
            accountMaster.CompanyId = Guid.Parse(CompanyId);
            accountMaster.AccountCode = common.AccountMasterCode();
            accountMaster.AccountName = frm["AccountName"];
            accountMaster.OpeningBalance = Convert.ToDecimal(frm["AccountBalance"]);
            accountMaster.BankOrGL = Convert.ToInt32(frm["AccountType"]);
            accountMaster.IsActive = frm["IsActive"] == "on" ? true : false;
            accountMaster.CreatedBy = 0;
            accountMaster.CreatedDate = DateTime.Now;
            accountMaster.UpdatedBy = 0;
            accountMaster.UpdatedDate = DateTime.Now;
            var data = context.AccountMasters.Add(accountMaster);
            context.SaveChanges();
            return RedirectToAction("AccountMastersIndex");
        }

        [HttpGet]
        public ActionResult AccountMastersEdit(int Id)
        {
            var data = context.AccountMasters.FirstOrDefault(x => x.AccountMasterId == Id);
            return View(data);
        }


        [HttpPost]
        public ActionResult AccountMastersEdit(AccountMaster accountMaster)
        {
            var data = context.AccountMasters.Where(x => x.AccountMasterId == accountMaster.AccountMasterId).FirstOrDefault();
            if (data != null)
            {
                data.AccountName = accountMaster.AccountName;
                data.OpeningBalance = accountMaster.OpeningBalance;
                data.AccountCode = accountMaster.AccountCode;
                data.BankOrGL = accountMaster.BankOrGL;
                data.IsActive = accountMaster.IsActive;
                data.UpdatedDate = DateTime.Now;
                data.UpdatedBy = 0;
                context.SaveChanges();
            }
            return RedirectToAction("AccountMastersIndex");
        }
       
        /*****************************************************************************************************************************/
        //Customer

        public ActionResult CustomerIndex()
        {
            var ListOfData = context.CustomerDetails.ToList();
            return View(ListOfData);
        }
        public JsonResult CustomerList()
        {
            var listofData = context.CustomerDetails.ToList();
            return Json(listofData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddCustomer(CustomerDetail objData)
        {
            if (objData != null)
            {
                var CompanyId = common.CompanyId();
                objData.CompanyId = Guid.Parse(CompanyId);
                objData.CustomerCode = common.CustomerCode();
                objData.CreatedDate = DateTime.Now;
                objData.UpdatedBy = 6;
                objData.UpdatedDate = DateTime.Now;
                // Check if the CustomerName already exists in the database
                var existingCustomer = context.CustomerDetails.FirstOrDefault(s => s.CustomerName == objData.CustomerName);
                if (existingCustomer != null)
                {
                    return Json(new { success = false, message = "Customer name already exists" }, JsonRequestBehavior.AllowGet);
                }
                // Save the data to the database
                context.CustomerDetails.Add(objData);
                context.SaveChanges();
                return Json(new { success = true, message = "Data Saved Successfully" }, JsonRequestBehavior.AllowGet);

            }
            return Json("validation failed");
        }
        [HttpGet]
        public JsonResult EditCustomer(int id)
        {
            var customer = context.CustomerDetails.FirstOrDefault(x => x.CustomerDetailsId == id);
            return Json(customer, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateCustomer(CustomerDetail frm)
        {
            var cnvrtedId = frm.CustomerDetailsId;
            var data = context.CustomerDetails.Where(x => x.CustomerDetailsId == cnvrtedId).FirstOrDefault();
            if (data != null)
            {
                data.CustomerDetailsId = frm.CustomerDetailsId;
                data.IsActive = frm.IsActive;
                data.CustomerName = frm.CustomerName;
                data.UpdatedBy = 6;
                data.UpdatedDate = DateTime.Now;
                // Save the data to the database
                context.CustomerDetails.AddOrUpdate(data);
                context.SaveChanges();
                return Json(new { success = true, message = "Data Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json("Failed");
        }
        public JsonResult CustomerDelete(string CustomerDetailsId)
        {
            var a = Convert.ToInt32(CustomerDetailsId);
            var data = context.CustomerDetails.Where(x => x.CustomerDetailsId == a).FirstOrDefault();
            context.CustomerDetails.Remove(data);
            context.SaveChanges();
            ViewBag.Messsage = "Record Delete Successfully";
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        /***************************************************************************************************************/
        [HttpGet]
        public ActionResult CustomerCreate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CustomerCreate(FormCollection frm)
        {
            var CompanyId = common.CompanyId();
            customerDetail.CompanyId = Guid.Parse(CompanyId);
            customerDetail.CustomerCode = common.CustomerCode();
            customerDetail.CustomerName = frm["CustomerName"];
            customerDetail.IsActive = frm["IsActive"] == "on" ? true : false;
            customerDetail.CreatedBy = 0;
            customerDetail.CreatedDate = DateTime.Now;
            customerDetail.UpdatedBy = 0;
            customerDetail.UpdatedDate = DateTime.Now;
            var data = context.CustomerDetails.Add(customerDetail);
            context.SaveChanges();
            return RedirectToAction("CustomerIndex");
        }

        [HttpGet]
        public ActionResult CustomerEdit(int Id)
        {
            var data = context.CustomerDetails.FirstOrDefault(x => x.CustomerDetailsId == Id);
            return View(data);
        }


        [HttpPost]
        public ActionResult CustomerEdit(CustomerDetail customerDetail)
        {
            var data = context.CustomerDetails.Where(x => x.CustomerDetailsId == customerDetail.CustomerDetailsId).FirstOrDefault();

            if (data != null)
            {
                data.CustomerCode = customerDetail.CustomerCode;
                data.CustomerName = customerDetail.CustomerName;
                data.IsActive = customerDetail.IsActive;
                data.UpdatedDate = DateTime.Now;
                data.UpdatedBy = 0;
                context.SaveChanges();
            }
            return RedirectToAction("CustomerIndex");
        }
        
        /***************************************************************************************************************/
        //Supply

        public ActionResult SupplyIndex()
        {
            var ListOfData = context.SupplyDetails.ToList();
            return View(ListOfData);
        }
        public JsonResult SupplyList()
        {
            var listofData = context.SupplyDetails.ToList();
            return Json(listofData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AddSupply(SupplyDetail objData)
        {
            if (objData != null)
            {
                var CompanyId = common.CompanyId();
                objData.CompanyId = Guid.Parse(CompanyId);
                objData.FinanceSupplyCode = common.FinanceSupplyCode();
                objData.CreatedDate = DateTime.Now;
                objData.UpdatedBy = 6;
                objData.UpdatedDate = DateTime.Now;
                // Check if the SupplyName already exists in the database
                var existingSupplyName = context.SupplyDetails.FirstOrDefault(s => s.FinanceSupplyName == objData.FinanceSupplyName);
                if (existingSupplyName != null)
                {
                    return Json(new { success = false, message = "Supply Name already exists" }, JsonRequestBehavior.AllowGet);
                }
                // Save the data to the database
                context.SupplyDetails.Add(objData);
                context.SaveChanges();
                return Json(new { success = true, message = "Data Saved Successfully" }, JsonRequestBehavior.AllowGet);

            }
            return Json("validation failed");
        }
        [HttpGet]
        public JsonResult EditSupply(int id)
        {
            var supply = context.SupplyDetails.FirstOrDefault(x => x.SupplyDetailsId == id);
            return Json(supply, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateSupply(SupplyDetail frm)
        {
            var cnvrtedId = frm.SupplyDetailsId;
            var data = context.SupplyDetails.Where(x => x.SupplyDetailsId == cnvrtedId).FirstOrDefault();
            if (data != null)
            {
                data.SupplyDetailsId = frm.SupplyDetailsId;
                data.IsActive = frm.IsActive;
                data.FinanceSupplyName = frm.FinanceSupplyName;
                data.UpdatedBy = 6;
                data.UpdatedDate = DateTime.Now;
                // Save the data to the database
                context.SupplyDetails.AddOrUpdate(data);
                context.SaveChanges();
                return Json(new { success = true, message = "Data Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return Json("Failed");
        }
        public JsonResult SupplyDelete(string SupplyDetailsId)
        {
            var a = Convert.ToInt32(SupplyDetailsId);
            var data = context.SupplyDetails.Where(x => x.SupplyDetailsId == a).FirstOrDefault();
            context.SupplyDetails.Remove(data);
            context.SaveChanges();
            ViewBag.Messsage = "Record Delete Successfully";
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        /***************************************************************************************************************/
        [HttpGet]
        public ActionResult SupplyCreate()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SupplyCreate(FormCollection frm)
        {
            var CompanyId = common.CompanyId();
            supplyDetail.CompanyId = Guid.Parse(CompanyId);
            supplyDetail.FinanceSupplyCode = common.FinanceSupplyCode();
            supplyDetail.FinanceSupplyName = frm["SupplyName"];
            supplyDetail.IsActive = frm["IsActive"] == "on" ? true : false;
            supplyDetail.CreatedBy = 0;
            supplyDetail.CreatedDate = DateTime.Now;
            supplyDetail.UpdatedBy = 0;
            supplyDetail.UpdatedDate = DateTime.Now;
            var data = context.SupplyDetails.Add(supplyDetail);
            context.SaveChanges();
            return RedirectToAction("SupplyIndex");
        }

        [HttpGet]
        public ActionResult SupplyEdit(int Id)
        {
            var data = context.SupplyDetails.FirstOrDefault(x => x.SupplyDetailsId == Id);
            if (data == null)
            {
                return HttpNotFound();
            }
            return View(data);
        }


        [HttpPost]
        public ActionResult SupplyEdit(SupplyDetail supplyDetail)


        {
            if (ModelState.IsValid)
            {
                var data = context.SupplyDetails.Where(x => x.SupplyDetailsId == supplyDetail.SupplyDetailsId).FirstOrDefault();

                if (data != null)
                {
                    data.FinanceSupplyCode = supplyDetail.FinanceSupplyCode;
                    data.FinanceSupplyName = supplyDetail.FinanceSupplyName;
                    data.IsActive = supplyDetail.IsActive;
                    data.UpdatedDate = DateTime.Now;
                    data.UpdatedBy = 0;
                    context.SaveChanges();
                }
                return RedirectToAction("SupplyIndex");
            }
            return View(supplyDetail);
        }       
        /**************************************************************************************************************************/
        /*Payment*/
        public ActionResult paymentDetailIndex()
        {
            var data = context.PaymentDetails.ToList();
            return View(data);
        }
        [HttpGet]
        public ActionResult paymentDetailCreate()

        {   //BANK MASTER DROPDOWN
            var bank = context.FinanceBankMasters.ToList();
            var Doc = bank.Select(p => new { BankId = p.BankId, bankname = p.BankName });
            ViewBag.BankList = new SelectList(Doc, "BankId", "bankname");
            //CUSTOMER DETAILS DROPDOWN
            var customer = context.CustomerDetails.ToList();
            var CD = customer.Select(c => new { CustomerId = c.CustomerDetailsId, Customername = c.CustomerName, customercode = c.CustomerCode });
            ViewBag.CustomerList = new SelectList(CD, "CustomerId", "Customername");
            //SUPPLY DETAILS DROPDOWN
            var supply = context.SupplyDetails.ToList();
            var SD = supply.Select(s => new { SupplyId = s.SupplyDetailsId, supplyname = s.FinanceSupplyName, supplycode = s.FinanceSupplyCode });
            ViewBag.SupplyList = new SelectList(SD, "SupplyId", "supplyname");
            //ACCOUNT MASTER DROPDOWN
            var AccountMaster = context.AccountMasters.ToList();
            var AM = AccountMaster.Select(g => new { AccountmasterId = g.AccountMasterId, Acname = g.AccountName, AccountCode = g.AccountCode });
            ViewBag.AMList = new SelectList(AM, "AccountmasterId", "Acname", "AccountCode");
            return View();
        }

        [HttpPost]

        public JsonResult paymentDetailCreate(FormCollection frm, HttpPostedFileBase file)
        {

            var CompanyId = common.CompanyId();
            int Id;
            var a = frm["PaymentDetailsId"];
            if (!int.TryParse(frm["PaymentDetailsId"], out Id))
            {
                Id = 0;
            }
            if (Id == 0)
            {
                paymentDetail.CompanyId = Guid.Parse(CompanyId);
                paymentDetail.CashOrCheck = Convert.ToInt32(frm["PaymentMethod"]);
                paymentDetail.ReciptOrPayment = Convert.ToInt32(frm["ReceiptOrPayment"]);
                int detailsType;
                if (!int.TryParse(frm["DetailsType"], out detailsType))
                {
                    detailsType = 0;
                }
                paymentDetail.DetailsType = detailsType;
                paymentDetail.DebitOrCredit = Convert.ToInt32(frm["DebitOrCredit"]);
                paymentDetail.BankId = Convert.ToInt32(frm["BankList"]);
                if (!string.IsNullOrEmpty(frm["Date"]))
                {
                    paymentDetail.Date = DateTime.Parse(frm["Date"]);
                }
                paymentDetail.IsActive = frm["IsActive"] == "on" ? true : false;
                paymentDetail.CreatedBy = 0;
                paymentDetail.CreatedDate = DateTime.Now;
                paymentDetail.UpdatedBy = 0;
                paymentDetail.UpdatedDate = DateTime.Now;
                if (file != null && file.ContentLength > 0)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string uploadDirectory = Server.MapPath("~/FinanceDocument");
                    if (!Directory.Exists(uploadDirectory))
                    {
                        Directory.CreateDirectory(uploadDirectory);
                    }
                    string filePath = Path.Combine(uploadDirectory, filename);
                    file.SaveAs(filePath);
                    var DocPath = "~/FinanceDocument/" + filename;
                    paymentDetail.UploadDocument = DocPath;
                    paymentDetail.TotalAmount = 0;
                }

            }
            if (Id == 0)
            {
                var data = context.PaymentDetails.Add(paymentDetail);
                context.SaveChanges();
                Id = data.PaymentDetailsId;
            }
            else
            {
                var data = context.PaymentDetails.Where(x => x.PaymentDetailsId == Id).FirstOrDefault();
                data.DetailsType = Convert.ToInt32(frm["DetailsType"]);
                
                int choose = Convert.ToInt32(frm["DetailsType"]);
                if (choose == 1 || choose == 2 && choose != 3)
                {
                    //SUB TABLE 
                    FinanceSubDetail.CompanyId = Guid.Parse(CompanyId);
                    FinanceSubDetail.CustomerOrSupply = Convert.ToInt32(frm["DetailsType"]);
                    FinanceSubDetail.CorS = Convert.ToInt32(frm["DetailsType"]) == 1 ? Convert.ToInt32(frm["CustomerList"]) :
                        (Convert.ToInt32(frm["DetailsType"]) == 2 ? Convert.ToInt32(frm["SupplyList"]) : 0);
                    decimal amount;
                    if (decimal.TryParse(frm["Amount"], out amount))
                    {
                        FinanceSubDetail.Amount = amount;
                    }
                    FinanceSubDetail.Comments = frm["Comments"];
                    FinanceSubDetail.IsActive = frm["IsActive"] == "on";
                    FinanceSubDetail.CreatedBy = 0;
                    FinanceSubDetail.CreatedDate = DateTime.Now;
                    FinanceSubDetail.UpdatedBy = 0;
                    FinanceSubDetail.UpdatedDate = DateTime.Now;
                    FinanceSubDetail.PaymentDetailsId = Id;
                    data.TotalAmount = amount;

                    var SubDetails = context.FinanceSubDetails.Add(FinanceSubDetail);
                    context.SaveChanges();
                }
                if (choose == 3)
                {
                    GeneralLedger(frm, Id);
                }
            }
            return Json(/*new{ Id, success = true, message = "Data Saved Successfully" },*/ Id, JsonRequestBehavior.AllowGet);
        }

        //generalLedgerDetail
        public ActionResult GeneralLedger(FormCollection frm, int Id)
        {
            var a = frm["HRowCount"];
            GeneralLedgerDetail generalLedgerDetail = new GeneralLedgerDetail();
            var CompanyId = common.CompanyId();
            generalLedgerDetail.CompanyId = Guid.Parse(CompanyId);
            generalLedgerDetail.GeneralLedgerCode = common.GeneralLedgerCode();
            generalLedgerDetail.Comments = frm["glComments_" + a];
            var ab = frm["glAmount"];
            generalLedgerDetail.Amount = string.IsNullOrEmpty(frm["glAmount_" + a]) ? 0 : Convert.ToDecimal(frm["glAmount_" + a]);
            int accountMasterId;
            if (int.TryParse(frm["AMList"], out accountMasterId))
            {
                generalLedgerDetail.AccountMasterId = accountMasterId;
            }
            generalLedgerDetail.IsActive = true;
            generalLedgerDetail.CreatedBy = 0;
            generalLedgerDetail.CreatedDate = DateTime.Now;
            generalLedgerDetail.UpdatedBy = 0;
            generalLedgerDetail.UpdatedDate = DateTime.Now;
            generalLedgerDetail.PaymentDetailsId = Id;
            var data = context.PaymentDetails.Where(x => x.PaymentDetailsId == Id).FirstOrDefault();
            data.TotalAmount = Convert.ToDecimal(frm["totalAmount"]); 

           var generalLedgerDetails = context.GeneralLedgerDetails.Add(generalLedgerDetail);
           context.SaveChanges();
           return View();
        }
        //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
        [HttpGet]
        public ActionResult paymentDetailEdit(int Id)
        {
            var data = context.PaymentDetails.FirstOrDefault(x => x.PaymentDetailsId == Id);
            var customerdetails = context.CustomerDetails.FirstOrDefault(x => x.CustomerDetailsId==Id);
            var supplydetails = context.SupplyDetails.FirstOrDefault(x => x.SupplyDetailsId==Id);
            var accountdetails = context.AccountMasters.FirstOrDefault(x => x.AccountMasterId == Id);
            //BANK MASTER DROPDOWN
            var bank = context.FinanceBankMasters.ToList();
            var Doc = bank.Select(p => new { BankId = p.BankId, bankname = p.BankName });
            ViewBag.BankList = new SelectList(Doc, "BankId", "bankname", data.BankId);

            //CUSTOMER DETAILS DROPDOWM
            var customer = context.CustomerDetails.ToList();
            var cus = customer.Select(p => new { CustomerDetailsId = p.CustomerDetailsId, CustomerName = p.CustomerName });
            ViewBag.CustomerList = new SelectList(cus, "CustomerDetailsId", "CustomerName", customerdetails.CustomerDetailsId);

            //SUPPLY DETAILS DROPDOWN
            var supply = context.SupplyDetails.ToList();
            var sup = supply.Select(p => new { SupplyDetailsId = p.SupplyDetailsId, FinanceSupplyName = p.FinanceSupplyName });
            ViewBag.SupplyList = new SelectList(sup, "CustomerDetailsId", "CustomerName", supplydetails.SupplyDetailsId);

            //ACCOUNT DETAILS DROPDOWM
            var account = context.AccountMasters.ToList();
            var acc = account.Select(p => new { AccountMasterId = p.AccountMasterId, AccountName = p.AccountName });
            ViewBag.AMList = new SelectList(acc, "AccountMasterId", "AccountName", accountdetails.AccountMasterId);


            var List = context.PayRollDetails.ToList();
            var AllData = (from a in context.PaymentDetails
                           join b in context.FinanceSubDetails on a.PaymentDetailsId equals b.PaymentDetailsId
                           join c in context.GeneralLedgerDetails on a.PaymentDetailsId equals c.PaymentDetailsId
                           select new FinanceVM
                           {
                               PaymentDetailsId = a.PaymentDetailsId,
                               CashOrCheck = a.CashOrCheck,
                               ReciptOrPayment = a.ReciptOrPayment,
                               DetailsType = a.DetailsType,
                               Date = a.Date,
                               UploadDocument = a.UploadDocument,
                               IsActive = a.IsActive,
                               DebitOrCredit = a.DebitOrCredit,
                               BankId = a.BankId,
                               FinanceSubDetails = b.FinanceSubDetails,
                               CustomerOrSupply = b.CustomerOrSupply,
                               CorS = b.CorS,
                               Amount = b.Amount,
                               Comments = b.Comments,
                               GeneralLedgerDetailsId = c.GeneralLedgerDetailsId,
                               GeneralLedgerCode = c.GeneralLedgerCode,
                               AccountMasterId = c.AccountMasterId
                           }
                         ).FirstOrDefault();
            return View(AllData);
        }

        [HttpPost]
        public ActionResult paymentDetailEdit(FormCollection frm, HttpPostedFileBase file)
        {
            var cnvrtedId = Convert.ToInt32(frm["PaymentDetailsId"]);
            var data = context.PaymentDetails.Where(x => x.PaymentDetailsId == cnvrtedId).FirstOrDefault();
            if (paymentDetail != null)
            {
                // Update paymentDetail with the data from the form
                /*paymentDetail.Amount = Convert.ToDecimal(frm["Amount"]);*/
                paymentDetail.CashOrCheck = Convert.ToInt32(frm["PaymentMethod"]);
                paymentDetail.ReciptOrPayment = Convert.ToInt32(frm["ReceiptOrPayment"]);
                paymentDetail.DetailsType = Convert.ToInt32(frm["DetailsType"]);
                /* paymentDetail.Comments = frm["Comments"];*/
                paymentDetail.BankId = Convert.ToInt32(frm["BankList"]);
                paymentDetail.Date = DateTime.Parse(frm["Date"]);
                paymentDetail.IsActive = frm["IsActive"] == "on";
                paymentDetail.UpdatedBy = 0;
                paymentDetail.UpdatedDate = DateTime.Now;

                // Handle file upload if a new file is provided
                if (file != null && file.ContentLength > 0)
                {
                    string filename = Guid.NewGuid().ToString().Substring(0, 5) + Path.GetExtension(file.FileName);
                    string uploadDirectory = Server.MapPath("~/FinanceDocument");
                    string filePath = Path.Combine(uploadDirectory, filename);
                    file.SaveAs(filePath);
                    var docPath = "~/FinanceDocument/" + filename;
                    paymentDetail.UploadDocument = docPath;
                }
                // Save changes to the database
                context.SaveChanges();
            }
            return RedirectToAction("paymentDetailIndex");
        }

        public JsonResult PaymentDelete(string PaymentDetailsId)
        {
            var a = Convert.ToInt32(PaymentDetailsId);
            var data = context.PaymentDetails.Where(x => x.PaymentDetailsId == a).FirstOrDefault();
            context.PaymentDetails.Remove(data);
            context.SaveChanges();
            ViewBag.Messsage = "Record Delete Successfully";
            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}
