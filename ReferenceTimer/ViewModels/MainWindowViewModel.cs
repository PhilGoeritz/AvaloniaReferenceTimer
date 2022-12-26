using System;
using System.Collections.Generic;
using System.Text;

namespace ReferenceTimer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ViewModelBase Content => new FileListViewModel();
    }
}
