using Avalonia.Media;

namespace Stylet.Avalonia.Primitive
{
    /// <summary>
    /// Interface for a MessageBoxViewModel. MessageBoxWindowManagerExtensions.ShowMessageBox will use the configured implementation of this
    /// </summary>
    public interface IMessageBoxViewModel
    {
        /// <summary>
        /// Setup the MessageBoxViewModel with the information it needs
        /// </summary>
        /// <param name="messageBoxText">A <see cref="string"/> that specifies the text to display.</param>
        /// <param name="caption">A <see cref="string"/> that specifies the title bar caption to display.</param>
        /// <param name="buttons">A <see cref="MessageBoxButton"/> value that specifies which button or buttons to display.</param>
        /// <param name="icon">A <see cref="MessageBoxImage"/> value that specifies the icon to display.</param>
        /// <param name="defaultResult">A <see cref="MessageBoxResult"/> value that specifies the default result of the message box.</param>
        /// <param name="cancelResult">A <see cref="MessageBoxResult"/> value that specifies the cancel result of the message box</param>
        /// <param name="flowDirection">The <see cref="FlowDirection"/> to use, overrides the <see cref="DefaultFlowDirection"/></param>
        /// <param name="textAlignment">The <see cref="TextAlignment"/> to use, overrides the <see cref="DefaultTextAlignment"/></param>
        void Setup(
            string messageBoxText,
            string caption,
            MessageBoxButton buttons,
            MessageBoxImage icon,
            MessageBoxResult defaultResult,
            MessageBoxResult cancelResult,
            FlowDirection? flowDirection = null,
            TextAlignment? textAlignment = null);
        
        /// <summary>
        /// Gets the button clicked by the user, after they've clicked it
        /// </summary>
        MessageBoxResult ClickedButton { get; }
    }
}
