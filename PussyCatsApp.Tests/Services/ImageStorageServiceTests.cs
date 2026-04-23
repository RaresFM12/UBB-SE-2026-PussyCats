using System.Text;
using PussyCatsApp.Services;

namespace PussyCatsApp.Tests.Services
{

    [TestClass]
    public class ImageStorageServiceTests
    {
        private string tempDir;
        private ImageStorageService service;

        [TestInitialize]
        public void SetUp()
        {
            tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);

            service = new ImageStorageService(tempDir);
        }

        [TestCleanup]
        public void TearDown()
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DataRow("image.bmp")]
        [DataRow("image.gif")]
        [DataRow("image.txt")]
        [DataRow("image.pdf")]
        [DataRow("image.exe")]
        [DataRow("image.zip")]
        [DataRow("image.docx")]
        [DataRow("image.hellojbmn")]
        public void SaveImage_UnsupportedFileType_ThrowsArgumentException(string fileName)
        {
            
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("fake image data"));

            service.SaveImage(fileStream, fileName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [DataRow("image")]
        public void SaveImage_NoFileExtension_ThrowsArgumentException(string fileName)
        {
            var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("fake image data"));

            service.SaveImage(fileStream, fileName);
        }


        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SaveImage_FileSizeTooBig_ThrowsException()
        {
            var fakeImage = new byte[6*1024*1024];
            var fileStream = new MemoryStream(fakeImage);

            service.SaveImage(fileStream,"myFileName.png");

        }

        [TestMethod]
        [DataRow("photo.jpg")]
        [DataRow("photo.png")]
        [DataRow("photo.jpeg")]
        [DataRow("photo.JPG")]
        [DataRow("photo.PNG")]
        [DataRow("photo.JPEG")]
        [DataRow("photo.JpG")]
        public void SaveImage_ValidImage_CreatesFileOnDisk(string fileName)
        {
            byte[] fakeImage = new byte[1024];
            using var stream = new MemoryStream(fakeImage);

            string savedPath = service.SaveImage(stream, fileName);

            Assert.IsTrue(File.Exists(savedPath));
        }

        [TestMethod]
        public void SaveImage_ValidImage_CreatedFileContentsMatch()
        {
            byte[] fakeImage = { 1, 2, 3, 4, 5, 123, 0, 12 };
            using var stream = new MemoryStream(fakeImage);

            string savedPath = service.SaveImage(stream, "fileName.png");

            Assert.IsTrue(File.Exists(savedPath));
            Assert.IsTrue(File.ReadAllBytes(savedPath).SequenceEqual(fakeImage));

        }

        [TestMethod]
        public void SaveImage_TwoImagesSameFileName_HaveUniqueFilePaths()
        {
            
            using var s1 = new MemoryStream(new byte[64]);
            using var s2 = new MemoryStream(new byte[64]);

            string path1 = service.SaveImage(s1, "a.jpg");
            string path2 = service.SaveImage(s2, "a.jpg");

            Assert.AreNotEqual(path1, path2);
            
        }


        [TestMethod]
        public void DeleteImage_ExistingFile_RemovesFileFromDisk()
        {
            using var stream = new MemoryStream(new byte[64]);
            string savedPath = service.SaveImage(stream, "photo.jpg");
            Assert.IsTrue(File.Exists(savedPath));

            service.DeleteImage(savedPath);

            Assert.IsFalse(File.Exists(savedPath));
        }

        [TestMethod]
        public void DeleteImage_NullOrWhitespacePath_DoesNotThrow()
        {
            service.DeleteImage("   ");
        }

        [TestMethod]
        public void DeleteImage_NonExistingFile_DoesNotThrow()
        {
            string nonExistingPath = Path.Combine(tempDir, "nonexistent.jpg");
            service.DeleteImage(nonExistingPath);
        }
    }
}
