using DevExpress.Xpf.Editors;
using System.Windows;

namespace Wpf.Core.Behaviors;

public static class PhoneMaskBehavior
{
    #region Attached Properties

    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
    "IsEnabled",
    typeof(bool),
    typeof(PhoneMaskBehavior),
    new PropertyMetadata(false, OnIsEnabledChanged));

    #endregion Attached Properties

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
        if (dependencyObject is not TextEdit textEdit)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            textEdit.GotFocus += TextEdit_GotFocus;
        }
        else
        {
            textEdit.GotFocus -= TextEdit_GotFocus;
        }
    }

    private static void TextEdit_GotFocus(object sender , RoutedEventArgs e)
    {
        if (sender is not TextEdit textEdit)
        {
            return;
        }

        textEdit.Dispatcher.BeginInvoke(() =>
        {
            textEdit.SelectionStart = 2;
            textEdit.SelectionLength = 0;
        });
    }

    #endregion Methods
}