namespace PussyCatsApp.Models
{
    public class Document
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string DocumentName { get; set; } = string.Empty;

        public string FilePath { get; set; }

        public System.DateTime UploadDate { get; set; }
    }
}