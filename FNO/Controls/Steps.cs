using System;
using System.Collections.Generic;
using Xamarin.Forms;
using PropertyChanged;

namespace FNO.Controls
{
    [AddINotifyPropertyChangedInterface]
    public class Steps
    {
        private int _count = -1;

        public DISPLAY_TYPE NavigationType { get; set; }
        public Command<BUTTON_TYPE> Command { get; set; }

        public bool Flg1 { get; set; }
        public bool Flg2 { get; set; }
        public bool Flg3 { get; set; }

        public IList<Action> _steps = new List<Action>();

        public Steps()
        {
            Command = new Command<Controls.BUTTON_TYPE>((Controls.BUTTON_TYPE type) =>
            {
                if (type == BUTTON_TYPE.OK || type == BUTTON_TYPE.NEXT) Next();
                else if (type == BUTTON_TYPE.BACK) Prev();
            });
        }

        public Steps Add(Action action)
        {
            _steps.Add(action);
            return this;
        }

        public void Next()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _count++;
                _steps[_count].Invoke();
            });
        }

        public void Prev()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _count--;
                _steps[_count].Invoke();
            });
        }

    }
}
