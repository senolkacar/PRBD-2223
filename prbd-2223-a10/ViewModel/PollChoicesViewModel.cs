using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPoll.Model;

namespace MyPoll.ViewModel;
public class PollChoicesViewModel : ViewModelCommon {
    public PollChoicesViewModel(Poll poll) {
        _poll = poll;
        _choices = (from c in Context.Choices
                    where c.PollId == poll.Id
                    orderby c.Label ascending
                    select c).ToList();
        var participants = (from u in Context.Users
                            join pt in Context.Participations on u.Id equals pt.UserId
                            where pt.PollId == poll.Id
                            orderby u.FullName ascending
                            select u).ToList();

        _participantVM = participants.Select(p => new PollParticipantChoicesViewModel(this, p, _choices)).ToList();
    }

    private bool _editMode;
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }

    public string PollName => Poll.Name;

    public string Creator => Poll.Creator.FullName;

    private List<Choice> _choices;
    public List<Choice> Choices => _choices;
    private Poll _poll;
    public Poll Poll => _poll;

    private List<PollParticipantChoicesViewModel> _participantVM;
    public List<PollParticipantChoicesViewModel> ParticipantVM => _participantVM;

    public void AskEditMode(bool editMode) {
        EditMode = editMode;
        foreach (var p in ParticipantVM) {
            p.Changes();
        }
    }

    public PollChoicesViewModel() {

    }
}

