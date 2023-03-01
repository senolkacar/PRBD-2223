namespace MyPoll.Model;

public class Administrator : User {
    public Administrator(int id, string fullname, string email, string password)
    :base(id,fullname,email,password) {
        
    }

}
