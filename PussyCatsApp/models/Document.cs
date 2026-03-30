namespace PussyCatsApp.models
{
    public class Document
    {
        public int DocumentId { get; set; }
        public int UserId { get; set; }
        public string StoredDocument { get; set; } = string.Empty;
        public string NameDocument { get; set; } = string.Empty;
    }
}
