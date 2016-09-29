namespace SmsBlip
{
    public interface ISmsStorage
    {
        int LastSmsIntercepted { get; set; }
        bool IsFirstSmsIntercepted { get; }
    }
}
