namespace Moneyreservation.Contracts;

public interface MoneyUnreserved
{
    public Guid OrderId { get; set; }
    public int Amount { get; set; }
}