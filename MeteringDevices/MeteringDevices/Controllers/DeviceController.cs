using System.Collections.Generic;
using MeteringDevices.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;

namespace MeteringDevices.Controllers
{
   
    public class DeviceController : Controller
    {
        private InstrumentationEntities db = new InstrumentationEntities();

        // GET: Device
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ListDevices()
        {
            var devices = await db.Прибор.OrderBy(d => d.Модель.Название_модели).ToListAsync();
            return View(devices);
        }

        public ActionResult AddDevice()
        {
            //выпадающий список
            int selectedIndex = 0;


            SelectList types = new SelectList(db.Тип, "Id_Type", "Тип1", selectedIndex); //для передачи в представление списка подразделений
            ViewBag.types = types;

            List<Модель> ModelsList = db.Модель.ToList(); // для передачи в представление списка должностей
            ModelsList.Clear();
            foreach (var t in db.Тип.Include(t => t.Модель))
            {
                if (t.Id_Type == selectedIndex)
                {
                    foreach (var m in t.Модель)
                    {

                        ModelsList.Add(m);
                    }
                }
            }
            SelectList models = new SelectList(ModelsList, "id_Model", "Модель1");
            ViewBag.models = models;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddDevice([Bind(Include = "Учетный_номер, Инвентарный_номер, Id_models, Дата_ввода_в_экслуатацию, Дата_поверки, Фамилия_ответственного")] Прибор device, int models)
        {
           
            if (ModelState.IsValid)
            {
                var position = await (from d in db.Прибор where d.Инвентарный_номер == device.Инвентарный_номер select d).ToListAsync();
                if (position.Count() == 0)
                {
                    device.Id_models = models;
                    db.Прибор.Add(device);
                    await db.SaveChangesAsync();
                }
            }

            return RedirectToAction("ListDevices"); // Список вернуть!!!

        }
        public async Task<ActionResult> GetModels(int id)
        {

            List<Модель> modelsList1 = new List<Модель>();
            var type = await (db.Тип.Include(t => t.Модель)).ToListAsync();
            foreach (var t in type)
            {
                if (t.Id_Type == id)
                {
                    foreach (var m in t.Модель)
                    {
                        modelsList1.Add(m);

                    }
                }
            }

            return PartialView(modelsList1.ToList());
        }
    }
}