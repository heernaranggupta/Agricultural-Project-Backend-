﻿using Inventory_Mangement_System.Model;
using Inventory_Mangement_System.Model.Common;
using System.Collections;
using System.Threading.Tasks;

namespace Inventory_Mangement_System.Repository
{
    public interface IIssueRepository
    {
        //View Issue Details
        Result ViewAllIssue(int? Id);

        //Issue Products
        Result IssueProduct(IssueModel issueModel,object LoginId);

        //Edit Issue Details
        Result EditIssue(UpdateIssue issueModel, int ID,object LoginId);

        //Get MainArea Dropdown
        Task<IEnumerable> GetArea();

        //Get SubArea Dropdown
        //Task<IEnumerable> GetSubArea(int id);

        //Get Product Dropdown with Unit And Quantity
        Task<IEnumerable> GetProduct();

       // Task<IEnumerable> GetProductTotalQuantity();
      //  Result total(IssueModel issueModel);
    }
}