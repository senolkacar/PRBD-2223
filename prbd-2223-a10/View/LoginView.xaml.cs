using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PRBD_Framework;

namespace MyPoll.View;

public partial class LoginView : WindowBase {
    public LoginView() {
        InitializeComponent();
    }

    private void btnCancel_Click(object sender,RoutedEventArgs e) {
        Close();
    }
}
