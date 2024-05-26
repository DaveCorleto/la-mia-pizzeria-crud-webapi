using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NuGet.Versioning;
using System.Reflection.Metadata;
using Test_MVC_2.Models;

namespace Test_MVC_2.Data

{
    public static class PizzaManager
    {
        //Uso dell'enum per gestire meglio nei metodi i vari tipi di risultato
        public enum ResultType
        {
            OK,
            Exception,
            NotFound
        }
        public static int ContaTuttelePizze()
        {
            //Creo un istanza temporanea di PizzaContext
            //e con la direttiva using comunico di chiudermi la "connessione"

            using PizzaContext db = new PizzaContext();
            return db.Pizzas.Count();
        }

        //Questo mi ritorna una Lista di tutte le pizze
        public static List<Pizza> RecuperaTutteLePizze()
        {
            using PizzaContext db = new PizzaContext();
            return db.Pizzas.ToList();
        }
        //Recupera la pizza dall'id
        public static Pizza RecuperaPizzaDaId(int id, bool includeReferences = true)
        {
            using PizzaContext db = new PizzaContext();
            //Se l'elemento pizza che sto recuperando dal db include una category mi verrà
            //restituita anche l'informazione della category

            if (includeReferences)
                //attraverso la lambda functions includo nella richiesta della pizza anche categorie e ingredienti
                return db.Pizzas.Where(x=>x.Id == id).Include(p=>p.Category).Include(p=>p.Ingredients).FirstOrDefault();

            //Altrimenti mi viene restituito solo l'elemento pizza con valore di category null

            return db.Pizzas.FirstOrDefault(p => p.Id == id);
        }

        public static List<Pizza> RecuperaPizzaDaNome(string Name)
        {
            using PizzaContext db = new PizzaContext();
            return db.Pizzas.Where(x => x.Name.ToLower().Contains(Name.ToLower())).ToList();
        }

        public static void InserisciPizza(Pizza pizza, List<string> selectedIngredients)
        {
            using PizzaContext db = new PizzaContext();

            pizza.Ingredients = new List<Ingredient>();

            if (selectedIngredients != null)
            {
                // Trasformiamo gli ID scelti in ingredienti da aggiungere tra i riferimenti in Pizza
                foreach (var ingredient in selectedIngredients)
                {
                    //converto l'ingrediente preso tramite form in int'
                    int id = int.Parse(ingredient);

                    // NON usiamo un GetIngredientById() perché userebbe un db context diverso
                    // e ciò causerebbe errore in fase di salvataggio - usiamo lo stesso context all'interno della stessa operazione
                    
                    var ingredientDalDb= db.Ingredients.FirstOrDefault(x => x.Id == id);
                    if (ingredientDalDb != null)
                    {
                        pizza.Ingredients.Add(ingredientDalDb);
                    }
                }
            }
            db.Pizzas.Add(pizza);
            db.SaveChanges();
        }

