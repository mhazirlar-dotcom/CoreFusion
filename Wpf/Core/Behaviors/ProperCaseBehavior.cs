using DevExpress.Xpf.Editors;
using System.Globalization;
using System.Windows;

namespace Wpf.Core.Behaviors;

public static class ProperCaseBehavior
{
    #region Attached Properties

    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
    "IsEnabled",
    typeof(bool),
    typeof(ProperCaseBehavior),
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
            textEdit.EditValueChanged += TextEdit_EditValueChanged;
        }
        else
        {
            textEdit.EditValueChanged -= TextEdit_EditValueChanged;
        }
    }

    private static void TextEdit_EditValueChanged(object sender , EditValueChangedEventArgs e)
    {
        if (sender is not TextEdit textEdit)
        {
            return;
        }

        if (textEdit.Tag is true)
        {
            return;
        }

        if (textEdit.EditValue is not string text || string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        string properText = ToProperCase(text);

        if (string.Equals(text , properText , StringComparison.Ordinal))
        {
            return;
        }

        int caretPosition = textEdit.SelectionStart;

        textEdit.Tag = true;
        textEdit.EditValue = properText;
        textEdit.SelectionStart = Math.Min(caretPosition , properText.Length);
        textEdit.Tag = false;
    }

    private static string ToProperCase(string text)
    {
        CultureInfo culture = CultureInfo.GetCultureInfo("tr-TR");

        return culture.TextInfo.ToTitleCase(text.ToLower(culture));
    }

    #endregion Methods
}