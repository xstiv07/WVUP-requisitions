using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebApplication9.Data;
using WebApplication9.Helpers;
using PagedList;
using WebApplication9.Models;
using System.Collections.Generic;
using WebApplication9.Data.Helpers;
using System.Web.Script.Serialization;

namespace WebApplication9.Controllers
{
    [Authorize]
    public class RequisitionController : Controller
    {
        Requisition1Entities db = new Requisition1Entities();
        static List<ItemCategory> iCategories;
        static long _requis;

        const int minExplanationLength = 10;
        const int pageSize = 3;
        const int approvalLimit = 3000;
        const string emailFrom = "xstiv07@gmail.com"; //purchasing email address goes here

        MyUserManager _userManager;

        public RequisitionController(){}

        public RequisitionController(MyUserManager userManager, MyRoleManager roleManager)
        {
            UserManager = userManager;
        }

        public MyUserManager UserManager
        {
            get{ return _userManager ?? HttpContext.GetOwinContext().GetUserManager<MyUserManager>(); }
            private set{ _userManager = value; }
        }

        public async Task<ActionResult> Create()
        {
            var user = await GetCurrentUser();
            var req = new Requisition();

            ViewBag.Divisions = db.Divisions.Where(x => x.Status == ConfigureStatusEnum.Active).ToList();

            var itemCategories = db.ItemCategories.Where(x => x.Status == ConfigureStatusEnum.Active).ToList();
            ViewBag.ItemCategories = itemCategories;
            iCategories = itemCategories;

            return View(req);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Requisition requisition)
        {
            if (ModelState.IsValid && requisition.Items.Count > 0)
            {
                var user = await GetCurrentUser();
                requisition.CFO_approval = false;

                requisition.User_Id = user.Id;
                requisition.Requisitioned_By = user.First_Name + " " + user.Last_Name;
                requisition.Status = StatusEnum.Submitted;
                requisition.Date_Created = DateTime.Now;

                foreach (var item in requisition.Items)
                {
                    if (item.Price * item.Quantity > approvalLimit)
                        requisition.CFO_approval = true;
                }

                db.Requisitions.Add(requisition);
                db.SaveChanges();
                
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];

                    // exists to tie together item and its files - based on the unique field in the db that is populated by the guid in the view
                    string unq = Request.Files.AllKeys[i].ToString();

                    if (file != null && file.ContentLength > 0)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\" + User.Identity.Name.ToString(), Server.MapPath(@"\")));
                        string pathString = Path.Combine(originalDirectory.ToString(), requisition.RequisitionId.ToString());
                        var fileName1 = Path.GetFileName(file.FileName);
                        bool isExists = Directory.Exists(pathString);

                        if (!isExists)
                            Directory.CreateDirectory(pathString);
                        var path = string.Format("{0}\\{1}", pathString, file.FileName);
                        file.SaveAs(path);

                        db.Files.Add(new WebApplication9.Data.File()
                        {
                            //searhing for the first occurence of unique Id - associating file with an item based on that
                            ItemId = db.Items.Where(x => x.Unique == unq).Select(x => x.ItemId).First(),
                            FileName = fileName1,
                            FileLink = "~/Uploads/" + User.Identity.Name.ToString() + "/" + requisition.RequisitionId.ToString() + "/" + fileName1
                        });
                    }
                }
                db.SaveChanges();

                _requis = requisition.RequisitionId;

                SendEmailNotification(requisition, user);

