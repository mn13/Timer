using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;

namespace MyTimer
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string time;
        public string Time
        {
            get { return time; }
            set {
                time = value.Trim();
                OnPropertyChanged("Time");
            }
        }
        private bool topmost;
        public bool Topmost
        {
            get { return topmost; }
            set
            {
                topmost = value;
                OnPropertyChanged("Topmost");
            }
        }
        private bool isEnable;
        public bool IsEnable
        {
            get { return isEnable; }
            set
            {
                isEnable = value;
                OnPropertyChanged("IsEnable");
            }
        } //доступен текстбокс
        private bool stop;
        public bool Stop
        {
            get { return stop; }
            set
            {
                stop = value;
                OnPropertyChanged("Stop");
            }
        }
        private System.Windows.WindowState windowState;
        public System.Windows.WindowState WindowState
        {
            get { return windowState; }
            set
            {
                windowState = value;
                OnPropertyChanged("WindowState");
            }
        }
        MyTimerClass timerObj;
        //System.Diagnostics.Stopwatch anotherTimer;
        public MainWindowViewModel()
        {
            timerObj = new MyTimerClass(
                (param)=>
                {
                    Time = param.ToString();
                },
                ()=>
                {
                    WindowState = System.Windows.WindowState.Normal; //таймер завершил работу
                    Topmost = true;
                    IsEnable = true;
                    Stop = true;
                    //цвет?
                });
            IsEnable = true;
            Stop = false;
        }
        protected void OnPropertyChanged(string PropertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(PropertyName));
            }
        }
        StartCommand _startCommand;
        public StartCommand startCommand
        {
            get
            {
                if (_startCommand == null)
                    {
                    _startCommand = new StartCommand((param) =>
                    {
                        string minutesPattern = @"^(\d?\d{1}){1}", secondsPattern = @"([0-5]{1}\d{1}){1}$", minutes, seconds;
                        minutes = Regex.Match(Time, minutesPattern).Value;
                        seconds = Regex.Match(Time, secondsPattern).Value;
                        timerObj.Start(minutes, seconds);
                        IsEnable = false;
                        Stop = false;
                    }, (param) =>
                    {
                        string pattern = @"^(\d?\d{1}){1}:([0-5]{1}\d{1}){1}$";
                        Regex regex = new Regex(pattern);
                        Match match = regex.Match(Time);
                        return match.Success;
                    });
                }
                return _startCommand;
            }
        }
        PauseCommand _pauseCommand;
        public PauseCommand pauseCommand
        {
            get
            {
                if (_pauseCommand == null)
                {
                    _pauseCommand = new PauseCommand((param) =>
                    {
                        timerObj.Pause();
                        IsEnable = !IsEnable;
                    });
                }
                return _pauseCommand;
            }
        }
        public void MyPrint(string timeValue)
        {
            Time = timeValue;
        }
    }
    public class StartCommand : ICommand
    {
        protected Action<Exception> _error;//при объявлении можно задать обработчик ошибок. Гениально!
        protected Action<Object> _execute;
        protected Predicate<Object> _canExecute;
        public StartCommand(Action<Object> execute, Predicate<Object> canExecute, Action<Exception> error=null)
        {
            _execute = execute;
            _canExecute = canExecute;
            _error = error;
        }
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute (object parameter)
        {
            try
            {
                _execute(parameter);//запуск таймера
            }
            catch(Exception ex)
            {
                if (_error != null)
                    _error(ex);
            }
        }
    }
    public class PauseCommand : ICommand
    {
        protected Action<Exception> _error;//при объявлении можно задать обработчик ошибок. Гениально!
        protected Action<Object> _execute;
        public PauseCommand(Action<Object> execute, Action stop = null, Action<Exception> error = null)
        {
            _execute = execute;
            _error = error;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute(object parameter)
        {
            try
            {
                _execute(parameter);
            }
            catch (Exception ex)
            {
                if (_error != null)
                    _error(ex);
            }
        }
    }
}
