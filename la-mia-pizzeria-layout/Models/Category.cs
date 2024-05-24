using System.ComponentModel.DataAnnotations;
using Test_MVC_2.Models;

namespace Test_MVC_2.Models
{
    public class Category
    {
        [Key] public int Id { get; set; }
        public string Title { get; set; }

        public List<Pizza> Pizzas { get; set; }

    }
}



