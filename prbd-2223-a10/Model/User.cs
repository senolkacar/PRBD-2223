using System.ComponentModel.DataAnnotations;
using PRBD_Framework;

namespace MyPoll.Model;

public class User : EntityBase<MyPollContext> {
    [Key]
    public int Id { get; set; }
    [Required]
    public string FullName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }

    public virtual ICollection<Poll> Polls { get; set; } = new HashSet<Poll>();
    public virtual ICollection<Participation> Participations { get; set; } = new HashSet<Participation>();
    public virtual ICollection<Vote> Votes { get; set; } = new HashSet<Vote>();

    public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    public User(string fullname, string email,string password) {
        FullName = fullname;
        Email = email;
        Password = password;
    }

    public User() { }
}
