using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PRBD_Framework;

namespace MyPoll.Model;

public class Participation : EntityBase<MyPollContext> {
   
    public int PollId { get; set; }
    [Required]
    public virtual Poll Poll { get; set; }

    public int UserId { get; set; }
    [Required]
    public virtual User User { get; set; }
}

