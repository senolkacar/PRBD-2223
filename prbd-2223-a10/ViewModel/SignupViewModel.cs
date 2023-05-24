using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel {
    public class SignupViewModel : ViewModelCommon {


        public ICommand SignUpCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        private string _email;

        public string Email {
            get => _email;
            set => SetProperty(ref _email, value, () => Validate());
        }

        private string _password;

        public string Password {
            get => _password;
            set => SetProperty(ref _password, value, () => Validate());
        }

        private string _confirmpassword;
        public string ConfirmPassword {
            get => _confirmpassword;
            set => SetProperty(ref _confirmpassword, value, () => Validate());
        }

        private string _fullname;
        public string FullName {
            get => _fullname;
            set => SetProperty(ref _fullname, value, () => Validate());
        }

        public SignupViewModel() {
            SignUpCommand = new RelayCommand(SignupAction,
                () => { return _email != null && _password != null && FullName != null && !HasErrors; });
            CancelCommand = new RelayCommand(() =>NotifyColleagues(App.Polls.POLL_LOGOUT));
        }

        private void SignupAction() {
            if (Validate()) {
                var user = new User { Email = Email, Password = SecretHasher.Hash(Password), FullName = FullName };
                Context.Users.Add(user);
                Context.SaveChanges();
                NotifyColleagues(App.Polls.POLL_LOGIN, user);
            }
        }
        public override bool Validate() {
            ClearErrors();
            string pattern = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,6}$";
            var user = Context.Users.SingleOrDefault(u => u.Email == Email);
            if (!Regex.IsMatch(Email, pattern)) {
                AddError(nameof(Email), "bad email format");
            } else if (user != null) {
                AddError(nameof(Email),"email already exists");
            }
            if (string.IsNullOrEmpty(Password)) {
                AddError(nameof(Password), "required");
            } else if (Password.Length < 3) {
                AddError(nameof(Password), "length must be >=3");
            }
            if (string.IsNullOrEmpty(ConfirmPassword)) {
                AddError(nameof(ConfirmPassword), "required");
            }else if (ConfirmPassword != Password) {
                AddError(nameof(Password), "must match password");
            }
            if (string.IsNullOrEmpty(FullName)) {
                AddError(nameof(FullName), "required");
            } else if (FullName.Length < 3) {
                AddError(nameof(FullName), "length must be >=3");
            }
            return !HasErrors;
        }
    }
}
