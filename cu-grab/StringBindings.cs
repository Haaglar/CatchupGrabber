using System.ComponentModel;

namespace cu_grab
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
