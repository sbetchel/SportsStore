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
    public class CatalogEditing
    {
        public void CanSaveEditedProducts() {
            var mockRepository = new Moq.Mock<IProductsRepository>();
            var product = new Product();

            var result = new AdminController(mockRepository.Object).Edit(product, null);

            mockRepository.Verify(x => x.SaveProduct(product));
            result.ShouldBeRedirectionTo(new { action = "Index" });
        }
    }
}
