using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using Test_MVC_2.Data;

namespace Test_MVC_2.Models
{
    public class PizzaFormModel
    {
        public Pizza pizza { get; set; }
        public List<Category>? Categories { get; set; }
        public List<SelectListItem>? Ingredients { get; set; } // Gli elementi degli ingredienti selezionabili per la select (analogo a Categories)
        public List<string>? SelectedIngredients { get; set; } // Gli ID degli elementi effettivamente selezionati

        public PizzaFormModel() { }

        public PizzaFormModel(Pizza p, List<Category> c)
        {
            this.pizza = p;
            this.Categories = c;
            //definisco gli ingredienti selezionati tramite una lista.
            SelectedIngredients = new List<string>();

            //Se la lista degli ingredienti non è vuota
            if (pizza.Ingredients != null)

                //aggiungo gli ingredienti selezionati agli ingredienti della pizza
                foreach (var i in pizza.Ingredients)
                    SelectedIngredients.Add(i.Id.ToString());
        }
        public void CreateIngredients()
        {
            // Inizializza la lista di SelectListItem per gli ingredienti
            this.Ingredients = new List<SelectListItem>();

            // Se la lista degli ingredienti selezionati è null, la inizializza come una nuova lista vuota
            if (this.SelectedIngredients == null)
                this.SelectedIngredients = new List<string>();

            // Ottengo tutti gli ingredienti disponibili dal database
            var ingredientsFromDB = PizzaManager.GetAllIngredients();

            // Itero attraverso ogni ingrediente
            foreach (var ingredient in ingredientsFromDB)
            {
                // Verifico se l'ingrediente corrente è nella lista degli ingredienti selezionati

                bool isSelected = this.SelectedIngredients.Contains(ingredient.Id.ToString());

                // Aggiunge un nuovo SelectListItem alla lista degli ingredienti
                this.Ingredients.Add(new SelectListItem()

                {
                    Text = ingredient.Name, 
                    Value = ingredient.Id.ToString(), // SelectListItem vuole una generica stringa, non un int
                    Selected = isSelected 
                });
            }


        }


    }
}