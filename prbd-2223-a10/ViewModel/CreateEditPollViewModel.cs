using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using MyPoll.Model;
using PRBD_Framework;
using static MyPoll.App;

namespace MyPoll.ViewModel;

public class CreateEditPollViewModel : ViewModelCommon {
    public CreateEditPollViewModel(Poll poll,bool isNew) {
        Poll = poll;
        IsNew = isNew;
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

    
    public CreateEditPollViewModel() { }

    public string Creator => IsNew ? CurrentUser.FullName : Poll.Creator.FullName;

    public string PollName => IsNew ? "" : Poll.Name;

    public List<User> Participants => IsNew ? new List<User>() : GetParticipants();

    public List<User> GetParticipants() {
        var list = (from u in Context.Users
                    join p in Context.Participations on u.Id equals p.UserId
                    where p.PollId == Poll.Id
                    select u).Distinct().ToList();
        return list;
    }

    public List<User> Users => GetUsers().Where(p => !GetParticipants().Any(u => u.Id == p.Id)).ToList();

    public List<User> GetUsers() {
        var list = (from u in Context.Users
                    where !(u is Administrator)
                   select u).ToList();
        return list;
    }

    public List<Choice> Choices => IsNew ? new List<Choice>() : GetChoices();

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
        if (SelectedUser != null && !isParticipant(SelectedUser.Id)) {
            var p = new Participation { PollId = Poll.Id, UserId = SelectedUser.Id };
            Context.Participations.Add(p);
            Context.SaveChanges();
            RaisePropertyChanged();
        }
        NotifyColleagues(App.Polls.POLL_CHANGED, Poll);

    }

    public bool isParticipant(int userId) {
        return Context.Participations.Any(p => p.UserId == userId && p.PollId == Poll.Id);
    }
    public bool MyParticipation => Context.Participations.Any(p => p.UserId == CurrentUser.Id && p.PollId == Poll.Id) ? false : true;
   
    public ICommand AddMyself => new RelayCommand(() => addMyself());
    public void addMyself() {
        if (Context.Participations.Any(p => p.UserId == CurrentUser.Id)) {
            var p = new Participation { PollId = Poll.Id, UserId = CurrentUser.Id };
            Context.Participations.Add(p);
            Context.SaveChanges();
            RaisePropertyChanged(nameof(MyParticipation),nameof(Participants),nameof(Users));
        }
        NotifyColleagues(App.Polls.POLL_CHANGED, Poll);
    }
    public bool UserRemainFromList => GetParticipants().Count() == GetUsers().Count() ? false : true; 


    public ICommand AddEveryBody => new RelayCommand(()=>addEverybody());

    public void addEverybody() {
        var pollParticipants = Context.Participations.Where(p => p.PollId == Poll.Id).ToList();
        var users = GetUsers();
        foreach (User u in users) {
            if (!pollParticipants.Any(p => p.UserId == u.Id)) {
                var p = new Participation { PollId = Poll.Id, UserId = u.Id };
                Context.Participations.Add(p);
            }
        }
        Context.SaveChanges();
        NotifyColleagues(App.Polls.POLL_CHANGED, Poll);
        RaisePropertyChanged(nameof(UserRemainFromList),nameof(Participants),nameof(Users));
    }

}

   

