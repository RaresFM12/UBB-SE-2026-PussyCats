using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.utilities;

namespace PussyCatsApp.Tests.ViewModels.UtilitiesTests
{
    [TestClass]
    public class DocumentValidatorTests
    {
        [TestMethod]
        public void TestNullOrWhiteSpaceDocumentThrowsError()
        {
            string documentNull = null;
            string documentEmpty = string.Empty;
            string selectedFilePath = "C://";

            var exception1 = Assert.ThrowsException<ArgumentException>(() =>
                DocumentValidator.ValidateDocumentInput(documentNull, selectedFilePath));

            Assert.AreEqual("Please enter a name for the document.", exception1.Message);

            var exception2 = Assert.ThrowsException<ArgumentException>(() =>
                DocumentValidator.ValidateDocumentInput(documentEmpty, selectedFilePath));

            Assert.AreEqual("Please enter a name for the document.", exception2.Message);
        }

        [TestMethod]
        public void TestNullOrWhiteSpaceSelectedFilePathThrowsError()
        {
            string selectedFilePathNull = null;
            string selectedFilePathEmpty = string.Empty;
            string document = "Document";

            var exception1 = Assert.ThrowsException<ArgumentException>(() =>
                DocumentValidator.ValidateDocumentInput(document, selectedFilePathNull)
            );

            Assert.AreEqual("Please select a file to upload.", exception1.Message);

            var exception2 = Assert.ThrowsException<ArgumentException>(() =>
                DocumentValidator.ValidateDocumentInput(document, selectedFilePathEmpty)
            );

            Assert.AreEqual("Please select a file to upload.", exception2.Message);
        }

        [TestMethod]
        public void TestCorrectInputReturnsTrue()
        {
            string document = "Document";
            string selectedFilePath = "C://";
            Assert.IsTrue(DocumentValidator.ValidateDocumentInput(document, selectedFilePath));
        }
    }
}
