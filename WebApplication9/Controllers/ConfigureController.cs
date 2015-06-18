using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication9.Data;
using WebApplication9.Data.Helpers;
using WebApplication9.Helpers;
using WebApplication9.Repository;

namespace WebApplication9.Controllers
{
    [Authorize(Roles = "Admin, Purchasing Department")]
    public class ConfigureController : Controller
    {
        Requisition1Entities db = new Requisition1Entities(); //remove soon
        private IRepository repo;

        public ConfigureController(IRepository repo)
        {
            this.repo = repo;
        }

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
                repo.AddAccount(model);
                return RedirectToAction("Accounts");
            }
            GetAccountData();
            return View(model);
        }

        public ActionResult EditAccount(int id)
        {
            var accountToEdit = repo.GetAccount(id);
            GetAccountData();
            return View(accountToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccount(Account model)
        {
            if (ModelState.IsValid)
            {
                var oldAccount = repo.GetAccount(model.Id);
                oldAccount.Status = ConfigureStatusEnum.Inactive;

                repo.AddAccount(model);
                
                return RedirectToAction("Accounts");
            }
            GetAccountData();
            return View(model);
        }

        public ActionResult Accounts()
        {
            var accounts = repo.GetAccounts();
            return View(accounts);
        }

        public ActionResult CreateDepartment()
        {
            ViewBag.Divisions = repo.GetDivisions();
            ViewBag.Users = repo.GetUsers();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDepartment(Department model)
        {
            if (ModelState.IsValid)
            {
                repo.AddDepartment(model);
                
                return RedirectToAction("Departments");
            }
            ViewBag.Users = repo.GetUsers();
            ViewBag.Divisions = repo.GetDivisions();
            return View(model);
        }

        public ActionResult EditDepartment(int id)
        {
            var departmentToEdit = repo.GetDepartment(id);
            ViewBag.Divisions = repo.GetDivisions();
            ViewBag.Users = repo.GetUsers();
            return View(departmentToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDepartment(Department model)
        {
            if (ModelState.IsValid)
            {
                //find an existing department and set its status to inactive
                var oldDepartment = repo.GetDepartment(model.Id);
                oldDepartment.Status = ConfigureStatusEnum.Inactive;

                repo.AddDepartment(model);
               
                return RedirectToAction("Departments");
            }
            ViewBag.Divisions = repo.GetDivisions();
            ViewBag.Users = repo.GetUsers();
            return View(model);
        }

        public ActionResult Departments()
        {
            var departments = repo.GetDepartments();
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
        public ActionResult EditFund(Fund model)
        {
            if (ModelState.IsValid)
            {
                //find an existing department and set its status to inactive
                var oldFund = db.Funds.Find(model.Id);
                oldFund.Status = ConfigureStatusEnum.Inactive;

                db.Funds.Add(model);

                db.SaveChanges();
                return RedirectToAction("Funds");
            }
            return View(model);
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
        public ActionResult EditDivision(Division model)
        {
            if (ModelState.IsValid)
            {
                //find an existing department and set its status to inactive
                var oldDivision = db.Divisions.Find(model.Id);
                oldDivision.Status = ConfigureStatusEnum.Inactive;

                db.Divisions.Add(model); //add a new department to a database based on the model user entered

                db.SaveChanges();
                return RedirectToAction("Divisions");
            }
            return View(model);
        }

        public ActionResult Divisions()
        {
            var divisions = db.Divisions.ToList();
            return View(divisions);
        }

        public JsonResult GetDivisionDepartments(int id)
        {
            var divisionDepartments = db.Departments.Where(x => x.DivisionId == id && x.Status == ConfigureStatusEnum.Active).ToList();
            var result = Json(divisionDepartments);

            return Json(new SelectList(divisionDepartments, "Id", "Name"));
        }

        private void GetAccountData()
        {
            GetActiveDepartmentsViewBag();

            var funds = db.Funds.Where(x => x.Status == ConfigureStatusEnum.Active).ToList();
            var divisions = db.Divisions.Where(x => x.Status == ConfigureStatusEnum.Active).ToList();
            ViewBag.Funds = funds;
            ViewBag.Divisions = divisions;
        }

        private void GetDepartmentsViewBag()
        {
            var departments = db.Departments.ToList();
            ViewBag.Departments = departments;
        }

        private void GetActiveDepartmentsViewBag()
        {
            var departments = db.Departments.Where(x => x.Status == ConfigureStatusEnum.Active).ToList();
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
        public ActionResult EditItemCategory(ItemCategory model)
        {
            if (ModelState.IsValid)
            {
                //find an existing department and set its status to inactive
                var oldItemCategory = db.ItemCategories.Find(model.ItemCategoryId);
                oldItemCategory.Status = ConfigureStatusEnum.Inactive;

                db.ItemCategories.Add(model); //add a new department to a database based on the model user entered

                db.SaveChanges();
                return RedirectToAction("ItemCategories");
            }
            return View(model);
        }

        public ActionResult ItemCategories()
        {
            var itemCategories = db.ItemCategories.ToList();
            return View(itemCategories);
        }

	}
}