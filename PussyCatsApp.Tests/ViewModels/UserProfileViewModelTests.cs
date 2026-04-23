using Moq;
using PussyCatsApp.services;
using PussyCatsApp.viewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PussyCatsApp.Models;
using PussyCatsApp.utilities;

namespace PussyCatsApp.Tests.ViewModels
{
    [TestClass]
    public class UserProfileViewModelTests
    {
        private Mock<IUserProfileService> mockUserProfileService;
        private Mock<IImageStorageService> mockImageStorageService;
        private Mock<ICompletenessService> mockCompletenessService;
        private UserProfileViewModel viewModel;

        private const int testUserId = 1;

        [TestInitialize]
        public void Setup()
        {
            mockUserProfileService = new Mock<IUserProfileService>();
            mockImageStorageService = new Mock<IImageStorageService>();
            mockCompletenessService = new Mock<ICompletenessService>();

            viewModel = new UserProfileViewModel(mockUserProfileService.Object, mockImageStorageService.Object, mockCompletenessService.Object);
        }

        [TestMethod]
        public async Task TestSetFreshnessTextWhenLoadingIfProfileIsNotNull()
        {
            
            var userProfile = new UserProfile
            {
                UserId = testUserId,
                LastUpdated = DateTime.Now.AddDays(-5) // Simulate last update was 5 days ago
            };
            viewModel.UserProfile = userProfile;

            mockUserProfileService.Setup(service => service.GetProfile(testUserId)).Returns(
                userProfile
            );

            await viewModel.LoadUserAsync(testUserId);
            string expectedAnswer = TimeFormatter.CalculateFreshnessLabel(userProfile.LastUpdated);

            Assert.AreEqual(expectedAnswer, viewModel.FreshnessText);
        }

        [TestMethod]
        public async Task TestSetCompletnessPercentageWhenLoadingIfProfileIsNotNull()
        {

            var userProfile = new UserProfile
            {
                UserId = testUserId,
                LastUpdated = DateTime.Now.AddDays(-5) // Simulate last update was 5 days ago
            };
            viewModel.UserProfile = userProfile;

            mockUserProfileService.Setup(service => service.GetProfile(testUserId)).Returns(
                userProfile
            );

            int expectedPercentage = 50;
            mockCompletenessService.Setup(service => service.CalculateCompleteness(userProfile)).Returns(expectedPercentage);

            await viewModel.LoadUserAsync(testUserId);

            Assert.AreEqual(expectedPercentage, viewModel.CompletenessPercentage);
        }

        [TestMethod]
        public async Task TestLoadAsyncDoesNothingWhenProfileIsNull()
        {
            UserProfile userProfile = null;
            viewModel.UserProfile = userProfile;

            mockUserProfileService.Setup(service => service.GetProfile(testUserId)).Returns(
                userProfile
            );

            await viewModel.LoadUserAsync(testUserId);
            Assert.AreEqual(string.Empty, viewModel.FreshnessText);
            Assert.AreEqual(0, viewModel.CompletenessPercentage);
            Assert.AreEqual(string.Empty, viewModel.NextEmptyFieldPrompt);
        }

    }
}
