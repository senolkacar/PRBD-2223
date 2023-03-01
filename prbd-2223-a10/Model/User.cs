using PRBD_Framework;

namespace MyPoll.Model;

public class User : EntityBase<MyPollContext> {
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public User(int id,string fullname, string email,string password) {
        Id = id;
        FullName = fullname;
        Email = email;
        Password = password;
    }
}
