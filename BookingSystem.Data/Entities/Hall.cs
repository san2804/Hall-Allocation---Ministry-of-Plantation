namespace BookingSystem.Data.Entities;

public class Hall
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Facilities { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
