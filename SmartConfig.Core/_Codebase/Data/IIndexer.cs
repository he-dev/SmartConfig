namespace SmartConfig.Data
{
    public interface IIndexer
    {
        string this[string propertyName] { get; set; }
    }
}
