using System;
using RepairCenter.Events;
using RepairCenter.Helpers;
using RepairCenter.Models;
using RepairCenter.Services;

namespace RepairCenter
{
    internal class Program
    {
        private static void Main()
        {
            DateTime today = DateTime.Today;
            var center = new RepairCenterService();

            // Subscribe to the events of the system.
            center.RepairCompleted += (sender, e) =>
                Console.WriteLine($"[EVENT] Repair completed: {e.Appliance.DeviceType} {e.Appliance.SerialNumber} " +
                                  $"({e.OldStatus} -> {e.NewStatus})");

            center.DeliveryDelayed += (sender, e) =>
                Console.WriteLine($"[EVENT] Delivery delayed: {e.Appliance.DeviceType} {e.Appliance.SerialNumber} " +
                                  $"is {e.DaysLate} day(s) late.");

            // Register different appliances in the same list.
            var fridge = new Refrigerator("RF-100001", "Samsung", "Ahmad Ali", today.AddDays(-3), today.AddDays(3), true);
            var washer = new WashingMachine("WM-200001", "LG", "Sara Hassan", today.AddDays(-2), today.AddDays(5), 8.5, false);
            var ac = new AirConditioner("AC-300001", "Gree", "Omar Khaled", today.AddDays(-4), today.AddDays(2), true);

            center.Register(fridge);
            center.Register(washer);
            center.Register(ac);

            // Repair work.
            fridge.StartRepair();
            fridge.AddRepairHours(3);
            fridge.AddSparePart(45m);

            washer.StartRepair();
            washer.AddRepairHours(2);
            washer.WaitForParts();          // waiting for a spare part
            washer.StartRepair();
            washer.AddSparePart(30m);
            washer.AddRepairHours(1.5);

            ac.StartRepair();
            ac.AddRepairHours(4);

            fridge.CompleteRepair();        // raises RepairCompleted
            washer.CompleteRepair();        // raises RepairCompleted

            // The air conditioner is still in repair, so a week later it is late.
            center.CheckDeliveryDelays(today.AddDays(7));

            Console.WriteLine();
            Console.WriteLine("--- All devices ---");
            center.PrintAllDevices();

            Console.WriteLine();
            Console.WriteLine($"Total cost of all devices : {center.CalculateTotalCost():F2}");
            Console.WriteLine($"Registered devices        : {HomeAppliance.TotalRegisteredDevices}");
            Console.WriteLine($"Revenue (completed)       : {HomeAppliance.TotalRevenue:F2}");
            Console.WriteLine($"Completed devices         : {RepairStatistics.CountByStatus(center.Appliances, RepairStatus.Completed)}");
            Console.WriteLine($"In-repair devices         : {RepairStatistics.CountByStatus(center.Appliances, RepairStatus.InRepair)}");

            // Encapsulation checks.
            Console.WriteLine();
            try { fridge.AddRepairHours(1); }
            catch (InvalidOperationException ex) { Console.WriteLine("Blocked: " + ex.Message); }

            try { new Refrigerator("123", "LG", "Test", today, today.AddDays(1), false); }
            catch (ArgumentException ex) { Console.WriteLine("Blocked: " + ex.Message); }
        }
    }
}
