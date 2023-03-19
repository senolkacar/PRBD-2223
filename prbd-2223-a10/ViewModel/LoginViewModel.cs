using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public class LoginViewModel : ViewModelBase<User, MyPollContext> {
    protected override void OnRefreshData() {

    }
}
