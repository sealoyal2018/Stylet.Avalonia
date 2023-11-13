using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;

namespace Stylet.Avalonia.Primitive
{
    /// <summary>
    /// Default implementation of IMessageBoxViewModel, and is therefore the ViewModel shown by default by ShowMessageBox
    /// </summary>
    public class MessageBoxViewModel : Screen, IMessageBoxViewModel
    {
        private static readonly Dictionary<MessageBoxButton, MessageBoxResult[]> ButtonToResults = new Dictionary<MessageBoxButton, MessageBoxResult[]>()
        {
            { MessageBoxButton.OK, new[] { MessageBoxResult.OK } },
            { MessageBoxButton.OKCancel, new[] { MessageBoxResult.OK, MessageBoxResult.Cancel } },
            { MessageBoxButton.YesNo, new[] { MessageBoxResult.Yes, MessageBoxResult.No } },
            { MessageBoxButton.YesNoCancel, new[] { MessageBoxResult.Yes, MessageBoxResult.No, MessageBoxResult.Cancel } },
        };

        private MessageBoxResult defaultButton;
        private MessageBoxResult cancelButton;
        private string text;
        private MessageBoxImage icon;
        private FlowDirection flowDirection;
        private TextAlignment textAlignment;
        
        
        /// <summary>
        /// Gets or sets the list of buttons which are shown in the View.
        /// </summary>
        public IObservableCollection<LabelledValue<MessageBoxResult>> ButtonList { get; protected set; }

        /// <summary>
        /// Gets or sets the item in ButtonList which is the Default button
        /// </summary>
        public MessageBoxResult DefaultButton
        {
            get
            {
                return defaultButton;
            }
            set
            {
                defaultButton = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the item in ButtonList which is the Cancel button
        /// </summary>
        public MessageBoxResult CancelButton
        {
            get
            {
                return cancelButton;
            }
            set
            {
                cancelButton = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the text which is shown in the body of the MessageBox
        /// </summary>
        public virtual string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(TextIsMultiline));
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Text contains many lines
        /// </summary>
        public virtual bool TextIsMultiline
        {
            get { return Text.Contains("\n"); }
        }

        /// <summary>
        /// Gets or sets the icon which the user specified
        /// </summary>
        public virtual MessageBoxImage Icon
        {
            get
            {
                return icon;
            }
            set
            {
                icon = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(ShowIcon));
            }
        }

        public bool ShowIcon => Icon == MessageBoxImage.None;

        /// <summary>
        /// Gets or sets which way the document should flow
        /// </summary>
        public virtual FlowDirection FlowDirection
        {
            get
            {
                return flowDirection;
            }
            set
            {
                flowDirection = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the text alignment of the message
        /// </summary>
        public virtual TextAlignment TextAlignment 
        {
            get
            {
                return textAlignment;
            }
            set
            {
                textAlignment = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets which button the user clicked, once they've clicked a button
        /// </summary>
        public virtual MessageBoxResult ClickedButton { get; protected set; }

        public void Setup(string text, string caption)
        {
            this.Setup(text,caption, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK,MessageBoxResult.None, FlowDirection.LeftToRight, TextAlignment.Center);
        }

        public void Setup(string text)
        {
            this.Setup(text, string.Empty);
        }
        
        
        /// <summary>
        /// Setup the MessageBoxViewModel with the information it needs
        /// </summary>
        /// <param name="messageBoxText">A <see cref="string"/> that specifies the text to display.</param>
        /// <param name="caption">A <see cref="string"/> that specifies the title bar caption to display.</param>
        /// <param name="buttons">A <see cref="System.Windows.MessageBoxButton"/> value that specifies which button or buttons to display.</param>
        /// <param name="icon">A <see cref="System.Windows.MessageBoxImage"/> value that specifies the icon to display.</param>
        /// <param name="defaultResult">A <see cref="MessageBoxResult"/> value that specifies the default result of the message box.</param>
        /// <param name="cancelResult">A <see cref="MessageBoxResult"/> value that specifies the cancel result of the message box</param>
        /// <param name="flowDirection">The <see cref="Avalonia.Media.FlowDirection"/> to use, overrides the <see cref="DefaultFlowDirection"/></param>
        /// <param name="textAlignment">The <see cref="Avalonia.Media.TextAlignment"/> to use, overrides the <see cref="DefaultTextAlignment"/></param>
        public void Setup(
            string messageBoxText,
            string? caption ,
            MessageBoxButton buttons,
            MessageBoxImage icon,
            MessageBoxResult defaultResult,
            MessageBoxResult cancelResult,
            FlowDirection? flowDirection = null,
            TextAlignment? textAlignment = null)
        {
            Text = messageBoxText;
            DisplayName = caption??"提示";
            Icon = icon;

            var buttonList = new BindableCollection<LabelledValue<MessageBoxResult>>();
            ButtonList = buttonList;
            foreach (var val in ButtonToResults[buttons])
            {
                var lbv = new LabelledValue<MessageBoxResult>(val.Text, val);
                buttonList.Add(lbv);
                if (val == defaultResult)
                    DefaultButton = val;
                else if (val == cancelResult)
                    CancelButton = val;
            }
            // If they didn't specify a button which we showed, then pick a default, if we can
            if (defaultResult == MessageBoxResult.None && ButtonList.Any())
                DefaultButton = buttonList[0].Value;
            
            if (cancelResult == MessageBoxResult.None && ButtonList.Any())
                CancelButton = buttonList.Last().Value;
            
            FlowDirection = flowDirection ?? FlowDirection.LeftToRight;
            TextAlignment = textAlignment ?? TextAlignment.Left;
            this.Refresh();
        }

        /// <summary>
        /// Called when MessageBoxView when the user clicks a button
        /// </summary>
        /// <param name="button">Button which was clicked</param>
        public void ButtonClicked(MessageBoxResult button)
        {
            ClickedButton = button;
            RequestClose(true);
        }
    }

    public record MessageBoxImage
    {
        public string Url { get; init; }
        private MessageBoxImage(string url)
        {
            Url = url;
        }
        
        public static readonly MessageBoxImage None = new MessageBoxImage(string.Empty);
        public static readonly MessageBoxImage Error = new MessageBoxImage("avares://Stylet.Avalonia/assets/error.png");
        public static readonly MessageBoxImage Question = new MessageBoxImage("avares://Stylet.Avalonia/assets/question.png");
        public static readonly MessageBoxImage Warning = new MessageBoxImage("avares://Stylet.Avalonia/assets/warning.png");
        public static readonly MessageBoxImage Information = new MessageBoxImage("avares://Stylet.Avalonia/assets/information.png");
    }

    public record MessageBoxResult
    {
        public string Text { get; init; }
        private MessageBoxResult(string text)
        {
            Text = text;
        }

        public static readonly MessageBoxResult None = new MessageBoxResult("None");
        public static readonly MessageBoxResult OK = new MessageBoxResult("OK");
        public static readonly MessageBoxResult Cancel = new MessageBoxResult("Cancel");
        public static readonly MessageBoxResult Yes = new MessageBoxResult("Yes");
        public static readonly MessageBoxResult No = new MessageBoxResult("No");
    }

    public record MessageBoxButton
    {
        public string Text { get; init; }

        private MessageBoxButton(string text) => Text = text;

        public static readonly MessageBoxButton OK = new MessageBoxButton("OK");
        public static readonly MessageBoxButton OKCancel = new MessageBoxButton("OKCancel");
        public static readonly MessageBoxButton YesNo = new MessageBoxButton("YesNo");
        public static readonly MessageBoxButton YesNoCancel = new MessageBoxButton("YesNoCancel");
    }
}
