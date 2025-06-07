using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    public class ExerciseClass
    {
        private int id;
        private string name = string.Empty;
        private string type = string.Empty;
        private int repetitions;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        [RegularExpression(@"^[А-ЯІЇЄ][а-яіїє]*(?:'[А-ЯІЇЄа-яіїє]+)?$", ErrorMessage = "Назва вправи має містити тільки українські літери, перша літера повинна бути великою (опціонально букви розділені апострофом).")]
        public string NameOfExercise
        {
            get { return name; }
            set { name = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        [Range(1,80, ErrorMessage = "Кількість повторень вправи має бути від 1 до 80 повторень.")]
        public int Repetitions
        {
            get { return repetitions; }
            set { repetitions = value; }
        }
        public override string ToString()
        {
            return $"{NameOfExercise} - {Repetitions} повторень";
        }
    }
}

