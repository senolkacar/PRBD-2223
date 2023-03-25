using PRBD_Framework;
using MyPoll.Model;
using System.Windows.Controls;

namespace MyPoll.View;

public partial class MainView : WindowBase {
    public MainView() {
        InitializeComponent();
        Register<Poll>(App.Polls.POLL_DISPLAY,
            poll => DisplayPoll(poll));
        
    }

    private void DisplayPoll(Poll poll) {
        if (poll != null) {
            OpenTab(poll.Name,poll.Name,()=>new PollChoicesView(poll));
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

    private void RenameTab(string header) {
        if(tabControl.SelectedItem is TabItem tab) {
            MyTabControl.RenameTab(tab, header);
            tab.Tag = header;
        }
    }

 
}
