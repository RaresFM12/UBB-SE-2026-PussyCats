using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Utilities;

namespace PussyCatsApp.Tests.ViewModels.UtilitiesTests
{
    [TestClass]
    public class DocumentValidatorTests
    {
        [TestMethod]
        public void TestNullDocumentThrowsError()
        {
            string documentNull = null;
            string documentEmpty = string.Empty;
            string selectedFilePath = "C://";

            var exception = Assert.ThrowsException<ArgumentException>(() =>
                DocumentValidator.ValidateDocumentInput(documentNull, selectedFilePath));

            Assert.AreEqual("Please enter a name for the document.", exception.Message);
        }

        [TestMethod]
        public void TestEmptyStringDocumentThrowsError()
        {
            string documentEmpty = string.Empty;
            string selectedFilePath = "C://";
            var exception = Assert.ThrowsException<ArgumentException>(() =>
                DocumentValidator.ValidateDocumentInput(documentEmpty, selectedFilePath));

            Assert.AreEqual("Please enter a name for the document.", exception.Message);
        }

        [TestMethod]
        public void TestNullSelectedFilePathThrowsError()
        {
            string selectedFilePathNull = null;
            string document = "Document";

            var exception = Assert.ThrowsException<ArgumentException>(() =>
                DocumentValidator.ValidateDocumentInput(document, selectedFilePathNull)
            );

            Assert.AreEqual("Please select a file to upload.", exception.Message);
        }

        [TestMethod]
        public void TestEmptySelectedFilePathThrowsError()
        {
            string selectedFilePathEmpty = string.Empty;
            string document = "Document";

            var exception = Assert.ThrowsException<ArgumentException>(() =>
                DocumentValidator.ValidateDocumentInput(document, selectedFilePathEmpty)
            );

            Assert.AreEqual("Please select a file to upload.", exception.Message);
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
