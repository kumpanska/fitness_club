using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    public class ClientClass:Person
    {
        public override string FullName()
        {
            return $"Клієнт: {base.FullName()}";
        }
        public string FullNameText => FullName();
    }
}
