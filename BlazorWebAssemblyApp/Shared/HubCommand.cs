namespace BlazorWebAssemblyApp.Shared
{
    public static class HubCommand
    {
        public static string RegisterHost => nameof(IHostGuestHub.RegisterHost);
        public static string RegisterGuest => nameof(IHostGuestHub.RegisterGuest);
        public static string HostConnected => nameof(IHostGuestHub.HostConnected);
        public static string HostPassMade => nameof(IHostGuestHub.HostPassMade);
        public static string HostPlayMade => nameof(IHostGuestHub.HostPlayMade);
        public static string HostNewGame => nameof(IHostGuestHub.HostNewGame);
        public static string GuestConnected => nameof(IHostGuestHub.GuestConnected);
        public static string GuestPassMade => nameof(IHostGuestHub.GuestPassMade);
        public static string GuestPlayMade => nameof(IHostGuestHub.GuestPlayMade);
    }
}
