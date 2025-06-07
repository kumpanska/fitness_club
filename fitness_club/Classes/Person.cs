using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace fitness_club.Classes
{
    public class Person
    {
        private int id;
        private string name = string.Empty;
        private string lastName = string.Empty;
        private string middleName = string.Empty;
        private string phoneNumber = string.Empty;
        private string email = string.Empty;
        public int Id
        {
            get { return id; }
            set {  id = value; }
        }
        [RegularExpression(@"^[А-ЯІЇЄ][а-яіїє]*(?:'[А-ЯІЇЄа-яіїє]+)?$",ErrorMessage= "Ім'я має містити тільки українські літери, перша літера повинна бути великою (опціонально букви розділені апострофом).")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        [RegularExpression(@"^[А-ЯІЇЄ][а-яіїє]*(?:'[А-ЯІЇЄа-яіїє]+)?$", ErrorMessage = "Прізвище має містити тільки українські літери, перша літера повинна бути великою (опціонально букви розділені апострофом).")]
        public string   LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }
        [RegularExpression(@"^[А-ЯІЇЄ][а-яіїє]*(?:'[А-ЯІЇЄа-яіїє]+)?$", ErrorMessage = "По батькові має містити тільки українські літери, перша літера повинна бути великою (опціонально букви розділені апострофом).")]
        public string MiddleName
        {
            get { return middleName; }
            set { middleName = value; }
        }
        [RegularExpression(@"^(?=(?:.*\d){10,15})[\s\d\+]+$", ErrorMessage = "Номер телефону повинен мати цифри, пробіли та знак '+'.")]
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }
        [RegularExpression(@"^[a-z\d]+\@[a-z]+\.[a-z]{2,}$", ErrorMessage = "Електронна пошта має містити лише малі латинські літери (опціонально з цифрами), одну крапку, один знак '@'.")]
        [StringLength(30,MinimumLength =6, ErrorMessage="Ім'я користувача електронної пошти містить від 6 до 30 символів")]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public virtual string FullName()
        { 
          return $"{LastName} {Name} {MiddleName}";
        }
        public virtual string FullNameText => FullName();
    }
}
