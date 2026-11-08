using System;
using RepairCenter.Events;
using RepairCenter.Helpers;
using RepairCenter.Interfaces;
 
namespace RepairCenter.Models
{
    /// <summary>
    /// Abstract base class for every appliance. Implements IRepairable and holds
    /// the shared data. Each concrete appliance must define its own repair cost.
    /// </summary>
    public abstract class HomeAppliance : IRepairable
    {
        // ---------- Private fields (Encapsulation) ----------
        private readonly string _serialNumber;          // fixed after creation
        private readonly DateTime _receivedDate;        // fixed after creation
        private readonly DateTime _expectedDeliveryDate; // fixed after creation
        private string _brand;
        private string _customerName;
        private RepairStatus _status;
        private double _repairHours;
        private decimal _partsCost;

        // ---------- Static members ----------
        private static int _totalRegisteredDevices;
        private static decimal _totalRevenue;

        /// <summary>Number of devices ever created in the system.</summary>
        public static int TotalRegisteredDevices => _totalRegisteredDevices;

        /// <summary>Sum of the cost of all repairs that were completed.</summary>
        public static decimal TotalRevenue => _totalRevenue;

        // ---------- Event ----------
        public event StatusChangedHandler StatusChanged;

        // ---------- Constructor ----------
        protected HomeAppliance(string serialNumber, string brand, string customerName,
                                DateTime receivedDate, DateTime expectedDeliveryDate)
        {
            if (!DeviceValidator.IsValidSerialNumber(serialNumber))
                throw new ArgumentException("Invalid serial number. Expected format: AA-123456.", nameof(serialNumber));
            if (!DeviceValidator.IsValidName(brand))
                throw new ArgumentException("Brand is required.", nameof(brand));
            if (!DeviceValidator.IsValidName(customerName))
                throw new ArgumentException("Customer name is required.", nameof(customerName));
            if (!DeviceValidator.IsValidDateRange(receivedDate, expectedDeliveryDate))
                throw new ArgumentException("Expected delivery date cannot be before the received date.", nameof(expectedDeliveryDate));

            _serialNumber = serialNumber;
            _brand = brand.Trim();
            _customerName = customerName.Trim();
            _receivedDate = receivedDate;
            _expectedDeliveryDate = expectedDeliveryDate;
            _status = RepairStatus.Received;

            _totalRegisteredDevices++;
        }

        // ---------- Properties ----------

        // Read-only after creation: no setter at all.
        public string SerialNumber => _serialNumber;
        public DateTime ReceivedDate => _receivedDate;
        public DateTime ExpectedDeliveryDate => _expectedDeliveryDate;

        public RepairStatus Status => _status;
        public double RepairHours => _repairHours;
        public decimal PartsCost => _partsCost;

        public string Brand
        {
            get => _brand;
            set
            {
                EnsureEditable();
                if (!DeviceValidator.IsValidName(value))
                    throw new ArgumentException("Brand is required.");
                _brand = value.Trim();
            }
        }

        public string CustomerName
        {
            get => _customerName;
            set
            {
                EnsureEditable();
                if (!DeviceValidator.IsValidName(value))
                    throw new ArgumentException("Customer name is required.");
                _customerName = value.Trim();
            }
        }

        /// <summary>Each concrete class names its own device type (Refrigerator, ...).</summary>
        public abstract string DeviceType { get; }

        // ---------- Controlled modification of repair data ----------
        public void AddRepairHours(double hours)
        {
            EnsureEditable();
            if (!DeviceValidator.IsPositive(hours))
                throw new ArgumentOutOfRangeException(nameof(hours), "Hours must be greater than zero.");
            _repairHours += hours;
        }

        public void AddSparePart(decimal cost)
        {
            EnsureEditable();
            if (cost <= 0)
                throw new ArgumentOutOfRangeException(nameof(cost), "Part cost must be greater than zero.");
            _partsCost += cost;
        }

        // ---------- IRepairable ----------
        public void StartRepair()
        {
            if (_status != RepairStatus.Received && _status != RepairStatus.WaitingForParts)
                throw new InvalidOperationException($"Cannot start repair while status is {_status}.");
            ChangeStatus(RepairStatus.InRepair);
        }

        public void WaitForParts()
        {
            if (_status != RepairStatus.InRepair)
                throw new InvalidOperationException($"Cannot wait for parts while status is {_status}.");
            ChangeStatus(RepairStatus.WaitingForParts);
        }

        public void CompleteRepair()
        {
            if (_status != RepairStatus.InRepair)
                throw new InvalidOperationException($"Cannot complete repair while status is {_status}.");
            ChangeStatus(RepairStatus.Completed);
            _totalRevenue += CalculateRepairCost();   // counted once: status is now Completed
        }

        public void DeliverToCustomer()
        {
            if (_status != RepairStatus.Completed)
                throw new InvalidOperationException($"Cannot deliver while status is {_status}.");
            ChangeStatus(RepairStatus.Delivered);
        }

        /// <summary>Abstract: every appliance type calculates its cost differently.</summary>
        public abstract decimal CalculateRepairCost();

        // ---------- Shared helpers ----------

        /// <summary>Labor + spare parts. Subclasses add their own extras on top.</summary>
        protected decimal CalculateBaseCost(decimal hourlyRate)
        {
            return (decimal)_repairHours * hourlyRate + _partsCost;
        }

        public virtual string GetDetails()
        {
            return $"[{DeviceType}] {_brand} | S/N: {_serialNumber} | Customer: {_customerName} | " +
                   $"Status: {_status} | Due: {_expectedDeliveryDate:yyyy-MM-dd} | Cost: {CalculateRepairCost():F2}";
        }

        protected virtual void OnStatusChanged(StatusChangedEventArgs e)
        {
            StatusChanged?.Invoke(this, e);
        }

        private void ChangeStatus(RepairStatus newStatus)
        {
            RepairStatus oldStatus = _status;
            _status = newStatus;
            OnStatusChanged(new StatusChangedEventArgs(this, oldStatus, newStatus));
        }

        /// <summary>Data of a finished device cannot be changed anymore.</summary>
        private void EnsureEditable()
        {
            if (_status == RepairStatus.Completed || _status == RepairStatus.Delivered)
                throw new InvalidOperationException("The repair is closed; data can no longer be modified.");
        }
    }
}
