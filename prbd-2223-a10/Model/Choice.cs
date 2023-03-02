using System.ComponentModel.DataAnnotations;
using PRBD_Framework;

namespace MyPoll.Model;

public class Choice : EntityBase<MyPollContext> {
    public int Id { get; set; }
    public int PollId { get; set; }
    [Required]
    public virtual Poll Poll { get; set; }
    [Required]
    public string Label { get; set; }
    public virtual ICollection<Vote> Votes { get; set; } = new HashSet<Vote>();


    public Choice() { }
    public Choice(int pollid, string label) {
        PollId = pollid;
        Label= label;
    }
}
