namespace Byndyusoft.Logging
{
    public class StructuredActivityEventItem
    {
        public string Name { get; }

        public object Value { get; }

        public string Description { get; }

        public StructuredActivityEventItem(string name, object value, string description)
        {
            Name = name;
            Value = value;
            Description = description;
        }
    }
}