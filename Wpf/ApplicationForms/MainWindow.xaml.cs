using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Api;
using Abstractions.Core.DependencyInjection;
using Abstractions.Core.Results;
using DevExpress.Xpf.Editors;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    private readonly List<MenuVisual> _menuVisuals = [];
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

        BuildMenu();
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

        CompanyModel? company = e.NewValue as CompanyModel;

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

        PeriodSelectionItem? selectedPeriod = e.NewValue as PeriodSelectionItem;

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

        await _companyPeriodForm.LoadCompanyAsync(company.Id , company.Name);
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

    private void BuildMenu()
    {
        MainMenuPanel.Children.Clear();
        _menuVisuals.Clear();

        List<MenuDefinition> menuDefinitions =
        [
            new MenuDefinition(
                "Ana Sayfa",
                ShowHome),

            new MenuDefinition(
                "Muhasebe",
                children:
                [
                    new MenuDefinition(
                        "Hesap Planı",
                        () => ShowPlaceholder("Hesap Planı")),

                    new MenuDefinition(
                        "Fişler",
                        () => ShowPlaceholder("Fişler")),

                    new MenuDefinition(
                        "Muhasebe Raporları",
                        () => ShowPlaceholder("Muhasebe Raporları"))
                ]),

            new MenuDefinition(
                "Ticari",
                children:
                [
                    new MenuDefinition(
                        "Cari",
                        children:
                        [
                            new MenuDefinition(
                                "Cari Kartlar",
                                () => ShowPlaceholder("Cari Kartlar")),

                            new MenuDefinition(
                                "Cari Hareketler",
                                () => ShowPlaceholder("Cari Hareketler"))
                        ]),

                    new MenuDefinition(
                        "Ürünler",
                        () => ShowPlaceholder("Ürünler")),

                    new MenuDefinition(
                        "Tedarikçiler",
                        () => ShowPlaceholder("Tedarikçiler")),

                    new MenuDefinition(
                        "Satın Alma",
                        () => ShowPlaceholder("Satın Alma")),

                    new MenuDefinition(
                        "Depolar",
                        () => ShowPlaceholder("Depolar"))
                ]),

            new MenuDefinition(
                "İnsan Kaynakları",
                () => ShowPlaceholder("İnsan Kaynakları")),

            new MenuDefinition(
                "CRM",
                () => ShowPlaceholder("CRM")),

            new MenuDefinition(
                "Yönetici",
                children:
                [
                    new MenuDefinition(
                        "Firma İşlemleri",
                        children:
                        [
                            new MenuDefinition(
                                "Firma Girişi",
                                OpenNewCompanyForm),

                            new MenuDefinition(
                                "Firma Listesi",
                                OpenCompanyListForm)
                        ])
                ]),

            new MenuDefinition(
                "Raporlar",
                () => ShowPlaceholder("Raporlar"))
        ];

        foreach (MenuDefinition menuDefinition in menuDefinitions)
        {
            MenuVisual menuVisual = CreateMenuVisual(menuDefinition , null);

            MainMenuPanel.Children.Add(menuVisual.Item);

            _menuVisuals.Add(menuVisual);
        }
    }

    private MenuVisual CreateMenuVisual(MenuDefinition definition , MenuVisual? parent)
    {
        Border item = new()
        {
            Style = (Style)FindResource("CascadingMenuItemStyle")
        };

        Grid content = new();

        ColumnDefinition textColumn = new()
        {
            Width = new GridLength(1 , GridUnitType.Star)
        };

        ColumnDefinition arrowColumn = new()
        {
            Width = GridLength.Auto
        };

        content.ColumnDefinitions.Add(textColumn);
        content.ColumnDefinitions.Add(arrowColumn);

        TextBlock text = new()
        {
            Text = definition.Text,
            Style = (Style)FindResource("CascadingMenuTextStyle")
        };

        Grid.SetColumn(text , 0);

        content.Children.Add(text);

        if (definition.HasChildren)
        {
            TextBlock arrow = new()
            {
                Text = "›",
                Style = (Style)FindResource("CascadingMenuArrowStyle"),
                Margin = new System.Windows.Thickness(12, 0, 0, 0)
            };

            Grid.SetColumn(arrow , 1);

            content.Children.Add(arrow);
        }

        item.Child = content;

        MenuVisual visual = new(definition , parent , item);

        item.MouseLeftButtonUp += (_ , _) => MenuItem_Click(visual);

        if (definition.HasChildren)
        {
            Popup popup = new()
            {
                PlacementTarget = item,
                Placement = System.Windows.Controls.Primitives.PlacementMode.Right,
                AllowsTransparency = true,
                StaysOpen = true,
                Focusable = false,
                PopupAnimation = System.Windows.Controls.Primitives.PopupAnimation.Fade,
                HorizontalOffset = 4
            };

            Border popupBorder = new()
            {
                Background = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(31, 41, 55)),

                BorderBrush = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(55, 65, 81)),

                BorderThickness = new System.Windows.Thickness(1),

                CornerRadius = new CornerRadius(4),

                Padding = new System.Windows.Thickness(6),

                Width = 220
            };

            StackPanel popupPanel = new();

            popup.Child = popupBorder;
            popupBorder.Child = popupPanel;

            visual.Popup = popup;

            foreach (MenuDefinition childDefinition in definition.Children)
            {
                MenuVisual childVisual = CreateMenuVisual(childDefinition , visual);

                popupPanel.Children.Add(childVisual.Item);

                visual.Children.Add(childVisual);

                _menuVisuals.Add(childVisual);
            }

            item.MouseEnter += (_ , _) =>
            {
                if (visual.Parent is not null)
                {
                    CloseSiblingMenus(visual);
                }
            };
        }

        return visual;
    }

    private void MenuItem_Click(MenuVisual visual)
    {
        if (visual.Definition.HasChildren)
        {
            if (visual.Popup is null)
            {
                return;
            }

            if (visual.Popup.IsOpen)
            {
                CloseDescendants(visual);

                visual.Popup.IsOpen = false;

                return;
            }

            CloseSiblingMenus(visual);

            visual.Popup.IsOpen = true;

            return;
        }

        CloseAllMenus();

        visual.Definition.Action?.Invoke();
    }

    private void CloseSiblingMenus(MenuVisual visual)
    {
        List<MenuVisual> siblings;

        if (visual.Parent is null)
        {
            siblings = _menuVisuals
                .Where(item => item.Parent is null && !ReferenceEquals(item , visual))
                .ToList();
        }
        else
        {
            siblings = visual.Parent.Children
                .Where(item => !ReferenceEquals(item , visual))
                .ToList();
        }

        foreach (MenuVisual sibling in siblings)
        {
            CloseMenu(sibling);
        }
    }

    private void CloseMenu(MenuVisual visual)
    {
        CloseDescendants(visual);

        if (visual.Popup is not null)
        {
            visual.Popup.IsOpen = false;
        }
    }

    private void CloseDescendants(MenuVisual visual)
    {
        foreach (MenuVisual child in visual.Children)
        {
            CloseDescendants(child);

            if (child.Popup is not null)
            {
                child.Popup.IsOpen = false;
            }
        }
    }

    private void CloseAllMenus()
    {
        foreach (MenuVisual visual in _menuVisuals)
        {
            if (visual.Popup is not null)
            {
                visual.Popup.IsOpen = false;
            }
        }
    }

    private void OpenNewCompanyForm()
    {
        _companyForm.PrepareForNew();

        HideHome();

        ContentArea.Content = _companyForm;
    }

    private void OpenCompanyListForm()
    {
        HideHome();

        ContentArea.Content = _companyListForm;
    }

    private async Task InitializeAsync()
    {
        if (_isInitializing)
        {
            return;
        }

        _isInitializing = true;

        try
        {
            IDataResult<List<CompanyModel>> result =
                await _companyApiClient.GetAllAsync();

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

    private void ShowHome()
    {
        ContentArea.Content = null;

        HomeContent.Visibility = System.Windows.Visibility.Visible;

        UpdateHomeContext();
    }

    private void HideHome()
    {
        HomeContent.Visibility = System.Windows.Visibility.Collapsed;
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
            Margin = new System.Windows.Thickness(40),

            Background = System.Windows.Media.Brushes.White,

            BorderBrush = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromRgb(229, 231, 235)),

            BorderThickness = new System.Windows.Thickness(1),

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

    private sealed class MenuDefinition
    {
        #region Properties

        public string Text { get; }

        public Action? Action { get; }

        public List<MenuDefinition> Children { get; }

        public bool HasChildren => Children.Count > 0;

        #endregion Properties

        #region Constructors

        public MenuDefinition(string text , Action? action = null , List<MenuDefinition>? children = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(text);

            Text = text;
            Action = action;
            Children = children ?? [];
        }

        #endregion Constructors
    }

    private sealed class MenuVisual
    {
        #region Properties

        public MenuDefinition Definition { get; }

        public MenuVisual? Parent { get; }

        public Border Item { get; }

        public Popup? Popup { get; set; }

        public List<MenuVisual> Children { get; } = [];

        #endregion Properties

        #region Constructors

        public MenuVisual(MenuDefinition definition , MenuVisual? parent , Border item)
        {
            ArgumentNullException.ThrowIfNull(definition);
            ArgumentNullException.ThrowIfNull(item);

            Definition = definition;
            Parent = parent;
            Item = item;
        }

        #endregion Constructors
    }

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