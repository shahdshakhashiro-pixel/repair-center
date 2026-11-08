namespace RepairCenter.Models
{
    /// <summary>Lifecycle states of a device inside the repair center.</summary>
    public enum RepairStatus
    {
        Received,        // device registered, repair not started
        InRepair,        // technician is working on it
        WaitingForParts, // repair paused until spare parts arrive
        Completed,       // repair finished, waiting for the customer
        Delivered        // handed over to the customer
    }
}
