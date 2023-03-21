using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class LoginViewModel : ViewModelCommon {

    public ICommand LoginCommand { get; set; }

    private string _email;

    public string Email {
        get => _email;
        set => SetProperty(ref _email, value, () => Validate());
    }

    private string _password;

    public string Password {
        get => _password;
        set => SetProperty(ref _password,value, () => Validate());
    }

    public LoginViewModel() {
        LoginCommand = new RelayCommand(LoginAction,
           () => { return _email != null && _password != null && !HasErrors; });
    }

    private void LoginAction() {
        if (Validate()) {
            var user = Context.Users.FirstOrDefault(u => u.Email == Email);
            NotifyColleagues(App.Polls.POLL_LOGIN, user);
        }
    }
    public override bool Validate() {
        ClearErrors();
        var user = Context.Users.FirstOrDefault(u => u.Email == Email);
        string pattern = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,6}$";
        if (!Regex.IsMatch(Email, pattern)) {
            AddError(nameof(Email), "bad email format");
        }else if (user == null) {
            AddError(nameof(Email), "mail doesnt exists");
        } else {
            if (string.IsNullOrEmpty(Password)) {
                AddError(nameof(Password), "required");
            } else if (Password.Length < 3) {
                AddError(nameof(Password), "length must be >=3");
            } else if(user != null && !SecretHasher.Verify(Password,user.Password)) {
                AddError(nameof(Password), "wrong password");
            }
        }

        return !HasErrors;
       
    }

    protected override void OnRefreshData() {

    }
}
