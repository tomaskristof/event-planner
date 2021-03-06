﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EventPlanner.Models.Models.Shared;

namespace EventPlanner.Models.Models.CreateAndEdit
{
    public class EventModel: IValidatableObject
    {
        public EventModel()
        {
            Places = new List<FourSquareVenueModel>();
            Dates = new List<DatesModel>();
        }

        public Guid? Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Title", ResourceType = typeof(Resources.Event))]
        public string Title { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Resources.Event))]
        public string Desc { get; set; }

        [Range(1,99)]
        [Display(Name = "Expected_length", ResourceType = typeof(Resources.Event))]
        public int ExpectedLength { get; set; }
        
        [Display(Name = "Others_can_edit", ResourceType = typeof(Resources.Event))]
        public bool OthersCanEdit { get; set; }

        [Display(Name = "Places", ResourceType = typeof(Resources.Event))]
        public IList<FourSquareVenueModel> Places { get; set; }

        [Display(Name = "Dates", ResourceType = typeof(Resources.Event))]
        public IList<DatesModel> Dates { get; set; }


        public class DatesModel
        {
            public DateTime Date { get; set; }

            public IList<TimeModel> Times { get; set; }
        }

        public class TimeModel
        {
            public TimeModel()
            {
            }

            public TimeModel(Guid id, string time)
            {
                Id = id;
                Time = time;
            }

            public Guid? Id { get; set; }
            public string Time { get; set; }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Places == null || Places.Count == 0)
            {
                yield return new ValidationResult("You have to pick at least one place.", new [] {"Places"});
            }
            if (Dates == null || Dates.Count == 0)
            {
                yield return new ValidationResult("You have to pick at least one date.", new[] { "Dates" });
            }
            else
            {
                foreach(var date in Dates)
                {
                    if (date.Times == null || date.Times.Count == 0)
                    {
                        yield return new ValidationResult("You have to pick at least one time for each date.", new[] { "Dates.Times" });
                    }
                }
            }
        }
    }
}
