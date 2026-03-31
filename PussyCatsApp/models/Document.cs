namespace PussyCatsApp.models
{
    public class Document
    {
        private int id;
        private int userId;
        private string documentName;
        private string filePath;
        private System.DateTime uploadDate;

        public int Id { get; private set; }

        public int UserId { get; private set; }

        public string DocumentName { get; private set; }

        public string FilePath { get; private set; }

        public System.DateTime UploadDate { get; private set; }
    }
}