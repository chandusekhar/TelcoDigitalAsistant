namespace Microsoft.Telco.DigitalAssistant.Bot.Dialogs.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Telco.DigitalAssistant.Bot.Dialogs;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector;
    using Microsoft.Telco.DigitalAssistant.Logging;

    [TestClass()]
    public class RootDialogTests
    {
        [TestInitialize]
        public void Initialize()
        {
            LoggingManager.Initialize();
        }

        [TestMethod()]
        public void NoneTest()
        {
            //MessagesController controller = new MessagesController();
            //controller.Post(GetTextActivity("what is the weather today in Madrid?")).Wait();
        }

        private Activity GetTextActivity(string message)
        {
            return new Activity(
                "message",
                Guid.NewGuid().ToString(),
                DateTime.UtcNow,
                null,
                "http://localhost:9000",
                "emulator",
                new ChannelAccount("2c1c7fa3", "User1"),
                new ConversationAccount(true, "8a684db5", "Conver1"),
                new ChannelAccount("56800324", "yoloBot"),
                null, null, null, null, null, null, null, message);
        }

        //private Activity GetTextActivityReply(string message, string id)
        //{
        //    return new Activity(
        //        "message", id,
        //        Guid.NewGuid(),
        //        DateTime.UtcNow,
        //        null,
        //        "http://localhost:9000",
        //        "emulator",
        //        new ChannelAccount("2c1c7fa3", "User1"),
        //        new ConversationAccount(true, "8a684db5", "Conver1"),
        //        new ChannelAccount("56800324", "yoloBot"),
        //        null, null, null, null, null, null, null, message, null, null, null, null, null);
        //}
    }
}