using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

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

        _participantVM = participants.Select(p => new PollParticipantChoicesViewModel(this, p, poll)).ToList();
    }

    private bool _editMode;
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }

    private bool _addCommentVisibility;

    public bool AddCommentVisibility {
        get => _addCommentVisibility;
        set => SetProperty(ref _addCommentVisibility, value,AddCommentVisChanged);

    }

    private void AddCommentVisChanged() {
        RaisePropertyChanged(nameof(AddCommentVisibility));
    }

    public ICommand AddComment => new RelayCommand(() => AddCommentVisibility = true);

    public ICommand NewComment => new RelayCommand(() => AddNewComment());

    private string _commentTxt;
    public string CommentTxt {
        get => _commentTxt;
        set => SetProperty(ref _commentTxt, value);
    }

    public string PollName => Poll.Name;

    public string Creator => Poll.Creator.FullName;

    private List<Choice> _choices;
    public List<Choice> Choices => _choices;
    private Poll _poll;
    public Poll Poll => _poll;

    public void AddNewComment() {
        Comment comment = new Comment {
            UserId = CurrentUser.Id,
            PollId = Poll.Id,
            Text = CommentTxt,
            Timestamp = DateTime.Now
        };
        Context.Add(comment);
        Context.SaveChanges();
        UpdateComments();
        AddCommentVisibility = false;
    }

    public void UpdateComments() {
        RaisePropertyChanged(nameof(Comments));
    }

    private List<PollParticipantChoicesViewModel> _participantVM;
    public List<PollParticipantChoicesViewModel> ParticipantVM => _participantVM;

    public void AskEditMode(bool editMode) {
        EditMode = editMode;
        foreach (var p in ParticipantVM) {
            p.Changes();
        }
    }

    public List<Comment> Comments => GetComments();

    public List<Comment> GetComments() {
        var list = Context.Comments
            .Where(c=>c.PollId == Poll.Id)
            .OrderByDescending(c=>c.Timestamp)
            .ToList();
        return list;
    }

    public PollChoicesViewModel() {

    }
}

