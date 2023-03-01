namespace MyPoll.Model;
public class Comment {
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PollId { get; set; }
    public string Text { get; set; }
    public DateTime TimeStamp { get; set; }

    public Comment(int id, int userid, int pollid, string text,DateTime timestamp) {
        Id = id;
        UserId = userid;
        PollId = pollid;
        Text = text;
        TimeStamp = timestamp;
    }

}
