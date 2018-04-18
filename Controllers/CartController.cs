using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Webshop.Project.Core.Models;
using Webshop.Project.Core.Repositories.Implementations;
using Webshop.Project.Core.Services.Implementations;

namespace Webshop.Controllers
{
    public class CartController : Controller
    {
        private readonly string connectionString;
        private CartService cartService;
        // GET: /<controller>/

        public CartController(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("ConnectionString");
            cartService = new CartService(new CartRepository(this.connectionString));
        }

        public ActionResult Index(int CartId)
        {
            var cartId = this.GetOrCreateCartId();

            List<CartViewModel> cartItems = cartService.GetCart(CartId);

                    return View(cartItems);               
        }

        [HttpPost]
        public ActionResult AddToCart(int id)
        {
            var cartId = this.GetOrCreateCartId();
            var quantity = 1;
            this.cartService.InsertCart(id, cartId, quantity);
           
                return RedirectToAction("Index", "Products");
        }

        public string GetOrCreateCartId()
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
