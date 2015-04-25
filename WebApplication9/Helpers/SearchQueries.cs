using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication9.Data;
using WebApplication9.Models;

namespace WebApplication9.Helpers
{
    public class SearchQueries
    {
        public static IQueryable<Requisition> ProcessSearchInput(SearchViewModel model, IQueryable<Requisition> results)
        {
            if (model.AccountNumber > 0)
                results = results.Where(x => x.AccountId == model.AccountNumber);

            if (model.AccountName > 0)
                results = results.Where(x => x.AccountId == model.AccountName);

            if (model.FundNumber > 0)
                results = results.Where(x => x.Account.FundId == model.FundNumber);

            if (model.DivisionName > 0)
                results = results.Where(x => x.Account.Department.DivisionId == model.DivisionName);

            if (model.DepartmentId > 0)
                results = results.Where(x => x.Account.Department.Id == model.DepartmentId);

            if (model.RequisitionID > 0)
                results = results.Where(x => x.RequisitionId == model.RequisitionID);

            if (model.RequisitionedBy != null)
                results = results.Where(x => x.Requisitioned_By.Contains(model.RequisitionedBy));

            if (model.Status > 0)
                results = results.Where(x => x.Status == model.Status);

            if (model.TotalFrom > 0 && model.TotalUpTo > 0)
                results = results.Where(x => x.Items.Sum(p => (p.Price * p.Quantity)) >= model.TotalFrom).Where(x => x.Items.Sum(p => p.Price * p.Quantity) <= model.TotalUpTo);

            if (model.TotalFrom > 0 && model.TotalUpTo == 0)
                results = results.Where(x => x.Items.Sum(p => (p.Price * p.Quantity)) >= model.TotalFrom);

            if (model.TotalFrom == 0 && model.TotalUpTo > 0)
                results = results.Where(x => x.Items.Sum(p => (p.Price * p.Quantity)) <= model.TotalUpTo);

            if (model.Date_Submitted_From != DateTime.MinValue && model.Date_Submitted_To != DateTime.MinValue)
                results = results.Where(x => x.Date_Created >= model.Date_Submitted_From).Where(x => x.Date_Created <= model.Date_Submitted_To);

            if (model.Date_Submitted_From != DateTime.MinValue && model.Date_Submitted_To == DateTime.MinValue)
                results = results.Where(x => x.Date_Created >= model.Date_Submitted_From);

            if (model.Date_Submitted_From == DateTime.MinValue && model.Date_Submitted_To != DateTime.MinValue)
                results = results.Where(x => x.Date_Created <= model.Date_Submitted_To);
            return results.OrderByDescending(x => x.Date_Created);
        }
    }
}