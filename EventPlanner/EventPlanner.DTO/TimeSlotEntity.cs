﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventPlanner.Entities
{
    [Table("TimeSlots")]
    public class TimeSlotEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { set; get; }

        [ForeignKey("Event")]
        public Guid EventId { get; set; }
        public virtual EventEntity Event { set; get; }

        [Required]
        public DateTime DateTime { set; get; }

        public virtual IList<VoteForDateEntity> VotesForDate { set; get; }
    }
}
