﻿using System;
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

        public List<ItemCategory> GetItemCategories()
        {
            return repo.ItemCategories.Where(x => x.Status == ConfigureStatusEnum.Active).OrderBy(x => x.Name).ToList();
        }

        public List<Division> GetDivisions()
        {
            return repo.Divisions.Where(x => x.Status == ConfigureStatusEnum.Active).OrderBy(x => x.Name).ToList();
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
    }
}