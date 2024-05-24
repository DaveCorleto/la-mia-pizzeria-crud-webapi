using System.ComponentModel.DataAnnotations;

namespace Test_MVC_2.Models
{
    public class Ingredient
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }

        //Per segnalare che per ogni ingrediente ci possono essere molte pizze:
        public List<Pizza> Pizzas { get; set; }
    }
}

