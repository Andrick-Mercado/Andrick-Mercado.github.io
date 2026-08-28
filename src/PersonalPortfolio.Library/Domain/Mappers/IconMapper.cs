using MudBlazor;

namespace PersonalPortfolio.Library.Domain.Mappers;

public static class IconMapper
{
    public static string GetIcon(Icon icon)
    {
        return icon switch
        {
            Icon.AccountCircle => Icons.Material.Filled.AccountCircle,
            Icon.Lightbulb => Icons.Material.Filled.Lightbulb,
            Icon.Construction => Icons.Material.Filled.Construction,
            Icon.AutoGraph => Icons.Material.Filled.AutoGraph,
            Icon.InsertDriveFile => Icons.Material.Filled.InsertDriveFile,
            Icon.Work => Icons.Material.Filled.Work,
            Icon.School => Icons.Material.Filled.School,
            Icon.Code => Icons.Material.Filled.Code,
            Icon.SportsEsports => Icons.Material.Filled.SportsEsports,
            Icon.Cloud => Icons.Material.Filled.Cloud,
            Icon.Storage => Icons.Material.Filled.Storage,
            Icon.Groups => Icons.Material.Filled.Groups,
            Icon.Terminal => Icons.Material.Filled.Terminal,
            Icon.Web => Icons.Material.Filled.Web,
            Icon.RocketLaunch => Icons.Material.Filled.RocketLaunch,
            Icon.EditNote => Icons.Material.Filled.EditNote,
            Icon.ViewInAr => Icons.Material.Filled.ViewInAr,
            Icon.Rocket => Icons.Material.Filled.Rocket,
            Icon.Psychology => Icons.Material.Filled.Psychology,
            Icon.BugReport => Icons.Material.Filled.BugReport,
            Icon.Flutter => Icons.Material.Filled.Air,
            Icon.Terrain => Icons.Material.Filled.Terrain,
            Icon.Waves => Icons.Material.Filled.Waves,
            Icon.AccountBalance => Icons.Material.Filled.AccountBalance,
            Icon.Lock => Icons.Material.Filled.Lock,
            Icon.VolunteerActivism => Icons.Material.Filled.VolunteerActivism,
            Icon.Visibility => Icons.Material.Filled.Visibility,
            Icon.Hub => Icons.Material.Filled.Hub,
            Icon.Extension => Icons.Material.Filled.Extension,
            Icon.GridOn => Icons.Material.Filled.GridOn,
            _ => Icons.Material.Filled.DeviceUnknown
        };
    }
}