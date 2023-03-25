using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using FontAwesome6;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;
    public class PollVoteViewModel : ViewModelCommon {
        public PollVoteViewModel(User participant,Choice choice) {
            HasVoted = participant.Votes.Any(v => v.Choice.Id == choice.Id);

            Votes = participant.Votes.FirstOrDefault(v => v.Choice.Id == choice.Id,
                new Vote() {User = participant,Choice = choice});
            ChangeVotes = new RelayCommand(() => HasVoted = !HasVoted);
        }
        public Vote Votes { get; private set; }
        public PollVoteViewModel() { }
        private bool _editMode;
        public bool EditMode {
            get => _editMode;
            set => SetProperty(ref _editMode, value);
        }
        public ICommand ChangeVotes { get; set; }
        private bool _hasVoted;
        public bool HasVoted {
            get => _hasVoted;
            set =>SetProperty(ref _hasVoted, value);
        }

        public EFontAwesomeIcon VotedIcon => HasVoted ? EFontAwesomeIcon.Solid_Check : EFontAwesomeIcon.None;
        public Brush VoteColor => HasVoted ? Brushes.Green : Brushes.White;

        public string VoteToolTip => HasVoted ? "Yes" : "No";

    }
