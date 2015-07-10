using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.Controllers;
using SportsStore.Models;

namespace SportsStore.UnitTests
{
    [TestFixture]
    public class CatalogBrowsing
    {
        [Test]
        public void CanViewASinglePageOfProducts() {
            // arrange: if there are 5 products in the repository
            IProductsRepository repository = UnitTestHelpers.MockProductsRepository(
                new Product { Name = "P1" }, new Product { Name = "P2" },
                new Product { Name = "P3" }, new Product { Name = "P4" },
                new Product { Name = "P5" }
            );

            var controller = new ProductsController(repository);
            controller.PageSize = 3;

            // act: then when the user asks for the second page (pagesize=3)...
            var result = controller.List(null, 2);
            
            // Assert: they will just see the last two products
            var viewModel = (ProductsListViewModel)result.ViewData.Model;
            var displayedProducts = viewModel.Products;
            displayedProducts.Count.ShouldEqual(2);
            displayedProducts[0].Name.ShouldEqual("P4");
            displayedProducts[1].Name.ShouldEqual("P5");
        }

        [Test]
        public void CanViewProductsFromAllCategories() {
            // Arrange: if two products are in two different categories...
            IProductsRepository repository = UnitTestHelpers.MockProductsRepository(
                new Product { Name = "Artemis", Category = "Greek" },
                new Product { Name = "Neptune", Category = "Roman" }
                );
            var controller = new ProductsController(repository);

            // act: ... then we ask for the all products category
            var result = controller.List(null, 1);

            // assert: we get both products
            var viewModel = (ProductsListViewModel)result.ViewData.Model;
            viewModel.Products.Count.ShouldEqual(2);
            viewModel.Products[0].Name.ShouldEqual("Artemis");
            viewModel.Products[1].Name.ShouldEqual("Neptune");
        }

        public void CanViewProductsFromASingleCategory() {
            // Arrange: if two products are in two different categories...
            IProductsRepository repository = UnitTestHelpers.MockProductsRepository(
                new Product { Name = "Artemis", Category = "Greek" },
                new Product { Name = "Neptune", Category = "Roman" }
                );
            var controller = new ProductsController(repository);

            var result = controller.List("Roman", 1);

            var viewModel = (ProductsListViewModel)result.ViewData.Model;
            viewModel.Products.Count.ShouldEqual(1);
            viewModel.Products[0].Name.ShouldEqual("Neptune");
            viewModel.CurrentCategory.ShouldEqual("Roman");
        }
    }
}
