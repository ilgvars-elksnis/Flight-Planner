using FlightPlanner.Core.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace FlightPlanner.Validation
{
    public class NoResultsValidation : IStringValidation
    {
        public bool IsValid(object entity)
        {
            if (entity is bool hasResults)
            {
                return hasResults;
            }
            return false;
        }
    }
}
