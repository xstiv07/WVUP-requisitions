using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication9.Data.Helpers;
using System.Security.Principal;
using WebApplication9.Data;
using PagedList;

namespace WebApplication9.Repository
{
    public class Repository : IRepository
    {
        private Requisition1Entities repo;

        public Repository(Requisition1Entities db)
        {
            this.repo = db;
        }

        public List<ItemCategory> GetActiveItemCategories()
        {
            return repo.ItemCategories.Where(x => x.Status == ConfigureStatusEnum.Active).OrderBy(x => x.Name).ToList();
        }

        public List<ItemCategory> GetItemCategories()
        {
            return repo.ItemCategories.ToList();
        }

        public List<Division> GetDivisions()
        {
            return repo.Divisions.ToList();
        }

        public void AddRequisition(Requisition requisition)
        {
            repo.Requisitions.Add(requisition);
            repo.SaveChanges();
        }

        public void AddExtraInfo(ReqAdd extra)
        {
            repo.ReqAdds.Add(extra);
            repo.SaveChanges();
        }

        public void AddFile(string unq, string fileName1, Requisition requisition, MyUser user)
        {
            repo.Files.Add(new File()
            {
                //searhing for the first occurence of unique Id - associating file with an item based on that
                ItemId = repo.Items.Where(x => x.Unique == unq).Select(x => x.ItemId).First(),
                FileName = fileName1,
                FileLink = "~/Uploads/" + user.First_Name + "_" + user.Last_Name + "/" + requisition.RequisitionId.ToString() + "/" + fileName1
            });
        }

        public void Save()
        {
            repo.SaveChanges();
        }

        public Department GetCurrentRequisitionDepartment(Requisition requisition)
        {
            return repo.Departments.Where(x => x.Id == requisition.DepartmentId).First();
        }

        public Requisition GetRequisition(long _requis)
        {
            return repo.Requisitions.Where(x => x.RequisitionId == _requis).FirstOrDefault();
        }

        public List<File> GetFiles(int id)
        {
            return repo.Files.Where(x => x.ItemId == id).ToList();
        }

        public Requisition GetCurrentUserRequistion(int id, MyUser user)
        {
            return repo.Requisitions.Where(x => x.RequisitionId == id && x.User_Id == user.Id).FirstOrDefault();
        }

        public IPagedList<Requisition> GetCurrentUserRequisitions(MyUser user, int pageNumber, int pageSize)
        {
            return repo.Requisitions.Where(x => x.User_Id == user.Id).OrderByDescending(x => x.Date_Created).ToPagedList(pageNumber, pageSize);
        }

        public IPagedList<Requisition> GetCFORequisitions(int pageNumber, int pageSize)
        {
            return repo.Requisitions.Where(x => x.CFO_approval == true && x.Status == StatusEnum.Submitted).OrderByDescending(x => x.Date_Created).ToPagedList(pageNumber, pageSize);
        }


        public IPagedList<Requisition> GetRequisitionsToReview(MyUser user, int pageNumber, int pageSize)
        {
            return repo.Requisitions.Where(x => x.Account.Department.ManagerId == user.Id && ((x.Status == StatusEnum.Aproved_CFO) || x.CFO_approval == false && x.Status == StatusEnum.Submitted)).OrderByDescending(x => x.Date_Created).ToPagedList(pageNumber, pageSize);
        }

        public IPagedList<Requisition> GetApprovedRequisitions(int pageNumber, int pageSize)
        {
            return repo.Requisitions.Where(x => x.Status == StatusEnum.Approved).OrderByDescending(x => x.Date_Created).ToPagedList(pageNumber, pageSize);
        }

        public IPagedList<Requisition> GetOrderedRequisitions(int pageNumber, int pageSize)
        {
            return repo.Requisitions.Where(x => x.Status == StatusEnum.Ordered).OrderByDescending(x => x.Date_Created).ToPagedList(pageNumber, pageSize);
        }

        public IPagedList<Requisition> GetCompletedRequisitions(int pageNumber, int pageSize)
        {
            return repo.Requisitions.Where(x => x.Status == StatusEnum.Complete).OrderByDescending(x => x.Date_Created).ToPagedList(pageNumber, pageSize);
        }


