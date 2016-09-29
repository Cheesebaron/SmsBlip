using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Java.Util;

namespace SmsBlip
{
    [Service]
    public class SmsBlipService : Service
    {
        const string ContentSmsUri = "content://sms";
        const int OneSecond = 1000;

        ContentResolver contentResolver;
        SmsObserver smsObserver;
        AlarmManager alarmManager;
        bool initialized;

        public override IBinder OnBind (Intent intent) => null;

        public override StartCommandResult OnStartCommand (Intent intent, 
            StartCommandFlags flags, int startId)
        {
            if (!initialized)
                InitializeService ();

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy ()
        {
            base.OnDestroy ();
            FinishService ();
        }

        public override void OnTaskRemoved (Intent rootIntent)
        {
            base.OnTaskRemoved (rootIntent);
            RestartService ();
        }

        void InitializeService ()
        {
            initialized = true;
            InitializeDependencies ();
            RegisterSmsContentObserver ();
        }

        void InitializeDependencies ()
        {
            if (!AreDependenciesInitialized ()) {
                InitializeContentResolver ();
                InitializeSmsObserver ();
            }
        }

        bool AreDependenciesInitialized ()
        {
            return contentResolver != null && smsObserver != null;
        }

        void InitializeSmsObserver ()
        {
            var handler = new Handler ();
            SmsCursorParser smsCursorParser = InitializeSmsCursorParser ();
            smsObserver = new SmsObserver (contentResolver, handler, 
                                           smsCursorParser);
        }

        SmsCursorParser InitializeSmsCursorParser ()
        {
            var preferences = 
                GetSharedPreferences ("sms_preferences", FileCreationMode.Private);
            var smsStorage = new SharedPreferencesSmsStorage (preferences);
            return new SmsCursorParser (smsStorage);
        }

        void InitializeContentResolver ()
        {
            contentResolver = ContentResolver;
        }

        void FinishService ()
        {
            initialized = false;
            UnregisterSmsContentObserver ();
        }

        void RegisterSmsContentObserver ()
        {
            var smsUri = Uri.Parse (ContentSmsUri);
            var notifyForDescendents = true;
            contentResolver.RegisterContentObserver (smsUri, 
                notifyForDescendents, smsObserver);
        }

        void UnregisterSmsContentObserver ()
        {
            contentResolver.UnregisterContentObserver (smsObserver);
        }

        void RestartService ()
        {
            var intent = new Intent (this, typeof(SmsBlipService));
            var pendingIntent = PendingIntent.GetService (this, 0, intent, 0);
            long now = new Date().Time;
            AlarmManager.Set (AlarmType.RtcWakeup, now + OneSecond, pendingIntent);
        }

        AlarmManager AlarmManager
            => alarmManager = alarmManager ?? 
                GetSystemService (AlarmService).JavaCast<AlarmManager> ();

        void SetSmsObserver (SmsObserver smsObserver)
        {
            this.smsObserver = smsObserver;
        }

        void SetContentResolver (ContentResolver contentResolver)
        {
            this.contentResolver = contentResolver;
        }

        void SetAlarmManager (AlarmManager alarmManager)
        {
            this.alarmManager = alarmManager;
        }
    }
}
