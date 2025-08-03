using System;
using System.Collections.Generic;

// SRP: Single Responsibility - Command execution and history management
// OCP: Open for extension (new commands can be added)
// DIP: Depends on IPatientCommand abstraction
public class CommandManager
{
    private readonly Stack<IPatientCommand> _undoStack = new Stack<IPatientCommand>();
    private readonly Stack<IPatientCommand> _redoStack = new Stack<IPatientCommand>();
    private const int MAX_HISTORY = 10;
    private readonly INotificationService _notificationService;

    public CommandManager(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public void ExecuteCommand(IPatientCommand command)
    {
        command.Execute();
        _undoStack.Push(command);

        if (_undoStack.Count > MAX_HISTORY)
        {
            var tempList = new List<IPatientCommand>();
            for (int i = 0; i < MAX_HISTORY; i++)
            {
                if (_undoStack.Count > 0)
                    tempList.Add(_undoStack.Pop());
            }
            _undoStack.Clear();
            for (int i = tempList.Count - 1; i >= 0; i--)
            {
                _undoStack.Push(tempList[i]);
            }
        }

        _redoStack.Clear();
    }

    public void UndoLastAction()
    {
        if (_undoStack.Count > 0)
        {
            var command = _undoStack.Pop();
            command.Undo();
            _redoStack.Push(command);
            _notificationService.ShowSuccess("Undo completed.");
        }
        else
        {
            _notificationService.ShowError("No actions to undo.");
        }
    }

    public void RedoLastAction()
    {
        if (_redoStack.Count > 0)
        {
            var command = _redoStack.Pop();
            command.Execute();
            _undoStack.Push(command);
            _notificationService.ShowSuccess("Redo completed.");
        }
        else
        {
            _notificationService.ShowError("No actions to redo.");
        }
    }
}