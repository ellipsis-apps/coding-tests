using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ellipsis.apps.uno.Presentation;

public partial class ShellModel : ObservableObject
{
    [ObservableProperty]
    private object? selectedItem;

    public ObservableCollection<NavigationViewItemBase> MenuItems { get; } = new()
    {
        new NavigationViewItem { Content = "Home",       Icon = new SymbolIcon { Symbol = Symbol.Home },        Tag = nameof(HomePage) },
        new NavigationViewItem { Content = "Purchases",  Icon = new SymbolIcon { Symbol = Symbol.Document },     Tag = nameof(PurchasesPage) },
        // Add more menu items here
    };

    public ObservableCollection<NavigationViewItemBase> FooterMenuItems { get; } = new()
    {
        new NavigationViewItem { Content = "Settings",   Icon = new SymbolIcon { Symbol = Symbol.Setting },     Tag = nameof(SettingsPage) },
    };
}
