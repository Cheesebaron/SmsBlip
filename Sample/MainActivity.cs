using Android.App;
using Android.Widget;
using Android.OS;
using SmsBlip;
using System;

namespace Sample
{
    [Activity(Label = "Sample", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var startButton = FindViewById<Button>(Resource.Id.bt_start_service);
            var stopButton = FindViewById<Button>(Resource.Id.bt_stop_service);

            startButton.Click += (sender, e) => InitializeSmsBlipService();
            stopButton.Click += (sender, e) => StopSmsBlipService();
        }

        public class SmsListener : ISmsListener
        {
            public Action<Sms> ReceivedSms;
            public Action<Sms> SentSms;

            public void OnSmsReceived(Sms sms)
            {
                ReceivedSms?.Invoke(sms);
            }

            public void OnSmsSent(Sms sms)
            {
                SentSms?.Invoke(sms);
            }
        }

        void InitializeSmsBlipService()
        {
            var listener = new SmsListener();
            listener.ReceivedSms = ShowSmsToast;
            listener.SentSms = ShowSmsToast;

            SmsBlip.SmsBlip.InitializeSmsBlipService(this, listener);
        }

        void StopSmsBlipService()
        {
            SmsBlip.SmsBlip.StopSmsBlipService(this);
        }

        void ShowSmsToast(Sms sms)
        {
            Toast.MakeText(this, sms.ToString(), ToastLength.Long).Show();
        }
    }
}