        //Alla Update (EditaPizza) Anzichè i parametri singoli gli passo direttamente l'oggetto Pizza
        public static bool EditaPizza(int id, Pizza pizza, List<string> selectedIngredients)
        {
            try
            {
                // Non posso riusare GetPizza()
                // perché il DbContext deve continuare a vivere
                // affinché possa accorgersi di quali modifiche deve salvare
                using PizzaContext db = new PizzaContext();
                var pizzaDaModificare = db.Pizzas.Where(p => p.Id == id).Include(p => p.Ingredients).FirstOrDefault();
                if (pizzaDaModificare == null)
                    return false;
                pizzaDaModificare.Name = pizza.Name;
                pizzaDaModificare.Description = pizza.Description;
                pizzaDaModificare.Price = pizza.Price;
                pizzaDaModificare.CategoryId = pizza.CategoryId;

                //Svuoto ingredients con la funzione Clear in modo da non aggiungere doppioni

                pizzaDaModificare.Ingredients.Clear();
                if (selectedIngredients != null)
                {
                    foreach (var ingredient in selectedIngredients)
                    {
                        int ingredientId = int.Parse(ingredient);
                        var ingredientFromDb = db.Ingredients.FirstOrDefault(x => x.Id == ingredientId);
                        if (ingredientFromDb != null)
                            pizzaDaModificare.Ingredients.Add(ingredientFromDb);
                    }
                }

                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //Versione della Update per Api
        //public static bool ModificaPizza(int id, string name, string description, string url, float price, int? categoryid, List<string> ingredients)
        //{
        //    using PizzaContext db = new PizzaContext();
        //    var pizza = db.Pizzas.Where(p => p.Id == id).Include(p => p.Ingredients).FirstOrDefault();

        //    if (pizza == null)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        pizza.Name = name;
        //        pizza.Description = description;
        //        pizza.Url = url;
        //        pizza.Price = price;
        //        pizza.CategoryId = categoryid;

        //        pizza.Ingredients.Clear();
        //        if (ingredients != null)
        //        {
        //            foreach (var ingrediente in ingredients)
        //            {
        //                int ingredienteId = int.Parse(ingrediente);
        //                var ingredienteDB = db.Ingredients.FirstOrDefault(x => x.Id == ingredienteId);

        //                if (ingredienteDB != null)
        //                {
        //                    pizza.Ingredients.Add(ingredienteDB);
        //                }
        //            }
        //        }

        //        db.SaveChanges();

        //        return true;
        //    }
        //}

        public static bool EditaPizzaApi(int id, string name, string description, string url, float price, int? categoryId, List<string> selectedIngredients)
        {
            try
            {
                using PizzaContext db = new PizzaContext();
                var pizzaDaModificare = db.Pizzas
                    .Where(p => p.Id == id)
                    .Include(p => p.Ingredients)
                    .FirstOrDefault();

                if (pizzaDaModificare == null)
                    return false;

                pizzaDaModificare.Name = name;
                pizzaDaModificare.Description = description;
                pizzaDaModificare.Url = url;
                pizzaDaModificare.Price = price;
                pizzaDaModificare.CategoryId = categoryId;

                pizzaDaModificare.Ingredients.Clear();
                if (selectedIngredients != null)
                {
                    foreach (var ingredientIdStr in selectedIngredients)
                    {
                        if (int.TryParse(ingredientIdStr, out int ingredientId))
                        {
                            var ingredientFromDb = db.Ingredients.FirstOrDefault(x => x.Id == ingredientId);
                            if (ingredientFromDb != null)
                            {
                                pizzaDaModificare.Ingredients.Add(ingredientFromDb);
                            }
                        }
                    }
                }

                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static bool PiallaPizza(int id)
        {
            try
            {
               //Qui recupero la pizza selezionata con il metodo Rec...NON creo un nuovo context del db perchè sta già lavorando quello
               //che utilizza RecuperaPizzaDaId...

                var pizzaDaCancellare = RecuperaPizzaDaId(id, false); // db.Pizzas.FirstOrDefault(p => p.Id == id);
                if (pizzaDaCancellare == null)
                    return false;

                //qui invece DEVO creare un nuovo contesto per essere sicuro che la rimozione dell'oggetto pizza venga tracciato dal DB'

                using PizzaContext db = new PizzaContext();
                db.Remove(pizzaDaCancellare);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public static void PopolaDB()
        {
            if (PizzaManager.ContaTuttelePizze() == 0)
            {
                //string name, string description, string url, float price

                PizzaManager.InserisciPizza(new Pizza("Margherita", "Farina 0 Pomodoro mozzarella olio basilico", "img/pizza-1.jpg", 5.5f) ,new());
                PizzaManager.InserisciPizza(new Pizza("Moenese", "Farina 0 Pomodoro Puzzone funghi olio basilico", "img/pizza-2.jpg", 5.5f),new());
                PizzaManager.InserisciPizza(new Pizza("Schmidt", "Pomodoro mozzarella uovo pancetta e cetriolini", "img/pizza-3.jpg" ,9.5f),new());
                PizzaManager.InserisciPizza(new Pizza("Capricciosa", "Farina 0 Pomodoro mozzarella prosciutto cotto carciofi funghi olive olio", "img/pizza-4.jpg", 8.0f) ,new());
                PizzaManager.InserisciPizza(new Pizza("Quattro Formaggi", "Farina 0 Mozzarella gorgonzola parmigiano fontina olio", "img/pizza-5.jpg", 7.5f) ,new());
                PizzaManager.InserisciPizza(new Pizza("Diavola", "Farina 0 Pomodoro mozzarella salame piccante olio basilico", "img/pizza-6.jpg", 6.5f) ,new());

            }

        }

        

        public static List<Category> GetAllCategories()
        {
            using PizzaContext db = new PizzaContext();
            return db.Categories.ToList();
        }
        public static List<Ingredient> GetAllIngredients()
        {
            using PizzaContext db = new PizzaContext();
            return db.Ingredients.ToList();
        }

        internal static void InserisciPizza(Pizza pizza)
        {
            throw new NotImplementedException();
        }
    }
}
