using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Webshop.Models
{
    public class CheckoutViewModel
    {
        public List<CartViewModel> Cart { get; set; }
        public int Sum { get; set; }
    }
}
