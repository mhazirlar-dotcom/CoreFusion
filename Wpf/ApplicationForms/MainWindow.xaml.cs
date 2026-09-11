using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Api;
using Abstractions.Core.DependencyInjection;
using Abstractions.Core.Results;
using DevExpress.Xpf.Editors;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Companies;
using Wpf.State;
using MessageBox = System.Windows.MessageBox;

[assembly: System.Runtime.Versioning.SupportedOSPlatform("windows")]

namespace Wpf.ApplicationForms;

public partial class MainWindow : Window, ITransientService
{
    #region Fields

    private readonly ICompanyApiClient _companyApiClient;
    private readonly ICompanyPeriodApiClient _companyPeriodApiClient;
    private readonly CompanyState _companyState;
    private readonly WorkingContext _workingContext;
    private readonly CompanyListForm _companyListForm;
    private readonly CompanyForm _companyForm;
    private readonly CompanyPeriodForm _companyPeriodForm;
    private readonly ObservableCollection<PeriodSelectionItem> _periods = [];
    private bool _isInitializing;

    #endregion Fields

    #region Constructors

    public MainWindow(ICompanyApiClient companyApiClient , ICompanyPeriodApiClient companyPeriodApiClient , CompanyState companyState , WorkingContext workingContext , CompanyListForm companyListForm , CompanyForm companyForm , CompanyPeriodForm companyPeriodForm)
    {
        ArgumentNullException.ThrowIfNull(companyApiClient);
        ArgumentNullException.ThrowIfNull(companyPeriodApiClient);
        ArgumentNullException.ThrowIfNull(companyState);
        ArgumentNullException.ThrowIfNull(workingContext);
        ArgumentNullException.ThrowIfNull(companyListForm);
        ArgumentNullException.ThrowIfNull(companyForm);
        ArgumentNullException.ThrowIfNull(companyPeriodForm);

        _companyApiClient = companyApiClient;
        _companyPeriodApiClient = companyPeriodApiClient;
        _companyState = companyState;
        _workingContext = workingContext;
        _companyListForm = companyListForm;
        _companyForm = companyForm;
        _companyPeriodForm = companyPeriodForm;

        InitializeComponent();

        CompanyComboBoxEdit.ItemsSource = _companyState.Companies;
        PeriodComboBoxEdit.ItemsSource = _periods;

        _workingContext.ContextChanged += WorkingContext_ContextChanged;
        _companyListForm.NewCompanyRequested += CompanyListForm_NewCompanyRequested;
        _companyListForm.EditCompanyRequested += CompanyListForm_EditCompanyRequested;
        _companyListForm.PeriodsRequested += CompanyListForm_PeriodsRequested;
    }

    #endregion Constructors

    #region Events

    private async void MainWindow_Loaded(object sender , RoutedEventArgs e)
    {
        await InitializeAsync();
    }

    private async void CompanyComboBoxEdit_EditValueChanged(object sender , EditValueChangedEventArgs e)
    {
        if (_isInitializing)
        {
            return;
        }

        CompanyModel? company = (CompanyModel)e.NewValue;

        if (company is null)
        {
            return;
        }

        _companyState.SetActiveCompany(company);
        _workingContext.SetCompany(company);

        await LoadPeriodsAsync(company.Id);
    }

    private void PeriodComboBoxEdit_EditValueChanged(object sender , EditValueChangedEventArgs e)
    {
        if (_isInitializing)
        {
            return;
        }

        PeriodSelectionItem? selectedPeriod = (PeriodSelectionItem)e.NewValue;

        if (selectedPeriod is null)
        {
            return;
        }

        _workingContext.SetPeriod(selectedPeriod.Period);
    }

    private void WorkingContext_ContextChanged(object? sender , EventArgs e)
    {
        UpdateHomeContext();
    }

    private void HomeButton_Click(object sender , RoutedEventArgs e)
    {
        ShowHome();
    }

    private void CommercialButton_Click(object sender , RoutedEventArgs e)
    {
        ToggleSubMenu(CommercialSubMenu);
    }

