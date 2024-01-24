namespace Command_;

public class DelegateCommand : CommandBase
{
    private readonly Func<object, bool> _canExecuteMethod;
    private readonly Action<object> _executeMethod;

    public DelegateCommand(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
    {
        _canExecuteMethod = canExecuteMethod;
        _executeMethod = executeMethod;
    }

    protected override bool CanExecuteCmd(object parameter)
    {
        return _canExecuteMethod(parameter);
    }

    protected override void ExecuteCmd(object parameter)
    {
        _executeMethod(parameter);
    }
}
