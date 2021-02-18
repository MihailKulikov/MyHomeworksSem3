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

        public static RoutedUICommand GoTo { get; } = new RoutedUICommand
        (
            "Go to",
            "Go to",
            typeof(CustomCommands)
        );
    }
}
