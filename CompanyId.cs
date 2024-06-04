using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DGHCM.Controllers
{
    public class CompanyId
    {
        public static string GenerateCompanyId()
        {
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            string randomComponent = Guid.NewGuid().ToString().Substring(0, 4); 
            string CompanyId = timestamp + randomComponent;
            return Guid.NewGuid().ToString();
        }
    }
}