﻿using System;
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
        private List<Vote> _originalVotes;
        public PollParticipantChoicesViewModel(PollChoicesGridViewModel pollChoicesViewModel,User participant,Poll poll) {
            _pollChoicesViewModel = pollChoicesViewModel;
            _poll = poll;
            Participant = participant;
            RefreshVotes(Poll);

            EditCommand = new RelayCommand(() => EditMode = true);
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
            DeleteCommand = new RelayCommand(Delete);
        }

        private PollChoicesGridViewModel _pollChoicesViewModel;

        public ICommand EditCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand DeleteCommand { get; }


        public List<Choice> choices => Poll.GetChoices();

        private Poll _poll;

        public Poll Poll {
            get => _poll;
        }

        public User Participant { get; }

        private bool _editMode;
        public bool EditMode {
            get => _editMode;
            set => SetProperty(ref _editMode, value, EditModeChanged);
        }

        private void EditModeChanged() {
        if (EditMode) {
            _originalVotes = Participant.Votes.ToList();
        }
        foreach (PollVoteViewModel voteVM in _pollVoteVM) {
                voteVM.EditMode = EditMode;
            }
        _pollChoicesViewModel.AskEditMode(EditMode);
        }

        public void Changes() {
            RaisePropertyChanged(nameof(Editable));
        }

        public bool Editable => !EditMode && !ParentEditMode && !Poll.Closed && (UserConnected || IsAdmin);
        public bool ParentEditMode => _pollChoicesViewModel.EditMode;

        public bool UserConnected => Participant.Id == CurrentUser.Id;

        private List<PollVoteViewModel> _pollVoteVM = new();

    public List<PollVoteViewModel> PollVoteVM {
        get => _pollVoteVM;
        private set => SetProperty(ref _pollVoteVM, value);
    }

    public void RefreshVotes(Poll poll) {
        PollVoteVM = poll.GetChoices()
            .Select(c => new PollVoteViewModel(Participant,c))
            .ToList();
    }

    private void Save() {
        EditMode = false;
        var votesToUpdate = PollVoteVM.Where(v => v.HasVoted).Select(v => v.Votes).ToList();
        var existingVotes = Participant.Votes.Where(v => !votesToUpdate.Contains(v)).ToList();
        Participant.Votes = existingVotes.Concat(votesToUpdate).ToList();
        Context.SaveChanges();
        RefreshVotes(Poll);
        NotifyColleagues(App.Polls.POLL_CHANGED,Poll);
    }

    private void Cancel() {
        EditMode = false;
        if (_originalVotes != null) {
            Participant.Votes = _originalVotes;
            _originalVotes = null;
        }
        RefreshVotes(Poll);
    }

    private void Delete() {
        Participant.Votes.Clear();
        Context.SaveChanges();
        RefreshVotes(Poll);
    }


}

