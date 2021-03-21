using System.Web.Mvc;
using MeteringDevices.Models;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;
using System.Net;
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

           var typeAndModels = await (db.Тип.OrderBy(t => t.Тип1).Include(t => t.Модель)).ToListAsync();
           

         
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
                    return RedirectToAction("ListTypeAndModelDevices");
                }
               
            }
            return View("~/Views/Home/Error.cshtml");
           

        }
        public ActionResult CreateModel()
        {
            int countType = db.Тип.Count();
            if (countType != 0)
            {
                SelectList types = new SelectList(db.Тип, "Id_Type", "Тип1");
                ViewBag.types = types;
            }
            else
            {
                ViewBag.err = 1;  
            }
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
                    return RedirectToAction("ListTypeAndModelDevices");
                }
               
            }
            return View("~/Views/Home/Error.cshtml");
           

        }
   

        public async Task<ActionResult> DeleteType(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var type = await db.Тип.FindAsync(id);
            if (type == null)
            {
                return HttpNotFound();
            }
            return View(type);
        }

     
        [HttpPost, ActionName("DeleteType")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteType(int id)
        {
            var type = await db.Тип.FindAsync(id); 
            var models = await (from m in db.Модель where m.Id_type == id select m).ToListAsync(); 
            foreach (var el in models)
            {
                var resultDelete = await (from d in db.Прибор where d.Id_models == el.Id_models select d).ToListAsync(); 
                foreach (var device in resultDelete)
                {
                    db.Прибор.Remove(device);
                }
            }

            foreach (var model in models)
            {
                db.Модель.Remove(model);
            }

            db.Тип.Remove(type); 
            await db.SaveChangesAsync();

            return RedirectToAction("ListTypeAndModelDevices");
        }


        public async Task<ActionResult> DeleteModel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var type = await db.Модель.FindAsync(id);
            if (type == null)
            {
                return HttpNotFound();
            }
            return View(type);
        }

        // POST: Разделы/Delete/5
        [HttpPost, ActionName("DeleteModel")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteModel(int id)
        {
            var model = await db.Модель.FindAsync(id);
            var deviceOfModel = await (from d in db.Прибор where d.Id_models == id select d).ToListAsync();
            foreach (var el in deviceOfModel)
            {                             
               db.Прибор.Remove(el);
                
            }
           
            db.Модель.Remove(model);
            await db.SaveChangesAsync();

            return RedirectToAction("ListTypeAndModelDevices");
        }

        
    }
}