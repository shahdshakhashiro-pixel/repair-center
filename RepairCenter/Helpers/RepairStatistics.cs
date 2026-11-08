using System.Collections.Generic;
using System.Linq;
using RepairCenter.Models;

namespace RepairCenter.Helpers
{
    /// <summary>Static class: counts and totals grouped by repair status.</summary>
    public static class RepairStatistics
    {
        public static int CountByStatus(IEnumerable<HomeAppliance> appliances, RepairStatus status)
        {
            return appliances.Count(a => a.Status == status);
        }

        public static decimal TotalCostByStatus(IEnumerable<HomeAppliance> appliances, RepairStatus status)
        {
            return appliances.Where(a => a.Status == status).Sum(a => a.CalculateRepairCost());
        }
    }
}
