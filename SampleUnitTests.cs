using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BusinessLogic.Objects.Enums;
using MGAM.TournEvent.Data;
using MGAM.TournEvent.Data.Objects;
using NUnit.Framework;

namespace BusinessLogicTest
{
    [TestFixture]
    public class AdManagementTest
    {
        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [SetUp]
        public void MyTestInitialize()
        {
            Util.ResetDb();
        }
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        #region Helper Functions

        private static int AddMedia()
        {
            return AdManagement.AddMedia("file.name", "physical.location", "web.location", "display.name", MediaType.Image, 100, 100, 1200, 0, string.Empty);
        }

        private static int AddStoryboard()
        {
            return AdManagement.AddStoryboard("Storyboard 1");
        }

        private int AddStoryboardEvent(int storyboardId)
        {
            return AdManagement.AddStoryboardEvent(storyboardId, 10, EventType.Text, -1, "My Text", EventType.Text, -1, "My Other Text");
        }

        #endregion

        [Test]
        public void AddMediaTest()
        {
            int mediaId = AddMedia();
            Assert.AreNotEqual(0, mediaId);
            Media media = AdManagement.GetMediaForType(MediaType.Image).SingleOrDefault(a => a.MediaId == mediaId);
            Assert.IsInstanceOf<Media>(media);
            Assert.AreEqual("file.name", media.FileName);
            Assert.AreEqual("physical.location", media.PhysicalLocation);
            Assert.AreEqual("web.location", media.WebLocation);
            Assert.AreEqual("display.name", media.DisplayName);
            Assert.AreEqual("Image", media.MediaType);
            Assert.AreEqual(100, media.Height);
            Assert.AreEqual(100, media.Width);
            Assert.AreEqual(1200, media.Size);
            Assert.AreEqual(0, media.Seconds);
        }

        [Test]
        public void AddStoryboardTest()
        {
            int storyboardId = AddStoryboard();
            Assert.AreNotEqual(0, storyboardId);

            var story = AdManagement.GetStoryboard(storyboardId);
            Assert.IsInstanceOf<Storyboard>(story);
            Assert.AreEqual("Storyboard 1", story.StoryboardName);
        }

        [Test]
        public void AddStoryboardEventTest()
        {
            int storyboardId = AddStoryboard();
            int eventId = AddStoryboardEvent(storyboardId);
            Assert.AreNotEqual(0, eventId);

            StoryboardEvent eventData = AdManagement.GetStoryboardEvent(eventId);
            Assert.IsInstanceOf<StoryboardEvent>(eventData);
            Assert.AreEqual("Text", eventData.LeftEventType.ToString());
            Assert.AreEqual("My Text", eventData.LeftText.ToString());
            Assert.AreEqual("Text", eventData.RightEventType.ToString());
            Assert.AreEqual("My Other Text", eventData.RightText.ToString());
        }

        [Test]
        public void DeleteMediaTest()
        {
            int mediaId = AddMedia();

            Media dt = AdManagement.GetMediaForType(MediaType.Image).Single(a => a.MediaId == mediaId);
            System.IO.FileInfo fi = new System.IO.FileInfo(dt.PhysicalLocation);
            System.IO.FileInfo thumb = new System.IO.FileInfo(fi.FullName + "_thumb.jpg");
            
            AdManagement.DeleteMedia(mediaId);
            Assert.AreEqual(false, AdManagement.GetAllMedia().Any(a => a.MediaId == mediaId));
            Assert.AreEqual(false, fi.Exists);
            Assert.AreEqual(false, thumb.Exists);
        }

        [Test]
        public void DeleteStoryboardTest()
        {
            int storyboardId = AddStoryboard();
            AdManagement.DeleteStoryboard(storyboardId);
            Assert.IsTrue(AdManagement.GetStoryboard(storyboardId) == null);
        }

        [Test]
        public void DeleteStoryboardEventTest()
        {
            int storyboardId = AddStoryboard();
            int eventId = AddStoryboardEvent(storyboardId);
            AdManagement.DeleteStoryboardEvent(eventId);
            Assert.AreEqual(true, AdManagement.GetStoryboardEvents(storyboardId) == null);
        }

