using System.Collections.Generic;
using MeteringDevices.Models;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net.Mail;

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
            var devices = await db.Прибор.OrderBy(d => d.Id_device).ToListAsync();
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
        public async Task<ActionResult> AddDevice([Bind(Include = "Инвентарный_номер,  Фамилия_ответственного")] Прибор device, int? models, System.DateTime? dateCheck, System.DateTime? dateExp)
        {

            if (ModelState.IsValid && models != null)
            {
                var position = await (from d in db.Прибор where d.Инвентарный_номер == device.Инвентарный_номер select d).ToListAsync();
                if (position.Count() == 0)
                {
                    
                    device.Id_models = (int)models;
                    if (dateCheck != null) device.Дата_поверки = dateCheck;
                    if (dateExp != null) device.Дата_ввода_в_экслуатацию = dateExp;
                    db.Прибор.Add(device);
                    await db.SaveChangesAsync();
                    return RedirectToAction("ListDevices");
                }
            }
            
            return View("~/Views/Home/Error.cshtml");

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
                string dataExp = "", dataCheck = "";
                var modelsEl = await db.Модель.FindAsync(device.Id_models);

                List<int> idType = new List<int>();
                List<string> nameType = new List<string>();

                List<int> idModel = new List<int>();
                List<string> nameModel = new List<string>();
                foreach (var el in db.Тип)
                {
                    idType.Add(el.Id_Type);
                    nameType.Add(el.Тип1);
                }
                ViewBag.idType = idType;
                ViewBag.nameType = nameType;

                foreach (var el in db.Модель)
                {
                    idModel.Add(el.Id_models);
                    nameModel.Add(el.Название_модели);
                }
                ViewBag.idModel = idModel;
                ViewBag.nameModel = nameModel;

                ViewBag.selectModel = device.Id_models;
                ViewBag.selectType = device.Модель.Id_type;

                ViewBag.countType = db.Тип.Count();
                ViewBag.countModel = db.Модель.Count();
                //int selectedIndex = modelsEl.Id_type;
                //var typeList = db.Тип.ToList();
                //SelectList types = new SelectList(typeList, "Id_Type", "Тип1", selectedIndex);
                //ViewBag.types = types;

                //List<Модель> ModelsList = new List<Модель>();


                //foreach (var m in db.Модель)
                //{
                //    if (m.Id_type == selectedIndex)
                //    {
                //        ModelsList.Add(m);
                //    }

                //}
                //SelectList models = new SelectList(ModelsList, "Id_models", "Название_модели", modelsEl.Id_models);
                //ViewBag.models = models;

                if (device.Дата_ввода_в_экслуатацию != null)
                {
                    dataExp = device.Дата_ввода_в_экслуатацию.ToString();
                    dataExp = dataExp.Remove(10);
                    string[] dataExpArr = dataExp.Split('.');
                    dataExp = "";
                    dataExp = dataExpArr[2] + "-" + dataExpArr[1] + "-" + dataExpArr[0];
                }

           
                if (device.Дата_поверки != null)
                {
                    dataCheck = device.Дата_поверки.ToString();
                    dataCheck = dataCheck.Remove(10);
                    string[] dataCheckArr = dataCheck.Split('.');
                    dataCheck = "";
                    dataCheck = dataCheckArr[2] + "-" + dataCheckArr[1] + "-" + dataCheckArr[0];
                }

                ViewBag.dataExp = dataExp;
                ViewBag.dataCheck = dataCheck;

                return View(device);

            }
                return RedirectToAction("ListDevices");
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDevice([Bind(Include = "Id_device, Инвентарный_номер,  Фамилия_ответственного")] Прибор device, int models, System.DateTime? dateCheck, System.DateTime? dateExp)
        {

            if (ModelState.IsValid)
            {
                
                   device.Id_models = models;
                   if (dateCheck != null) device.Дата_поверки = dateCheck;
                   if (dateExp != null) device.Дата_ввода_в_экслуатацию = dateExp;

                db.Entry(device).State = EntityState.Modified;
                   await db.SaveChangesAsync();
                
            }
            return RedirectToAction("ListDevices");
        }

        public async Task<ActionResult> DeleteDevice(int? id)
        {
            if (id != null)
            {
                var device = await db.Прибор.FindAsync(id);
                return View(device);
            }
            return RedirectToAction("ListDevices");
        }

        [HttpPost, ActionName("DeleteDevice")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteDevice(int id)
        {
            var device = await db.Прибор.FindAsync(id);                     
            db.Прибор.Remove(device);
                           
            await db.SaveChangesAsync();

            return RedirectToAction("ListDevices");
        }

        //public async Task<ActionResult> SendMessage(int? id)
        //{
        //    if (id != null)
        //    {
        //        var device = await db.Прибор.FindAsync(id);
        //        ViewBag.mail = device.Фамилия_ответственного;
        //        return View(device);
        //    }
        //    return RedirectToAction("ListDevices");
        //}

        //[HttpPost, ActionName("SendMessage")]
        //public async Task<ActionResult> SendMessage(string mail, string message, string senderName)
        //{
        //    string pattern = @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
        //        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$";
                           
        //        if (Regex.IsMatch(mail, pattern, RegexOptions.IgnoreCase)) // корректный адрес
        //        {
        //        // отправитель - устанавливаем адрес и отображаемое в письме имя
        //        MailAddress from = new MailAddress("somemail@gmail.com", senderName);
        //        // кому отправляем
        //        MailAddress to = new MailAddress(mail);
        //        // создаем объект сообщения
        //        MailMessage m = new MailMessage(from, to);
        //        // тема письма
        //        m.Subject = "Сообщение о проблеме оборудования";
        //        // текст письма
        //        m.Body = "<h2>" + message + "</h2>";
        //        // письмо представляет код html
        //        m.IsBodyHtml = true;
        //        // адрес smtp-сервера и порт, с которого будем отправлять письмо
        //        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
        //        // логин и пароль
        //        smtp.Credentials = new NetworkCredential("somemail@gmail.com", "mypassword");
        //        smtp.EnableSsl = true;
        //        smtp.Send(m);
                

        //    }
        //        else // некорректный адрес
        //        {
                    
        //        }
            
        //    return RedirectToAction("ListDevices");
        //}
    }
}