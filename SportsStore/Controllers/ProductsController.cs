using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Concrete;
using SportsStore.Models;

namespace SportsStore.Controllers
{
    public class ProductsController : Controller
    {
        public int PageSize = 4;
        private IProductsRepository productsRepository;
        public ProductsController(IProductsRepository pr)
        {
            // This is just temporary until we have more infrastructure in place 
            productsRepository = pr;
        }

        public ViewResult List(string category, int page = 1)
        {
            var productsToShow = (category == null) 
                ? productsRepository.Products
                : productsRepository.Products.Where(x => x.Category == category);

            var viewModel = new ProductsListViewModel
            {
                Products = productsToShow.Skip((page - 1) * PageSize).Take(PageSize).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = productsToShow.Count()
                },
                CurrentCategory = category
            };
            return View(viewModel); // Passed to view as ViewData.Model (or simply Model)
        }
    }
}
