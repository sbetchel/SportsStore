using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using System.Data.Linq;

namespace SportsStore.Domain.Concrete
{
    public class SqlProductsRepository : IProductsRepository
    {
        private Table<Product> productsTable;
        public SqlProductsRepository(string connectionString)
        {
            productsTable = (new DataContext(connectionString)).GetTable<Product>();
        }

        public IQueryable<Entities.Product> Products
        {
            get { return productsTable; }
        }

        public void SaveProduct(Product product) {
            if (product.ProductID == 0)
                productsTable.InsertOnSubmit(product);
            else if (productsTable.GetOriginalEntityState(product) == null) {
                productsTable.Attach(product);
                productsTable.Context.Refresh(RefreshMode.KeepCurrentValues, product);
            }

            productsTable.Context.SubmitChanges();
        }

        public void DeleteProduct(Product product) {
            productsTable.DeleteOnSubmit(product);
            productsTable.Context.SubmitChanges();
        }
    }
}
