using System;
 
namespace RepairCenter.Models
{
    public class Refrigerator : HomeAppliance
    {
        private const decimal HourlyRate = 15m;
        private const decimal GasRefillFee = 25m;

        private readonly bool _needsGasRefill;

        public Refrigerator(string serialNumber, string brand, string customerName,
                            DateTime receivedDate, DateTime expectedDeliveryDate,
                            bool needsGasRefill)
            : base(serialNumber, brand, customerName, receivedDate, expectedDeliveryDate)
        {
            _needsGasRefill = needsGasRefill;
        }

        public bool NeedsGasRefill => _needsGasRefill;

        public override string DeviceType => "Refrigerator";

        // Labor + parts + a fixed fee when the cooling gas must be refilled.
        public override decimal CalculateRepairCost()
        {
            return CalculateBaseCost(HourlyRate) + (_needsGasRefill ? GasRefillFee : 0m);
        }

        public override string GetDetails()
        {
            return base.GetDetails() + $" | Gas refill: {(_needsGasRefill ? "Yes" : "No")}";
        }
    }
}
