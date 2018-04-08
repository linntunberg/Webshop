using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webshop.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int Zipcode { get; set; }
        public int CardNumber { get; set; }
        public int Sum { get; set; }
        public string CardId { get; set; }
    }
}
