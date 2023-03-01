namespace MyPoll.Model;

public enum VoteType {
    YES, NO, MAYBE
}

public class Vote {
    public int UserId { get; set; }
    public int ChoiceId { get; set; }

    public VoteType VoteType { get;set; }

    public Vote(int userid, int choiceid, VoteType votetype) {
        UserId = userid;
        ChoiceId = choiceid;
        VoteType = votetype;
    }
}

