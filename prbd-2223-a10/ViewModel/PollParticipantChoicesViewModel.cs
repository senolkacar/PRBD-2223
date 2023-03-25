using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore.Metadata;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;
    public class PollParticipantChoicesViewModel : ViewModelCommon {

        public PollParticipantChoicesViewModel(PollChoicesViewModel pollChoicesViewModel,User participant,List<Choice> choices) {
            _pollChoicesViewModel = pollChoicesViewModel;
            _choices = choices;
            Participant = participant;
            RefreshVotes();

            EditCommand = new RelayCommand(() => EditMode = true);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            DeleteCommand = new RelayCommand(Delete);
        }

        private PollChoicesViewModel _pollChoicesViewModel;

        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand DeleteCommand { get; }

        private List<Choice> _choices;

        public User Participant { get; }

        private bool _editMode;
        public bool EditMode {
            get => _editMode;
            set => SetProperty(ref _editMode, value, EditModeChanged);
        }

        private void EditModeChanged() {
            foreach(PollVoteViewModel voteVM in _pollVoteVM) {
                voteVM.EditMode = EditMode;
            }
        _pollChoicesViewModel.AskEditMode(EditMode);
        }

        public void Changes() {
            RaisePropertyChanged(nameof(Editable));
        }

        public bool Editable => !EditMode && !ParentEditMode;
        public bool ParentEditMode => _pollChoicesViewModel.EditMode;

        private List<PollVoteViewModel> _pollVoteVM = new();

    public List<PollVoteViewModel> PollVoteVM {
        get => _pollVoteVM;
        private set => SetProperty(ref _pollVoteVM, value);
    }

    public void RefreshVotes() {
        PollVoteVM = _choices
            .Select(c => new PollVoteViewModel(Participant,c))
            .ToList();
    }

    private void Save() {
        EditMode = false;
        Participant.Votes = PollVoteVM.Where(v => v.HasVoted).Select(v=>v.Votes).ToList();
       RefreshVotes();
    }

    private void Cancel() {
        EditMode = false;
        RefreshVotes();
    }

    private void Delete() {
        Participant.Votes.Clear();
        Context.SaveChanges();
        RefreshVotes();
    }


}

