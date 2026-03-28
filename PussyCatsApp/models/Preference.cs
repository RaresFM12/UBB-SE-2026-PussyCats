namespace PussyCatsApp.models
{
    public class Preference
    {
        public int PreferenceId { get; set; }
        public int UserId { get; set; }
        public string PreferenceType { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
