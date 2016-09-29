namespace SmsBlip
{
    public class Sms
    {
        public string Address { get; }
        public string Date { get; }
        public string Message { get; }
        public SmsType Type { get; }

        public Sms (string address, string date, string message, SmsType type)
        {
            Address = address;
            Date = date;
            Message = message;
            Type = type;
        }

        public override bool Equals (object obj)
        {
            var sms = obj as Sms;
            if (sms == null) return false;

            if (this == obj) return true;

            if (Address == null && sms.Address != null) return false;
            if (Address != null && sms.Address == null) return false;
            if (!Address.Equals (sms.Address)) return false;

            if (Date == null && sms.Date != null) return false;
            if (Date != null && sms.Date == null) return false;
            if (!Date.Equals (sms.Date)) return false;

            if (Message == null && sms.Message != null) return false;
            if (Message != null && sms.Message == null) return false;
            if (!Message.Equals (sms.Message)) return false;

            if (Type != sms.Type) return false;

            return true;
        }

        public override string ToString ()
        {
            return string.Format (
                $"[Sms: Address={Address}, Date={Date}, Message={Message}, Type={Type}]");
        }

        public override int GetHashCode ()
        {
            var result = Address?.GetHashCode () ?? 0;
            result = 17 * result + Date?.GetHashCode () ?? 0;
            result = 17 * result + Message?.GetHashCode () ?? 0;
            result = 17 * result + Type.GetHashCode ();
            return result;
        }
    }
}
