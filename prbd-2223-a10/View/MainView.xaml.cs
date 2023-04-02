using PRBD_Framework;
using MyPoll.Model;
using System.Windows.Controls;
using static MyPoll.App;
using System.Windows;

namespace MyPoll.View;

public partial class MainView : WindowBase {
    public MainView() {
        InitializeComponent();
        Register<Poll>(App.Polls.POLL_DISPLAY,
            poll => DisplayPoll(poll));
        Register<Poll>(App.Polls.POLL_ADD,poll=>CreateEditPoll(poll,true));
        Register<Poll>(App.Polls.POLL_EDIT,poll => CreateEditPoll(poll,false));
    }

    private void DisplayPoll(Poll poll) {
        if (poll != null) {
            OpenTab(poll.Name,poll.Name,()=>new PollChoicesView(poll));
        }
    }

    private void CreateEditPoll(Poll poll, bool isNew) {
        if (poll != null) {
            var tab = tabControl.FindByTag(poll.Name);
            if (tab == null) {
                // Tab does not exist, create a new tab with the CreateEditPollView
                OpenTab(isNew ? "<New Poll>" : poll.Name, poll.Name, () => new CreateEditPollView(poll, isNew));
            } else {
                // Tab already exists, update its content to the CreateEditPollView
                var createEditPollView = new CreateEditPollView(poll, isNew);
                tab.Content = createEditPollView;
                tabControl.SetFocus(tab);
            }
        }
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
