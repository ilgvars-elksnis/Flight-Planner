using FlightPlanner.Core.Interfaces;
using FlightPlanner.Core.Models;

namespace FlightPlanner.Validation
{
    public class SearchEmptyValidation : IStringValidation
    {
        public bool IsValid(object entity)
        {
            if (entity is string search)
            {
                return !string.IsNullOrEmpty(search);
            }
            return false;
        }

    }
}