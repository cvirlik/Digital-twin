using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_twin.Dataset.Types.Secondary
{
    public class Tag : ViewModelBase
    {
        private string _key;
        private string _value;
        public Tag(string k, string v) { 
            Key = k; Value = v;
        }          
        public string Key { 
            get { return _key; } 
            set
            {
                _key = value;
                OnPropertyChanged(nameof(Key));
            }
        }
        public string Value {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
    }
}
