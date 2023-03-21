using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

public abstract class ViewModelCommon : ViewModelBase<User, MyPollContext> {
    public static bool IsAdmin => App.IsLoggedIn && App.CurrentUser is Administrator;
    public static bool IsNotAdmin => !IsAdmin;
}
