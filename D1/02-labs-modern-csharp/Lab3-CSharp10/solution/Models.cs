namespace Lab3.Models;

public class Customer
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public override string ToString() => $"{Name} ({Email})";
}
