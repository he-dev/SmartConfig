namespace SmartConfig.Data
{
    public interface IIndexable
    {
        string this[string propertyName] { get; set; }
    }
}
