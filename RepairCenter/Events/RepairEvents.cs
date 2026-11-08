using System;
using RepairCenter.Models;

namespace RepairCenter.Events
{
    /// <summary>Data sent with every status-change notification.</summary>
    public class StatusChangedEventArgs : EventArgs
    {
        public HomeAppliance Appliance { get; }
        public RepairStatus OldStatus { get; }
        public RepairStatus NewStatus { get; }
        public DateTime ChangedAt { get; }

        public StatusChangedEventArgs(HomeAppliance appliance, RepairStatus oldStatus, RepairStatus newStatus)
        {
            Appliance = appliance;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            ChangedAt = DateTime.Now;
        }
    }

    /// <summary>Data sent when a device passes its promised delivery date.</summary>
    public class DeliveryDelayedEventArgs : EventArgs
    {
        public HomeAppliance Appliance { get; }
        public int DaysLate { get; }

        public DeliveryDelayedEventArgs(HomeAppliance appliance, int daysLate)
        {
            Appliance = appliance;
            DaysLate = daysLate;
        }
    }

    // Delegates: define the signature of the methods that can subscribe to the events.
    public delegate void StatusChangedHandler(object sender, StatusChangedEventArgs e);
    public delegate void DeliveryDelayedHandler(object sender, DeliveryDelayedEventArgs e);
}
