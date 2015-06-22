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

                if (model.Status != oldAccount.Status)
                {
                    oldAccount.Status = model.Status;
                    repo.Save();
                }
                else
                {
                    oldAccount.Status = ConfigureStatusEnum.Inactive;
                    repo.AddAccount(model);
                }

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

                if (model.Status != oldDepartment.Status) // if the status has changed - set the status of a department to the selected one
                {
                    oldDepartment.Status = model.Status;
                    repo.Save();
                }
                else // if we are editing any department info other than the status
                //set the current department status to incative and create an active department with new parameters
                {
                    oldDepartment.Status = ConfigureStatusEnum.Inactive;
                    repo.AddDepartment(model);
                }

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
                repo.AddFunds(model);
                return RedirectToAction("Funds");
            }
            return View(model);
        }

        public ActionResult EditFund(int id)
        {
            var fundToEdit = repo.GetFund(id);
            return View(fundToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditFund(Fund model)
        {
            if (ModelState.IsValid)
            {
                var oldFund = repo.GetFund(model.Id);

                if (model.Status != oldFund.Status)
                {
                    oldFund.Status = model.Status;
                    repo.Save();
                }
                else
                {
                    oldFund.Status = ConfigureStatusEnum.Inactive;
                    repo.AddFunds(model);
                }

                return RedirectToAction("Funds");
            }
            return View(model);
        }

        public ActionResult Funds()
        {
            var funds = repo.getFunds();
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
                repo.AddDivision(model);
                
                return RedirectToAction("Divisions");
            }
            GetDepartmentsViewBag();
            return View(model);
        }

        public ActionResult EditDivision(int id)
        {
            var divisionToEdit = repo.GetDivision(id);

            return View(divisionToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDivision(Division model)
        {

            if (ModelState.IsValid)
            {
                var oldDivision = repo.GetDivision(model.Id);

                if (model.Status != oldDivision.Status)
                {
                    oldDivision.Status = model.Status;
                    repo.Save();
                }
                else
                {
                    oldDivision.Status = ConfigureStatusEnum.Inactive;
                    repo.AddDivision(model);
                }

                return RedirectToAction("Divisions");
            }
            return View(model);
        }

        public ActionResult Divisions()
        {
            var divisions = repo.GetDivisions();
            return View(divisions);
        }

        public JsonResult GetDivisionDepartments(int id)
        {
            var divisionDepartments = repo.GetActiveDivsionDepartments(id);
            var result = Json(divisionDepartments);

            return Json(new SelectList(divisionDepartments, "Id", "Name"));
        }

        private void GetAccountData()
        {
            GetActiveDepartmentsViewBag();

            var funds = repo.GetActiveFunds();
            var divisions = repo.GetActiveDivisions();
            ViewBag.Funds = funds;
            ViewBag.Divisions = divisions;
        }

        private void GetDepartmentsViewBag()
        {
            var departments = repo.GetDepartments();
            ViewBag.Departments = departments;
        }

        private void GetActiveDepartmentsViewBag()
        {
            var departments = repo.GetActiveDepartments();
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
                repo.AddItemCategory(model);
                
                return RedirectToAction("ItemCategories");
            }
            return View(model);
        }

        public ActionResult EditItemCategory(int id)
        {
            var itemCategoryToEdit = repo.GetItemCategory(id);
            return View(itemCategoryToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditItemCategory(ItemCategory model)
        {
            if (ModelState.IsValid)
            {
                var oldItemCategory = repo.GetItemCategory(model.ItemCategoryId);

                if (model.Status != oldItemCategory.Status)
                {
                    oldItemCategory.Status = model.Status;
                    repo.Save();
                }
                else
                {
                    oldItemCategory.Status = ConfigureStatusEnum.Inactive;
                    repo.AddItemCategory(model);
                }

                return RedirectToAction("ItemCategories");
            }
            return View(model);
        }

        public ActionResult ItemCategories()
        {
            var itemCategories = repo.GetItemCategories();
            return View(itemCategories);
        }

	}
}