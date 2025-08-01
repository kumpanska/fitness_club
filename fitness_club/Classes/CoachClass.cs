﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    public class CoachClass : Person
    {
        private string fitnessServices = string.Empty;
        public string FitnessServices
        {
            get { return fitnessServices; }
            set { fitnessServices = value; }
        }
        public override string FullName()
        {
            return $"Тренер: {base.FullName()}";
        }
        public override string FullNameText => FullName();

    }
}
