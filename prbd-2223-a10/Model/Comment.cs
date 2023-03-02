using System.ComponentModel.DataAnnotations;
using PRBD_Framework;

namespace MyPoll.Model;
public class Comment : EntityBase<MyPollContext> {
    public int Id { get; set; }
    [Required]
    public int UserId { get; set; }
    [Required]
    public virtual User User { get; set; }
    [Required]
    public int PollId { get; set; }
    [Required]
    public virtual Poll Poll { get; set; }
    [Required]
    public string Text { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;

    public Comment(int userid, int pollid, string text,DateTime timestamp) {
        UserId = userid;
        PollId = pollid;
        Text = text;
        Timestamp = timestamp;
    }

    public Comment() { }

}
