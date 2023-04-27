using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel {
    public class PollChoiceListViewModel : ViewModelCommon {
        public PollChoiceListViewModel(Choice choice) {
            Choice = choice;
            OldLabel = choice.Label;
            EditMode = false;
            Editable = true;
            EditChoiceCMD = new RelayCommand(EditChoice);
            CancelChoiceCMD = new RelayCommand(CancelChoice);
        }

        private Choice _choice;
        public Choice Choice {
            get => _choice;
            set => SetProperty(ref _choice, value);
        }

        private bool _editable;
        public bool Editable {
            get => _editable;
            set => SetProperty(ref _editable, value);
        }

        private bool _editMode;
        public bool EditMode {
            get => _editMode;
            set => SetProperty(ref _editMode, value);
        }

        private string _oldLabel;
        public string OldLabel {
            get => _oldLabel;
            set => SetProperty(ref _oldLabel, value);
        }

        public ICommand EditChoiceCMD { get; set; }
        public ICommand CancelChoiceCMD { get; set; }

        private void EditChoice() {
            EditMode = true;
            Editable = false;
        }

        private void CancelChoice() {
            Choice.Label = OldLabel;
            RaisePropertyChanged(nameof(Choice));
            EditMode = false;
            Editable = true;

        }

        public int ChoiceVote => Choice.Votes.Count;


        public PollChoiceListViewModel() { }

    }
}
