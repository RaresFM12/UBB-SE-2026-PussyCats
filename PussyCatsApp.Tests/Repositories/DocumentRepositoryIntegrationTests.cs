using PussyCatsApp.Repositories;
using PussyCatsApp.Tests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.models;

namespace PussyCatsApp.Tests.Repositories
{
    [TestClass]
    public class DocumentRepositoryIntegrationTests
    {
        private DocumentRepository Repository;

        [TestInitialize]
        public void SetUp()
        {
            TestDatabaseHelper.ClearAllTables();
            Repository = new DocumentRepository(TestDatabaseHelper.ConnectionString);
        }


        [TestMethod]
        public void GetDocumentsByUserId_UserHasTwoDocuments_ExpectsDocumentsBeingReturnedInOrder()
        {
            int userId = TestDatabaseHelper.InsertUser();
            TestDatabaseHelper.InsertDocument(userId, "Test Document 1");
            TestDatabaseHelper.InsertDocument(userId, "Test Document 2");

            List<Document> documents = Repository.GetDocumentsByUserId(userId);

            Assert.AreEqual("Test Document 2", documents[1].DocumentName);
        }

        [TestMethod]
        public void GetDocumentsByUserId_InvalidServer_ExpectsNoDocument()
        {
            var repo = new DocumentRepository("Server=InvalidServerName;Database=Fake;Connect Timeout=1;");

            var result = repo.GetDocumentsByUserId(1);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetDocumentById_InvalidServer_ExpectsNoDocument()
        {
            var repo = new DocumentRepository("Server=InvalidServerName;Database=Fake;Connect Timeout=1;");

            var result = repo.GetDocumentById(1);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void AddDocument_InvalidServer_ExpectsNotCrashing()
        {
            var repo = new DocumentRepository("Server=InvalidServerName;Database=Fake;Connect Timeout=1;");

            repo.AddDocument(new Document { UserId = 1 });
        }

        [TestMethod]
        public void DeleteDocument_InvalidServer_ExpectsNoCrashing()
        {
            var repo = new DocumentRepository("Server=InvalidServerName;Database=Fake;Connect Timeout=1;");

            repo.DeleteDocument(1);
        }


        [TestMethod]
        public void AddDocument_ValidDocument_ExpectsDocumentBeingSaved()
        {
            int userId = TestDatabaseHelper.InsertUser();
            Document document = new Document { UserId = userId, DocumentName = "Test Document", UploadDate= DateTime.Now };
            Repository.AddDocument(document);

            List<Document> documents = Repository.GetDocumentsByUserId(userId);

            Assert.AreEqual("Test Document", documents[0].DocumentName);
        }

        [TestMethod]
        public void GetDocumentById_UserHasOneDocument_ExpectsDocumentBeingReturned()
        {
            int userId = TestDatabaseHelper.InsertUser();
            int documentId = TestDatabaseHelper.InsertDocument(userId, "Test Document");

            Document document = Repository.GetDocumentById(documentId);

            Assert.AreEqual("Test Document", document.DocumentName);
        }

        [TestMethod]
        public void DeleteDocument_UserHasOneDocument_ExpectsDocumentBeingDeleted()
        {
            int userId = TestDatabaseHelper.InsertUser();
            int documentId = TestDatabaseHelper.InsertDocument(userId, "Test Document");

            Repository.DeleteDocument(documentId);
            Document document = Repository.GetDocumentById(documentId);

            Assert.IsNull(document);
        }

        [TestMethod]
        public void MapRowToDocument_NullFilePath_ExpectsNull()
        {
            int userId = TestDatabaseHelper.InsertUser();
            int docId = TestDatabaseHelper.InsertDocument(userId, "NoPath.pdf", null);

            var result = Repository.GetDocumentById(docId);

            Assert.IsNull(result.FilePath);
        }

        [TestMethod]
        public void MapRowToDocument_NullUploadDate_ExpectsMinValue()
        {
            int userId = TestDatabaseHelper.InsertUser();
            int docId = TestDatabaseHelper.InsertDocument(userId, "NoDate.pdf", null);

            var result = Repository.GetDocumentById(docId);

            Assert.AreEqual(DateTime.MinValue, result.UploadDate);
        }

      
    }
}
