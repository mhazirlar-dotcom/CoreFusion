using System.Windows;
using DevExpress.Xpf.Editors;
using Wpf.Core.Editors;

namespace Wpf.Core.Behaviors;

public static class EditorValidationBehavior
{
    #region Dependency Properties

    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
    "IsEnabled",
    typeof(bool),
    typeof(EditorValidationBehavior),
    new PropertyMetadata(false, OnIsEnabledChanged));

    public static readonly DependencyProperty IsValidProperty = DependencyProperty.RegisterAttached(
    "IsValid",
    typeof(bool),
    typeof(EditorValidationBehavior),
    new PropertyMetadata(true));

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

    public static void SetIsValid(DependencyObject element , bool value)
    {
        element.SetValue(IsValidProperty , value);
    }

    public static bool GetIsValid(DependencyObject element)
    {
        return (bool)element.GetValue(IsValidProperty);
    }

    private static void OnIsEnabledChanged(DependencyObject dependencyObject , DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not TextEdit textEdit)
        {
            return;
        }

        if (e.NewValue is true)
        {
            textEdit.EditValueChanged += TextEdit_EditValueChanged;
            Validate(textEdit);
            return;
        }

        textEdit.EditValueChanged -= TextEdit_EditValueChanged;
        SetIsValid(textEdit , true);
    }

    private static void TextEdit_EditValueChanged(object sender , EditValueChangedEventArgs e)
    {
        if (sender is TextEdit textEdit)
        {
            Validate(textEdit);
        }
    }

    private static void Validate(TextEdit textEdit)
    {
        EditorInputType inputType = EditorInputBehavior.GetInputType(textEdit);

        if (inputType != EditorInputType.Email)
        {
            SetIsValid(textEdit , true);
            return;
        }

        string value = textEdit.Text ?? string.Empty;
        SetIsValid(textEdit , EditorInputRules.IsValidEmail(value));
    }

    #endregion Methods
}