namespace Byndyusoft.Logging
{
    /// <summary>
    ///     Параметр структурного лога
    /// </summary>
    public class StructuredActivityEventItem
    {
        public StructuredActivityEventItem(string name, object value, string description)
        {
            Name = name;
            Value = value;
            Description = description;
        }

        /// <summary>
        ///     Имя параметра
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Значение параметра
        /// </summary>
        public object Value { get; }

        public string Description { get; }
    }
}