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