        public List<Department> GetDepartments()
        {
            return repo.Departments.ToList();
        }

        public List<Account> GetAccounts()
        {
            return repo.Accounts.ToList();
        }

        public List<Fund> getFunds()
        {
            return repo.Funds.ToList();
        }

        public IQueryable<Requisition> GetAllRequisitions()
        {
            return repo.Requisitions.Where(x => x.RequisitionId > 0);
        }

        public List<Department> GetActiveDepartments(int id)
        {
            return repo.Departments.Where(x => x.DivisionId == id && x.Status == ConfigureStatusEnum.Active).OrderBy(x => x.Name).ToList();
        }

        public List<Account> GetActiveAccounts(int id)
        {
            return repo.Accounts.Where(x => x.DepartmentId == id && x.Status == ConfigureStatusEnum.Active).OrderBy(x => x.Name).ToList();
        }


        public void AddAccount(Account model)
        {
            repo.Accounts.Add(model);
            repo.SaveChanges();
        }

        public Account GetAccount(int id)
        {
            return repo.Accounts.Find(id);
        }

        public List<User> GetUsers()
        {
            return repo.Users.ToList();
        }


        public void AddDepartment(Department model)
        {
            repo.Departments.Add(model);
            repo.SaveChanges();
        }

        public Department GetDepartment(int id)
        {
            return repo.Departments.Find(id);
        }


        public void AddFunds(Fund model)
        {
            repo.Funds.Add(model);
            repo.SaveChanges();
        }


        public Fund GetFund(int id)
        {
            return repo.Funds.Find(id);
        }


        public void AddDivision(Division model)
        {
            repo.Divisions.Add(model);
            repo.SaveChanges();
        }

        public Division GetDivision(int id)
        {
            return repo.Divisions.Find(id);
        }

        public List<Department> GetActiveDepartments()
        {
            return repo.Departments.Where(x => x.Status == ConfigureStatusEnum.Active).ToList();
        }


        public void AddItemCategory(ItemCategory model)
        {
            repo.ItemCategories.Add(model);
            repo.SaveChanges();
        }

        public ItemCategory GetItemCategory(int id)
        {
            return repo.ItemCategories.Find(id);
        }

        public List<Fund> GetActiveFunds()
        {
            return repo.Funds.Where(x => x.Status == ConfigureStatusEnum.Active).ToList();
        }

        public List<Division> GetActiveDivisions()
        {
            return repo.Divisions.Where(x => x.Status == ConfigureStatusEnum.Active).ToList();
        }

        public List<Department> GetActiveDivsionDepartments(int id)
        {
            return repo.Departments.Where(x => x.DivisionId == id && x.Status == ConfigureStatusEnum.Active).ToList();
        }


        public IQueryable<Requisition> GetCurrentUserRequisitions(MyUser user)
        {
            return repo.Requisitions.Where(x => x.User_Id == user.Id);
        }


        public void AddFiles(HttpRequestBase Request, MyUser user, Requisition requisition)
        {
            for (int i = 0; i < Request.Files.Count; i++) // getting all files from the request and saving them to the db
            {
                HttpPostedFileBase file = Request.Files[i];

                // exists to tie together item and its files - based on the unique field in the db that is populated by the guid in the view
                string unq = Request.Files.AllKeys[i].ToString();

                if (file != null && file.ContentLength > 0)
                {
                    var originalDirectory = new System.IO.DirectoryInfo(string.Format("{0}Uploads\\" + user.First_Name + "_" + user.Last_Name, System.Web.HttpContext.Current.Server.MapPath(@"\")));
                    string pathString = System.IO.Path.Combine(originalDirectory.ToString(), requisition.RequisitionId.ToString());
                    var fileName1 = System.IO.Path.GetFileName(file.FileName);
                    bool isExists = System.IO.Directory.Exists(pathString);

                    if (!isExists)
                        System.IO.Directory.CreateDirectory(pathString);
                    var path = string.Format("{0}\\{1}", pathString, file.FileName);
                    file.SaveAs(path);

                    this.AddFile(unq, fileName1, requisition, user);
                }
            }
            this.Save();
        }
    }
}