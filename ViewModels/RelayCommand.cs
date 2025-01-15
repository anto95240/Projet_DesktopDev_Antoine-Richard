using System;
using System.Windows.Input;

public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;
    private readonly Func<T, bool> _canExecute;

    public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        // Si le paramètre est null, ou si c'est du type attendu (T), alors on active la commande
        if (parameter == null || parameter is T)
        {
            return _canExecute == null || _canExecute((T)parameter);
        }

        // Si le paramètre n'est pas du type attendu, la commande est désactivée
        return false;
    }


    public void Execute(object parameter)
    {
        if (parameter is T typedParameter)
        {
            _execute(typedParameter);
        }
        else
        {
            throw new InvalidOperationException($"Le paramètre doit être du type {typeof(T).Name}, mais il est de type {parameter.GetType().Name}.");
        }
    }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}
