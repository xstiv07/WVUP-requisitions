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
using WebApplication9.Repository;

namespace WebApplication9.Controllers
{
    [Authorize]
    public class RequisitionController : Controller
    {
        private IRepository repo; 

        private static List<ItemCategory> iCategories;
        private static long _requis;

        private const int minExplanationLength = 10; // for dm and cfo to fill explanation
        private const int pageSize = 3; // controls pagination - items per page
        private const int approvalLimit = 3000;

        private MyUserManager _userManager;

        public RequisitionController(IRepository repo)
        {
            this.repo = repo;
        }

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

            ViewBag.Divisions = repo.GetDivisions();
            ViewBag.ItemCategories = repo.GetItemCategories();

            iCategories = ViewBag.ItemCategories;

            return View(req);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Requisition requisition)
        {
            if (ModelState.IsValid && requisition.Items.Count > 0)
            {
                var user = await GetCurrentUser();
                TimeZoneInfo easternTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                TimeSpan offset = easternTimeZone.GetUtcOffset(DateTime.Now); //used for appharbor to fix the date offset

                requisition.CFO_approval = false; // trigger for CFO approval

                requisition.User_Id = user.Id;
                requisition.Requisitioned_By = user.First_Name + " " + user.Last_Name;
                requisition.Status = StatusEnum.Submitted;
                requisition.Date_Created = DateTime.Now.Add(offset);

                foreach (var item in requisition.Items) //checking every item in the req to see if any are over the limit
                {
                    if (item.Price * item.Quantity > approvalLimit)
                        requisition.CFO_approval = true;
                }

                repo.AddRequisition(requisition);
                
                for (int i = 0; i < Request.Files.Count; i++) // getting all files from the request and saving them to the db
                {
                    HttpPostedFileBase file = Request.Files[i];

                    // exists to tie together item and its files - based on the unique field in the db that is populated by the guid in the view
                    string unq = Request.Files.AllKeys[i].ToString();

                    if (file != null && file.ContentLength > 0)
                    {
                        var originalDirectory = new DirectoryInfo(string.Format("{0}Uploads\\" + user.First_Name + "_" + user.Last_Name, Server.MapPath(@"\")));
                        string pathString = Path.Combine(originalDirectory.ToString(), requisition.RequisitionId.ToString());
                        var fileName1 = Path.GetFileName(file.FileName);
                        bool isExists = Directory.Exists(pathString);

                        if (!isExists)
                            Directory.CreateDirectory(pathString);
                        var path = string.Format("{0}\\{1}", pathString, file.FileName);
                        file.SaveAs(path);

                        repo.AddFile(unq, fileName1, requisition, user);
                    }
                }
                repo.Save();

                _requis = requisition.RequisitionId;

                SendEmailNotification(requisition, user); //sends email to the DM that requisition from user x was submitted

                return RedirectToAction("Display");
            }
            //will fire if for some reason javascript validation didn't work and user was able to submitt invalid form to the server
            ViewBag.Divisions = repo.GetDivisions();
            ViewBag.ItemCategories = repo.GetItemCategories();
            return View(requisition);
        }

        private void SendEmailNotification(Requisition requisition, MyUser user)
        {
            var currentRequisitionDepartment = repo.GetCurrentRequisitionDepartment(requisition);
            var departmentManager = UserManager.FindById((long)currentRequisitionDepartment.ManagerId);
            string targetEmail = departmentManager.Email;

            var email = new NewRequisitionEmail
            {
                To = targetEmail,
                Recepient = departmentManager.First_Name,
                UserName = user.First_Name + " " + user.Last_Name
            };

            email.Send();
        }

        //calls a partial item view that is triggered when user click add another item in the create view
        public ActionResult ItemEntryRow()
        {
            ViewBag.ItemCategories = iCategories;
            return PartialView("_ItemEntryEditor", new Item());
        }

        //displays a requisition when user submits a req
        public ActionResult Display()
        {
            var requisitionToDisplay = repo.GetRequisition(_requis);
            return View(requisitionToDisplay);
        }

        //populates req files and calls a file partial view with this data
        public PartialViewResult Attachments(int id)
        {
            List<WebApplication9.Data.File> files = repo.GetFiles(id);
            return PartialView(files);
        }

        public FileResult Download(string link, string name)
        {
            return File(link, MediaTypeNames.Application.Octet, name);
        }

        //controls print action
        public async Task<ActionResult> Print(int id)
        {
            var user = await GetCurrentUser();
            Requisition reqToPrint = null;

            //if it is a regualr user or a department manager - limit the query to navigate only user's own requisitions
            //simple users shouldn't be able to navigate to someone else's print view via the url
            if (user.Roles.Count != 0)
                reqToPrint = repo.GetRequisition(id);
            else
                reqToPrint = repo.GetCurrentUserRequistion(id, user);

            if (reqToPrint == null)
                return HttpNotFound();

            return View(reqToPrint);
        }

        //controls void req action, works similar to the print action
        public async Task<ActionResult> Void(int id)
        {
            var user = await GetCurrentUser();
            Requisition reqToVoid = null;

            //if it is a regular user - limit the query to navigate only user's own requisitions
            if (user.Roles.Count != 0)
                reqToVoid = repo.GetRequisition(id);
            else
                reqToVoid = repo.GetCurrentUserRequistion(id, user);

            if (reqToVoid != null)
            {
                reqToVoid.Status = StatusEnum.Void;
                repo.Save();
            }
            else
                return HttpNotFound();

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadError(){
            return View();
        }

        //controls CFO review action
        [Authorize(Roles = "Admin, Chief Financial Officer")]
        public async Task<ActionResult> ReviewCFO(int? page)
        {
            var user = await GetCurrentUser();
            int pageNumber = (page ?? 1);

            var reqsToReview = repo.GetCFORequisitions(pageNumber, pageSize);

            if (Request.IsAjaxRequest())
            {
                return PartialView("Review", reqsToReview);
            }

            return View(reqsToReview);
        }

        [HttpPost]
        public async Task<JsonResult> ReviewCFO(int id, string decision, string explanation)
        {
            var currentReq = repo.GetRequisition(id);
            var user = await GetCurrentUser();

            if (decision == "approve")
                currentReq.Status = StatusEnum.Aproved_CFO;
            else
                currentReq.Status = StatusEnum.Rejected_CFO;

            currentReq.CFO_Decision_By = user.First_Name + " " + user.Last_Name;
            currentReq.CFO_Explanation = explanation;
            repo.Save();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        //controls DM review action
        [Authorize(Roles = "Admin, Department Manager")]
        public async Task<ActionResult> Review(int? page)
        {
            var user = await GetCurrentUser();
            int pageNumber = (page ?? 1);

            var reqsToReview = repo.GetRequisitionsToReview(user, pageNumber, pageSize);

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
                var currentReq = repo.GetRequisition(id);
                var user = await GetCurrentUser();

                if (decision == "approve")
                    currentReq.Status = StatusEnum.Approved;
                else
                    currentReq.Status = StatusEnum.Rejected;

                currentReq.Explanation = explanation;
                currentReq.Decision_By = user.First_Name + " " + user.Last_Name;
                
                repo.Save();
                return Json(String.Empty, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        public async Task<ActionResult> MyRequisitions(int? page)
        {
            int pageNumber = (page ?? 1);

            var user = await GetCurrentUser();

            var myRequisitions = repo.GetCurrentUserRequisitions(user, pageNumber, pageSize);

            if (Request.IsAjaxRequest())
                return PartialView("MyRequisitions", myRequisitions);

            return View(myRequisitions);
        }

        [Authorize(Roles = "Admin, Purchasing Department")]
        public ActionResult Manage(int? page)
        {
            int pageNumber = (page ?? 1);

            var reqsToManage = repo.GetApprovedRequisitions(pageNumber ,pageSize);

            if (Request.IsAjaxRequest())
                return PartialView("Manage", reqsToManage);

            return View(reqsToManage);
        }

        [HttpPost]
        public ActionResult Manage(ReqAdd extra, int id)
        {
            if (ModelState.IsValid)
            {
                var currentRequisition = repo.GetRequisition(id);
                currentRequisition.Status = StatusEnum.Ordered; //changed status complete to status ordered
                extra.RequisitionId = id;
                repo.AddExtraInfo(extra);
                return Json(String.Empty, JsonRequestBehavior.AllowGet);
            }
            return View();
        }

        [Authorize(Roles = "Admin, Purchasing Department")]
        public ActionResult Ordered(int? page)
        {
            int pageNumber = (page ?? 1);
            var orderedRequisitions = repo.GetOrderedRequisitions(pageNumber, pageSize);

            if (Request.IsAjaxRequest())
                return PartialView(orderedRequisitions);

            return View(orderedRequisitions);
        }

        [Authorize(Roles = "Admin, Purchasing Department")]
        public JsonResult MarkComplete(int id)
        {
            var requisitionMarkComplete = repo.GetRequisition(id);
            requisitionMarkComplete.Status = StatusEnum.Complete;
            repo.Save();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin, Purchasing Department")]
        public ActionResult Completed(int? page)
        {
            int pageNumber = (page ?? 1);
            var completedRequistioins = repo.GetCompletedRequisitions(pageNumber, pageSize);
            
            if (Request.IsAjaxRequest())
                return PartialView(completedRequistioins);

            return View(completedRequistioins);
        }

        [Authorize(Roles = "Admin, Purchasing Department")]
        public ActionResult Search()
        {
            if (Request.IsAjaxRequest())
                return HttpNotFound();

            ViewBag.Departments = repo.GetDepartments();
            ViewBag.Accounts = repo.GetAccounts();
            ViewBag.Divisions = repo.GetDivisions();
            ViewBag.Funds = repo.getFunds();

            return View();
        }
        //partial view that displays search results - called from the search view
        public ActionResult _SearchResult(SearchViewModel model, int? page)
        {

            int pageNumber = (page ?? 1);
            IQueryable<Requisition> results = null;

            //if the model data is not empty performm search, otherwise return empty list
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

                results = repo.GetAllRequisitions();
                results = SearchQueries.ProcessSearchInput(model, results); // performs filtering based on user query (model)

                ViewBag.total = results.Count();

                return PartialView(results.ToPagedList(pageNumber, pageSize)); // returns only results for the current page (page limit)
            }

            return PartialView(results.ToPagedList(pageNumber, pageSize));
        }

        //helper method to get all departments frmo the db
        public JsonResult GetActiveDepartments(int id)
        {
            var departments = repo.GetActiveDepartments(id);
            var result = Json(departments);

            return Json(new SelectList(departments, "Id", "Name"));
        }


        public JsonResult GetActiveAccounts(int id)
        {
            var departmentAccounts = repo.GetActiveAccounts(id);
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