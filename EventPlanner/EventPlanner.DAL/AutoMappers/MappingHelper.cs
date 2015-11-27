﻿using System;
using System.Collections.Generic;
using System.Linq;
using EventPlanner.Models.Domain;
using EventPlanner.Models.Enums;
using EventPlanner.Models.Models.CreateAndEdit;
using EventPlanner.Models.Models.Vote;

namespace EventPlanner.DAL.AutoMappers
{
    public static class MappingHelper
    {
        public static Geometry MapToGeometry(double lng, double lat)
        {
            return new Geometry() { type = "Point", coordinates = new double[] { lng, lat } };
        }

        public static Properties MapToProperties(string name, string address)
        {
            return new Properties() { name = name, address = address };
        }

        public static IList<TimeSlot> MapToTimeSlot(IList<EventModel.DatesModel> dates)
        {
            return dates.SelectMany(d => d.Times.Select(time => new TimeSlot()
            {
                Id = time.Id,
                DateTime = d.Date.Add(TimeSpan.Parse(time.Time))
            }).ToList()).ToList();
        }

        public static IList<EventModel.DatesModel> MapToDatesModel(IList<TimeSlot> timeSlots)
        {
            return timeSlots.GroupBy(ts => ts.DateTime.Date, ts => ts)
                .Select(
                    tsGrp =>
                        new EventModel.DatesModel()
                        {
                            Date = tsGrp.Key.Date,
                            Times = tsGrp.Select(ts => new EventModel.TimeModel(ts.Id, ts.DateTime.ToString("HH:mm:ss"))).ToList()
                        }).ToList();
        }

        public static OptionViewModel MapToOptionViewModel(TimeSlot timeSlot, string userId)
        {
            return new OptionViewModel()
            {
                Id = timeSlot.Id,
                Title = timeSlot.DateTime.ToShortDateString(),
                Desc = timeSlot.DateTime.ToLongTimeString(),
                UsersVote = MapToUsersVoteModel(timeSlot.VotesForDate, userId),
                Votes = MapToVotesViewModel(timeSlot.VotesForDate)
            };
        }

        public static OptionViewModel MapToOptionViewModel(PlaceViewModel place, string userId)
        {
            return new OptionViewModel()
            {
                Id = place.Id,
                Title = place.Venue.Name,
                Desc = place.Venue.AddressInfo,
                UsersVote = MapToUsersVoteModel(place.VotesForPlace, userId),
                Votes = MapToVotesViewModel(place.VotesForPlace)
            };
        }

        public static VotesViewModel MapToVotesViewModel(IList<VoteForDate> votes)
        {
            return new VotesViewModel()
            {
                Yes =
                    votes?.Where(vote => vote.WillAttend == WillAttend.Yes).Select(vote => vote.User?.Name ?? string.Empty).ToArray() ??
                    new string[] {},
                Maybe =
                    votes?.Where(vote => vote.WillAttend == WillAttend.Maybe).Select(vote => vote.User?.Name ?? string.Empty).ToArray() ??
                    new string[] {},
                No =
                    votes?.Where(vote => vote.WillAttend == WillAttend.No).Select(vote => vote.User?.Name ?? string.Empty).ToArray() ??
                    new string[] {}
            };
        }

        public static VotesViewModel MapToVotesViewModel(IList<VoteForPlace> votes)
        {
            return new VotesViewModel()
            {
                Yes =
                    votes?.Where(vote => vote.WillAttend == WillAttend.Yes).Select(vote => vote.User?.Name ?? string.Empty).ToArray() ??
                    new string[] { },
                Maybe =
                    votes?.Where(vote => vote.WillAttend == WillAttend.Maybe).Select(vote => vote.User?.Name ?? string.Empty).ToArray() ??
                    new string[] { },
                No =
                    votes?.Where(vote => vote.WillAttend == WillAttend.No).Select(vote => vote.User?.Name ?? string.Empty).ToArray() ??
                    new string[] { }
            };
        }

        public static UsersVoteModel MapToUsersVoteModel(IList<VoteForDate> votes, string userId)
        {
            return new UsersVoteModel()
            {
                Id = votes.SingleOrDefault(v => v.UserId == userId)?.Id ?? Guid.Empty,
                WillAttend = votes.SingleOrDefault(v => v.UserId == userId)?.WillAttend.ToString() ?? null
            };
        }

        public static UsersVoteModel MapToUsersVoteModel(IList<VoteForPlace> votes, string userId)
        {
            return new UsersVoteModel()
            {
                Id = votes.SingleOrDefault(v => v.UserId == userId)?.Id ?? Guid.Empty,
                WillAttend = votes.SingleOrDefault(v => v.UserId == userId)?.WillAttend.ToString() ?? null
            };
        }
    }
}
