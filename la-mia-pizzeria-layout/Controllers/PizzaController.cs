using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using Test_MVC_2.Data;
using Test_MVC_2.Models;
namespace Test_MVC_2.Controllers
{

    public class PizzaController : Controller
    {
        //logger che viene utilizzato per registrare informazioni, avvisi ed errori relativi alle operazioni
        //del PizzaController.Il logger è iniettato nel controller tramite dependency injection, e il 
        //modificatore readonly garantisce che il logger venga assegnato una sola volta, in modo sicuro, 
        //nel costruttore del controller.

        private readonly ILogger<PizzaController> _logger;
        public PizzaController(ILogger<PizzaController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View(PizzaManager.RecuperaTutteLePizze());
        }

        [HttpGet]
        public IActionResult GetPizza(int id)
        {
            try
            {
                var pizza = PizzaManager.RecuperaPizzaDaId(id);
                if (pizza != null)
                    return View(pizza);
                else
                    //return NotFound();
                    return View("Errore", new ErroreViewModel($"La pizza {id} non è stata trovata!"));
            }
            catch (Exception e)
            {
                return View("Errore", new ErroreViewModel(e.Message));
                //return BadRequest(e.Message);
            }
        }

        [Authorize(Roles = "USER,ADMIN")]
        public IActionResult Show(int id)
        {
            var pizza = PizzaManager.RecuperaPizzaDaId(id);
            var categories = PizzaManager.GetAllCategories(); // Supponiamo che tu abbia un metodo per recuperare tutte le categorie
            var model = new PizzaFormModel(pizza, categories);
            model.CreateIngredients(); // Assicurati di popolare gli ingredienti
            return View(model);
        }

        [Authorize("ADMIN")]
        [HttpGet]
        public IActionResult Create() 
        {
            Pizza p = new Pizza("Inserisci nome", "Inserisci Descrizione", 52.3f);
            //Creo una List di Categories chiamando il metodo GetAll..

            List<Category> categories = PizzaManager.GetAllCategories();
            //Istanzio un oggetto pizzaFormModel che conterrà la pizza appena creata e la lista
            //di categories 
            PizzaFormModel model = new PizzaFormModel(p, categories);
            return View(model);
        }

        [Authorize("ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        //PeriodicTimer crea a DB la uova pizza gli passo un oggetto PizzaFormModel
        public IActionResult Create(PizzaFormModel pizzaDaInserire)
        {
            //Se i dati NON sono validi:

            if (ModelState.IsValid == false)
            {
                // Ritorno la form di prima con i dati della pizza
                // precompilati dall'utente
                pizzaDaInserire.Categories = PizzaManager.GetAllCategories();
                pizzaDaInserire.CreateIngredients();

                return View("Create", pizzaDaInserire);
            }
            //Altrimenti se SONO validi inserisco a db la nuova pizza

            PizzaManager.InserisciPizza(pizzaDaInserire.pizza, pizzaDaInserire.SelectedIngredients);

            // Richiamo la action Index affinché vengano mostrate tutte le pizze
            // inclusa quella nuova
            return RedirectToAction("Index");

        }


        [Authorize("ADMIN")]
        [HttpGet]

        public IActionResult Update(int id) // Restituisce la form per l'update di una pizza
        {
            //Recupero la pizza (id) e la assegno a var pizza 
            var pizza = PizzaManager.RecuperaPizzaDaId(id);

            if (pizza == null)
                return NotFound();
            //Popolo la form con i dati
            PizzaFormModel model = new PizzaFormModel(pizza, PizzaManager.GetAllCategories());
            model.CreateIngredients();
            return View(model);
        }

        [Authorize("ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int id, PizzaFormModel pizzaDaModificare) // Restituisce la form per la creazione di pizze
        {
            if (ModelState.IsValid == false)
            {
                // Ritorno la form di prima con i dati della pizza
                // precompilati dall'utente
                pizzaDaModificare.Categories = PizzaManager.GetAllCategories();
                pizzaDaModificare.CreateIngredients();
                return View("UpdatePizza", pizzaDaModificare);
            }

            var pizzamodificata = PizzaManager.EditaPizza(id, pizzaDaModificare.pizza, pizzaDaModificare.SelectedIngredients);
            if (pizzamodificata)
            {
                // Richiamiamo la action Index affinché vengano mostrate tutte le pizze
                return RedirectToAction("Index");
            }
            else
                return NotFound();

        }

        //Solo l'ADMIN può cancellare le pizze

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {

            var pizzaCancellata = PizzaManager.PiallaPizza(id);
            if (pizzaCancellata)
            {
                // Richiamiamo la action Index affinché vengano mostrate tutte le pizze
                return RedirectToAction("Index");
            }
            else
                return NotFound();
        }

        //Frammento scritto dall'insegnante...
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        //Duration = 0: Indica che la durata della cache è zero secondi.Questo significa che la risposta 
        //non deve essere memorizzata nella cache.

        //Location = ResponseCacheLocation.None: Specifica che la risposta non deve essere memorizzata 
        //nella cache né sul client né su nessun proxy intermedio.

        //NoStore = true: Indica che nessuna parte della risposta deve essere memorizzata.Insieme a 
        //Duration e Location, questo assicura che la risposta non venga mai memorizzata.

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
