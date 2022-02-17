﻿using Inventory_Mangement_System.Middleware;
using Inventory_Mangement_System.Model;
using Inventory_Mangement_System.Model.Common;
using ProductInventoryContext;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_Mangement_System.Repository
{
    public class ProductionRepository : IProductionRepository
    {
        public Result AddProductionDetails(ProductionModel productionModel)
        {
            using (ProductInventoryDataContext context = new ProductInventoryDataContext())
            {
                ProductionDetail productionDetail = new ProductionDetail();
                var v = (from x in context.Vegetables
                         where x.VegetableName.ToLower() == productionModel.Vegetablenm.ToLower()
                         select x).FirstOrDefault();
                if(v == null)
                {
                    v = new Vegetable()
                    {
                        VegetableName = productionModel.Vegetablenm
                    };
                    context.Vegetables.InsertOnSubmit(v);
                    context.SubmitChanges();
                }
                int vg = (from obj in context.Vegetables
                          where obj.VegetableName == productionModel.Vegetablenm
                          select obj.VegetableID).SingleOrDefault();
                
                productionDetail.MainAreaID = productionModel.MainAreaDetails.Id;
                productionDetail.SubAreaID = productionModel.SubAreaDetails.Id;
                productionDetail.VegetableID = vg;
                productionDetail.Quantity = productionModel.Quantity;
                productionDetail.DateTime = DateTime.Now;
                productionDetail.LoginID = 1;
                productionDetail.Remark = productionModel.Remark;
                context.ProductionDetails.InsertOnSubmit(productionDetail);
                context.SubmitChanges();
                return new Result()
                {
                    Message = string.Format($"{productionModel.Vegetablenm} Production details Added Successfully."),
                    Status = Result.ResultStatus.success,
                    Data = productionModel.Vegetablenm,
                };
            }
        }
        /*  public Result AddProductionDetails(ProductionModel productionModel)
          {
              using (ProductInventoryDataContext context = new ProductInventoryDataContext())
              {
                  ProductionDetail productionDetail = new ProductionDetail();
                  UserLoginDetails login = new UserLoginDetails();
                  ProductionModel pm = new ProductionModel();
                  var UserMACAddress = login.GetMacAddress().Result;


                  var vn1 = (from m in productionModel.ProductionLists
                             from y in context.Vegetables
                             where m.Vegetablenm.ToLower() == y.VegetableName.ToLower()
                             select new
                             {
                                 VegetableName = y.VegetableName
                             }).ToList();
                  if (vn1.Count() == 0)
                  {
                      var vegetablename = (from m in productionModel.ProductionLists
                                           select new Vegetable()
                                           {
                                               VegetableName = m.Vegetablenm
                                           }).ToList();
                      context.Vegetables.InsertAllOnSubmit(vegetablename);
                      context.SubmitChanges();
                  }

                  var mac = context.LoginDetails.FirstOrDefault(c => c.SystemMAC == UserMACAddress);
                  var pd = (from obj in pm.ProductionLists
                            select obj).ToList();

                  var productionlist = (from m in productionModel.ProductionLists

                                        select new ProductionDetail()
                                        {
                                            MainAreaID = m.MainAreaDetails.Id,
                                            SubAreaID = m.SubAreaDetails.Id,
                                            VegetableID = (from obj in context.Vegetables
                                                           where obj.VegetableName == m.Vegetablenm
                                                           select obj.VegetableID).SingleOrDefault(),
                                            Quantity = m.Quantity,
                                            Remark = m.Remark,
                                            DateTime = DateTime.Now,
                                            LoginID = 1

                                        }).ToList();
                  context.ProductionDetails.InsertAllOnSubmit(productionlist);
                  context.SubmitChanges();



                  return new Result()
                  {
                      Message = string.Format($"Production details Added Successfully."),
                      Status = Result.ResultStatus.success,
                      Data = DateTime.Now,
                  };
              }
          }
  */
        public Result ViewAllProductionDetails()
        {
            using (ProductInventoryDataContext context = new ProductInventoryDataContext())
            {
                return new Result()
                {
                    Status=Result.ResultStatus.success,
                    Data = (from pd in context.ProductionDetails
                            join m in context.MainAreas
                            on pd.MainAreaID equals m.MainAreaID into JoinTableMA
                            from MA in JoinTableMA.DefaultIfEmpty()
                            join s in context.SubAreas
                            on pd.SubAreaID equals s.SubAreaID into JoinTableSA
                            from SA in JoinTableSA.DefaultIfEmpty()
                            join v in context.Vegetables
                            on pd.VegetableID equals v.VegetableID into JoinTableVN
                            from VN in JoinTableVN.DefaultIfEmpty()
                            select new
                            {
                                ProductionID = pd.ProductionID,
                                MainAreaDetails = new Model.Common.IntegerNullString() { Id = pd.MainArea.MainAreaID, Text = pd.MainArea.MainAreaName },
                                SubAreaDetails = new Model.Common.IntegerNullString() { Id = pd.SubArea.SubAreaID, Text = pd.SubArea.SubAreaName },
                                Vegetablenm = VN.VegetableName,
                                Quantity = pd.Quantity,
                                Remark=pd.Remark,
                                UserName = (from n in context.LoginDetails
                                            where n.LoginID == pd.LoginID
                                            select n.UserName).FirstOrDefault(),
                                DateTime = String.Format("{0:dd-MM-yyyy hh:mm tt}", pd.DateTime),
                            }).ToList()
                };
            }
        }

        public async Task<IEnumerable> ViewProductionById(int id)
        {
            using (ProductInventoryDataContext context = new ProductInventoryDataContext())
            {
                var qs = (from pd in context.ProductionDetails
                          join m in context.MainAreas
                          on pd.MainAreaID equals m.MainAreaID into JoinTableMA
                          from MA in JoinTableMA.DefaultIfEmpty()
                          join s in context.SubAreas
                          on pd.SubAreaID equals s.SubAreaID into JoinTableSA
                          from SA in JoinTableSA.DefaultIfEmpty()
                          join v in context.Vegetables
                          on pd.VegetableID equals v.VegetableID into JoinTableVN
                          from VN in JoinTableVN.DefaultIfEmpty()
                          where pd.ProductionID == id
                          select new
                          {
                              MainAreaName = MA.MainAreaName,
                              SubAreaName = SA.SubAreaName,
                              VegetableName = VN.VegetableName,
                              QuantityOfVegetable = pd.Quantity
                          }).ToList();
                return qs;
            }
        }

        public Result Editproduction(ProductionModel productionModel, int id)
        {
            using (ProductInventoryDataContext context = new ProductInventoryDataContext())
            {
                ProductionDetail productionDetail = new ProductionDetail();
                var pro = context.ProductionDetails.SingleOrDefault(x => x.ProductionID == id);
                var v = (from x in context.Vegetables
                         where x.VegetableName.ToLower() == productionModel.Vegetablenm.ToLower()
                         select x).FirstOrDefault();
                if (v == null)
                {
                    v = new Vegetable()
                    {
                        VegetableName = productionModel.Vegetablenm
                    };
                    context.Vegetables.InsertOnSubmit(v);
                    context.SubmitChanges();
                }
                int vg = (from obj in context.Vegetables
                          where obj.VegetableName == productionModel.Vegetablenm
                          select obj.VegetableID).SingleOrDefault();

                pro.MainAreaID = productionModel.MainAreaDetails.Id;
                pro.SubAreaID = productionModel.SubAreaDetails.Id;
                pro.VegetableID = vg;
                pro.Quantity = productionModel.Quantity;
                pro.DateTime = DateTime.Now;
                pro.LoginID = 1;
                pro.Remark = productionModel.Remark;
                context.SubmitChanges();
                return new Result()
                {
                    Message = string.Format($"{productionModel.Vegetablenm} Production details Added Successfully."),
                    Status = Result.ResultStatus.success,
                    Data = productionModel.Vegetablenm,
                };
            }
        }
        
       /* public Result EditProduction(ProductionModel productionModel, int id)
        {
            using (ProductInventoryDataContext context = new ProductInventoryDataContext())
            {
                UserLoginDetails login = new UserLoginDetails();
                ProductionDetail productionDetail = new ProductionDetail();
                var UserMACAddress = login.GetMacAddress().Result;
                var mac = context.LoginDetails.FirstOrDefault(c => c.SystemMAC == UserMACAddress);

                var veg = (from p in productionModel.ProductionLists
                           from v in context.Vegetables
                           where p.Vegetablenm.ToLower() == v.VegetableName.ToLower()
                           select new
                           {
                               VegetableName = v.VegetableName
                           }).ToList();
                if (veg.Count() == 0)
                {
                    var vegetablename = (from p in productionModel.ProductionLists
                                         select new Vegetable()
                                         {
                                             VegetableName = p.Vegetablenm
                                         }).ToList();
                    context.Vegetables.InsertAllOnSubmit(vegetablename);
                    context.SubmitChanges();
                }

                var vg = (from m in productionModel.ProductionLists
                          from y in context.Vegetables
                          where m.Vegetablenm.ToLower() == y.VegetableName.ToLower()
                          select new
                          {
                              VegetableID = y.VegetableID
                          }).ToList();


                var qs = (from obj in productionModel.ProductionLists
                          select obj).SingleOrDefault();

                var pd = (from obj in context.ProductionDetails
                          where obj.ProductionID == id
                          select obj).SingleOrDefault();

                return new Result()
                {
                    Message = string.Format($"Production details Added Successfully."),
                    Status = Result.ResultStatus.success,
                    Data = DateTime.Now,
                };
            }
        }*/

    }
}
