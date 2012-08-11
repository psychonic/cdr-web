using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SteamKit2;
using SteamKit2.Blob;

namespace CDRUpdater
{
    public enum ESubscriptionBillingType : ushort
    {
        NoCost = 0,
        BillOnceOnly = 1,
        BillMonthly = 2,
        ProofOfPrepurchaseOnly = 3,
        GuestPass = 4,
        HardwarePromo = 5,
        Gift = 6,
        AutoGrant = 7,
        OEMTicket = 8,
    }

    public class AppLaunchOption
    {
        [BlobField(1)]
        public string Description { get; set; }

        [BlobField(2)]
        public string CommandLine { get; set; }

        [BlobField(3)]
        public int IconIndex { get; set; }

        [BlobField(4)]
        public bool NoDesktopShortcut { get; set; }

        [BlobField(5)]
        public bool NoStartMenuShortcut { get; set; }

        [BlobField(6)]
        public bool LongRunningUnattended { get; set; }

        [BlobField(7)]
        public string Platform { get; set; }
    }


    public class AppVersion
    {
        [BlobField(1)]
        [SqlColumn("description")]
        public string Description { get; set; }

        [BlobField(2)]
        [SqlColumn("version_id")]
        public uint VersionID { get; set; }

        [BlobField(3)]
        [SqlColumn("is_not_available")]
        public bool IsNotAvailable { get; set; }

        [BlobField(4)]
        [SqlColumn("launch_option_ids")]
        public List<int> LaunchOptionIDs { get; set; }

        [BlobField(5)]
        [SqlColumn("depot_key")]
        public string DepotEncryptionKey { get; set; }

        [BlobField(6)]
        [SqlColumn("is_encryption_key_available")]
        public bool IsEncryptionKeyAvailable { get; set; }

        [BlobField(7)]
        [SqlColumn("is_rebased")]
        public bool IsRebased { get; set; }

        [BlobField(8)]
        [SqlColumn("is_long_version_roll")]
        public bool IsLongVersionRoll { get; set; }
    }

    public class AppFilesystem
    {
        [BlobField(1)]
        [SqlColumn("app_id_filesystem")]
        public int AppID { get; set; }

        [BlobField(2)]
        [SqlColumn("mount_name")]
        public string MountName { get; set; }

        [BlobField(3)]
        [SqlColumn("is_optional")]
        public bool IsOptional { get; set; }

        [BlobField(4)]
        [SqlColumn("platform")]
        public string Platform { get; set; }
    }

    public class App
    {
        [BlobField(1)]
        [SqlColumn("app_id")]
        public uint AppID { get; set; }

        [BlobField(2)]
        [SqlColumn("name")]
        public string Name { get; set; }

        [BlobField(3)]
        [SqlColumn("install_dir")]
        public string InstallDirName { get; set; }

        [BlobField(4)]
        [SqlColumn("min_cache_size")]
        public uint MinCacheFileSizeMB { get; set; }

        [BlobField(5)]
        [SqlColumn("max_cache_size")]
        public uint MaxCacheFileSizeMB { get; set; }

        [BlobField(6)]
        [SqlColumn("launch_options")]
        public List<AppLaunchOption> LaunchOptions { get; set; }

        [BlobField(8)]
        [SqlColumn("on_first_launch")]
        public int OnFirstLaunch { get; set; }

        [BlobField(9)]
        [SqlColumn("is_bandwidth_greedy")]
        public bool IsBandwidthGreedy { get; set; }

        [BlobField(10)]
        public List<AppVersion> Versions { get; set; }

        [BlobField(11)]
        [SqlColumn("current_version_id")]
        public uint CurrentVersionID { get; set; }

        [BlobField(12)]
        public List<AppFilesystem> Filesystems { get; set; }

        [BlobField(13)]
        [SqlColumn("trickle_version_id")]
        public int TrickleVersionID { get; set; }

        [BlobField(14)]
        [SqlColumn("user_defined")]
        public Dictionary<string, string> UserDefined { get; set; }

        [BlobField(15)]
        [SqlColumn("beta_version_password")]
        public string BetaVersionPassword { get; set; }

        [BlobField(16)]
        [SqlColumn("beta_version_id")]
        public int BetaVersionID { get; set; }

        [BlobField(17)]
        [SqlColumn("legacy_install_dir")]
        public string LegacyInstallDirName { get; set; }

        [BlobField(18)]
        [SqlColumn("skip_mfp_overwrite")]
        public bool SkipMFPOverwrite { get; set; }

