using PRBD_Framework;
using MyPoll.Model;
using System.Windows.Controls;
using static MyPoll.App;

namespace MyPoll.View;

public partial class MainView : WindowBase {
    public MainView() {
        InitializeComponent();
        Register<Poll>(App.Polls.POLL_DISPLAY,
            poll => DisplayPoll(poll));
        Register<User>(App.Polls.POLL_ADD,CurrentUser=>CreatePoll());
        
    }

    private void DisplayPoll(Poll poll) {
        if (poll != null) {
            OpenTab(poll.Name,poll.Name,()=>new PollChoicesView(poll));
        }
    }

    private void CreatePoll() {
        OpenTab("<New Poll>", "<New Poll>",() => new CreatePollView());
    }

    private void OpenTab(string header, string tag, Func<UserControlBase> createView) {
        var tab = tabControl.FindByTag(tag);
        if (tab == null) {
            tabControl.Add(createView(),header,tag);
        } else {
            tabControl.SetFocus(tab);
        }
    }


 
}
