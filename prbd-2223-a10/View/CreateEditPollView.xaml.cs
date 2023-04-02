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
using MyPoll.ViewModel;

namespace MyPoll.View;

    public partial class CreateEditPollView : UserControlBase
    {
        public CreateEditPollView(Poll poll,bool isNew)
        {
            InitializeComponent();
            DataContext = new CreateEditPollViewModel(poll,isNew);
        }
    }

