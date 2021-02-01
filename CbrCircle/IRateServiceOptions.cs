namespace CbrCircle
{
    public interface IRateServiceOptions
    {
        string Api { get; set; }
        string Currency { get; set; }
        decimal Radius { get; set; }
    }
}