﻿using FlightPlanner.Core.Interfaces;
using FlightPlanner.Core.Models;

namespace Flight_Planner.Validations
{
    public class SameAirportValidator : IValidate
    {
        public bool IsValid(Flight flight)
        {
            return flight?.To?.AirportCode?.Trim()?.ToLower() != 
                   flight?.From?.AirportCode?.Trim()?.ToLower();
        }
    }
}
