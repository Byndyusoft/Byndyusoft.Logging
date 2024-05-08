namespace Byndyusoft.Logging
{
    /// <summary>
    ///     Параметр структурного лога
    /// </summary>
    public class StructuredActivityEventItem
    {
        public StructuredActivityEventItem(string name, object value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        ///     Имя параметра
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Значение параметра
        /// </summary>
        public object Value { get; }
    }
}