// TODO: Convert this traditional namespace block to a file-scoped namespace
//       Replace the block with: namespace Lab3.Models;
//       Then remove the extra indentation level

namespace Lab3.Models
{
    public class Customer
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public override string ToString() => $"{Name} ({Email})";
    }
}
