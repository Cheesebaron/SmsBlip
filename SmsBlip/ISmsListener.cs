namespace SmsBlip
{
    public interface ISmsListener
    {
        void OnSmsSent (Sms sms);
        void OnSmsReceived (Sms sms);
    }
}
