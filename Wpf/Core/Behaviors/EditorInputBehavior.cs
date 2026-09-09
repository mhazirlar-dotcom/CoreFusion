using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Editors;
using Wpf.Core.Editors;

namespace Wpf.Core.Behaviors;

public static class EditorInputBehavior
{
    #region Dependency Properties

    public static readonly DependencyProperty InputTypeProperty = DependencyProperty.RegisterAttached(
    "InputType",
    typeof(EditorInputType),
    typeof(EditorInputBehavior),
    new PropertyMetadata(EditorInputType.None, OnInputTypeChanged));

    #endregion Dependency Properties

    #region Methods

    public static void SetInputType(DependencyObject element , EditorInputType value)
    {
        element.SetValue(InputTypeProperty , value);
    }

    public static EditorInputType GetInputType(DependencyObject element)
    {
        return (EditorInputType)element.GetValue(InputTypeProperty);
    }

    private static void OnInputTypeChanged(DependencyObject dependencyObject , DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not TextEdit textEdit)
        {
            return;
        }

        if (e.OldValue is EditorInputType oldType && oldType != EditorInputType.None)
        {
            textEdit.PreviewTextInput -= TextEdit_PreviewTextInput;
            System.Windows.DataObject.RemovePastingHandler(textEdit , TextEdit_Pasting);
            textEdit.EditValueChanged -= TextEdit_EditValueChanged;
        }

        if (e.NewValue is EditorInputType newType && newType != EditorInputType.None)
        {
            textEdit.PreviewTextInput += TextEdit_PreviewTextInput;
            System.Windows.DataObject.AddPastingHandler(textEdit , TextEdit_Pasting);
            textEdit.EditValueChanged += TextEdit_EditValueChanged;
        }
    }

    private static void TextEdit_PreviewTextInput(object sender , TextCompositionEventArgs e)
    {
        if (sender is not TextEdit textEdit)
        {
            return;
        }

        EditorInputType inputType = GetInputType(textEdit);

        switch (inputType)
        {
            case EditorInputType.Phone:
            case EditorInputType.Digits:
                e.Handled = e.Text.Any(character => !char.IsDigit(character));
                break;

            case EditorInputType.Decimal:
                e.Handled = e.Text.Any(character => !char.IsDigit(character) && character != ',' && character != '.');
                break;

            case EditorInputType.Date:
                e.Handled = e.Text.Any(character => !char.IsDigit(character) && character != '.');
                break;
        }
    }

    private static void TextEdit_Pasting(object sender , System.Windows.DataObjectPastingEventArgs e)
    {
        if (sender is not TextEdit textEdit)
        {
            return;
        }

        if (!e.DataObject.GetDataPresent(System.Windows.DataFormats.Text))
        {
            e.CancelCommand();
            return;
        }

        string pastedText = e.DataObject.GetData(System.Windows.DataFormats.Text) as string ?? string.Empty;
        EditorInputType inputType = GetInputType(textEdit);

        switch (inputType)
        {
            case EditorInputType.Phone:
            case EditorInputType.Digits:

                if (pastedText.Any(character => !char.IsDigit(character)))
                {
                    e.CancelCommand();
                }

                break;

            case EditorInputType.Decimal:

                if (pastedText.Any(character => !char.IsDigit(character) && character != ',' && character != '.'))
                {
                    e.CancelCommand();
                }

                break;

            case EditorInputType.Date:

                if (pastedText.Any(character => !char.IsDigit(character) && character != '.'))
                {
                    e.CancelCommand();
                }

                break;
        }
    }

    private static void TextEdit_EditValueChanged(object sender , EditValueChangedEventArgs e)
    {
        if (sender is not TextEdit textEdit || textEdit.EditValue is null)
        {
            return;
        }

        string value = textEdit.EditValue.ToString() ?? string.Empty;

        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        EditorInputType inputType = GetInputType(textEdit);

        switch (inputType)
        {
            case EditorInputType.ProperCase:

                SetText(textEdit , EditorInputRules.ToTurkishProperCase(value));

                break;

            case EditorInputType.Phone:

                SetText(textEdit , EditorInputRules.FormatPhone(value));

                break;

            case EditorInputType.Email:

                SetText(textEdit , EditorInputRules.NormalizeEmail(value));

                break;

            case EditorInputType.Decimal:

                SetText(textEdit , EditorInputRules.NormalizeDecimal(value));

                break;

            case EditorInputType.Digits:

                SetText(textEdit , EditorInputRules.KeepDigits(value));

                break;
        }
    }

    private static void SetText(TextEdit textEdit , string value)
    {
        if (textEdit.Text == value)
        {
            return;
        }

        int caretPosition = textEdit.CaretIndex;

        textEdit.EditValue = value;
        textEdit.CaretIndex = Math.Min(caretPosition , value.Length);
    }

    #endregion Methods
}