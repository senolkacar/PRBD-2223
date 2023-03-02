﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRBD_Framework;

namespace MyPoll.Model;


    public enum PollType {
        Single, Multiple
    }

    public class Poll : EntityBase<MyPollContext> {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool Closed { get; set; }
        public PollType Type { get; set; }

        [Required]
        public int CreatorId { get; set; }

        [Required]
        public virtual User Creator { get; set; }

        public virtual ICollection<Participation> Participations { get; set; } = new HashSet<Participation>(); 

        public Poll(string name) {
            Name = name;
        }
        public Poll() { }
    }

