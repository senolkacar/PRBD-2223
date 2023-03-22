using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyPoll.Model;
using PRBD_Framework;
using static MyPoll.App;

namespace MyPoll.ViewModel;
    public class PollCardViewModel : ViewModelCommon {
    private readonly Poll _poll;

    public Poll Poll {
        get => _poll;
        private init => SetProperty(ref _poll, value);
    }

    public string Name => Poll.Name;
    public string Creator => Poll.Creator.FullName;
    public bool IsClosed => Poll.Closed;
    public int ParticipantCount => Context.Participations.Count(p => p.PollId == Poll.Id);
    public int VoteCount => (from v in Context.Votes
                                  join r in Context.Choices on v.ChoiceId equals r.Id
                                  where r.PollId == Poll.Id
                                  select v).Count();


    public PollCardViewModel(Poll poll) {
        Poll = poll;
    }
}
