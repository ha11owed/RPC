using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GaLib.Interop.Proxy
{
    public interface IBase
    {
        string BaseProp { get; set; }
        IBase GetBase();
        event EventHandler BaseEvent;
    }

    public interface ITest : IBase, INotifyPropertyChanged
    {
        int Test();
    }

    public class TestImpl : ITest
    {
        public int Test()
        {
            return 9999;
        }

        public string BaseProp { get; set; }

        public IBase GetBase()
        {
            return this;
        }

        public event EventHandler BaseEvent;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged()
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs("Impl"));
        }
    }
}
