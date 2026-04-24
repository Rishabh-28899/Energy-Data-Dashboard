using System.ComponentModel.DataAnnotations;


namespace Emergy_report.models

{
    public class Emerguapp
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }           // Date column
        public int Block { get; set; }               // 15-minute time block (1-96)
        public string? StartTime { get; set; }        // Start Time (e.g., 0:00)
        public string? EndTime { get; set; }          // End Time (e.g., 0:15)
        public double FrequencyHz { get; set; }      // Freq Hz
        public double ActualGeneration { get; set; } // AG (MW)
        public double DeclaredCapacity { get; set; } // DC (MW)
        public double ScheduledGeneration { get; set; } // SG (MW)
        public double AgcAdjustment { get; set; }    // AGC (MW)
        public double OverInjection { get; set; }    // Over-Inj (MW)
        public double UnderInjection { get; set; }   // Under-Inj (MW)
        public decimal TotalCharge { get; set; }     // Total Charge (Monetary value)
    }
    }

