namespace PussyCatsApp.models
{
    public class Document
    {
        private int documentId;
        private int userId;
        private string documentName;
        private string filePath;
        private System.DateTime uploadDate;

        public int Id { get; set; }

        public int UserId { get; set; }

        public string DocumentName { get; set; }

        public string FilePath { get; set; }

        public System.DateTime UploadDate { get; set; }
    }
}