        [BlobField(19)]
        [SqlColumn("use_filesystem_dvr")]
        public bool UseFilesystemDvr { get; set; }

        [BlobField(20)]
        [SqlColumn("manifest_only")]
        public bool ManifestOnlyApp { get; set; }

        [BlobField(21)]
        [SqlColumn("app_of_manifest_only")]
        public uint AppOfManifestOnlyCache { get; set; }

        [SqlColumn("sub_count")]
        public int virtual_sub_count { get; set; }
    }

    public class SubRateLimit
    {
        [BlobField(1)]
        public uint Limit { get; set; }

        [BlobField(2)]
        public uint PeriodInMinutes { get; set; }
    }

    public class SubDiscount
    {
        [BlobField(1)]
        public string Name { get; set; }

        [BlobField(2)]
        public uint DiscountInCents { get; set; }

        [BlobField(3)]
        public List<SubDiscountQualifier> DiscountQualifiers { get; set; }
    }


    public class SubDiscountQualifier
    {
        [BlobField(1)]
        public string Name { get; set; }

        [BlobField(2)]
        public uint SubscriptionRequired { get; set; }

        [BlobField(3)]
        public bool IsDisqualifier { get; set; }
    }

    public class Sub 
    {
        [BlobField(1)]
        [SqlColumn("sub_id")]
        public uint SubID { get; set; }

        [BlobField(2)]
        [SqlColumn("name")]
        public string Name { get; set; }

        [BlobField(3)]
        [SqlColumn("billing_type")]
        public ESubscriptionBillingType BillingType { get; set; }

        [BlobField(4)]
        [SqlColumn("cost_in_cents")]
        public uint CostInCents { get; set; }

        [BlobField(5)]
        [SqlColumn("period_in_minutes")]
        public int PeriodInMinutes { get; set; }

        [BlobField(6)]
        public List<uint> AppIDs { get; set; }

        [BlobField(7)]
        [SqlColumn("on_subscribe_run_app_id")]
        public int OnSubscribeRunAppID { get; set; }

        [BlobField(8)]
        [SqlColumn("on_subscribe_run_launch_option_index")]
        public int OnSubscribeRunLaunchOptionIndex { get; set; }

        [BlobField(9)]
        [SqlColumn("rate_limits")]
        public List<SubRateLimit> RateLimits { get; set; }

        [BlobField(10)]
        [SqlColumn("discounts")]
        public List<SubDiscount> Discounts { get; set; }

        [BlobField(11)]
        [SqlColumn("is_preorder")]
        public bool IsPreorder { get; set; }

        [BlobField(12)]
        [SqlColumn("requires_shipping_address")]
        public bool RequiresShippingAddress { get; set; }

        [BlobField(13)]
        [SqlColumn("domestic_cost_in_cents")]
        public uint DomesticCostInCents { get; set; }

        [BlobField(14)]
        [SqlColumn("international_cost_in_cents")]
        public uint InternationalCostInCents { get; set; }

        [BlobField(15)]
        [SqlColumn("required_key_type")]
        public uint RequiredKeyType { get; set; }

        [BlobField(16)]
        [SqlColumn("is_cyber_cafe")]
        public bool IsCyberCafe { get; set; }

        [BlobField(17)]
        [SqlColumn("game_code")]
        public int GameCode { get; set; }

        [BlobField(18)]
        [SqlColumn("game_code_description")]
        public string GameCodeDescription { get; set; }

        [BlobField(19)]
        [SqlColumn("is_disabled")]
        public bool IsDisabled { get; set; }

        [BlobField(20)]
        [SqlColumn("requires_cd")]
        public bool RequiresCD { get; set; }

        [BlobField(21)]
        [SqlColumn("territory_code")]
        public uint TerritoryCode { get; set; }

        [BlobField(22)]
        [SqlColumn("is_steam3_subscription")]
        public bool IsSteam3Subscription { get; set; }

        [BlobField(23)]
        [SqlColumn("extended_info")]
        public Dictionary<string, string> ExtendedInfo { get; set; }

        [SqlColumn("app_count")]
        public int virtual_app_count { get; set; }
    }

    public class CDR
    {
        [BlobField(0)]
        public ushort VersionNum { get; set; }

        [BlobField(1)]
        public List<App> Apps { get; set; }

        [BlobField(2)]
        public List<Sub> Subs { get; set; }

        [BlobField(3)]
        public MicroTime LastChangedExistingAppOrSubscriptionTime { get; set; }


    }
}
