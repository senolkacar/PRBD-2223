using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    [NotMapped]
    public double Score => GetScore();

    public Choice() {

    }
    public Choice(int pollid, string label) {
        PollId = pollid;
        Label= label;
    }

    

    public double GetScore() {
        var TotalVotes = Context.Votes.Where(v => v.ChoiceId == Id);
        int yesCount = TotalVotes.Count(v=>v.Type == VoteType.Yes);
        int maybeCount = TotalVotes.Count(v=>v.Type == VoteType.Maybe);
        int noCount = TotalVotes.Count(v=>v.Type == VoteType.No);

        return yesCount*1 + maybeCount*0.5 + noCount*-1;
                 
    }


}
