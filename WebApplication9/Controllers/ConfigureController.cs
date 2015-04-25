using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication9.Data;

namespace WebApplication9.Controllers
{
    [Authorize(Roles = "Admin, Purchasing Department")]
    public class ConfigureController : Controller
    {
        Requisition1Entities db = new Requisition1Entities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateAccount()
        {
            GetAccountData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAccount(Account model)
        {
            if (ModelState.IsValid)
            {
                db.Accounts.Add(model);
                db.SaveChanges();
                return RedirectToAction("Accounts");
            }
            GetAccountData();
            return View(model);
        }

        public ActionResult EditAccount(int id)
        {
            var accountToEdit = db.Accounts.Where(x => x.Id == id).First();
            GetAccountData();
            return View(accountToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccount(Account account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Accounts");
            }
            GetAccountData();
            return View(account);
        }

        public ActionResult Accounts()
        {
            var accounts = db.Accounts.ToList();
            return View(accounts);
        }

        public ActionResult CreateDepartment()
        {
            ViewBag.Divisions = db.Divisions.ToList();
            ViewBag.Users = db.Users.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDepartment(Department model)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(model);
                db.SaveChanges();
                return RedirectToAction("Departments");
            }
            ViewBag.Users = db.Users.ToList();
            ViewBag.Divisions = db.Divisions.ToList();
            return View(model);
        }

        public ActionResult EditDepartment(int id)
        {
            var departmentToEdit = db.Departments.Where(x => x.Id == id).First();
            ViewBag.Divisions = db.Divisions.ToList();
            ViewBag.Users = db.Users.ToList();
            return View(departmentToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Departments");
            }
            ViewBag.Divisions = db.Divisions.ToList();
            ViewBag.Users = db.Users.ToList();
            return View(department);
        }

        public ActionResult Departments()
        {
            var departments = db.Departments.ToList();
            return View(departments);
        }

        public ActionResult CreateFund()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateFund(Fund model)
        {
            if (ModelState.IsValid)
            {
                db.Funds.Add(model);
                db.SaveChanges();
                return RedirectToAction("Funds");
            }
            return View(model);
        }

        public ActionResult EditFund(int id)
        {
            var fundToEdit = db.Funds.Where(x => x.Id == id).First();
            return View(fundToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFund(Fund fund)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fund).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Funds");
            }
            return View(fund);
        }

        public ActionResult Funds()
        {
            var funds = db.Funds.ToList();
            return View(funds);
        }

        public ActionResult CreateDivision()
        {
            GetDepartmentsViewBag();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDivision(Division model)
        {
            if (ModelState.IsValid)
            {
                db.Divisions.Add(model);
                db.SaveChanges();
                return RedirectToAction("Divisions");
            }
            GetDepartmentsViewBag();
            return View(model);
        }

        public ActionResult EditDivision(int id)
        {
            var divisionToEdit = db.Divisions.Where(x => x.Id == id).First();
            return View(divisionToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDivision(Division division)
        {
            if (ModelState.IsValid)
            {
                db.Entry(division).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Divisions");
            }
            return View(division);
        }

        public ActionResult Divisions()
        {
            var divisions = db.Divisions.ToList();
            return View(divisions);
        }

        public JsonResult GetDivisionDepartments(int id)
        {
            var divisionDepartments = db.Departments.Where(x => x.DivisionId == id).ToList();
            var result = Json(divisionDepartments);

            return Json(new SelectList(divisionDepartments, "Id", "Name"));
        }

        private void GetAccountData()
        {
            GetDepartmentsViewBag();

            var funds = db.Funds.ToList();
            var divisions = db.Divisions.ToList();
            ViewBag.Funds = funds;
            ViewBag.Divisions = divisions;
        }

        private void GetDepartmentsViewBag()
        {
            var departments = db.Departments.ToList();
            ViewBag.Departments = departments;
        }

        public ActionResult CreateItemCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateItemCategory(ItemCategory model)
        {
            if (ModelState.IsValid)
            {
                db.ItemCategories.Add(model);
                db.SaveChanges();
                return RedirectToAction("ItemCategories");
            }
            return View(model);
        }

        public ActionResult EditItemCategory(int id)
        {
            var itemCategoryToEdit = db.ItemCategories.Where(x => x.ItemCategoryId == id).First();
            return View(itemCategoryToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditItemCategory(ItemCategory itemCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(itemCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ItemCategories");
            }
            return View(itemCategory);
        }

        public ActionResult ItemCategories()
        {
            var itemCategories = db.ItemCategories.ToList();
            return View(itemCategories);
        }

	}
}