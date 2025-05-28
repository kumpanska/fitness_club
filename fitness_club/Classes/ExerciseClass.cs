using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    public class ExerciseClass
    {
        private int id;
        private string? name;
        private string? type;
        private int repetitions;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public string? NameOfExercise
        {
            get { return name; }
            set { name = value; }
        }
        public string? Type
        {
            get { return type; }
            set { type = value; }
        }
        public int Repetitions
        {
            get { return repetitions; }
            set { repetitions = value; }
        }
    }
}

