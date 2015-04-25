using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication9.Data.MetaData;

namespace WebApplication9.Data
{
    [MetadataType(typeof(ItemMetadata))]
    public partial class Item
    {
    }

    [MetadataType(typeof(RequisitionMetadata))]
    public partial class Requisition
    {
    }
    [MetadataType(typeof(RequisitionExtrasMetadata))]
    public partial class ReqAdd
    {
    }

    [MetadataType(typeof(DepartmentMetaData))]
    public partial class Department
    {
    }
    [MetadataType(typeof(DivisionMetaData))]
    public partial class Division
    {
    }

    [MetadataType(typeof(FundMetaData))]
    public partial class Fund
    {
    }

    [MetadataType(typeof(ItemCategoryMetaData))]
    public partial class ItemCategory
    {
    }

    [MetadataType(typeof(AccountMetaData))]
    public partial class Account
    {
    }

}
