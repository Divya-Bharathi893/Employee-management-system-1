using Antlr.Runtime.Misc;
using DGHCM.Models;
using DGHCM.ViewModel;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace DGHCM.Controllers
{
    public class EmployeeAttendanceController : Controller
    {
        HumanCapitalManagementEntities context = new HumanCapitalManagementEntities();
        CommonClass common = new CommonClass();
        public ActionResult EmployeeAttendanceIndex()
        {
            var distinctDates = context.EmployeeAttendanceMasterDetails
                                .Select(x => DbFunctions.TruncateTime(x.AttendanceDate))
                                .Distinct()
                                .ToList();

            var attendanceByDate = new List<AttendanceSummaryVM>();

            foreach (var date in distinctDates)
            {
                var truncatedDate = date.Value;

                var attendanceDetails = context.EmployeeAttendanceMasterDetails
                                        .Where(x => DbFunctions.TruncateTime(x.AttendanceDate) == truncatedDate)
                                        .ToList();

                var totalEmployees = attendanceDetails.Count;
                var presentCount = attendanceDetails.Count(a => a.AttendanceStatus == "Present");
                var absentCount = attendanceDetails.Count(a => a.AttendanceStatus == "Absent");
                var permissionHours = CalculatePermissionHours(attendanceDetails);
                
                attendanceByDate.Add(new AttendanceSummaryVM
                {
                    AttendanceDate = truncatedDate,
                    TotalEmployees = totalEmployees,
                    PresentDays = presentCount,
                    AbsentDays = absentCount,
                    PermissionDays=Convert.ToInt32(permissionHours),
                });
            }

            return View(attendanceByDate);
        }

        private double CalculatePermissionHours(List<EmployeeAttendanceMasterDetail> attendanceDetails)
        {
            double totalPermissionHours = 0;
            foreach (var attendance in attendanceDetails)
            {
                if (attendance.AttendanceStatus == "Present" && attendance.Check_In != TimeSpan.Zero && attendance.Check_Out != TimeSpan.Zero)
                {
                    TimeSpan checkIn = attendance.Check_In;
                    TimeSpan checkOut = attendance.Check_Out;

                    TimeSpan timeDifference = checkOut - checkIn;
                    double permissionHours = timeDifference.TotalHours;

                    totalPermissionHours += permissionHours;
                }
            }
            return totalPermissionHours;
        }

       
        [HttpGet]
        public ActionResult EmployeeAttendanceCreate()
        {           
            var listitems = context.EmployeeDetails.ToList();
            var names = listitems.Select(x => x.EmployeeId);
            ViewBag.EmployeeList = new SelectList(names, "EmployeeId");
            var a = listitems.Select(e => new { e.EmployeeId, FullName = e.FirstName + " " + e.LastName });
            ViewBag.Employee = new SelectList(a, "EmployeeId", "FullName");
            var add = context.EmployeeAttendanceMasterDetails.Where(x => x.AttendanceDate == DateTime.Today)
            .Where(x => x.AttendanceDate == DateTime.Today).GroupBy(x => new { x.EmployeeId, x.AttendanceDate }).Select(g => g.FirstOrDefault()).ToList();
            if (add.Count != 0)
            {
                var result = (from one in context.EmployeeAttendanceMasterDetails.Where(x => x.AttendanceDate == DateTime.Today)
                              .GroupBy(x => new { x.EmployeeId, x.AttendanceDate }).Select(g => g.FirstOrDefault())
                              join two in context.EmployeeDetails
                                            on one.EmployeeId equals two.EmployeeId

                              select new AttendanceMasterVM
                              {
                                  EmployeeName = two.FirstName + " " + two.LastName,
                                  EmployeeId = two.EmployeeId,
                                  AttendanceId = one.AttendanceId,
                                  Check_In = one.Check_In,
                                  Check_Out = one.Check_Out,
                                  AttendanceStatus = one.AttendanceStatus,
                                  Comments = one.Comments,
                                  IsActive = one.IsActive,
                                  TotalHours = one.TotalHours,
                                  AttendanceDate = DateTime.Now,
                              }).ToList();
                return View(result);
            }
            else
            {
                foreach (var item in listitems)
                {
                    EmployeeAttendanceMasterDetail detail = new EmployeeAttendanceMasterDetail();
                    var CompanyId = common.CompanyId();
                    detail.CompanyId = Guid.Parse(CompanyId);
                    detail.EmployeeId = item.EmployeeId;
                    detail.EmployeeName = item.FirstName + " " + item.LastName;
                    detail.Check_In = TimeSpan.Zero;
                    detail.Check_Out = TimeSpan.Zero;
                    detail.AttendanceStatus = "";
                    detail.Comments = "";
                    detail.IsActive = true;
                    detail.AttendanceDate = DateTime.Today;
                    detail.TotalHours = 8;
                    detail.CreatedDate = DateTime.Now;
                    detail.UpdatedDate = DateTime.Now;
                    detail.CreatedBy = 0;
                    detail.UpdatedBy = 0;
                    var data = context.EmployeeAttendanceMasterDetails.Add(detail);
                    context.SaveChanges();
                }
                return View(new AttendanceMasterVM());
            }
        }

        [HttpGet]
        public ActionResult saveAttendance(string attendanceId)
        {
            var a = Convert.ToInt32(attendanceId);
            var data = context.EmployeeAttendanceMasterDetails.Where(x => x.AttendanceId == a).FirstOrDefault();
            return View(data);
        }

        [HttpPost]
        public ActionResult saveAttendance(string attendanceId, string employeeId, string employeeName, string checkIn, string checkOut, string status, string comments)
        {
            try
            {
                var a = Convert.ToInt32(attendanceId);
                var b = Convert.ToInt32(employeeId);
                var attendance = context.EmployeeAttendanceMasterDetails.FirstOrDefault(x => x.AttendanceId == a && x.EmployeeId == b);
                if (attendance != null)
                {
                    //var attendance = new EmployeeAttendanceMasterDetail();
                    var CompanyId = common.CompanyId();
                    attendance.CompanyId = Guid.Parse(CompanyId);

                    if (checkIn != null)
                    {
                        attendance.Check_In = TimeSpan.Parse(checkIn);
                    }

                    if (checkOut != null)
                    {
                        attendance.Check_Out = TimeSpan.Parse(checkOut);
                    }

                    attendance.AttendanceStatus = status ?? "";
                    attendance.Comments = comments ?? "";

                    attendance.AttendanceDate = DateTime.Today;
                    attendance.EmployeeName = employeeName;
                    attendance.IsActive = true;
                    attendance.TotalHours = 8;
                    attendance.CreatedDate = DateTime.Now;
                    attendance.UpdatedDate = DateTime.Now;
                    attendance.CreatedBy = 0;
                    attendance.UpdatedBy = 0;

                    context.EmployeeAttendanceMasterDetails.AddOrUpdate(attendance);
                    context.SaveChanges();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult GetAttendanceRecords(DateTime date)
        {
            var attendanceRecords = context.EmployeeAttendanceMasterDetails
                .Where(x => x.AttendanceDate == date).ToList();
            return Json(attendanceRecords, JsonRequestBehavior.AllowGet);
        }
      
        [HttpGet]
        public ActionResult EmployeeAttendanceEdit(DateTime date)
        {
            var attendanceDate = date;
            var attendanceRecords = context.EmployeeAttendanceMasterDetails.Where(x => x.AttendanceDate == date).ToList();
            ViewBag.AttendanceDate = attendanceDate;
            return View(attendanceRecords);
        }

        [HttpPost]
        public ActionResult UpdateAttendance(string attendanceId, string checkIn, string checkOut, string status, string comments)
        {
            try
            {
                var a = Convert.ToInt32(attendanceId);
                var attendance = context.EmployeeAttendanceMasterDetails.FirstOrDefault(x => x.AttendanceId == a);
                if (attendance != null)
                {
                    if (checkIn != null)
                    {
                        attendance.Check_In = TimeSpan.Parse(checkIn);
                    }

                    if (checkOut != null)
                    {
                        attendance.Check_Out = TimeSpan.Parse(checkOut);
                    }
                    
                    attendance.AttendanceStatus = status ?? "";
                    attendance.Comments = comments ?? "";
                    attendance.UpdatedDate = DateTime.Now;
                    attendance.UpdatedBy = 0;
                
                    context.SaveChanges();

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, error = "Attendance not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        public ActionResult AttendanceReport()
        {
            return View();
        }

    }
}
    







