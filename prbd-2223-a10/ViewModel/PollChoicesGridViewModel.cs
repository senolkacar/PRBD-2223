using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;
using static MyPoll.App;

namespace MyPoll.ViewModel;
public class PollChoicesGridViewModel : ViewModelCommon {
    public PollChoicesGridViewModel(Poll poll) {
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
        PollClosed = Poll.Closed;
        EditPoll = new RelayCommand(() => NotifyColleagues(App.Polls.POLL_EDIT, _poll));
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

    private bool _pollClosed;
    public bool PollClosed {
        get => _pollClosed;
        set {
            if (SetProperty(ref _pollClosed, value)) {
                Poll.Closed = value;
            }
        }
    }


    private void AddCommentVisChanged() {
        RaisePropertyChanged(nameof(AddCommentVisibility));
    }

    public ICommand ReOpen => new RelayCommand(() => ReOpenPoll());

    public ICommand AddComment => new RelayCommand(() => AddCommentClicked());

    public ICommand DeleteComment => new RelayCommand<Comment>(RemoveComment);

    public ICommand EditPoll { get; set; }


    public ICommand DeletePoll => new RelayCommand(() => deletePoll());
   
    private string _commentTxt;
    public string CommentTxt {
        get => _commentTxt;
        set => SetProperty(ref _commentTxt, value);
    }

    public ICommand NewComment => new RelayCommand(AddNewComment, () => { return !String.IsNullOrEmpty(_commentTxt); });


    private void ReOpenPoll() {
        PollClosed = false;
        Context.SaveChanges();
        RaisePropertyChanged(nameof(ReOpenButtonVisibility),nameof(AddCommentTitleVisibility));
        NotifyColleagues(App.Polls.POLL_CHANGED, Poll);
    }

    private void AddCommentClicked() {
        AddCommentVisibility = true;
        RaisePropertyChanged(nameof(AddCommentTitleVisibility));
    }

    private void RemoveComment(Comment comment) {
        Context.Comments.Remove(comment);
        Context.SaveChanges();
        UpdateComments();
    }

    private void deletePoll() {
        MessageBoxResult msgRes;
        string msg = "Are you sure you want to delete this poll ? This action cannot be recovered!";
        msgRes = MessageBox.Show(msg, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Information);
        if (msgRes == MessageBoxResult.No) {
            return;
        } else {
            Poll.Delete();
            NotifyColleagues(App.Polls.POLL_CHANGED, Poll);
            NotifyColleagues(App.Polls.POLL_CLOSE_TAB, Poll);
        }
       
    }

    public bool AddCommentTitleVisibility => !AddCommentVisibility && !PollClosed;

    public bool DeleteCommentVisibility => IsAdmin || CurrentUser == Poll?.Creator;
    public string EditDeleteButtonVisibility => CurrentUser == Poll?.Creator || IsAdmin ? "visible" : "hidden";

    public bool ReOpenButtonVisibility => (CurrentUser == Poll?.Creator || IsAdmin) && PollClosed;
    public string PollName => Poll?.Name;
    public string Creator => Poll?.Creator?.FullName;

    private List<Choice> _choices;
    public List<Choice> Choices => _choices;
    private Poll _poll;
    public Poll Poll => _poll;

    private void AddNewComment() {
        if(!string.IsNullOrEmpty(CommentTxt)) { 
        Comment comment = new Comment {
            UserId = CurrentUser.Id,
            PollId = Poll.Id,
            Text = CommentTxt,
            Timestamp = DateTime.Now
        };
        Context.Add(comment);
        Context.SaveChanges();
        CommentTxt = "";
        UpdateComments();
        AddCommentVisibility = false;
        RaisePropertyChanged(nameof(AddCommentTitleVisibility));
        }
    }

    private void UpdateComments() {
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
        if (Poll == null)
            return new List<Comment>();
        var list = Context.Comments
            .Where(c=>c.PollId == Poll.Id)
            .OrderByDescending(c=>c.Timestamp)
            .ToList();
        return list;
    }

    protected override void OnRefreshData() {
        
    }

    public PollChoicesGridViewModel() {
        
    }
}

