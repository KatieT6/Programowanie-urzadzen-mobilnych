using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using PresentationModel;


namespace PresentationViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        private ModelAbstractApi ModelAPI { get;}
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}