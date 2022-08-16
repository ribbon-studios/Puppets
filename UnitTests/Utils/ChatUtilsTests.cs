using Microsoft.VisualStudio.TestTools.UnitTesting;
using Puppets.Utils;

namespace UnitTests.Utils
{
    [TestClass]
    public class ChatUtilsTests
    {
        [TestMethod]
        public void CleanupMessage_shouldCleanupAutoTranslateTags()
        {
            var expectedMessage = PTU.Faker.Random.AlphaNumeric(5);

            Assert.AreEqual(expectedMessage, ChatUtils.CleanupMessage(ChatUtils.AutoTranslateOpen + expectedMessage + ChatUtils.AutoTranslateOpen));
        }

        [TestMethod]
        public void CleanupMessage_shouldCleanupSurroundingWhitespace()
        {
            var expectedMessage = PTU.Faker.Random.AlphaNumeric(5) + " " + PTU.Faker.Random.AlphaNumeric(5);

            Assert.AreEqual(expectedMessage, ChatUtils.CleanupMessage(PTU.Faker.Random.String2(5, " ") + expectedMessage + PTU.Faker.Random.String2(5, " ")));
        }

        [TestMethod]
        public void CleanupSender_shouldCleanupPartyListNumbers()
        {
            var expectedMessage = PTU.Faker.Random.AlphaNumeric(5);

            Assert.AreEqual(expectedMessage, ChatUtils.CleanupSender(ChatUtils.Number1 + expectedMessage));
        }

        [TestMethod]
        public void CleanupSender_shouldCleanupGroupSymbols()
        {
            var expectedMessage = PTU.Faker.Random.AlphaNumeric(5);

            Assert.AreEqual(expectedMessage, ChatUtils.CleanupSender(ChatUtils.Circle + expectedMessage));
        }

        [TestMethod]
        public void CleanupSender_shouldCleanupSurroundingWhitespace()
        {
            var expectedMessage = PTU.Faker.Random.AlphaNumeric(5) + " " + PTU.Faker.Random.AlphaNumeric(5);

            Assert.AreEqual(expectedMessage, ChatUtils.CleanupSender(PTU.Faker.Random.String2(5, " ") + expectedMessage + PTU.Faker.Random.String2(5, " ")));
        }
    }
}