                return RedirectToAction("Display");
            }

            ViewBag.Divisions = db.Divisions.ToList();
            ViewBag.ItemCategories = iCategories;
            return View(requisition);
        }

        private void SendEmailNotification(Requisition requisition, MyUser user)
        {
            var currentRequisitionDepartment = db.Departments.Where(x => x.Id == requisition.DepartmentId).First();
            var departmentManager = UserManager.FindById((long)currentRequisitionDepartment.ManagerId);
            string targetEmail = departmentManager.Email;

            var email = new NewRequisitionEmail
            {
                To = targetEmail,
                From = emailFrom, //purchasing department email goes here
                Recepient = departmentManager.First_Name,
                UserName = user.First_Name + " " + user.Last_Name
            };

            email.Send();
        }

        public ActionResult ItemEntryRow()
        {
            ViewBag.ItemCategories = iCategories;
            return PartialView("_ItemEntryEditor", new Item());
        }

        public ActionResult Display()
        {
            var requisitionToDisplay = db.Requisitions.Where(x => x.RequisitionId == _requis).First();
            return View(requisitionToDisplay);
        }

        public PartialViewResult Attachments(int id)
        {
            var files = db.Files.Where(x => x.ItemId == id).ToList();
            return PartialView(files);
        }

        public FileResult Download(string link, string name)
        {
            return File(link, MediaTypeNames.Application.Octet, name);
        }

        public async Task<ActionResult> Print(int id)
        {
            var user = await GetCurrentUser();
            Requisition reqToPrint = null;

            //if it is a regualr user or a department manager - limit the query to navigate only user's own requisitions
            if (user.Roles.Count != 0)
                reqToPrint = db.Requisitions.Where(x => x.RequisitionId == id).FirstOrDefault();
            else
                reqToPrint = db.Requisitions.Where(x => x.RequisitionId == id && x.User_Id == user.Id).FirstOrDefault();

            if (reqToPrint == null)
                return HttpNotFound();

            return View(reqToPrint);
        }

        public async Task<ActionResult> Void(int id)
        {
            var user = await GetCurrentUser();
            Requisition reqToVoid = null;

            //if it is a regular user - limit the query to navigate only user's own requisitions
            if (user.Roles.Count != 0)
                reqToVoid = db.Requisitions.Where(x => x.RequisitionId == id).FirstOrDefault();
            else
                reqToVoid = db.Requisitions.Where(x => x.RequisitionId == id && x.User_Id == user.Id).FirstOrDefault();

            if (reqToVoid != null)
            {
                reqToVoid.Status = StatusEnum.Void;
                db.SaveChanges();
            }
            else
                return HttpNotFound();

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadError(){
            return View();
        }

        [Authorize(Roles = "Admin, Chief Financial Officer")]
        public async Task<ActionResult> ReviewCFO(int? page)
        {
            var user = await GetCurrentUser();
            int pageNumber = (page ?? 1);

            var reqsToReview = db.Requisitions.Where(x => x.CFO_approval == true && x.Status == StatusEnum.Submitted).OrderByDescending(x => x.Date_Created).ToPagedList(pageNumber, pageSize);

            if (Request.IsAjaxRequest())
            {
                return PartialView("Review", reqsToReview);
            }

            return View(reqsToReview);
        }

        [HttpPost]
        public async Task<JsonResult> ReviewCFO(int id, string decision, string explanation)
        {
            var currentReq = db.Requisitions.Where(x => x.RequisitionId == id).First();
            var user = await GetCurrentUser();

            if (decision == "approve")
                currentReq.Status = StatusEnum.Aproved_CFO;
            else
                currentReq.Status = StatusEnum.Rejected_CFO;

            currentReq.CFO_Decision_By = user.First_Name + " " + user.Last_Name;
            currentReq.CFO_Explanation = explanation;
            db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin, Department Manager")]
        public async Task<ActionResult> Review(int? page)
        {
            var user = await GetCurrentUser();
            int pageNumber = (page ?? 1);

            var reqsToReview = db.Requisitions.Where(x => x.Account.Department.ManagerId == user.Id && ((x.Status == StatusEnum.Aproved_CFO) || x.CFO_approval == false && x.Status == StatusEnum.Submitted)).OrderByDescending(x => x.Date_Created).ToPagedList(pageNumber, pageSize);

            if (Request.IsAjaxRequest())
            {
                return PartialView("Review", reqsToReview);
            }

            return View(reqsToReview);
        }

        [HttpPost]
        public async Task <ActionResult> Review(int id, string decision, string explanation)
        {
            if (explanation.Length >= minExplanationLength){
                var currentReq = db.Requisitions.Find(id);
                var user = await GetCurrentUser();

                if (decision == "approve")
                    currentReq.Status = StatusEnum.Approved;
                else
                    currentReq.Status = StatusEnum.Rejected;

                currentReq.Explanation = explanation;
                currentReq.Decision_By = user.First_Name + " " + user.Last_Name;
                
                db.SaveChanges();
                return Json(String.Empty, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public async Task<ActionResult> MyRequisitions(int? page)
        {
            int pageNumber = (page ?? 1);

            var user = await GetCurrentUser();

            var myRequisitions = db.Requisitions.Where(x => x.User_Id == user.Id).OrderByDescending(x => x.Date_Created).ToPagedList(pageNumber, pageSize);

            if (Request.IsAjaxRequest())
                return PartialView("MyRequisitions", myRequisitions);

            return View(myRequisitions);
        }

        [Authorize(Roles = "Admin, Purchasing Department")]
        public ActionResult Manage(int? page)
        {
            int pageNumber = (page ?? 1);

            var reqsToManage = db.Requisitions.Where(x => x.Status == StatusEnum.Approved).OrderByDescending(x => x.Date_Created).ToPagedList(pageNumber, pageSize);

            if (Request.IsAjaxRequest())
                return PartialView("Manage", reqsToManage);

            return View(reqsToManage);
        }

        [HttpPost]
        public ActionResult Manage(ReqAdd extra, int id)
        {
            if (ModelState.IsValid)
            {
                var currentRequisition = db.Requisitions.Find(id);
                currentRequisition.Status = StatusEnum.Ordered; //changed status complete to status ordered
                extra.RequisitionId = id;
                db.ReqAdds.Add(extra);
                db.SaveChanges();
                return Json(String.Empty, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        [Authorize(Roles = "Admin, Purchasing Department")]
        public ActionResult Ordered(int? page)
        {
            int pageNumber = (page ?? 1);
            var orderedRequisitions = db.Requisitions.Where(x => x.Status == StatusEnum.Ordered).OrderByDescending(x => x.Date_Created).ToPagedList(pageNumber, pageSize);

            if (Request.IsAjaxRequest())
                return PartialView(orderedRequisitions);

            return View(orderedRequisitions);
        }

        [Authorize(Roles = "Admin, Purchasing Department")]
        public JsonResult MarkComplete(int id)
        {
            var requisitionMarkComplete = db.Requisitions.Find(id);
            requisitionMarkComplete.Status = StatusEnum.Complete;
            db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin, Purchasing Department")]
        public ActionResult Completed(int? page)
        {
            int pageNumber = (page ?? 1);
            var completedRequistioins = db.Requisitions.Where(x => x.Status == StatusEnum.Complete).OrderByDescending(x => x.Date_Created).ToPagedList(pageNumber, pageSize);
            
            if (Request.IsAjaxRequest())
                return PartialView(completedRequistioins);

            return View(completedRequistioins);
        }

        [Authorize(Roles = "Admin, Purchasing Department")]
        public ActionResult Search()
        {
            if (Request.IsAjaxRequest())
                return HttpNotFound();
            ViewBag.Departments = db.Departments.ToList();
            ViewBag.Accounts = db.Accounts.ToList();
            ViewBag.Divisions = db.Divisions.ToList();
            ViewBag.Funds = db.Funds.ToList();
            return View();
        }

        public ActionResult _SearchResult(SearchViewModel model, int? page)
        {

            int pageNumber = (page ?? 1);
            IQueryable<Requisition> results = null;

            if (model.AccountNumber > 0
                || model.AccountName > 0
                || model.FundNumber > 0
                || model.DivisionName > 0
                || model.RequisitionID > 0
                || model.DepartmentId > 0
                || model.Status > 0
                || model.TotalFrom > 0
                || model.TotalUpTo > 0
                || model.Date_Submitted_From != DateTime.MinValue
                || model.Date_Submitted_To != DateTime.MinValue
                || model.RequisitionedBy != null)
            {

                results = db.Requisitions.Where(x => x.RequisitionId > 0);
                results = SearchQueries.ProcessSearchInput(model, results);

                ViewBag.total = results.Count();

                return PartialView(results.ToPagedList(pageNumber, pageSize));
            }

            return PartialView(results.ToPagedList(pageNumber, pageSize));
        }

        public JsonResult GetActiveDepartments(int id)
        {
            var departments = db.Departments.Where(x => x.DivisionId == id && x.Status == ConfigureStatusEnum.Active).ToList();
            var result = Json(departments);

            return Json(new SelectList(departments, "Id", "Name"));
        }


        public JsonResult GetActiveAccounts(int id)
        {
            var departmentAccounts = db.Accounts.Where(x => x.DepartmentId == id && x.Status == ConfigureStatusEnum.Active).ToList();
            var result = Json(departmentAccounts);

            return Json(new SelectList(departmentAccounts, "Id", "Name"));
        }


        private async Task<MyUser> GetCurrentUser()
        {
            long currentUserId = long.Parse(User.Identity.GetUserId());
            var user = await UserManager.FindByIdAsync(currentUserId);
            return user;
        }

    }
}