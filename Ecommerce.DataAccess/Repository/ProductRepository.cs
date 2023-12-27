using Eccomerce.DataAccess.Data;
using Ecommerce.DataAccess.Repository.IRepository;
using Ecommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Product obj)
        {
            //_db.Products.Update(obj);
            var objFromDB = _db.Products.FirstOrDefault(u=>u.ID == obj.ID);
            if (objFromDB != null)
            {
                objFromDB.Title = obj.Title;
                objFromDB.Description = obj.Description;
                objFromDB.ISBN = obj.ISBN;
                objFromDB.Author = obj.Author;
                objFromDB.ListPrice= obj.ListPrice;
                objFromDB.Price = obj.Price;
                objFromDB.CategoryID= obj.CategoryID;
                objFromDB.Price50= obj.Price50;
                objFromDB.Price100= obj.Price100;
                objFromDB.ProductImages = obj.ProductImages;

                //if (obj.ImageURL != null) { 
                //objFromDB.ImageURL = obj.ImageURL;
                //}
            }
        }
    }
}
