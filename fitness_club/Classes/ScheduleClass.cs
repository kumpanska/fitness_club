using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fitness_club.Classes
{
    public class ScheduleClass
    {
     public int Id { get; set; }
        public int CoachId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public int FitnessServicesId { get; set; }
        public string? CoachFullName { get; set; }
        public string? FitnessServiceName { get; set; }

      
    }
}
