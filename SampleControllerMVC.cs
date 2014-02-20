using BusinessLogic.Objects.Enums;
using MGAM.TournEvent.Data;
using MGAM.TournEvent.Data.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using TournEvent.Models;

namespace TournEvent.Controllers
{
    public class StoryboardEventController : Controller
    {
        [HttpGet]
        public JsonResult GetMediaByType(string mediatype)
        {
            MediaType o = (MediaType)Enum.Parse(typeof(MediaType), mediatype);
            return Json(AdManagement.GetMediaForType(o), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public PartialViewResult Create(int id)
        {
            StoryboardEventModel model = new StoryboardEventModel();
            model.Images = AdManagement.GetMediaForType(MediaType.Image).Where(i => new FileInfo(i.PhysicalLocation).Exists == true);
            model.Videos = AdManagement.GetMediaForType(MediaType.Video).Where(v => new FileInfo(v.PhysicalLocation).Exists == true);
            model.EventTypes = Enum.GetValues(typeof(EventType)).Cast<EventType>().Where(a => a != EventType.None).Select(type => new SelectListItem { Text = type.ToString(), Value = ((int)type).ToString() });
            model.StoryboardEvent = new StoryboardEvent { StoryboardId = id, Duration = 15, LeftEventType = EventType.Image, RightEventType = EventType.Image };

            return PartialView("DialogCreate", model);
        }

        [HttpGet]
        public PartialViewResult Edit(int id)
        {
            StoryboardEventModel model = new StoryboardEventModel();
            model.Images = AdManagement.GetMediaForType(MediaType.Image).Where(i => new FileInfo(i.PhysicalLocation).Exists == true);
            model.Videos = AdManagement.GetMediaForType(MediaType.Video).Where(v => new FileInfo(v.PhysicalLocation).Exists == true);
            model.EventTypes = Enum.GetValues(typeof(EventType)).Cast<EventType>().Where(a => a != EventType.None).Select(type => new SelectListItem { Text = type.ToString(), Value = ((int)type).ToString() });
            model.StoryboardEvent = AdManagement.GetStoryboardEvent(id);
            return PartialView("DialogCreate", model);
        }

        [HttpPost]
        public JsonResult MoveEvent(int storyboardId, int eventId, int newPosition)
        {
            List<StoryboardEvent> events = AdManagement.GetStoryboardEvents(storyboardId);
            int oldPosition = events.Single(a => a.EventId == eventId).EventPosition;
            if (oldPosition < newPosition)
            {
                events.Where(a => a.EventPosition > oldPosition && a.EventPosition <= newPosition).ToList().ForEach(b => AdManagement.MoveStoryboardEventTo(b.EventId, b.EventPosition - 1));
                AdManagement.MoveStoryboardEventTo(eventId, newPosition);
            }
            else if (oldPosition > newPosition)
            {
                events.Where(a => a.EventPosition < oldPosition && a.EventPosition >= newPosition).ToList().ForEach(b => AdManagement.MoveStoryboardEventTo(b.EventId, b.EventPosition + 1));
                AdManagement.MoveStoryboardEventTo(eventId, newPosition);
            }

            return Json(true);
        }
    }
}