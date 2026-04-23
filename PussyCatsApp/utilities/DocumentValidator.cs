using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PussyCatsApp.Utilities
{
    public class DocumentValidator
    {
        public static bool ValidateDocumentInput(string documentName, string selectedFilePath)
        {
            if (string.IsNullOrWhiteSpace(documentName))
            {
                throw new ArgumentException("Please enter a name for the document.");
            }

            if (string.IsNullOrEmpty(selectedFilePath))
            {
                throw new ArgumentException("Please select a file to upload.");
            }

            return true;
        }
    }
}
