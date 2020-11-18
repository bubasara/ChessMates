using Caliburn.Micro;
using ChessMates.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessMates.ViewModels
{
    /*  Backend code for UI & shell for Model   */
    /*  Presentation logic  */
    class ShellViewModel : Conductor<object>
    {
        private string _firstName;
        private string _lastName;
        private int _fideRank;
        private int _birthYear;
        private string _country;

        public ShellViewModel()
        {
        }

        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
                NotifyOfPropertyChange(() => FirstName);
            }
        }
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
                NotifyOfPropertyChange(() => LastName);
            }
        }
        public int FideRank
        {
            get
            {
                return _fideRank;
            }
            set
            {
                _fideRank = value;
                NotifyOfPropertyChange(() => FideRank);
            }
        }
        public int BirthYear
        {
            get
            {
                return _birthYear;
            }
            set
            {
                _birthYear = value;
                NotifyOfPropertyChange(() => BirthYear);
            }
        }
        public string Country
        {
            get
            {
                return _country;
            }
            set
            {
                _country = value;
                NotifyOfPropertyChange(() => Country);
            }
        }
    }
}