    private void CurrentAccountButton_Click(object sender , RoutedEventArgs e)
    {
        ToggleSubMenu(CurrentAccountSubMenu);
    }

    private void AccountingButton_Click(object sender , RoutedEventArgs e)
    {
        ToggleSubMenu(AccountingSubMenu);
    }

    private void CompaniesButton_Click(object sender , RoutedEventArgs e)
    {
        HideHome();
        ContentArea.Content = _companyListForm;
    }

    private void ProductsButton_Click(object sender , RoutedEventArgs e)
    {
        ShowPlaceholder("Ürünler");
    }

    private void SuppliersButton_Click(object sender , RoutedEventArgs e)
    {
        ShowPlaceholder("Tedarikçiler");
    }

    private void PurchasingButton_Click(object sender , RoutedEventArgs e)
    {
        ShowPlaceholder("Satın Alma");
    }

    private void WarehousesButton_Click(object sender , RoutedEventArgs e)
    {
        ShowPlaceholder("Depolar");
    }

    private void CurrentCardsButton_Click(object sender , RoutedEventArgs e)
    {
        ShowPlaceholder("Cari Kartlar");
    }

    private void CurrentTransactionsButton_Click(object sender , RoutedEventArgs e)
    {
        ShowPlaceholder("Cari Hareketler");
    }

    private void AccountPlanButton_Click(object sender , RoutedEventArgs e)
    {
        ShowPlaceholder("Hesap Planı");
    }

    private void VouchersButton_Click(object sender , RoutedEventArgs e)
    {
        ShowPlaceholder("Fişler");
    }

    private void ReportsButton_Click(object sender , RoutedEventArgs e)
    {
        ShowPlaceholder("Raporlar");
    }

    private void CrmButton_Click(object sender , RoutedEventArgs e)
    {
        ShowPlaceholder("CRM");
    }

    private void HumanResourcesButton_Click(object sender , RoutedEventArgs e)
    {
        ShowPlaceholder("İnsan Kaynakları");
    }

    private void CompanyListForm_NewCompanyRequested(object? sender , EventArgs e)
    {
        _companyForm.PrepareForNew();
        ContentArea.Content = _companyForm;
    }

    private async void CompanyListForm_EditCompanyRequested(object? sender , CompanyModel company)
    {
        ArgumentNullException.ThrowIfNull(company);

        ContentArea.Content = _companyForm;

        await _companyForm.LoadCompanyAsync(company.Id);
    }

    private async void CompanyListForm_PeriodsRequested(object? sender , CompanyModel company)
    {
        ArgumentNullException.ThrowIfNull(company);

        ContentArea.Content = _companyPeriodForm;

        await _companyPeriodForm.LoadCompanyAsync(company.Id);
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

    #region Methods

    private async Task InitializeAsync()
    {
        if (_isInitializing)
        {
            return;
        }

        _isInitializing = true;

        try
        {
            IDataResult<List<CompanyModel>> result = await _companyApiClient.GetAllAsync();

            _companyState.SetCompanies(result.Data);

            CompanyModel? activeCompany = _companyState.ActiveCompany;

            if (activeCompany is null)
            {
                CompanyComboBoxEdit.EditValue = null;
                _periods.Clear();
                _workingContext.Clear();
                ShowHome();

                return;
            }

            CompanyComboBoxEdit.EditValue = activeCompany;

            _workingContext.SetCompany(activeCompany);

            await LoadPeriodsAsync(activeCompany.Id);
            ShowHome();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message ,
                "CoreFusion" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Error);
        }
        finally
        {
            _isInitializing = false;
        }
    }

    private async Task LoadPeriodsAsync(Guid companyId)
    {
        try
        {
            IDataResult<List<CompanyPeriodModel>> result =
                await _companyPeriodApiClient.GetByCompanyIdAsync(companyId);

            _periods.Clear();

            foreach (CompanyPeriodModel period in result.Data.OrderBy(item => item.StartDate))
            {
                _periods.Add(new PeriodSelectionItem(period));
            }

            if (_periods.Count == 0)
            {
                PeriodComboBoxEdit.EditValue = null;
                _workingContext.Clear();

                CompanyModel? company = _companyState.ActiveCompany;

                if (company is not null)
                {
                    _workingContext.SetCompany(company);
                }

                return;
            }

            PeriodSelectionItem? selectedPeriod = GetDefaultPeriod(_periods);
            PeriodComboBoxEdit.EditValue = selectedPeriod;

            if (selectedPeriod is not null)
            {
                _workingContext.SetPeriod(selectedPeriod.Period);
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message ,
                "Dönemler" ,
                MessageBoxButton.OK ,
                MessageBoxImage.Error);
        }
    }

