namespace MyPoll.Model;

public class Participation {
    public int PollId { get; set; }
    public int UserId { get; set; }


    public Participation(int pollid, int userid) {
        PollId = pollid;
        UserId = userid;
    }
} 
