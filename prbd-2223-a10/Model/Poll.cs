namespace MyPoll.Model;


    public enum PollType {
        SINGLE, MULTIPLE
    }

    public class Poll {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CreatorId { get; set; }
        public bool Closed { get; set; }
        public PollType PollType { get; set; }
        public Poll(int id,string name, int creatorid) {
            Id = id;
            Name = name;
            CreatorId = creatorid;
        }
        
    }

