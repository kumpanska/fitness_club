using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    public class RatingClass:Person
    {
        private int ratingId;
        private int coachId;
        private double averageMark;
        private int number;
        public int RatingId
        {
            get { return ratingId; }
            set { ratingId = value; }
        }
        public int CoachId
        {
            get { return coachId; }
            set { coachId = value; }
        }
        public double AverageMark
        {
            get { return averageMark; }
            set { averageMark = value; }
        }
        public string CoachFullName
        {
            get { return $"{LastName} {Name} {MiddleName}"; }
        }
        public int Number
        {
            get { return number; }
            set { number = value; }
        }

    }
}
