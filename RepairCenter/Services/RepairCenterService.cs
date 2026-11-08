using System;
using System.Collections.Generic;
using System.Linq;
using RepairCenter.Events;
using RepairCenter.Interfaces;
using RepairCenter.Models;

namespace RepairCenter.Services
{
    /// <summary>
    /// Main class: keeps a List of appliances and handles them polymorphically.
    /// It never needs to know the concrete type (Refrigerator, WashingMachine, ...).
    /// </summary>
    public class RepairCenterService
    {
        private readonly List<HomeAppliance> _appliances = new List<HomeAppliance>();

        public event StatusChangedHandler RepairCompleted;
        public event DeliveryDelayedHandler DeliveryDelayed;

        public IReadOnlyList<HomeAppliance> Appliances => _appliances.AsReadOnly();

        public void Register(HomeAppliance appliance)
        {
            if (appliance == null)
                throw new ArgumentNullException(nameof(appliance));
            if (_appliances.Any(a => a.SerialNumber == appliance.SerialNumber))
                throw new InvalidOperationException($"Device {appliance.SerialNumber} is already registered.");

            _appliances.Add(appliance);
            appliance.StatusChanged += OnApplianceStatusChanged;   // subscribe to the device event
        }

        // Handler: raise RepairCompleted when a device reaches the Completed status.
        private void OnApplianceStatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.NewStatus == RepairStatus.Completed)
                RepairCompleted?.Invoke(this, e);
        }

        /// <summary>Raises DeliveryDelayed for every unfinished device past its promised date.</summary>
        public void CheckDeliveryDelays(DateTime today)
        {
            foreach (HomeAppliance appliance in _appliances)
            {
                bool unfinished = appliance.Status != RepairStatus.Completed &&
                                  appliance.Status != RepairStatus.Delivered;

                if (unfinished && today.Date > appliance.ExpectedDeliveryDate.Date)
                {
                    int daysLate = (today.Date - appliance.ExpectedDeliveryDate.Date).Days;
                    DeliveryDelayed?.Invoke(this, new DeliveryDelayedEventArgs(appliance, daysLate));
                }
            }
        }

        // Polymorphism: each item calls its own CalculateRepairCost().
        public decimal CalculateTotalCost()
        {
            decimal total = 0m;
            foreach (IRepairable item in _appliances)
                total += item.CalculateRepairCost();
            return total;
        }

        // Polymorphism: each item calls its own GetDetails().
        public void PrintAllDevices()
        {
            foreach (HomeAppliance appliance in _appliances)
                Console.WriteLine(appliance.GetDetails());
        }

        public IEnumerable<HomeAppliance> GetByStatus(RepairStatus status)
        {
            return _appliances.Where(a => a.Status == status);
        }

        public HomeAppliance FindBySerialNumber(string serialNumber)
        {
            return _appliances.FirstOrDefault(a => a.SerialNumber == serialNumber);
        }
    }
}
