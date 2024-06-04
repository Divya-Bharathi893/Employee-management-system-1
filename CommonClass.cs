using DGHCM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DGHCM.Controllers
{
    public class CommonClass
    {
        HumanCapitalManagementEntities context = new HumanCapitalManagementEntities();
        //public static string GenerateCompanyId()
        //{
        //    string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        //    string randomComponent = Guid.NewGuid().ToString().Substring(0, 4); 
        //    string CompanyId = timestamp + randomComponent;
        //    return CompanyId;
        //}
        public string CompanyId()
        {
            // Generating company ID
            /*string companyId = CompanyId.GenerateCompanyId();*/
            var a = "dc62b4d7-d1d6-4b59-bbeb-b5da52249c74";
            return a;
        }
        public string AccountMasterCode()
        {
            var lastAccount = context.AccountMasters.OrderByDescending(a => a.AccountCode).FirstOrDefault();
            var newAccountCode = 1;
            if (lastAccount != null)
            {
                newAccountCode =lastAccount.AccountCode.Length+1;
            }
            string accountCode = "ACC" + newAccountCode.ToString().PadLeft(4, '0');
            return accountCode;
        }
        /*public string CustomerCode()
        {
            var lastCustomer = context.CustomerDetails.OrderByDescending(a => a.CustomerCode).FirstOrDefault();
            var newCustomerCode = 1;
            if (lastCustomer != null)
            {
                newCustomerCode = lastCustomer.CustomerCode.Length + 1;
            }
            string customerCode = "ACC" + newCustomerCode.ToString().PadLeft(4, '0');
            return customerCode;
        }*/
        public string CustomerCode()
        {
            var lastCustomer = context.CustomerDetails.OrderByDescending(a => a.CustomerCode).FirstOrDefault();
            var newCustomerCode = 1;
            if (lastCustomer != null)
            {
                newCustomerCode = int.Parse(lastCustomer.CustomerCode.Substring(3)) + 1;
            }
            string customerCode = "ACC" + newCustomerCode.ToString().PadLeft(4, '0');
            return customerCode;
        }
        public string FinanceSupplyCode()
        {
            var lastSupplyCode = context.SupplyDetails.OrderByDescending(a => a.FinanceSupplyCode).FirstOrDefault();
            var newSupplyCode = 1;
            if (lastSupplyCode != null)
            {
                newSupplyCode = lastSupplyCode.FinanceSupplyCode.Length + 1;
            }
            string supplyCode = "ACC" + newSupplyCode.ToString().PadLeft(4, '0');
            return supplyCode;
        }
        public string GeneralLedgerCode()
        {
            var lastGeneralLedgerCode = context.GeneralLedgerDetails.OrderByDescending(a => a.GeneralLedgerCode).FirstOrDefault();
            var newGeneralLedgerCode = 1;
            if (lastGeneralLedgerCode != null)
            {
                newGeneralLedgerCode = lastGeneralLedgerCode.GeneralLedgerCode.Length + 1;
            }
            string GeneralLedgerCode = "ACC" + newGeneralLedgerCode.ToString().PadLeft(4, '0');
            return GeneralLedgerCode;
        }
    }
}