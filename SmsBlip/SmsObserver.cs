using Android.Content;
using Android.Database;
using Android.Net;
using Android.OS;

namespace SmsBlip
{
    public class SmsObserver : ContentObserver
    {
        static Uri SmsUri = Uri.Parse("content://sms/");
        static Uri SmsSentUri = Uri.Parse("content://sms/sent");
        static Uri SmsInboxUri = Uri.Parse("content://sms/inbox");
        const string ProtocolColumnName = "protocol";
        const string SmsOrder = "date DESC";

        ContentResolver contentResolver;
        SmsCursorParser smsCursorParser;

        public SmsObserver (ContentResolver contentResolver, Handler handler, 
                            SmsCursorParser smsCursorParser) : base(handler)
        {
            this.contentResolver = contentResolver;
            this.smsCursorParser = smsCursorParser;
        }

        public override bool DeliverSelfNotifications () => true;

        public override void OnChange (bool selfChange)
        {
            base.OnChange (selfChange);

            ICursor cursor = null;
            try {
                cursor = GetSmsContentObserverCursor ();
                if (cursor != null && cursor.MoveToFirst ()) {
                    ProcessSms (cursor);
                }
            } finally {
                Close (cursor);
            }
        }

        void ProcessSms (ICursor cursor)
        {
            ICursor smsCursor = null;
            try {
                string protocol = cursor.GetString (
                    cursor.GetColumnIndex (ProtocolColumnName));
                smsCursor = GetSmsDetailsCursor (protocol);
                var sms = ParseSms (smsCursor);
                NotifySmsListener (sms);
            } finally {
                Close (smsCursor);
            }
        }

        void NotifySmsListener (Sms sms)
        {
            if (sms != null) {
                if (SmsType.Sent == sms.Type) {
                    SmsBlip.SmsListener?.OnSmsSent (sms);
                } else {
                    SmsBlip.SmsListener?.OnSmsReceived (sms);
                }
            }
        }

        ICursor GetSmsDetailsCursor (string protocol)
        {
            ICursor smsCursor;
            if (IsProtocolForOutgoingSms (protocol)) {
                //SMS Sent
                smsCursor = GetSmsDetailsCursor (SmsSentUri);
            } else {
                //SMS Received
                smsCursor = GetSmsDetailsCursor (SmsInboxUri);
            }
            return smsCursor;
        }

        ICursor GetSmsContentObserverCursor ()
        {
            string[] projection = null;
            string selection = null;
            string[] selectionArgs = null;
            string sortOrder = null;
            return contentResolver.Query (SmsUri, projection, selection, 
                                          selectionArgs, sortOrder);
        }

        bool IsProtocolForOutgoingSms (string protocol) => protocol == null;

        ICursor GetSmsDetailsCursor (Uri smsUri)
        {
            if (smsUri == null) return null;
            return contentResolver.Query (smsUri, null, null, null, SmsOrder);
        }

        Sms ParseSms (ICursor cursor) => smsCursorParser.Parse (cursor);

        void Close (ICursor cursor)
        {
            cursor?.Close ();
        }
    }
}
