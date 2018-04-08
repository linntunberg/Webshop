using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webshop.Models;
using MySql.Data.MySqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;


namespace Webshop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly string connectionString;
      
        public ActionResult Index()
        {
            var cartId = GetOrCreateCartId();

            using (var connection = new MySqlConnection(this.connectionString))
            {
                try
                {
                    var cartItems = connection.Query<CartViewModel>("SELECT carts.cartId, sum(carts.quantity) as quantity, carts.productId, products.price, products.item, products.image FROM products INNER JOIN carts ON carts.productId = products.id WHERE carts.cartId = @cartId GROUP BY carts.productId; ",
                            new { cartId }).ToList();

                    var sum = cartItems.Select(c => c.Price * c.Quantity).Sum();

                    var checkoutItems = new CheckoutViewModel();
                    checkoutItems.Cart = cartItems;
                    checkoutItems.Sum = sum;

                    return View(checkoutItems);
                }
                catch (Exception)
                {
                    return NotFound();
                }
            }
        }

        [HttpPost]
        public ActionResult Order(string FirstName, string LastName, string Email, string Address, string Country, int Zipcode, int CardNumber, string CartId, int Sum)
        {
            using (var connection = new MySqlConnection(this.connectionString))
            {

                try
                {

                    connection.Execute(
                        "INSERT INTO Orders(FirstName, LastName, Email, Address, Country, Zipcode, CardNumber, Sum, CartId) VALUES(@firstname, @lastname, @email, @address, @country, @zipcode, @cardnumber, @sum, @cartId)",
                        new { FirstName, LastName, Email, Address, Country, Zipcode, CardNumber, CartId, Sum });


                    var orderInfo = connection.Query<OrderViewModel>("Select * from Orders where Orders.CartId = @CartId;",
                                                                 new { CartId }).ToList();


                    var checkoutItems = connection.Query<CartViewModel>(
                        "SELECT carts.CartId, Sum(carts.quantity) as quantity, carts.productId, products.price, products.item, products.image FROM products INNER JOIN carts ON carts.productId = products.id WHERE carts.cartId = @cartId GROUP BY carts.productId;",
                        new { CartId }).ToList();

                    var orderId = orderInfo[0].Id;

                    foreach (var item in checkoutItems)
                    {

                        connection.Execute(
                            "INSERT INTO OrderRows(OrderId, ProductId, Item, Price, Quantity) VALUES(@orderId, @productId, @item, @price, @quantity)",
                            new { orderId, item.ProductId, item.Item, item.Price, item.Quantity });
                    }

                    

                }
                catch (Exception)
                {
                    return NotFound();
                }

            }
            this.Response.Cookies.Delete("CartId");

            return RedirectToAction("Index", "Products");
        }


        public CheckoutController(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("ConnectionString");
        }

        private string GetOrCreateCartId()
        {
            if (this.Request.Cookies.ContainsKey("CartId"))
            {
                return this.Request.Cookies["CartId"];
            }
            var cartId = Guid.NewGuid().ToString();
            this.Response.Cookies.Append("CartId", cartId);
            return cartId;
        }
    }
}
