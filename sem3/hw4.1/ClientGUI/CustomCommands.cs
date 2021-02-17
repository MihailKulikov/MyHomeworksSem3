using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ClientGUI
{
    public static class CustomCommands
    {
        public static RoutedUICommand Connect { get; } = new RoutedUICommand
        (
            "Connect!",
            "Connect",
            typeof(CustomCommands)
        );
    }
}
