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
            int selectedIndex = 1;


            SelectList types = new SelectList(db.Тип, "Id_Type", "Тип1", selectedIndex); //для передачи в представление списка типов приборов
            ViewBag.types = types;

            List<Модель> ModelsList = new List<Модель>(); // для передачи в представление списка моделей

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
            SelectList models = new SelectList(ModelsList, "Id_models", "Название_модели");
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

            return RedirectToAction("ListDevices");

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


        public async Task<ActionResult> EditDevice(int? id)
        {
            if (id != null)
            {
                var device = await db.Прибор.FindAsync(id);
                if (device == null) return RedirectToAction("ListDevices"); // ошибку вернуть

                var modelsEl = await db.Модель.FindAsync(device.Id_models);
                int selectedIndex = modelsEl.Id_type;
                var typeList = db.Тип.ToList();
                SelectList types = new SelectList(typeList, "Id_Type", "Тип1", selectedIndex);
                ViewBag.types = types;

                List<Модель> ModelsList = new List<Модель>();


                foreach (var m in db.Модель)
                {
                    if (m.Id_type == selectedIndex)
                    {
                        ModelsList.Add(m);
                    }

                }
                SelectList models = new SelectList(ModelsList, "Id_models", "Название_модели", modelsEl.Id_models);
                ViewBag.models = models;

                return View(device);

            }
                return RedirectToAction("ListDevices");
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDevice([Bind(Include = "Учетный_номер, Инвентарный_номер, Id_models, Дата_ввода_в_экслуатацию, Дата_поверки, Фамилия_ответственного")] Прибор device, int models)
        {

            if (ModelState.IsValid)
            {
                
                    device.Id_models = models;
                    db.Прибор.Add(device);
                    db.Entry(device).State = EntityState.Modified;
                   await db.SaveChangesAsync();
                
            }
            return RedirectToAction("ListDevices");
        }
    }
}