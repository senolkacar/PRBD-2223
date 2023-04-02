using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PRBD_Framework;
using MyPoll.Model;
using Microsoft.EntityFrameworkCore.Metadata;
using MyPoll.ViewModel;

namespace MyPoll.View {
    public partial class PollChoicesView : UserControlBase {
        public PollChoicesView(Poll poll) {
            InitializeComponent();
            DataContext = new PollChoicesViewModel(poll);
        }

    }
}
