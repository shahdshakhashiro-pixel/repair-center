using System;

namespace RepairCenter.Models
{
    public class AirConditioner : HomeAppliance
    {
        private const decimal HourlyRate = 18m;
        private const decimal SiteVisitFee = 30m;

        private readonly bool _requiresSiteVisit;

        public AirConditioner(string serialNumber, string brand, string customerName,
                              DateTime receivedDate, DateTime expectedDeliveryDate,
                              bool requiresSiteVisit)
            : base(serialNumber, brand, customerName, receivedDate, expectedDeliveryDate)
        {
            _requiresSiteVisit = requiresSiteVisit;
        }

        public bool RequiresSiteVisit => _requiresSiteVisit;

        public override string DeviceType => "Air Conditioner";

        // Labor + parts + a visit fee when the unit is repaired at the customer's home.
        public override decimal CalculateRepairCost()
        {
            return CalculateBaseCost(HourlyRate) + (_requiresSiteVisit ? SiteVisitFee : 0m);
        }

        public override string GetDetails()
        {
            return base.GetDetails() + $" | Site visit: {(_requiresSiteVisit ? "Yes" : "No")}";
        }
    }
}
