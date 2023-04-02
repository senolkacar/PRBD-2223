using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class PollViewModel: ViewModelCommon {
    private ObservableCollection<PollCardViewModel> _polls;

    public ObservableCollection<PollCardViewModel> Polls {
        get => _polls;
        set => SetProperty(ref _polls, value);
    }
    private string _filter;
    public string Filter {
        get => _filter;
        set => SetProperty(ref _filter, value,OnRefreshData);
    }

    public ICommand ClearFilter { get; set; }
    public ICommand DisplayPollDetails { get; set; }

    public ICommand CreatePoll{ get; set; }

    public PollViewModel(): base() {
        OnRefreshData();

        CreatePoll = new RelayCommand(() => NotifyColleagues(App.Polls.POLL_ADD, new Poll()));

        ClearFilter = new RelayCommand(() => Filter = "");

        DisplayPollDetails = new RelayCommand<PollCardViewModel>(vm => {
            NotifyColleagues(App.Polls.POLL_DISPLAY, vm.Poll);
        });
        Register<Poll>(App.Polls.POLL_CHANGED, poll => OnRefreshData()) ;
    }

    protected override void OnRefreshData() {
        IQueryable<Poll> polls = string.IsNullOrEmpty(Filter) ? Poll.GetAll() : Poll.GetFiltered(Filter);
        var filteredPolls = from p in polls
                            where p.Creator.FullName == CurrentUser.FullName ||
                            p.Participations.Any(f => CurrentUser!=null && f.UserId == CurrentUser.Id)
                            orderby p.Name
                             select p;
        Polls = new ObservableCollection<PollCardViewModel>(filteredPolls.Select(p => new PollCardViewModel(p)));
    }
}
