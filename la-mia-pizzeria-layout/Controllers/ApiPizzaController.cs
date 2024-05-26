using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Test_MVC_2.Models;
using Test_MVC_2.Data;
using Microsoft.Extensions.Hosting;


namespace Test_MVC_2.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ApiPizzaController : ControllerBase
    {
        //- restituire la lista di tutte le nostre pizze(deve essere possibile passare un parametro di filtro e restituire le pizze il cui titolo contiene il filtro inviato)
        //- restituire una pizza in base all’id
        //- creare una nuova pizza
        //- modificare una pizza in base all’id
        //- cancellare una pizza


        [HttpGet("{name?}")]
        public IActionResult RecuperaPizze(string? name)
        {
            if (name == null)
                return Ok(PizzaManager.RecuperaTutteLePizze());
            return Ok(PizzaManager.RecuperaPizzaDaNome(name)); 
        }

        
        [HttpGet("{id}")]
        public IActionResult RecuperaPizzaDaId(int id) // QUERY PARAM https://.../api/ApiPizza/RecuperaPizzaDaId
        {
            var Pizza = PizzaManager.RecuperaPizzaDaId(id);
            if (Pizza == null)
                return NotFound("ERRORE");
            return Ok(Pizza);
        }

        [HttpPost]
        public IActionResult CreaPizza([FromBody] Pizza Pizza)
        {
            PizzaManager.InserisciPizza(Pizza);
            return Ok();
        }



        [HttpPut("{id}")]
        public IActionResult UpdatePizza(int id, [FromBody] dynamic updateRequest)
        {
            var pizzaDaModificare = PizzaManager.RecuperaPizzaDaId(id);
            if (pizzaDaModificare == null)
            {
                return NotFound();
            }

            Pizza pizza = new Pizza
            {
                Name = updateRequest.name,
                Description = updateRequest.description,
                Url = updateRequest.url,
                Price = (float)updateRequest.price,
                CategoryId = updateRequest.categoryId
            };

            List<string> selectedIngredients = new List<string>();
            foreach (var ingredient in updateRequest.selectedIngredients)
            {
                selectedIngredients.Add(ingredient.ToString());
            }

            bool success = PizzaManager.EditaPizzaApi(id, pizza.Name, pizza.Description, pizza.Url, pizza.Price, pizza.CategoryId, selectedIngredients);

            if (!success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Errore durante l'aggiornamento della pizza.");
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePizza(int id)
        {

            if (PizzaManager.PiallaPizza(id))
            {
                return Ok();
            }
            return NotFound();
        }



    }
}
