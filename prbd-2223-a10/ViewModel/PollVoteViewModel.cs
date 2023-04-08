using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using FontAwesome6;
using Microsoft.Extensions.Logging;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;
    public class PollVoteViewModel : ViewModelCommon {
    private readonly User participant;
    private readonly Choice choice;
    public PollVoteViewModel(User participant,Choice choice) {
        this.participant = participant;
        this.choice = choice;
        HasVoted = participant.Votes.Any(v => v.ChoiceId == choice.Id);
        SelectedVoteType = (from v in Context.Votes
                            where v.ChoiceId == choice.Id && v.UserId == participant.Id
                            select v.Type).FirstOrDefault();
        Votes = participant.Votes.FirstOrDefault(v => v.ChoiceId == choice.Id,
            new Vote() { User = participant, Choice = choice, Type = SelectedVoteType });

        VoteYes = new RelayCommand(() => SetYes() );
        VoteMaybe = new RelayCommand(() => SetMaybe());
        VoteNo = new RelayCommand(() => SetNo());
    }

    public User Participant => participant;
    public Choice Choice => choice;
    public Vote Votes { get; private set; }
        public PollVoteViewModel() { }
        private bool _editMode;
        public bool EditMode {
            get => _editMode;
            set => SetProperty(ref _editMode, value);
        }

        private VoteType _selectedVoteType;
        public VoteType SelectedVoteType {
            get => _selectedVoteType;
            set {
            if (SetProperty(ref _selectedVoteType, value)) {
                RaisePropertyChanged(nameof(Icon));
                RaisePropertyChanged(nameof(Color));
                RaisePropertyChanged(nameof(YesFgColor));
                RaisePropertyChanged(nameof(MaybeFgColor));
                RaisePropertyChanged(nameof(NoFgColor));
                RaisePropertyChanged(nameof(YesVoteToolTip));
                RaisePropertyChanged(nameof(MaybeVoteToolTip));
                RaisePropertyChanged(nameof(NoVoteToolTip));
            }
        }
    }

        public void SetYes() {
            SelectedVoteType = VoteType.Yes;
            HasVoted = !HasVoted;
            Votes.Type = VoteType.Yes;
        }

        public void SetMaybe() {
            SelectedVoteType = VoteType.Maybe;
            HasVoted = !HasVoted;
            Votes.Type = VoteType.Maybe;
    }

        public void SetNo() {
            SelectedVoteType = VoteType.No;
            HasVoted = !HasVoted;
            Votes.Type = VoteType.No;
    }
        
        public ICommand VoteYes { get; set; }
        public ICommand VoteMaybe { get; set; }
        public ICommand VoteNo { get; set; }
        private bool _hasVoted;
        public bool HasVoted {
            get => _hasVoted;
        set {
            if (SetProperty(ref _hasVoted, value)) {
                RaisePropertyChanged(nameof(Icon));
                RaisePropertyChanged(nameof(Color));
                RaisePropertyChanged(nameof(YesFgColor));
                RaisePropertyChanged(nameof(MaybeFgColor));
                RaisePropertyChanged(nameof(NoFgColor));
                RaisePropertyChanged(nameof(YesVoteToolTip));
                RaisePropertyChanged(nameof(MaybeVoteToolTip));
                RaisePropertyChanged(nameof(NoVoteToolTip));
                 if (!value && participant.Votes.Any(v => v.ChoiceId == Choice.Id)) {
                participant.Votes.Remove(Votes);
            }
            }
        }

        }

    public EFontAwesomeIcon GetIcon() {
        if (HasVoted && SelectedVoteType == VoteType.Yes) {
            return EFontAwesomeIcon.Solid_Check;
        } else if (HasVoted && SelectedVoteType == VoteType.Maybe) {
            return EFontAwesomeIcon.Regular_CircleQuestion;
        } else if (HasVoted && SelectedVoteType == VoteType.No) {
            return EFontAwesomeIcon.Solid_X;
        } else {
            return EFontAwesomeIcon.None;
        }

    }

    public Brush GetColor() {
        if (SelectedVoteType == VoteType.Yes) {
            return Brushes.Green;
        } else if (SelectedVoteType == VoteType.Maybe) {
            return Brushes.Orange;
        } else if (SelectedVoteType == VoteType.No) {
            return Brushes.Red;
        } else {
            return Brushes.White;
        }
    }


    public EFontAwesomeIcon Icon => GetIcon();
    public Brush Color => GetColor();
    public Brush YesFgColor => HasVoted && SelectedVoteType == VoteType.Yes ? Brushes.Green : Brushes.LightGray;
    public Brush MaybeFgColor => HasVoted && SelectedVoteType == VoteType.Maybe? Brushes.Orange : Brushes.LightGray;
    public Brush NoFgColor => HasVoted && SelectedVoteType == VoteType.No? Brushes.Red : Brushes.LightGray;
    public string YesVoteToolTip => "Yes";
    public string MaybeVoteToolTip => "Maybe";
    public string NoVoteToolTip => "No";

    }
