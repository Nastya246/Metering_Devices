//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------


namespace MeteringDevices.Models
{
    using System.ComponentModel.DataAnnotations;
    using System;
    using System.Collections.Generic;
    
    public partial class Прибор
    {
        public int Id_device { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения!")]
        public Nullable<int> Инвентарный_номер { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения!")]
        public int Id_models { get; set; }
        public Nullable<System.DateTime> Дата_ввода_в_экслуатацию { get; set; }
        public Nullable<System.DateTime> Дата_поверки { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения!")]
        public string Фамилия_ответственного { get; set; }
    
        public virtual Модель Модель { get; set; }
    }
}
