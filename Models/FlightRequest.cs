using FlightPlanner.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Flight_Planner.Models
{
    public class FlightRequest
    {
        public int ID { get; set; }
        public AirportRequest From { get; set; }
        public AirportRequest To { get; set; }
        [StringLength(100)]
        public string Carrier { get; set; }
        [StringLength(100)]
        public string DepartureTime { get; set; }
        [StringLength(100)]
        public string ArrivalTime { get; set; }
    }
}
