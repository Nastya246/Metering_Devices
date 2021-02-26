using System.Web.Mvc;
using MeteringDevices.Models;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;
namespace MeteringDevices.Controllers
{
    public class TypeAndModelsController : Controller
    {
        private InstrumentationEntities db = new InstrumentationEntities();
        // GET: TypeAndModels
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ListTypeAndModelDevices()
        {

           var typeAndModels = await (db.Тип.Include(t => t.Модель)).ToListAsync();

         
            return View(typeAndModels);
        }
        public ActionResult CreateType()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateType([Bind(Include = "Тип1")] Тип type)
        {
            if (ModelState.IsValid)
            {
                
                var typeRepeat = await (from t in db.Тип where t.Тип1.Replace(" ", "").ToLower() == type.Тип1.Replace(" ", "").ToLower() select t).ToListAsync();
                if (typeRepeat.Count() == 0)
                {                 
                    db.Тип.Add(type);
                    await db.SaveChangesAsync();
                    
                }                                 
                
            }
            return RedirectToAction("ListTypeAndModelDevices");

        }
        public ActionResult CreateModel()
        {
            SelectList types = new SelectList(db.Тип, "Id_Type", "Тип1");
            ViewBag.types = types;
            return View();
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateModel([Bind(Include = "Id_type, Название_модели ")] Модель model)
        {
            if (ModelState.IsValid)
            {               
                var modelRepeat = await (from m in db.Модель where m.Название_модели.Replace(" ", "").ToLower() == model.Название_модели.Replace(" ", "").ToLower() && m.Id_type == model.Id_type select m).ToListAsync();
                if (modelRepeat.Count() == 0)
                {
                    var id = 0;
                    while (db.Модель.Find(id) != null)
                    {
                        id++;
                    }
                    model.Id_models = id;
                    db.Модель.Add(model);
                    await db.SaveChangesAsync();

                }

            }
            return RedirectToAction("ListTypeAndModelDevices");

        }
    }
}