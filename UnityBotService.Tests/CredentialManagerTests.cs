using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using UnityBotService.Credentials;
using Xunit;

namespace UnityUpdateTweetBot.Tests
{
    public class CredentialManagerTests
    {
        [Fact]
        public async void CredentialManagerTest_ShouldFetchFromSecretsClientAndReturnCredentials()
        {
            var secretClient = new Mock<ISecretClient>();
            var logger = new Mock<ILogger<CredentialManager>>();
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterBearerTokenName))).Returns(() => Task.FromResult("bearer"));
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterConsumerKeyName))).Returns(() => Task.FromResult("key"));
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterConsumerSecretName))).Returns(() => Task.FromResult("secret"));
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterAccessTokenName))).Returns(() => Task.FromResult("accesstoken"));
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterAccessSecretName))).Returns(() => Task.FromResult("accesstokensecret"));
            var secretM = new CredentialManager(logger.Object, secretClient.Object);
            var creds = await secretM.GetTwitterCredentialsAsync();
            Assert.NotNull(creds);
            Assert.Equal("key", creds.ConsumerKey);
            Assert.Equal("secret", creds.ConsumerSecret);
            Assert.Equal("bearer", creds.BearerToken);
            Assert.Equal("accesstoken", creds.AccessToken);
            Assert.Equal("accesstokensecret", creds.AccessTokenSecret);
        }

        [Fact]
        public async void CredentialManagerTest_ShouldThrowWhenCredentialIsNullOrEmpty()
        {
            var secretClient = new Mock<ISecretClient>();
            var logger = new Mock<ILogger<CredentialManager>>();

            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterBearerTokenName))).Returns(async () => await Task.FromResult(""));
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterConsumerKeyName))).Returns(async () => await Task.FromResult(""));
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterConsumerSecretName))).Returns(async () => await Task.FromResult(""));
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterAccessTokenName))).Returns(async () => await Task.FromResult(""));
            secretClient.Setup(s => s.GetSecretAsync(It.Is<string>(secretId => secretId == CredentialManager.TwitterAccessSecretName))).Returns(async () => await Task.FromResult(""));
            var secretM = new CredentialManager(logger.Object, secretClient.Object);
            await Assert.ThrowsAsync<Exception>(async () => await secretM.GetTwitterCredentialsAsync());
        }
    }
}