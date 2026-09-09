using System.Windows;
using Control = System.Windows.Controls.Control;
using System.Windows.Input;
using Brushes = System.Windows.Media.Brushes;

namespace Wpf.Core.Behaviors;

public static class EditorFocusBehavior
{
    #region Dependency Properties

    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
    "IsEnabled",
    typeof(bool),
    typeof(EditorFocusBehavior),
    new PropertyMetadata(false, OnIsEnabledChanged));

    #endregion Dependency Properties

    #region Methods

    public static void SetIsEnabled(DependencyObject element , bool value)
    {
        element.SetValue(IsEnabledProperty , value);
    }

    public static bool GetIsEnabled(DependencyObject element)
    {
        return (bool)element.GetValue(IsEnabledProperty);
    }

    private static void OnIsEnabledChanged(DependencyObject dependencyObject , DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not Control control)
        {
            return;
        }

        if (e.NewValue is true)
        {
            control.GotKeyboardFocus += Control_GotKeyboardFocus;
            control.LostKeyboardFocus += Control_LostKeyboardFocus;
            return;
        }

        control.GotKeyboardFocus -= Control_GotKeyboardFocus;
        control.LostKeyboardFocus -= Control_LostKeyboardFocus;
    }

    private static void Control_GotKeyboardFocus(object sender , KeyboardFocusChangedEventArgs e)
    {
        if (sender is Control control)
        {
            control.Background = Brushes.Yellow;
        }
    }

    private static void Control_LostKeyboardFocus(object sender , KeyboardFocusChangedEventArgs e)
    {
        if (sender is Control control)
        {
            control.Background = Brushes.White;
        }
    }

    #endregion Methods
}