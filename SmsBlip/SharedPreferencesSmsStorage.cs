using System;
using Android.Content;

namespace SmsBlip
{

    public class SharedPreferencesSmsStorage : ISmsStorage
    {
        const string LastSmsParsed = "LastSmsParsed";
        const int DefaultSmsParsedValue = -1;
        readonly ISharedPreferences preferences;

        public SharedPreferencesSmsStorage (ISharedPreferences preferences)
        {
            if (preferences == null)
                throw new ArgumentNullException (nameof (preferences));

            this.preferences = preferences;
        }

        public bool IsFirstSmsIntercepted => 
            LastSmsIntercepted == DefaultSmsParsedValue;

        public int LastSmsIntercepted {
            get {
                return preferences.GetInt (LastSmsParsed, DefaultSmsParsedValue);
            }
            set {
                using (var editor = preferences.Edit ()) {
                    editor.PutInt (LastSmsParsed, value);
                    editor.Commit ();
                }
            }
        }
    }
}
