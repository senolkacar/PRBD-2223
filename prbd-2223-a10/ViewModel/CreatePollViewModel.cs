using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore.Storage;
using MyPoll.Model;
using PRBD_Framework;

namespace MyPoll.ViewModel;

    public class CreatePollViewModel : ViewModelCommon
    {
        public string Creator => CurrentUser.FullName;

    public CreatePollViewModel() { }

    }

