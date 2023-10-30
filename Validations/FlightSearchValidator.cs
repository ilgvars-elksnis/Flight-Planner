using FlightPlanner.Core.Interfaces;
using FlightPlanner.Core.Models;

namespace Flight_Planner.Validations
{
    public class FlightSearchValidator : IFlightSearchValidator
    {
        public bool IsValid(SearchFlightsRequest request)
        {
            if (string.IsNullOrEmpty(request.From) || string.IsNullOrEmpty(request.To) || request.From == request.To)
            {
                return false;
            }

            return true;
        }
    }
}
