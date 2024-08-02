using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Stylet.Avalonia.Extensions;

namespace Stylet.Avalonia.Primitive;

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

    #region Property

    /// <summary>
    /// Gets or sets the list of buttons which are shown in the View.
    /// </summary>
    public BindableCollection<LabelledValue<MessageBoxResult>> ButtonList { get; protected set; } =
        new BindableCollection<LabelledValue<MessageBoxResult>>();

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
            NotifyOfPropertyChange(nameof(IconUrl));
        }
    }

    public bool ShowIcon => Icon != MessageBoxImage.None;

    public Bitmap? IconUrl {
        get
        {
            if (Icon == MessageBoxImage.None)
                return null;
            return new Bitmap(AssetLoader.Open(new Uri(Icon.GetDescription())));
        }
    }

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
    #endregion
    
    /// <summary>
    /// Setup the MessageBoxViewModel with the information it needs
    /// </summary>
    /// <param name="text">A <see cref="string"/> that specifies the text to display.</param>
    /// <param name="caption">A <see cref="string"/> that specifies the title bar caption to display.</param>
    /// <param name="buttons">A <see cref="MessageBoxButton"/> value that specifies which button or buttons to display.</param>
    /// <param name="icon">A <see cref="MessageBoxImage"/> value that specifies the icon to display.</param>
    /// <param name="defaultResult">A <see cref="MessageBoxResult"/> value that specifies the default result of the message box.</param>
    /// <param name="cancelResult">A <see cref="MessageBoxResult"/> value that specifies the default result of the message box.</param>
    /// <param name="flowDirection">The <see cref="Avalonia.Media.FlowDirection"/> to use, overrides the <see cref="DefaultFlowDirection"/></param>
    /// <param name="textAlignment">The <see cref="Avalonia.Media.TextAlignment"/> to use, overrides the <see cref="DefaultTextAlignment"/></param>
    public void Setup(
        string text,
        string? caption ,
        MessageBoxButton buttons = MessageBoxButton.OK,
        MessageBoxImage icon = MessageBoxImage.None,
        MessageBoxResult defaultResult = MessageBoxResult.OK,
        MessageBoxResult cancelResult = MessageBoxResult.None,
        FlowDirection flowDirection = FlowDirection.LeftToRight,
        TextAlignment textAlignment = TextAlignment.Left)
    {
        Text = text;
        DisplayName = caption??"提示";
        Icon = icon;

        var buttonList = new BindableCollection<LabelledValue<MessageBoxResult>>();
        foreach (var val in ButtonToResults[buttons])
        {
            var lbv = new LabelledValue<MessageBoxResult>(val.GetDescription(), val);
            buttonList.Add(lbv);
            if (val == defaultResult)
                DefaultButton = val;
        }
        ButtonList = buttonList;
        // If they didn't specify a button which we showed, then pick a default, if we can
        if (defaultResult == MessageBoxResult.None && ButtonList.Any())
            DefaultButton = buttonList[0].Value;
        
        if (cancelResult == MessageBoxResult.None && ButtonList.Any())
            CancelButton = buttonList.Last().Value;
        
        FlowDirection = flowDirection;
        TextAlignment = textAlignment;
    }

    /// <summary>
    /// Called when MessageBoxView when the user clicks a button
    /// </summary>
    /// <param name="button">Button which was clicked</param>
    public void ButtonClicked(MessageBoxResult button)
    {
        ClickedButton = button;
        RequestClose(button is MessageBoxResult.OK or MessageBoxResult.Yes);
    }
}

public enum MessageBoxImage
{
    [Description("")]
    None = 0,
    [Description("avares://Stylet.Avalonia/Assets/error.png")]
    Error = 1,
    [Description("avares://Stylet.Avalonia/Assets/question.png")]
    Question = 2,
    [Description("avares://Stylet.Avalonia/Assets/warning.png")]
    Warning = 3,
    [Description("avares://Stylet.Avalonia/Assets/information.png")]
    Information = 4,
}


public enum MessageBoxResult
{
    [Description("None")]
    None = 0,
    
    [Description("OK")]
    OK = 1,
    
    [Description("Cancel")]
    Cancel = 2,
    
    [Description("Yes")]
    Yes = 3,
    
    [Description("No")]
    No = 4,
}

public enum MessageBoxButton
{
    [Description("OK")]
    OK,
    [Description("OKCancel")]
    OKCancel,
    [Description("YesNo")]
    YesNo,
    [Description("YesNoCancel")]
    YesNoCancel,
}
