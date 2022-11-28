using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PracticalControls.Common.Helpers
{
    public static class CommandHelper
    {
        // Lots of specialized registration methods to avoid new'ing up more common stuff (like InputGesture's) at the callsite, as that's frequently
        // repeated and increases code size.  Do it once, here.  

        public static void RegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, null);
        }

        public static void RegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler,
                                                    InputGesture inputGesture)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, inputGesture);
        }

        public static void RegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler,
                                                    Key key)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, new KeyGesture(key));
        }

        public static void RegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler,
                                                    InputGesture inputGesture, InputGesture inputGesture2)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, inputGesture, inputGesture2);
        }

        public static void RegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler,
                                                    CanExecuteRoutedEventHandler canExecuteRoutedEventHandler)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, null);
        }

        public static void RegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler,
                                                    CanExecuteRoutedEventHandler canExecuteRoutedEventHandler, InputGesture inputGesture)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, inputGesture);
        }

        public static void RegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler,
                                                    CanExecuteRoutedEventHandler canExecuteRoutedEventHandler, Key key)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, new KeyGesture(key));
        }

        public static void RegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler,
                                                    CanExecuteRoutedEventHandler canExecuteRoutedEventHandler, InputGesture inputGesture, InputGesture inputGesture2)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, inputGesture, inputGesture2);
        }

        public static void RegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler,
                                                    CanExecuteRoutedEventHandler canExecuteRoutedEventHandler,
                                                    InputGesture inputGesture, InputGesture inputGesture2, InputGesture inputGesture3, InputGesture inputGesture4)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler,
                                          inputGesture, inputGesture2, inputGesture3, inputGesture4);
        }

        public static void RegisterCommandHandler(Type controlType, RoutedCommand command, Key key, ModifierKeys modifierKeys,
                                                    ExecutedRoutedEventHandler executedRoutedEventHandler, CanExecuteRoutedEventHandler canExecuteRoutedEventHandler)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, new KeyGesture(key, modifierKeys));
        }


        // 'params' based method is private.  Call sites that use this bloat unwittingly due to implicit construction of the params array that goes into IL.
        private static void PrivateRegisterCommandHandler(Type controlType, RoutedCommand command, ExecutedRoutedEventHandler executedRoutedEventHandler,
                                                          CanExecuteRoutedEventHandler canExecuteRoutedEventHandler, params InputGesture[] inputGestures)
        {
            // Validate parameters
            Debug.Assert(controlType != null);
            Debug.Assert(command != null);
            Debug.Assert(executedRoutedEventHandler != null);
            // All other parameters may be null

            // Create command link for this command
            CommandManager.RegisterClassCommandBinding(controlType, new CommandBinding(command, executedRoutedEventHandler, canExecuteRoutedEventHandler));

            // Create additional input binding for this command
            if (inputGestures != null)
            {
                for (int i = 0; i < inputGestures.Length; i++)
                {
                    CommandManager.RegisterClassInputBinding(controlType, new InputBinding(command, inputGestures[i]));
                }
            }
        }

        internal static bool CanExecuteCommandSource(ICommandSource commandSource)
        {
            var command = commandSource.Command;
            if (command == null)
            {
                return false;
            }

            var commandParameter = commandSource.CommandParameter ?? commandSource;
            if (command is RoutedCommand routedCommand)
            {
                var target = commandSource.CommandTarget ?? commandSource as IInputElement;
                return routedCommand.CanExecute(commandParameter, target);
            }

            return command.CanExecute(commandParameter);
        }

        [SecurityCritical]
        [SecuritySafeCritical]
        internal static void ExecuteCommandSource(ICommandSource commandSource)
        {
            CriticalExecuteCommandSource(commandSource);
        }

        [SecurityCritical]
        internal static void CriticalExecuteCommandSource(ICommandSource commandSource)
        {
            var command = commandSource.Command;
            if (command == null)
            {
                return;
            }

            var commandParameter = commandSource.CommandParameter ?? commandSource;
            if (command is RoutedCommand routedCommand)
            {
                var target = commandSource.CommandTarget ?? commandSource as IInputElement;
                if (routedCommand.CanExecute(commandParameter, target))
                {
                    routedCommand.Execute(commandParameter, target);
                }
            }
            else
            {
                if (command.CanExecute(commandParameter))
                {
                    command.Execute(commandParameter);
                }
            }
        }

        internal static bool CanExecuteCommandSource(ICommandSource commandSource, ICommand theCommand)
        {
            var command = theCommand;
            if (command == null)
            {
                return false;
            }

            var commandParameter = commandSource.CommandParameter ?? commandSource;
            if (command is RoutedCommand routedCommand)
            {
                var target = commandSource.CommandTarget ?? commandSource as IInputElement;
                return routedCommand.CanExecute(commandParameter, target);
            }

            return command.CanExecute(commandParameter);
        }

        [SecurityCritical]
        [SecuritySafeCritical]
        internal static void ExecuteCommandSource(ICommandSource commandSource, ICommand? theCommand)
        {
            CriticalExecuteCommandSource(commandSource, theCommand);
        }

        [SecurityCritical]
        internal static void CriticalExecuteCommandSource(ICommandSource commandSource, ICommand? theCommand)
        {
            var command = theCommand;
            if (command == null)
            {
                return;
            }

            var commandParameter = commandSource.CommandParameter ?? commandSource;
            if (command is RoutedCommand routedCommand)
            {
                var target = commandSource.CommandTarget ?? commandSource as IInputElement;
                if (routedCommand.CanExecute(commandParameter, target))
                {
                    routedCommand.Execute(commandParameter, target);
                }
            }
            else
            {
                if (command.CanExecute(commandParameter))
                {
                    command.Execute(commandParameter);
                }
            }
        }
    }
}
