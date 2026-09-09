using Abstractions.Application.Models.Master;
using Abstractions.Core.DependencyInjection;
using System.Windows;
using System.Windows.Input;
using Wpf.Companies;

namespace Wpf.ApplicationForms;

public partial class MainWindow : Window, ITransientService
{
    #region Fields

    private readonly CompanyListForm _companyListForm;
    private readonly CompanyForm _companyForm;

    #endregion Fields

    #region Constructors

    public MainWindow(CompanyListForm companyListForm , CompanyForm companyForm)
    {
        ArgumentNullException.ThrowIfNull(companyListForm);
        ArgumentNullException.ThrowIfNull(companyForm);

        _companyListForm = companyListForm;
        _companyForm = companyForm;

        InitializeComponent();

        _companyListForm.NewCompanyRequested += CompanyListForm_NewCompanyRequested;
        _companyListForm.EditCompanyRequested += CompanyListForm_EditCompanyRequested;
    }

    #endregion Constructors

    #region Events

    private void CompaniesButton_Click(object sender , RoutedEventArgs e)
    {
        ContentArea.Content = _companyListForm;
    }

    private void CompanyListForm_NewCompanyRequested(object? sender , EventArgs e)
    {
        _companyForm.PrepareForNew();
        ContentArea.Content = _companyForm;
    }

    private async void CompanyListForm_EditCompanyRequested(object? sender , CompanyModel company)
    {
        ContentArea.Content = _companyForm;

        await _companyForm.LoadCompanyAsync(company.Id);
    }

    private void MinimizeButton_Click(object sender , RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void ExitButton_Click(object sender , RoutedEventArgs e)
    {
        System.Windows.Application.Current.Shutdown();
    }

    private void TitleBar_MouseLeftButtonDown(object sender , MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    #endregion Events
}