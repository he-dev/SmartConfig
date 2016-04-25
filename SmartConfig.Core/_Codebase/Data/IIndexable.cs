namespace SmartConfig.Data
{
    public interface IIndexable
    {
        // todo change type to object
        string this[string propertyName] { get; set; }
    }
}
