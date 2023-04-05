using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using MyPoll.Model;
using PRBD_Framework;
using static MyPoll.App;

namespace MyPoll.ViewModel;

public class CreateEditPollViewModel : ViewModelCommon {
    public CreateEditPollViewModel(Poll poll, bool isNew) {
        Poll = poll;
        IsNew = isNew;
        Users = new ObservableCollection<User>(AllUsers);
        Participants = new ObservableCollection<User>(GetParticipants());
        Choices = new ObservableCollection<Choice>(GetChoices());
        UserRemainFromList = Users.Count() > 0 ? true : false;
        NoChoice = IsNew ? "hidden" : Choices.Count > 0 ? "visible" : "hidden";
        NoParticipant = IsNew ? "hidden" : Participants.Count() > 0 ? "visible" : "hidden";
        MyParticipation = Context.Participations.Any(p => p.UserId == CurrentUser.Id && p.PollId == Poll.Id) ? false : true;
        RemoveParticipant = new RelayCommand<User>(removeParticipant);
        Save = new RelayCommand(SaveAction, CanSaveAction);
        Cancel = new RelayCommand(CancelAction);
        Delete = new RelayCommand(DeleteAction, () => !IsNew);
        RaisePropertyChanged();
    }
    private Poll _poll;
    public Poll Poll {
        get => _poll;
        set => SetProperty(ref _poll, value);
    }

    private bool _isNew;
    public bool IsNew {
        get => _isNew;
        set => SetProperty(ref _isNew, value);
    }

    private string _noChoice;
    public String NoChoice {
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

    private ObservableCollection<User> _participants;
    public ObservableCollection<User> Participants {
        get => _participants;
        set => SetProperty(ref _participants, value);
    }

    private ObservableCollection<User> _users;
    public ObservableCollection<User> Users {
        get => _users;
        set => SetProperty(ref _users, value);
    }

    public string Name {
        get => Poll?.Name;
        set => SetProperty(Poll.Name, value, Poll, (p, v) => { p.Name = v;
            //NotifyColleagues(App.Polls.POLL_NAME_CHANGED, Poll);
        });

    }

    private ObservableCollection<Choice> _choices;
    public ObservableCollection<Choice> Choices {
        get => _choices;
        set => SetProperty(ref _choices, value);
    }

    public ICommand Save { get; set; }
    public ICommand Cancel { get; set; }
    public ICommand Delete { get; set; }
    public ICommand RemoveParticipant { get; set; }

    public CreateEditPollViewModel() { }

    public string Creator => IsNew ? CurrentUser.FullName : Poll.Creator.FullName;

    public string PollName => IsNew ? "<New Poll>" : Poll.Name;


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

    public ICommand AddSelectedUser => new RelayCommand(() => AddUser());

    public void AddUser() {
        if (SelectedUser != null) {
            var p = new Participation { Poll = Poll, UserId = SelectedUser.Id };
            Context.Participations.Add(p);

            if (SelectedUser.Id == CurrentUser.Id) {
                MyParticipation = false;
            }
            Participants.Add(SelectedUser);
            Users.Remove(SelectedUser);
            if (Users.Count == 0) {
                UserRemainFromList = false;
            }
            NoParticipant = "visible";

            RaisePropertyChanged(nameof(Participants), nameof(NoParticipant), nameof(Users), nameof(MyParticipation), nameof(UserRemainFromList));
        }
        NotifyColleagues(App.Polls.POLL_CHANGED, Poll);


    }

    public bool isParticipant(int userId) {
        return Context.Participations.Any(p => p.UserId == userId && p.PollId == Poll.Id);
    }

 
    public ICommand AddMyself => new RelayCommand(() => addMyself());
    public void removeParticipant(User participant) {
        Participants.Remove(participant);
        var participation = Context.Participations.SingleOrDefault(p => p.UserId == participant.Id && p.PollId == Poll.Id);
        if (participation != null) {
            Context.Participations.Remove(participation);
        }
        Users.Add(participant);
        UserRemainFromList = true;
        if(participant == CurrentUser) {
            MyParticipation = true;
        }
        if(Participants.Count == 0) {
            NoParticipant = "hidden";
        }
        RaisePropertyChanged(nameof(MyParticipation), nameof(NoParticipant), nameof(Participants), nameof(Users));
        NotifyColleagues(App.Polls.POLL_CHANGED, Poll);
    }
    public void addMyself() {
        if (Context.Participations.Any(p => p.UserId == CurrentUser.Id)) {
            //var p = new Participation { PollId = Poll.Id, UserId = CurrentUser.Id };
            var p = new Participation { Poll = Poll, UserId = CurrentUser.Id };
            Context.Participations.Add(p);
            Participants.Add(CurrentUser);
            Users.Remove(CurrentUser);
            MyParticipation = false;
            if (Users.Count == 0) {
                UserRemainFromList = false;
            }
            NoParticipant = "visible";
            RaisePropertyChanged(nameof(MyParticipation),nameof(NoParticipant), nameof(Participants), nameof(Users));
        }
        NotifyColleagues(App.Polls.POLL_CHANGED, Poll);
    }


    private string _choice;

    public string Choice {
        get => _choice;
        set => SetProperty(ref _choice, value);
    }

    public bool isChoiceNull => Choice.IsNullOrEmpty() ? false : true;
    public ICommand AddChoice => new RelayCommand(() => addChoice());
    public void addChoice() {
        if (!Choice.IsNullOrEmpty()) {
            Choice choice = new Choice { Poll = Poll, Label = Choice };
            Context.Choices.Add(choice);
            Choices.Add(choice);
            Choice = "";
            NoChoice = "visible";
            RaisePropertyChanged(nameof(Choices));
        }
    }
    public ICommand AddEveryBody => new RelayCommand(() => addEverybody());

    public void addEverybody() {
        var pollParticipants = Participants.ToList();
        foreach (User u in Users) {
            if (!pollParticipants.Any(p => p.Id == u.Id)) {
                var p = new Participation { Poll = Poll, UserId = u.Id };
                Participants.Add(u);
                Context.Participations.Add(p);
                Users = new ObservableCollection<User>();

            }
        }
        UserRemainFromList = false;
        MyParticipation = false;
        NoParticipant = "visible";
        NotifyColleagues(App.Polls.POLL_CHANGED, Poll);
        RaisePropertyChanged(nameof(UserRemainFromList), nameof(Participants), nameof(NoParticipant),nameof(Users), nameof(MyParticipation));
    }
    public override void SaveAction() {
        //throw pollid exception for new poll
        if (IsNew) {
            Context.Polls.Add(Poll);
            IsNew = false;
        }
        Console.WriteLine(Context.ChangeTracker.DebugView.LongView);
        Context.SaveChanges();
        NotifyColleagues(App.Polls.POLL_CHANGED, Poll);
    }

    private bool CanSaveAction() {
        if (IsNew)
            return !string.IsNullOrEmpty(Name) && (NoChoice == "visible" && NoParticipant == "visible") ;
        return Context.ChangeTracker.HasChanges();
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

   
}

   

