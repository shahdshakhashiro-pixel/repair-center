using RepairCenter.Models;
 
namespace RepairCenter.Interfaces
{
    /// <summary>
    /// Basic operations shared by every device that can be repaired.
    /// </summary>
    public interface IRepairable
    {
        RepairStatus Status { get; }

        void StartRepair();
        void WaitForParts();
        void CompleteRepair();
        void DeliverToCustomer();

        decimal CalculateRepairCost();
    }
}
