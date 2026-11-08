using System;
using RepairCenter.Helpers;

namespace RepairCenter.Models
{
    public class WashingMachine : HomeAppliance
    {
        private const decimal HourlyRate = 12m;
        private const decimal DrumReplacementFee = 40m;

        private readonly double _loadCapacityKg;
        private readonly bool _needsDrumReplacement;

        public WashingMachine(string serialNumber, string brand, string customerName,
                              DateTime receivedDate, DateTime expectedDeliveryDate,
                              double loadCapacityKg, bool needsDrumReplacement)
            : base(serialNumber, brand, customerName, receivedDate, expectedDeliveryDate)
        {
            if (!DeviceValidator.IsPositive(loadCapacityKg))
                throw new ArgumentOutOfRangeException(nameof(loadCapacityKg), "Capacity must be greater than zero.");

            _loadCapacityKg = loadCapacityKg;
            _needsDrumReplacement = needsDrumReplacement;
        }

        public double LoadCapacityKg => _loadCapacityKg;
        public bool NeedsDrumReplacement => _needsDrumReplacement;

        public override string DeviceType => "Washing Machine";

        // Labor + parts + a fixed fee when the drum must be replaced.
        public override decimal CalculateRepairCost()
        {
            return CalculateBaseCost(HourlyRate) + (_needsDrumReplacement ? DrumReplacementFee : 0m);
        }

        public override string GetDetails()
        {
            return base.GetDetails() + $" | Capacity: {_loadCapacityKg} kg";
        }
    }
}
