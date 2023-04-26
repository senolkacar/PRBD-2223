using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel
{
    public class PollParticipantListViewModel : ViewModelCommon
    {
        public PollParticipantListViewModel(User participant, Poll poll) {
            Poll = poll;
            Participant = participant;
        }

        private Poll _poll;
        public Poll Poll {
            get => _poll;
            set => SetProperty(ref _poll, value);
        }

        private User _participant;
        public User Participant {
            get => _participant;
            set => SetProperty(ref _participant, value);
        }

        public string FullName => Participant.FullName;
        public int VoteCount => (from v in Context.Votes
                                 join r in Context.Choices on v.ChoiceId equals r.Id
                                 where r.PollId == Poll.Id && v.UserId == Participant.Id
                                 select v).Count();
 
    }
}
