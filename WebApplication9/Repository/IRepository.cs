using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication9.Data;
namespace WebApplication9.Repository
{
    public interface IRepository
    {

        void AddRequisition(Requisition requisition);

        void AddFile(string unq, string fileName1, Requisition requisition, MyUser user);

        void AddExtraInfo(ReqAdd extra);

        void Save();

        Department GetCurrentRequisitionDepartment(Requisition requisition);

        Requisition GetRequisition(long _requis);

        List<File> GetFiles(int id);

        Requisition GetCurrentUserRequistion(int id, MyUser user);

        IPagedList<Requisition> GetCFORequisitions(int pageNumber, int pageSize);

        IPagedList<Requisition> GetRequisitionsToReview(MyUser user, int pageNumber, int pageSize);

        IPagedList<Requisition> GetCurrentUserRequisitions(MyUser user, int pageNumber, int pageSize);

        IPagedList<Requisition> GetApprovedRequisitions(int pageNumber, int pageSize);

        IPagedList<Requisition> GetOrderedRequisitions(int pageNumber, int pageSize);

        IPagedList<Requisition> GetCompletedRequisitions(int pageNumber, int pageSize);

        List<Department> GetDepartments();

        List<Account> GetAccounts();

        List<Fund> getFunds();

        List<Division> GetDivisions();

        List<ItemCategory> GetItemCategories();

        IQueryable<Requisition> GetAllRequisitions();

        List<Department> GetActiveDepartments(int id);

        List<Account> GetActiveAccounts(int id);

        void AddAccount(Account model);

        Account GetAccount(int id);

        List<User> GetUsers();

        void AddDepartment(Department model);

        Department GetDepartment(int id);
    }
}
