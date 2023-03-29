using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

    public List<Choice> BestChoices => GetBestChoices();
    public string PollColor => GetPollColor();

    public string ChoiceTitle => BestChoicesTitle();

      public string GetPollColor() {
        string color = "";
        if (!Poll.Closed) {
            if (!HasAnswered) {
                color = "#cc424242";
            } else {
                color = "#cc024a1a";
            }
        } else {
            color = "#ccec8787";
        }
        return color;
    }

    public List<Choice> GetBestChoices() {
        List<Choice> choices = new List<Choice>();
        if (VoteCount != 0) {
           choices = Poll.GetBestChoices();
        }
        return choices;
    }

    public string BestChoicesTitle() {
        string title = "";
        if(Poll.GetBestChoices().Count > 1 && VoteCount!=0) {
            title = "Best choices";
        }else if(Poll.GetBestChoices().Count == 1) {
            title = "Best choice";
        }
        return title;
    }
    public int ParticipantCount => Context.Participations.Count(p => p.PollId == Poll.Id);
    public int VoteCount => (from v in Context.Votes
                                  join r in Context.Choices on v.ChoiceId equals r.Id
                                  where r.PollId == Poll.Id
                                  select v).Count();
    public bool HasAnswered => Context.Votes
       .Any(v => v.UserId == CurrentUser.Id && v.Choice.PollId == Poll.Id);

    public PollCardViewModel(Poll poll) {
        Poll = poll;
    }
}
