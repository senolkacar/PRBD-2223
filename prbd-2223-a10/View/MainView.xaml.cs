using PRBD_Framework;
using MyPoll.Model;
using System.Windows.Controls;
using static MyPoll.App;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;

namespace MyPoll.View;

public partial class MainView : WindowBase {
    public MainView() {
        InitializeComponent();
        Register<Poll>(App.Polls.POLL_DISPLAY,
            poll => DisplayPoll(poll));
        Register<Poll>(App.Polls.POLL_ADD,poll=>CreateEditPoll(poll,true));
        Register<Poll>(App.Polls.POLL_EDIT,poll => CreateEditPoll(poll,false));
        Register<Poll>(App.Polls.POLL_CLOSE_TAB,
            poll => DoCloseTab(poll));
        Register<Poll>(App.Polls.POLL_NAME_CHANGED,
            poll => DoRenameTab(string.IsNullOrEmpty(poll.Name) ? "<New Member>" : poll.Name));

    }

    private void DisplayPoll(Poll poll) {
        if (poll != null) {
            OpenTab(poll.Name,poll.Name,()=>new PollChoicesView(poll));
        }
    }
    private void DoRenameTab(string header) {
        if (tabControl.SelectedItem is TabItem tab) {
            MyTabControl.RenameTab(tab, header);
            tab.Tag = header;
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

    private void DoCloseTab(Poll poll) {
       var tab = tabControl.FindByTag(poll.Name);
        if (tab == null) {
            tabControl.CloseByTag(string.IsNullOrEmpty(poll.Name) ? "<New Poll>" : poll.Name);
        } else {
            var pollChoicesView = new PollChoicesView(poll);
            tab.Content = pollChoicesView;
            tabControl.SetFocus(tab);
        }

    }

    private void MenuLogout_Click(object sender, System.Windows.RoutedEventArgs e) {
        NotifyColleagues(App.Polls.POLL_LOGOUT);
    }

    private void WindowBase_KeyDown(object sender, KeyEventArgs e) {
        if (e.Key == Key.Q && Keyboard.IsKeyDown(Key.LeftCtrl))
            Close();
    }

    // Nécessaire pour pouvoir Dispose tous les UC et leur VM
    protected override void OnClosing(CancelEventArgs e) {
        base.OnClosing(e);
        tabControl.Dispose();
    }


}
