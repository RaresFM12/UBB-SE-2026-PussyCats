using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Services;

namespace PussyCatsApp.Tests.Services
{
    [TestClass]
    public class CVParsingServiceTests
    {
        [TestMethod]
        [DataRow(".json")]
        [DataRow(".JSON")]
        public void ParseCVFile_ValidJson_ReturnsMappedProfile(string fileType)
        {
            CVParsingService service = new CVParsingService();
            string jsonContent = @"{
                ""FirstName"": ""Hello"",
                ""LastName"": ""World"",
                ""Age"": 59,
                ""Email"": ""hw@gmail.com""
            }";

            var result = service.ParseCVFile(jsonContent, fileType);

            Assert.AreEqual("Hello", result.FirstName);
            Assert.AreEqual("World", result.LastName);
            Assert.AreEqual(59, result.Age);
            Assert.AreEqual("hw@gmail.com", result.Email);
        }

        [DataTestMethod]
        [DataRow(".xml")]
        [DataRow(".XML")]
        public void ParseCVFile_ValidXmlCVDataRootName_ReturnsMappedProfile(string fileType)
        {
            CVParsingService service = new CVParsingService();
            string xmlContent = @"
                <CVData>
                    <FirstName>Hello</FirstName>
                    <LastName>World</LastName>
                    <Age>25</Age>
                    <Email>hw@gmail.com</Email>
                </CVData>";

            var result = service.ParseCVFile(xmlContent, fileType);

            Assert.AreEqual("Hello", result.FirstName);
            Assert.AreEqual("World", result.LastName);
            Assert.AreEqual(25, result.Age); 
            Assert.AreEqual("hw@gmail.com", result.Email);
        }
        [DataTestMethod]
        [DataRow(".xml")]
        public void ParseCVFile_ValidXmlOtherRootName_ReturnsMappedProfile(string fileType)
        {
            CVParsingService service = new CVParsingService();
            string xmlContent = @"
                <Hello>
                    <FirstName>Hello</FirstName>
                    <LastName>World</LastName>
                    <Age>25</Age>
                    <Email>hw@gmail.com</Email>
                </Hello>";

            var result = service.ParseCVFile(xmlContent, fileType);

            Assert.AreEqual("Hello", result.FirstName);
            Assert.AreEqual("World", result.LastName);
            Assert.AreEqual(25, result.Age);
            Assert.AreEqual("hw@gmail.com", result.Email);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        [DataRow(".gata")]
        [DataRow(".pdf")]
        [DataRow(".docx")]
        public void ParseCVFile_InvalidFormatFile_ThrowsException(string formatType)
        {
            CVParsingService service = new CVParsingService();

            var result = service.ParseCVFile("fileContent", formatType);

        }

        [DataTestMethod]
        [ExpectedException(typeof(Exception))]
        public void ParseCVFile_InvalidXml_ThrowsException()
        {
            CVParsingService service = new CVParsingService();
            string xmlContent = @"
                <CVData>
                    <FirstNam>Hello</FirstName>
                    <LastName>World</LastName>
                    <Age>25</Age>
                    <Email>hw@gmail.com</Email>
                </CVData>";

            var result = service.ParseCVFile(xmlContent, ".xml");

        }


        [TestMethod]
        public void ParseCVFile_InvalidData_AppliesSanitizationAndDefaults()
        {
            var service = new CVParsingService();
            int currentYear = DateTime.Now.Year;

            string jsonContent = $@"{{
                ""FirstName"": ""{new string('A', 100)}"", 
                ""Age"": 100,
                ""Gender"": ""Holiday"",
                ""Email"": ""come sooner"",
                ""PhoneNumber"": ""+07-pls-pls"",
                ""ExpectedGraduationYear"": {currentYear + 50},
                ""Skills"": [""C#"", ""C#"", ""Java"", ""{new string('S', 80)}""],
                ""WorkExperiences"": [
                    {{ ""Company"": """", ""JobTitle"": ""Manager"" }},
                    {{ ""Company"": ""Valid"", ""JobTitle"": ""Valid"", ""StartDate"": ""1800-01-01"" }}
                ]
            }}";

            var result = service.ParseCVFile(jsonContent, ".json");

            
            Assert.AreEqual(50, result.FirstName.Length);

            Assert.AreEqual(0, result.Age);

            Assert.AreEqual(string.Empty, result.Gender);

            Assert.AreEqual(string.Empty, result.Email);

            Assert.AreEqual("+07", result.PhoneNumber);

            Assert.AreEqual(0, result.ExpectedGraduationYear);

            Assert.AreEqual(3, result.Skills.Count);
            Assert.AreEqual(60, result.Skills[2].Length);

            
            Assert.AreEqual(1, result.WorkExperiences.Count);
            Assert.AreEqual(DateTimeOffset.Now.Date, result.WorkExperiences[0].StartDate.Date);
        }

        [TestMethod]
        public void ParseCVFile_ProjectsAndActivities_FiltersInvalidAndLimitsCount()
        {
            var service = new CVParsingService();

            string xmlContent = @"
        <CVData>
            <Projects>
                <Project>
                    <Name>Valid Project</Name>
                    <Technologies>
                        <string>T1</string><string>T2</string><string>T3</string>
                        <string>T4</string><string>T5</string><string>T6</string>
                        <string>T7</string><string>T8</string><string>T9</string>
                        <string>T10</string><string>T11</string><string>T12</string>
                    </Technologies>
                </Project>
                <Project>
                    <Name></Name> <Description>Should be ignored</Description>
                </Project>
            </Projects>
            <ExtraCurricularActivities>
                <ExtraCurricularActivity>
                    <ActivityName>Volunteer</ActivityName>
                    <Organization>Charity</Organization>
                </ExtraCurricularActivity>
            </ExtraCurricularActivities>
        </CVData>";

            var result = service.ParseCVFile(xmlContent, ".xml");

            Assert.AreEqual(1, result.Projects.Count, "Empty name project should have been filtered out.");
            Assert.AreEqual(10, result.Projects[0].Technologies.Count, "Technologies should be capped at 10.");

            Assert.AreEqual(1, result.ExtraCurricularActivities.Count);
            Assert.AreEqual("Volunteer", result.ExtraCurricularActivities[0].ActivityName);
        }

    }
}
