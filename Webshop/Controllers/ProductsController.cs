using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Webshop.Models;
using Webshop.Project.Core.Models;
using MySql.Data.MySqlClient;
using Webshop.Repositories.Implementations;
using Webshop.Project.Core.Repositories;
using Webshop.Project.Core.Services.Implementations;

namespace Webshop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductService productService;
        private readonly string connectionString;
        
        public ProductsController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ConnectionString");
            productService = new ProductService(new ProductRepository(this.connectionString));
        }
        

        public ActionResult Index()
        {
            var products = this.productService.GetAll();
            return View(products);
        }

    }
}