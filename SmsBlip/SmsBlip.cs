using Android.Content;

namespace SmsBlip
{
    public class SmsBlip
    {
        public static ISmsListener SmsListener;

        public static void InitializeSmsBlipService (Context context, 
                                                      ISmsListener smsListener)
        {
            SmsListener = smsListener;
            var intent = new Intent (context, typeof(SmsBlipService));
            context.StartService(intent);
        }

        public static void StopSmsBlipService (Context context)
        {
            SmsListener = null;
            var intent = new Intent (context, typeof(SmsBlipService));
            context.StopService(intent);
        }
    }
}
