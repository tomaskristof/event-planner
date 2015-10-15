﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.DTO
{
    public class TimeSlot
    {
        public Guid Id { set; get; }

        [Required]
        public Guid EventId { set; get; }

        [Required]
        public DateTime DateTime { set; get; }

        public IEnumerable<VoteForDate> VotesForDate { set; get; }
    }
}