    private static PeriodSelectionItem? GetDefaultPeriod(IEnumerable<PeriodSelectionItem> periods)
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);

        List<PeriodSelectionItem> periodList = [.. periods];

        PeriodSelectionItem? currentPeriod = periodList.FirstOrDefault(
            item =>
                item.Period.StartDate <= today &&
                item.Period.EndDate >= today);

        if (currentPeriod is not null)
        {
            return currentPeriod;
        }

        return periodList
            .OrderBy(item => GetDistance(item.Period , today))
            .ThenBy(item => item.Period.StartDate)
            .FirstOrDefault();
    }

    private static int GetDistance(CompanyPeriodModel period , DateOnly today)
    {
        if (today < period.StartDate)
        {
            return period.StartDate.DayNumber - today.DayNumber;
        }

        if (today > period.EndDate)
        {
            return today.DayNumber - period.EndDate.DayNumber;
        }

        return 0;
    }

    private void ToggleSubMenu(FrameworkElement subMenu)
    {
        bool isVisible = subMenu.Visibility == Visibility.Visible;

        CommercialSubMenu.Visibility = Visibility.Collapsed;
        CurrentAccountSubMenu.Visibility = Visibility.Collapsed;
        AccountingSubMenu.Visibility = Visibility.Collapsed;

        subMenu.Visibility = isVisible
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    private void ShowHome()
    {
        ContentArea.Content = null;
        HomeContent.Visibility = Visibility.Visible;

        UpdateHomeContext();
    }

    private void HideHome()
    {
        HomeContent.Visibility = Visibility.Collapsed;
    }

    private void ShowPlaceholder(string title)
    {
        HideHome();

        Grid grid = new()
        {
            Background = System.Windows.Media.Brushes.Transparent
        };

        Border border = new()
        {
            Margin = new Thickness(40),
            Background = System.Windows.Media.Brushes.White,
            BorderBrush = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromRgb(229, 231, 235)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8)
        };

        TextBlock textBlock = new()
        {
            Text = $"{title}\n\nBu modül henüz hazırlanıyor.",
            FontSize = 20,
            Foreground = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromRgb(75, 85, 99)),
            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
            VerticalAlignment = System.Windows.VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
            MaxWidth = 700
        };

        border.Child = textBlock;
        grid.Children.Add(border);

        ContentArea.Content = grid;
    }

    private void UpdateHomeContext()
    {
        CompanyModel? company = _workingContext.ActiveCompany;
        CompanyPeriodModel? period = _workingContext.ActivePeriod;

        if (company is null)
        {
            HomeContextText.Text = "Lütfen firma seçiniz.";

            return;
        }

        if (period is null)
        {
            HomeContextText.Text =
                $"{company.Name} firması seçildi.\n\nBu firma için tanımlı bir dönem bulunmamaktadır.";

            return;
        }

        HomeContextText.Text =
            $"{company.Name} firmasının {period.StartDate:dd/MM/yyyy} - {period.EndDate:dd/MM/yyyy} döneminde çalışıyorsunuz.";
    }

    #endregion Methods

    #region Nested Types

    private sealed class PeriodSelectionItem
    {
        #region Properties

        public CompanyPeriodModel Period { get; }

        public string DisplayText =>
            $"{Period.StartDate:dd/MM/yyyy} - {Period.EndDate:dd/MM/yyyy}";

        #endregion Properties

        #region Constructors

        public PeriodSelectionItem(CompanyPeriodModel period)
        {
            ArgumentNullException.ThrowIfNull(period);

            Period = period;
        }

        #endregion Constructors
    }

    #endregion Nested Types
}