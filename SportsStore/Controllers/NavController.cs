using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;
using SportsStore.Models;

namespace SportsStore.Controllers
{
    public class NavController : Controller
    {
        private IProductsRepository productsRepository;
        public NavController(IProductsRepository productsRepository) {
            this.productsRepository = productsRepository;
        }

        public ViewResult Menu(string category) {
            Func<string, NavLink> makeLink = categoryName => new NavLink
            {
                Text = categoryName ?? "Home",
                RouteValues = new System.Web.Routing.RouteValueDictionary(new { controller = "Products", action = "List", category = categoryName, page = 1 }),
                IsSelected = categoryName == category
            };

            List<NavLink> navLinks = new List<NavLink>();
            navLinks.Add(makeLink(null));

            var categories = productsRepository.Products.Select(x => x.Category);
            foreach (string categoryName in categories.Distinct().OrderBy(x => x))
                navLinks.Add(makeLink(categoryName));

            return View(navLinks);
        }
    }
}
