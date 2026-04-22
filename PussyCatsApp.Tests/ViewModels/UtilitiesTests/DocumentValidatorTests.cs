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

            try
            {
                DocumentValidator.ValidateDocumentInput(documentNull, selectedFilePath);
                /// The method must throw an exception to be correct
                Assert.IsTrue(false);
            }
            catch (Exception exception)
            {
                Assert.AreEqual("Please enter a name for the document.", exception.Message);
            }

            try
            {
                DocumentValidator.ValidateDocumentInput(documentEmpty, selectedFilePath);
                /// The method must throw an exception to be correct
                Assert.IsTrue(false);
            }
            catch (Exception exception)
            {
                Assert.AreEqual("Please enter a name for the document.", exception.Message);
            }
        }

        [TestMethod]
        public void TestNullOrWhiteSpaceSelectedFilePathThrowsError()
        {
            string selectedFilePathNull = null;
            string selectedFilePathEmpty = string.Empty;
            string document = "Document";

            try
            {
                DocumentValidator.ValidateDocumentInput(document, selectedFilePathNull);
                /// The method must throw an exception to be correct
                Assert.IsTrue(false);
            }
            catch (Exception exception)
            {
                Assert.AreEqual("Please select a file to upload.", exception.Message);
            }

            try
            {
                DocumentValidator.ValidateDocumentInput(document, selectedFilePathEmpty);
                /// The method must throw an exception to be correct
                Assert.IsTrue(false);
            }
            catch (Exception exception)
            {
                Assert.AreEqual("Please select a file to upload.", exception.Message);
            }
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
