using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Converters;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using MyPoll.Model;
using PRBD_Framework;
using static MyPoll.App;

namespace MyPoll.ViewModel;

public class CreateEditPollViewModel : ViewModelCommon {
    public CreateEditPollViewModel(Poll poll, bool isNew) {
        Poll = poll;
        IsNew = isNew;
        Users = new ObservableCollection<User>(AllUsers);
        ParticipantList = new ObservableCollection<User>(GetParticipants());
        Participants = new ObservableCollection<PollParticipantListViewModel>(ParticipantList.Select(p => new PollParticipantListViewModel(p, poll)));
        ChoicesList = new ObservableCollection<Choice>(GetChoices());
        Choices = new ObservableCollection<PollChoiceListViewModel>(ChoicesList.Select(c => new PollChoiceListViewModel(c)));
        IsClosed = IsNew ? false : Poll.Closed;
        PollType = IsNew ? PollType.Multiple : Poll.Type;
        EditMode = false;
        UserRemainFromList = Users.Count() > 0 ? true : false;
        NoChoice = IsNew ? "hidden" : ChoicesList.Count > 0 ? "visible" : "hidden";
        NoParticipant = IsNew ? "hidden" : Participants.Count() > 0 ? "visible" : "hidden";
        MyParticipation = Context.Participations.Any(p => p.UserId == CurrentUser.Id && p.PollId == Poll.Id) ? false : true;
        RemoveChoiceCMD = new RelayCommand<Choice>(RemoveChoice);
        SaveChoiceCMD = new RelayCommand(SaveChoice);
        Save = new RelayCommand(SaveAction, CanSaveAction);
        Cancel = new RelayCommand(CancelAction);
        Delete = new RelayCommand(DeleteAction, () => !IsNew);
        RemoveParticipation = new RelayCommand<User>(DeleteParticipant);
        RaisePropertyChanged();
    }
    private Poll _poll;
    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);
    }

    private PollType _pollType;
    public PollType PollType {
        get => _pollType;
        set {
            if (SetProperty(ref _pollType, value)) {
                Poll.Type = value;
            }
            
        } 
    }

    private bool _isClosed;
    public bool IsClosed {
        get => _isClosed;
        set {
            if (SetProperty(ref _isClosed, value)) {
                Poll.Closed = value;
            }
        }
    }

    private bool _isNew;
    public bool IsNew {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }

    private string _noChoice;
    public string NoChoice {
        get => _noChoice;
        set => SetProperty(ref _noChoice, value);
    }

    private bool _userRemainFromList;
    public bool UserRemainFromList {
        get => _userRemainFromList;
        set => SetProperty(ref _userRemainFromList, value);
    }

    private string _noParticipant;
    public string NoParticipant {
        get => _noParticipant;
        set => SetProperty(ref _noParticipant, value);
    }

    private bool _myParticipation;
    public bool MyParticipation {
        get => _myParticipation;
        set => SetProperty(ref _myParticipation, value);
    }

    private ObservableCollection<User> _participantList;
    public ObservableCollection<User> ParticipantList {
        get => _participantList;
        set => SetProperty(ref _participantList, value);
    }

    private ObservableCollection<PollParticipantListViewModel> _participants;
    public ObservableCollection<PollParticipantListViewModel> Participants {
        get => _participants;
        set => SetProperty(ref _participants, value);
    }

    private ObservableCollection<User> _users;
    public ObservableCollection<User> Users {
        get => _users;
        set => SetProperty(ref _users, value);
    }

    private ObservableCollection<PollChoiceListViewModel> _choices;
    public ObservableCollection<PollChoiceListViewModel> Choices {
        get => _choices;
        set =>  SetProperty(ref _choices, value);
    }

    public string Name {
        get => Poll?.Name;
        set => SetProperty(Poll.Name, value, Poll, (p, v) => { p.Name = v;
            //NotifyColleagues(App.Polls.POLL_NAME_CHANGED, Poll);
        });

    }

    private string _choice;

    public string NewChoice {
        get => _choice;
        set => SetProperty(ref _choice, value);
    }

    private ObservableCollection<Choice> _choicesList;
    public ObservableCollection<Choice> ChoicesList {
        get => _choicesList;
        set => SetProperty(ref _choicesList, value);
    }

    private bool _editMode;
    public bool EditMode {
        get => _editMode;
        set => SetProperty(ref _editMode, value);
    }
    public ICommand Save { get; set; }
    public ICommand Cancel { get; set; }
    public ICommand Delete { get; set; }
    public ICommand RemoveParticipation { get; set; }

    public ICommand SaveChoiceCMD { get; set; }
    public ICommand RemoveChoiceCMD { get; set; }

    public ICommand AddMyself => new RelayCommand(() => addMyself());

    public ICommand AddSelectedUser => new RelayCommand(() => AddUser());
    public ICommand AddChoice => new RelayCommand(() => addChoice());

    public ICommand AddEveryBody => new RelayCommand(() => addEverybody());

    public ICommand ReOpen => new RelayCommand(()=> IsClosed = false);

    public CreateEditPollViewModel() { }

    public string Creator => IsNew ? CurrentUser?.FullName : Poll?.Creator?.FullName;

    public string PollName => IsNew ? "<New Poll>" : Poll?.Name;


    public List<User> GetParticipants() {
        var list = (from u in Context.Users
                    join p in Context.Participations on u.Id equals p.UserId
                    where p.PollId == Poll.Id
                    orderby u.FullName ascending
                    select u).Distinct().ToList();
        return list;
    }

    public List<User> AllUsers => GetUsers().Where(p => !GetParticipants().Any(u => u.Id == p.Id)).ToList();

    public List<User> GetUsers() {
        var list = (from u in Context.Users
                    orderby u.FullName ascending
                    select u).ToList();
        return list;
    }


    public List<Choice> GetChoices() {
        var list = (from c in Context.Choices
                    where c.PollId == Poll.Id
                    orderby c.Label ascending
                    select c).ToList();
        return list;
    }

    private User _selectedUser;

    public User SelectedUser {
        get => _selectedUser;
        set => SetProperty(ref _selectedUser, value);

    }

    private Choice _selectedChoice;
    public Choice SelectedChoice {
        get => _selectedChoice;
        set => SetProperty(ref _selectedChoice, value);
    }

    private bool _choiceIsModified = false;
    public bool ChoiceIsModified {
        get => _choiceIsModified;
        set => SetProperty(ref _choiceIsModified, value);
    }


   private void AddUser() {
            if (SelectedUser != null) {
            var p = new Participation { Poll = Poll, UserId = SelectedUser.Id };
            Context.Participations.Add(p);
            if (SelectedUser.Id == CurrentUser.Id) {
                MyParticipation = false;
            }
            ParticipantList.Add(SelectedUser);
            Users.Remove(SelectedUser);
            if (Users.Count == 0) {
                UserRemainFromList = false;
            }
            NoParticipant = "visible";
            RaisePropertyChanged(nameof(ParticipantList), nameof(NoParticipant), nameof(Users), nameof(MyParticipation), nameof(UserRemainFromList));
        }
        OnRefreshData();
    }

    private void addMyself() {
        if (Context.Participations.Any(p => p.UserId == CurrentUser.Id)) {
            var p = new Participation { Poll = Poll, User = CurrentUser };
            Context.Participations.Add(p);
            ParticipantList.Add(CurrentUser);
            Users.Remove(CurrentUser);
            MyParticipation = false;
            if (Users.Count == 0) {
                UserRemainFromList = false;
            }
            NoParticipant = "visible";
            RaisePropertyChanged(nameof(MyParticipation),nameof(NoParticipant), nameof(ParticipantList), nameof(Users));
        }
        OnRefreshData();
    }
    private void addEverybody() {
        var pollParticipants = ParticipantList.ToList();
        foreach (User u in Users) {
            if (!pollParticipants.Any(p => p.Id == u.Id)) {
                var p = new Participation { Poll = Poll, UserId = u.Id };
                ParticipantList.Add(u);
                Context.Participations.Add(p);

            }
        }
        Users = new ObservableCollection<User>();
        UserRemainFromList = false;
        MyParticipation = false;
        NoParticipant = "visible";
        NotifyColleagues(App.Polls.POLL_CHANGED, Poll);
        RaisePropertyChanged(nameof(UserRemainFromList), nameof(ParticipantList), nameof(NoParticipant), nameof(Users), nameof(MyParticipation));
        OnRefreshData();

    }
    private void addChoice() {
        if (!NewChoice.IsNullOrEmpty()) {
            Choice choice = new Choice { Poll = Poll, Label = NewChoice };
            ChoicesList.Add(choice);
            NewChoice = "";
            NoChoice = "visible";
            RaisePropertyChanged(nameof(ChoicesList));
            Context.Choices.Add(choice);
        }
        OnRefreshData();
    }

    private void RemoveChoice(Choice choice) {
        ChoicesList.Remove(choice);
        Context.Choices.Remove(choice);
        OnRefreshData();
    }

    private void SaveChoice() {
        if (ChoiceIsModified) {
            
            foreach (Choice choice in ChoicesList) {
                var contextChoice = Context.Choices.SingleOrDefault(c => c.Id == choice.Id);
                if (contextChoice != null) {
                    contextChoice.Label = choice.Label;
                    contextChoice.Votes = choice.Votes;
                }

            }
        }
        NotifyColleagues(App.Polls.POLL_CHANGED, Poll);
        OnRefreshData();
    }

   
    public override void SaveAction() {
        if (IsNew) {
            Context.Polls.Add(Poll);
            IsNew = false;
        }
        //Console.WriteLine(Context.ChangeTracker.DebugView.LongView);
        Context.SaveChanges();
        ChoiceIsModified = false;
        NotifyColleagues(App.Polls.POLL_CHANGED, Poll);
    }

    private bool CanSaveAction() {
        if (IsNew)
            return !string.IsNullOrEmpty(Name) && (NoChoice == "visible" && NoParticipant == "visible") ;
        return HasChanges && (Poll.Participations.Count()>0 && ChoicesList.Count()>0) || ChoiceIsModified;
    }
    public override void CancelAction() {
        Context.Entry(Poll).State = EntityState.Detached;
        Context.Entry(Poll).Reload();

        NotifyColleagues(App.Polls.POLL_CLOSE_TAB, Poll);
        RaisePropertyChanged();
    }


    private void DeleteAction() {
        CancelAction();
        Context.Polls.Remove(Poll);
        NotifyColleagues(App.Polls.POLL_CHANGED, Poll);
        NotifyColleagues(App.Polls.POLL_CLOSE_TAB, Poll);
    }

    private void DeleteParticipant(User user) {
   
        MessageBoxResult msgRes;
        int VoteCount = (from v in Context.Votes
                      join r in Context.Choices on v.ChoiceId equals r.Id
                      where r.PollId == Poll.Id && v.UserId == user.Id
                      select v).Count();
        if (VoteCount>0) {
            string msg = $"This participant has already {VoteCount} for this poll.\nIf you proceed and save the poll, his vote(s) will be deleted.\nDo you confirm";
            msgRes = MessageBox.Show(msg,"Confirmation",MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (msgRes == MessageBoxResult.No) {
                return;
            } else {
                var votesToRemove = Context.Votes.Where(v => v.UserId == user.Id && v.Choice.PollId == Poll.Id);
                Context.Votes.RemoveRange(votesToRemove);
            }
        }

        var participation = Context.Participations.SingleOrDefault(p => p.PollId == Poll.Id && p.UserId == user.Id);
        if (participation == null) {
            participation = Poll.Participations.SingleOrDefault(p => p.PollId == Poll.Id && p.UserId == user.Id);
        }
     Participants.Remove(Participants.Single(p => p.Participant.Id == user.Id));
     if( participation != null ) {
        Poll.Participations.Remove(participation);
      }
     
     ParticipantList.Remove(user);
     Users.Add(user);
    if (user == CurrentUser) {
        MyParticipation = true;
    }
    if(ParticipantList.Count == 0) {
        NoParticipant = "hidden";
    }
    if (Users.Count > 0) {
      UserRemainFromList = true;
    }

    }

    protected override void OnRefreshData(){
        //Console.WriteLine(this.GetHashCode());
        ChoiceIsModified = !GetChoices().SequenceEqual(ChoicesList.ToList());
        Participants = new ObservableCollection<PollParticipantListViewModel>(ParticipantList.Select(p => new PollParticipantListViewModel(p, Poll)));
        Choices = new ObservableCollection<PollChoiceListViewModel>(ChoicesList.Select(c => new PollChoiceListViewModel(c)));
    }





}

   

