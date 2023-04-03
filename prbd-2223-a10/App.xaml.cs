using System.Windows;
using MyPoll.Model;
using MyPoll.ViewModel;
using PRBD_Framework;

namespace MyPoll; 

public partial class App : ApplicationBase<User,MyPollContext> {

    public enum Polls {
        POLL_LOGIN,
        POLL_DISPLAY,
        POLL_ADD,
        POLL_EDIT,
        POLL_CHANGED,
        POLL_NAME_CHANGED,
        POLL_LOGOUT,
        POLL_REFRESH,
        POLL_CLOSE_TAB
    }

    protected override void OnStartup(StartupEventArgs e) {
        PrepareDatabase();
        Register<User>(this, Polls.POLL_LOGIN, user => {
            Login(user);
            NavigateTo<MainViewModel, User, MyPollContext>();
        });

        Register(this, Polls.POLL_LOGOUT, () => {
            Logout();
            NavigateTo<LoginViewModel, User, MyPollContext>();
        });

    }

    private static void PrepareDatabase() {
        // Clear database and seed data
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();

       

        // Cold start
        Console.Write("Cold starting database... ");
        Context.Users.Find(0);
        Console.WriteLine("done");
    }

    protected override void OnRefreshData() {
        if (CurrentUser?.Id != null)
            CurrentUser = User.GetById(CurrentUser.Id);
    }
}
