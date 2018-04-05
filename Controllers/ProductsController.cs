using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Webshop.Models;
using MySql.Data.MySqlClient;   

namespace Webshop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly string connectionString;
        private List<ProductsViewModel> products;
        public ProductsController(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("ConnectionString");
        }

        public ActionResult Index()
        {
            using (var connection = new MySqlConnection(this.connectionString))
            {
                products = connection.Query<ProductsViewModel>("select * from products").ToList();
                return View(products);
            }
        }

    }
}