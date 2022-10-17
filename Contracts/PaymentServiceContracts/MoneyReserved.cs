namespace Moneyreservation.Contracts;

public interface MoneyReserved
{
    public Guid OrderId { get; set; }
    public int Amount { get; set; }
}