using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.Controllers;
using SportsStore.HtmlHelpers;
using SportsStore.Models;
using NUnit.Framework;
using System.Web.Mvc;
using SportsStore.Domain.Services;
using Moq;

namespace SportsStore.UnitTests
{
    [TestFixture]
    public class ShoppingCart
    {
        [Test]
        public void CartStartsEmpty() {
            new Cart().Lines.Count.ShouldEqual(0);
        }

        [Test]
        public void CartCombinesLinesWithSameProduct() {
            Product p1 = new Product { ProductID = 1 };
            Product p2 = new Product { ProductID = 2 };

            var cart = new Cart();
            cart.AddItem(p1, 1);
            cart.AddItem(p1, 2);
            cart.AddItem(p2, 10);

            cart.Lines.Count.ShouldEqual(2);
            cart.Lines.First(x => x.Product.ProductID == 1).Quantity.ShouldEqual(3);
            cart.Lines.First(x => x.Product.ProductID == 2).Quantity.ShouldEqual(10);
        }

        [Test]
        public void CartCanBeCleared() {
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);

            cart.Clear();
            cart.Lines.Count.ShouldEqual(0);
        }

        [Test]
        public void CartTotalValueIsSumOfPriceTimesQuantity() {
            Cart cart = new Cart();
            cart.AddItem(new Product { ProductID = 1, Price = 5 }, 10);
            cart.AddItem(new Product { ProductID = 2, Price = 2.1m }, 3);
            cart.AddItem(new Product { ProductID = 3, Price = 1000 }, 1);

            cart.ComputeTotalValue().ShouldEqual(1056.3m);
        }

        [Test]
        public void CartCanRemoveLine() {
            Cart cart = new Cart();
            cart.AddItem(new Product { ProductID = 1 }, 2);

            cart.RemoveLine(new Product { ProductID = 1 });

            cart.Lines.Count.ShouldEqual(0);
        }

        [Test]
        public void CanAddProductToCart() {
            var mockProductRepository = UnitTestHelpers.MockProductsRepository(
                new Product { ProductID = 123 },
                new Product { ProductID = 456 }
            );

            var cartController = new CartController(mockProductRepository, null);
            var cart = new Cart();

            cartController.AddToCart(cart, 456, null);

            cart.Lines.Count.ShouldEqual(1);
            cart.Lines[0].Product.ProductID.ShouldEqual(456);
        }

        [Test]
        public void AfterAddingProductToCartUserGoesToYourCartScreen() {
            var mockProductsRepository = UnitTestHelpers.MockProductsRepository(
                new Product { ProductID = 1 }
            );
            var cartController = new CartController(mockProductsRepository, null);

            // Act: When a user adds a product to their cart... 
            var result = cartController.AddToCart(new Cart(), 1, "someReturnUrl");

            // Assert: Then the user is redirected to the Cart Index screen 
            result.ShouldBeRedirectionTo(new
            {
                action = "Index",
                returnUrl = "someReturnUrl"
            });
        }

        [Test]
        public void Can_View_Cart_Contents() {
            // Arrange/act: Given the user vists CartController's Index action... 
            var cart = new Cart();
            var result = new CartController(null, null).Index(cart, "someReturnUrl");

            // Assert: Then the view has their cart and the correct return URL 
            var viewModel = (CartIndexViewModel)result.ViewData.Model;
            viewModel.Cart.ShouldEqual(cart);
            viewModel.ReturnUrl.ShouldEqual("someReturnUrl");
        }

        [Test]
        public void Cannot_Check_Out_If_Cart_Is_Empty() {
            // Arrange/act: When a user tries to check out with an empty cart 
            var emptyCart = new Cart();
            var shippingDetails = new ShippingDetails();
            var result = new CartController(null, null)
                                .CheckOut(emptyCart, shippingDetails);

            // Assert 
            result.ShouldBeDefaultView();
        }

        [Test]
        public void Cannot_Check_Out_If_Shipping_Details_Are_Invalid() {
            // Arrange: Given a user has a non-empty cart 
            var cart = new Cart();
            cart.AddItem(new Product(), 1);

            // Arrange: ... but the shipping details are invalid 
            var cartController = new CartController(null, null);
            cartController.ModelState.AddModelError("any key", "any error");

            // Act: When the user tries to check out 
            var result = cartController.CheckOut(cart, new ShippingDetails());

            // Assert 
            result.ShouldBeDefaultView();
        }

        [Test]
        public void Can_Check_Out_And_Submit_Order() {
            var mockOrderSubmitter = new Mock<IOrderSubmitter>();

            // Arrange: Given a user has a non-empty cart & no validation errors 
            var cart = new Cart();
            cart.AddItem(new Product(), 1);
            var shippingDetails = new ShippingDetails();

            // Act: When the user tries to check out 
            var cartController = new CartController(null, mockOrderSubmitter.Object);
            var result = cartController.CheckOut(cart, shippingDetails);

            // Assert: Order goes to the order submitter & user sees "Completed" view 
            mockOrderSubmitter.Verify(x => x.SubmitOrder(cart, shippingDetails));
            result.ShouldBeView("Completed");
        }

        [Test]
        public void After_Checking_Out_Cart_Is_Emptied() {
            // Arrange/act: Given a valid order submission 
            var cart = new Cart();
            cart.AddItem(new Product(), 1);
            new CartController(null, new Mock<IOrderSubmitter>().Object).CheckOut(cart, new ShippingDetails());

            // Assert: The cart is emptied 
            cart.Lines.Count.ShouldEqual(0);
        }
    }
}
