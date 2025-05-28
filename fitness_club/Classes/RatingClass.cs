using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    internal class RatingClass
    {
        private int id;
        private int coachId;
        private double averageMark;
        public int Id
        {
            get { return id; }
            set { id = value; }
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

    }
}
