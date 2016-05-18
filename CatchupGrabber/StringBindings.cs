using System.ComponentModel;

namespace CatchupGrabber
{
    /// <summary>
    /// Holds all of the string bindings the main window uses
    /// </summary>
    class StringBindings : INotifyPropertyChanged 
    {
        //---Selected show Binding ---------------//
        private string _selectedShow ="";
        public string SelectedShow
        {
            get
            {
                return _selectedShow;
            }
            set
            {
                _selectedShow = value;
                OnPropertyChanged("SelectedShow");
            }
        }
        //---End Selected show Binding ---------------//


        //--- Selected Site Binding ---------------//
        private string _selectedSite = "";
        public string SelectedSite
        {
            get
            {
                return _selectedSite;
            }
            set
            {
                _selectedSite = value;
                OnPropertyChanged("SelectedSite");
            }
        }

        
        private string _selectedDescription = "";
        public string SelectedDescription
        {
            get
            {
                return _selectedDescription;
            }
            set
            {
                _selectedDescription = value;
                OnPropertyChanged("SelectedDescription");
            }
        }

        private string _error = "";
        public string Error
        {
            get
            {
                return _error;
            }
            set
            {
                _error = value;
                OnPropertyChanged("Error");
            }
        }

        //--- End Selected Site Binding ---------------//

        //Handles Updating on value changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
