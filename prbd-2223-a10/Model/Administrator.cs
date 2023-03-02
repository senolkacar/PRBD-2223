namespace MyPoll.Model;

public class Administrator : User {
    public Administrator() { }
    public Administrator(string FullName, string Email, string Password)
    :base(FullName,Email,Password) {
        
    }

}
