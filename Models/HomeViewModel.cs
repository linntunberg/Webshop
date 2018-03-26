using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class HomeViewModel
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public int Price { get; set; }
        public string Image { get; set; }
    }
}