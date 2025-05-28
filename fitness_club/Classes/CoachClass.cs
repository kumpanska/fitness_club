using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club
{
    public class CoachClass : Person
    {
        private string? fitnessServices;
        public string? FitnessServices
        {
            get { return fitnessServices; }
            set { fitnessServices = value; }
        }

    }
}
