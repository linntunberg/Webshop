using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webshop.Models;
using MySql.Data.MySqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;


namespace Webshop.Controllers
{
    public class CartController : Controller
    {
        private readonly string connectionString;
        // GET: /<controller>/
        public ActionResult Index()
        {
            var cartId = GetOrCreateCartId();

            using (var connection = new MySqlConnection(this.connectionString))
            {
                try
                {
                    var cartItems = connection.Query<CartViewModel>("SELECT carts.cartId, sum(carts.quantity) as quantity, carts.productId, products.price, products.item, products.image FROM products INNER JOIN carts ON carts.productId = products.id WHERE carts.cartId = @cartId GROUP BY carts.productId; ",
                            new { cartId }).ToList();
                    return View(cartItems);
                }
                catch (Exception)
                {
                    return NotFound();
                }
            }
        }

        [HttpPost]
        public ActionResult AddToCart(int id)
        {
            var cartId = GetOrCreateCartId();
            var quantity = 1;

            using (var connection = new MySqlConnection(this.connectionString))
            {
                try
                {
                    connection.Execute(
                        "INSERT INTO Carts (productId, cartId, quantity) VALUES(@id, @cartId, @quantity )",
                        new { id, cartId, quantity });
                }
                catch (Exception)
                {
                    return NotFound();
                }

                return RedirectToAction("Index", "Products");

            }
        }
       
        public CartController(IConfiguration configuration)
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
