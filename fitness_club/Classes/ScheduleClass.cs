using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    public class ScheduleClass
    {
        private int id;
        private int coachId;
        private DateTime date;
        private TimeSpan time;
        private int fitnessServicesId;
        private string? coachFullName;
        private string? fitnessServiceName;
        public int Id
        {
            get { return id; }
            set
            {
                id = value;
            }
        }
        public int CoachId
        {
            get { return coachId; }
            set
            {
                coachId = value;
            }
        }
        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
            }
        }
        public TimeSpan Time
        {
            get { return time; }
            set
            {
                time = value;
            }
        }
        public int FitnessServicesId
        {
            get { return fitnessServicesId; }
            set
            {
                fitnessServicesId = value;
            }
        }
        public string? CoachFullName
        {
            get { return coachFullName; }
            set
            {
                coachFullName = value;
            }
        }
        public string? FitnessServiceName
        {
            get { return fitnessServiceName; }
            set
            {
                fitnessServiceName = value;
            }
        }


    }
}
