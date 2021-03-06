﻿using System;
using AutoMapper;
using EventPlanner.Entities;
using EventPlanner.Models.Models.CreateAndEdit;
using EventPlanner.Models.Models.Shared;

namespace EventPlanner.DAL.AutoMappers
{
    /// <summary>
    ///     Automappers for all repository classes
    /// </summary>
    public static class AutoMappers
    {
        /// <summary>
        ///     Method that creates mappers for all models
        /// </summary>
        public static void Configure()
        {
            CreateEventMap();
            CreatePlaceMap();
            CreateTimeSlotMap();
            CreateVoteForDateMap();
            CreateVoteForPlaceMap();
            CreateEnumsMap();
            CreateUserMap();
            CreateFeatureMap();
        }

        private static void CreateEventMap()
        {
            Mapper.CreateMap<Models.Domain.Event, EventEntity>()
                .ForMember(ee => ee.Places, conf => conf.MapFrom(e => e.Places))
                .ForMember(ee => ee.TimeSlots, conf => conf.MapFrom(e => e.TimeSlots));

            Mapper.CreateMap<EventEntity, Models.Domain.Event>()
                .ForMember(e => e.Places, conf => conf.ResolveUsing(ee => ee.Places))
                .ForMember(e => e.TimeSlots, conf => conf.ResolveUsing(ee => ee.TimeSlots));

            Mapper.CreateMap<Models.Domain.Event, EventModel>()
                .ForMember(src => src.Dates, conf => conf.ResolveUsing(dest => MappingHelper.MapToDatesModel(dest.TimeSlots)));

            Mapper.CreateMap<EventModel, Models.Domain.Event>()
                .ForMember(e => e.Id, conf => conf.MapFrom(y => y.Id ?? Guid.Empty))
                .ForMember(e => e.Disabled, conf => conf.Ignore())
                .ForMember(e => e.Places, conf => conf.MapFrom(ee => ee.Places))
                .ForMember(e => e.TimeSlots, conf => conf.ResolveUsing(em => MappingHelper.MapToTimeSlot(em.Dates))); 
            
            Mapper.CreateMap<Models.Domain.Event, EventInfoViewModel>();
        }

        private static void CreatePlaceMap()
        {
            Mapper.CreateMap<Models.Domain.Place, PlaceEntity>()
                .ForMember(pe => pe.VotesForPlace, conf => conf.MapFrom(p => p.VotesForPlace));

            Mapper.CreateMap<PlaceEntity, Models.Domain.Place>()
                .ForMember(p => p.VotesForPlace, conf => conf.MapFrom(pe => pe.VotesForPlace));

            Mapper.CreateMap<Models.Domain.Place, FourSquareVenueModel>()
                .ForMember(p => p.Id, conf => conf.MapFrom(v => v.Id))
                .ForMember(p => p.VenueId, conf => conf.MapFrom(v => v.VenueId))
                .ForMember(p => p.EventId, conf => conf.MapFrom(v => v.EventId))
                .ForMember(p => p.Name, conf => conf.Ignore())
                .ForMember(p => p.AddressInfo, conf => conf.Ignore())
                .ForMember(p => p.City, conf => conf.Ignore())
                .ForMember(p => p.Lat, conf => conf.Ignore())
                .ForMember(p => p.Lng, conf => conf.Ignore());


            Mapper.CreateMap<FourSquareVenueModel, Models.Domain.Place>()
                .ForMember(p => p.VenueId, conf => conf.MapFrom(v => v.VenueId))
                .ForMember(p => p.Id, conf => conf.MapFrom(v => v.Id))
                .ForMember(p => p.EventId, conf => conf.MapFrom(v => v.EventId))
                .ForMember(p => p.VotesForPlace, conf => conf.Ignore());

            Mapper.CreateMap<Models.Domain.Place, Models.Models.Vote.PlaceViewModel>()
                .ForMember(p => p.VotesForPlace, conf => conf.MapFrom(p => p.VotesForPlace));

            Mapper.CreateMap<Models.Models.Vote.PlaceViewModel, Models.Models.Vote.PlaceMapViewModel>()
                .ForMember(p => p.VenueId, conf => conf.MapFrom(v => v.VenueId))
                .ForMember(p => p.Name, conf => conf.MapFrom(v => v.Venue.Name))
                .ForMember(p => p.AddressInfo, conf => conf.MapFrom(v => v.Venue.AddressInfo))
                .ForMember(p => p.City, conf => conf.MapFrom(v => v.Venue.City))
                .ForMember(p => p.Lat, conf => conf.MapFrom(v => v.Venue.Lat))
                .ForMember(p => p.Lng, conf => conf.MapFrom(v => v.Venue.Lng));
        }

        private static void CreateFeatureMap()
        {
            Mapper.CreateMap<Models.Models.Vote.PlaceViewModel, Models.Models.Vote.Feature>()
                .ForMember(p => p.id, conf => conf.MapFrom(v => v.VenueId))
                .ForMember(p => p.geometry, conf => conf.ResolveUsing(v => MappingHelper.MapToGeometry(v.Venue.Lng, v.Venue.Lat)))
                .ForMember(p => p.properties, conf => conf.ResolveUsing(v => MappingHelper.MapToProperties(v.Venue.Name, v.Venue.AddressInfo)));
        }

        private static void CreateTimeSlotMap()
        {
            Mapper.CreateMap<TimeSlotEntity, Models.Domain.TimeSlot>()
                .ForMember(t => t.VotesForDate, conf => conf.ResolveUsing(te => te.VotesForDate));


            Mapper.CreateMap<Models.Domain.TimeSlot, TimeSlotEntity>()
                .ForMember(te => te.VotesForDate, conf => conf.MapFrom(t => t.VotesForDate));
        }

        private static void CreateVoteForDateMap()
        {
            Mapper.CreateMap<VoteForDateEntity, Models.Domain.VoteForDate>()
                .ForMember(v => v.WillAttend, conf => conf.MapFrom(ve => ve.WillAttend));

            Mapper.CreateMap<Models.Domain.VoteForDate, VoteForDateEntity>()
                .ForMember(ve => ve.WillAttend, conf => conf.MapFrom(v => v.WillAttend));
        }

        private static void CreateVoteForPlaceMap()
        {
            Mapper.CreateMap<VoteForPlaceEntity, Models.Domain.VoteForPlace>()
                .ForMember(v => v.WillAttend, conf => conf.MapFrom(ve => ve.WillAttend));

            Mapper.CreateMap<Models.Domain.VoteForPlace, VoteForPlaceEntity>()
                .ForMember(ve => ve.WillAttend, conf => conf.MapFrom(v => v.WillAttend));
        }

        private static void CreateEnumsMap()
        {
            Mapper.CreateMap<Entities.Enums.WillAttend, Models.Enums.WillAttend>();

            Mapper.CreateMap<Models.Enums.WillAttend, Entities.Enums.WillAttend>();
        }

        private static void CreateUserMap()
        {
            Mapper.CreateMap<Entities.UserEntity, Models.Domain.User>();

            Mapper.CreateMap<Models.Domain.User, Entities.UserEntity>();
        }
    }
}