        [Test]
        public void GetAllMediaTest()
        {
            int mediaId = AddMedia();

            List<Media> actual = AdManagement.GetAllMedia();
            Assert.IsInstanceOf<List<Media>>(actual);
            Assert.IsTrue(actual.Any(a => a.MediaId == mediaId));
        }

        [Test]
        public void GetMediaTest()
        {
            int mediaId = AddMedia();

            Media media = AdManagement.GetMediaForType(MediaType.Image).Single(a => a.MediaId == mediaId);
            Assert.IsInstanceOf<Media>(media);
            Assert.AreEqual(mediaId, media.MediaId);

            List<Media> mediaAll = AdManagement.GetMediaForType(MediaType.Image);
            Assert.IsInstanceOf<List<Media>>(mediaAll);
        }

        [Test]
        public void GetStoryboardTest()
        {
            int storyboardId = AddStoryboard();

            Storyboard expected = AdManagement.GetStoryboard(storyboardId);
            Assert.IsInstanceOf<Storyboard>(expected);
            Assert.AreEqual(true, expected.StoryboardId.ToString() == storyboardId.ToString());
        }

        [Test]
        public void GetStoryboardEventTest()
        {
            int storyBoardId = AddStoryboard();
            int eventId = AddStoryboardEvent(storyBoardId);

            StoryboardEvent eventObject = AdManagement.GetStoryboardEvent(eventId);
            Assert.IsInstanceOf<StoryboardEvent>(eventObject);
            
            Assert.IsTrue(eventObject.EventId == eventId);
        }

        [Test]
        public void GetStoryboardEventsTest()
        {
            int storyBoardId = AddStoryboard();
            int eventId = AddStoryboardEvent(storyBoardId);

            List<StoryboardEvent> events = AdManagement.GetStoryboardEvents(storyBoardId);
            Assert.IsInstanceOf<List<StoryboardEvent>>(events);
            
            bool foundEvent = events.Any(dr => dr.EventId.ToString() == eventId.ToString());
            Assert.IsTrue(foundEvent);
        }

        [Test]
        public void GetStoryboardsTest()
        {
            int storyboardId = AddStoryboard();

            List<Storyboard> ds = AdManagement.GetStoryboards();
            Assert.IsInstanceOf<List<Storyboard>>(ds);
            
            Assert.AreEqual(true, ds.Any(dr => dr.StoryboardId.ToString() == storyboardId.ToString()));
        }

        [Test]
        public void MoveStoryboardEventTest()
        {
            int storyboardId = AddStoryboard();
            int eventId = AddStoryboardEvent(storyboardId);
            int eventId2 = AddStoryboardEvent(storyboardId);

            AdManagement.MoveStoryboardEvent(eventId, EventMoveDirection.Down);
            Assert.AreEqual(1, Convert.ToInt32(AdManagement.GetStoryboardEvent(eventId2).EventPosition));
            Assert.AreEqual(2, Convert.ToInt32(AdManagement.GetStoryboardEvent(eventId).EventPosition));
        }

        [Test]
        public void UpdateStoryboardTest()
        {
            int storyboardId = AddStoryboard();
            AdManagement.UpdateStoryboard(storyboardId, "New Storyboard Name", new List<int>()); // Note that this doesn't test updating the signs at all..
            Assert.AreEqual("New Storyboard Name", AdManagement.GetStoryboard(storyboardId).StoryboardName.ToString());
        }

        [Test]
        public void UpdateStoryboardEventTest()
        {
            int storyboardId = AddStoryboard();
            int eventId = AddStoryboardEvent(storyboardId);

            StoryboardEvent row = AdManagement.GetStoryboardEvent(eventId);
            int position = Convert.ToInt32(row.EventPosition);
            AdManagement.UpdateStoryboardEvent(eventId, position, 10, EventType.Text, -1, "Finklestein!", EventType.Text, -1, "Zippy!");
            row = AdManagement.GetStoryboardEvent(eventId);
            Assert.AreEqual("Finklestein!", row.LeftText.ToString());
        }
    }
}
