using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.Controllers;
using SportsStore.HtmlHelpers;
using SportsStore.Models;
using System.Web.Mvc;

namespace SportsStore.UnitTests
{
    [TestFixture]
    public class NavigationByCategory
    {
        [Test]
        public void NavMenuIncludesAlphabeticalListOfDistinctCategories() {
            var mockProductsRepository = UnitTestHelpers.MockProductsRepository(
                new Product { Category = "Vegetable", Name = "ProductA" },
                new Product { Category = "Animal", Name = "ProductB" },
                new Product { Category = "Vegetable", Name = "ProductC" },
                new Product { Category = "Mineral", Name = "ProductD" }
            );

            var controller = new NavController(mockProductsRepository);
            var result = controller.Menu(null);
            
            var categoryLinks = ((IEnumerable<NavLink>)result.ViewData.Model)
                                .Where(x => x.RouteValues["category"] != null);

            CollectionAssert.AreEqual(new[] { "Animal", "Mineral", "Vegetable" }, categoryLinks.Select(x => x.RouteValues["category"]));

            foreach (var link in categoryLinks) {
                link.RouteValues["controller"].ShouldEqual("Products");
                link.RouteValues["action"].ShouldEqual("List");
                link.RouteValues["page"].ShouldEqual(1);
                link.Text.ShouldEqual(link.RouteValues["category"]);
            }
        }

        [Test]
        public void NavMenu_Shows_Home_Link_At_Top() {
            // Arrange: Given any repository 
            var mockProductsRepository = UnitTestHelpers.MockProductsRepository();

            // Act: ... when we render the navigation menu 
            var result = new NavController(mockProductsRepository).Menu(null);

            // Assert: ... then the top link is to Home 
            var topLink = ((IEnumerable<NavLink>)result.ViewData.Model).First();
            topLink.RouteValues["controller"].ShouldEqual("Products");
            topLink.RouteValues["action"].ShouldEqual("List");
            topLink.RouteValues["page"].ShouldEqual(1);
            topLink.RouteValues["category"].ShouldEqual(null);
            topLink.Text.ShouldEqual("Home");
        }

        [Test]
        public void NavMenu_Highlights_Current_Category() {
            // Arrange: Given two categories... 
            var mockProductsRepository = UnitTestHelpers.MockProductsRepository(
                new Product { Category = "A", Name = "ProductA" },
                new Product { Category = "B", Name = "ProductB" }
            );

            // Act: ... when we render the navigation menu 
            var result = new NavController(mockProductsRepository).Menu("B");

            // Assert: ... then only the current category is highlighted 
            var highlightedLinks = ((IEnumerable<NavLink>)result.ViewData.Model)
                                    .Where(x => x.IsSelected).ToList();
            highlightedLinks.Count.ShouldEqual(1);
            highlightedLinks[0].Text.ShouldEqual("B");
        }
    }
}
