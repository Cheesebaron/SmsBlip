using Android.Database;
using Java.Util;

namespace SmsBlip
{
    public class SmsCursorParser
    {
        const string AddressColumnName = "address";
        const string DateColumnName = "date";
        const string BodyColumnName = "body";
        const string TypeColumnName = "type";
        const string IdColumnName = "_id";
        const int SmsMaxAgeMillis = 5000;

        readonly ISmsStorage smsStorage;

        public SmsCursorParser (ISmsStorage smsStorage)
        {
            this.smsStorage = smsStorage;
        }

        public Sms Parse (ICursor cursor)
        {
            if (!CanHandleCursor (cursor) || !cursor.MoveToNext ())
                return null;

            var smsParsed = ExtractSmsInfoFromCursor (cursor);

            var smsId = cursor.GetInt (cursor.GetColumnIndex (IdColumnName));
            var date = cursor.GetString (cursor.GetColumnIndex (DateColumnName));
            var smsDate = new Date (long.Parse(date));

            if (ShouldParseSms (smsId, smsDate))
                UpdateLastSmsParsed (smsId);
            else
                smsParsed = null;

            return smsParsed;
        }

        void UpdateLastSmsParsed (int smsId) =>
            smsStorage.LastSmsIntercepted = smsId;

        bool ShouldParseSms (int smsId, Date smsDate)
        {
            bool isFirstSmsParsed = IsFirstSmsParsed ();
            bool isOld = IsOld (smsDate);
            bool shouldParseId = ShouldParseSmsId (smsId);
            return (isFirstSmsParsed && !isOld) || 
                (!isFirstSmsParsed && shouldParseId);
        }

        static bool IsOld (Date smsDate)
        {
            var now = new Date ();
            return now.Time - smsDate.Time > SmsMaxAgeMillis;
        }

        bool ShouldParseSmsId (int smsId)
        {
            if (smsStorage.IsFirstSmsIntercepted)
                return false;

            var lastSmsIdIntercepted = smsStorage.LastSmsIntercepted;
            return smsId > lastSmsIdIntercepted;
        }

        bool IsFirstSmsParsed () => 
            smsStorage.IsFirstSmsIntercepted;

        Sms ExtractSmsInfoFromCursor (ICursor cursor)
        {
            var address = cursor.GetString (cursor.GetColumnIndex (AddressColumnName));
            var date = cursor.GetString (cursor.GetColumnIndex (DateColumnName));
            var msg = cursor.GetString (cursor.GetColumnIndex (BodyColumnName));
            var type = cursor.GetString (cursor.GetColumnIndex (TypeColumnName));

            return new Sms (address, date, msg, (SmsType)int.Parse(type));
        }

        bool CanHandleCursor (ICursor cursor) => cursor?.Count > 0;
    }
}
