namespace MyPoll.Model;

public class Choice {
    public int Id { get; set; }
    public int PollId { get; set; }
    public string Label { get; set; }

    public Choice(int id, int pollid, string label) {
        Id = id;
        PollId = pollid;
        Label= label;
    }
}
