using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club
{
    class Person
    {
        private int id;
        private string? name;
        private string? lastName;
        private string? phoneNumber;
        private string? email;
        public int Id
        {
            get { return id; }
            set {  id = value; }
        }
        public string? Name 
        {
            get { return name; }
            set { name = value; }
        }
        public string? LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }
        public string? PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }
        public string? Email
        {
            get { return email; }
            set { email = value; }
        }
    }
}
