using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string   LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }
        public string MiddleName
        {
            get { return middleName; }
            set { middleName = value; }
        }
        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public string FullName => $"{LastName} {Name} {MiddleName}";
    }
}
