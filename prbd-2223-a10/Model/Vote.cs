using System.ComponentModel.DataAnnotations;
using PRBD_Framework;

namespace MyPoll.Model;

public enum VoteType {
    Yes, No, Maybe
}

public class Vote : EntityBase<MyPollContext> {
    public int UserId { get; set; }
    [Required]
    public virtual User User { get; set; }
    public int ChoiceId { get; set; }
    [Required]
    public virtual Choice Choice { get; set; }

    public VoteType Type { get;set; }

    public Vote() { }
    public Vote(int userid, int choiceid, VoteType votetype) {
        UserId = userid;
        ChoiceId = choiceid;
        Type = votetype;
    }
}

