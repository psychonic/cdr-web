using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SteamKit2;

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
        [BlobField(FieldKey = CDRAppLaunchOptionFields.eFieldDescription, Depth = 1)]
        public string Description { get; set; }

        [BlobField(FieldKey = CDRAppLaunchOptionFields.eFieldCommandLine, Depth = 1)]
        public string CommandLine { get; set; }

        [BlobField(FieldKey = CDRAppLaunchOptionFields.eFieldIconIndex, Depth = 1)]
        public int IconIndex { get; set; }

        [BlobField(FieldKey = CDRAppLaunchOptionFields.eFieldNoDesktopShortcut, Depth = 1)]
        public bool NoDesktopShortcut { get; set; }

        [BlobField(FieldKey = CDRAppLaunchOptionFields.eFieldNoStartMenuShortcut, Depth = 1)]
        public bool NoStartMenuShortcut { get; set; }

        [BlobField(FieldKey = CDRAppLaunchOptionFields.eFieldLongRunningUnattended, Depth = 1)]
        public bool LongRunningUnattended { get; set; }

        [BlobField(FieldKey = CDRAppLaunchOptionFields.eFieldPlatform, Depth = 1)]
        public string Platform { get; set; }
    }

    public class AppVersion
    {
        [BlobField(FieldKey = CDRAppVersionFields.eFieldDescription, Depth = 1)]
        [SqlColumn("description")]
        public string Description { get; set; }

        [BlobField(FieldKey = CDRAppVersionFields.eFieldVersionId, Depth = 1)]
        [SqlColumn("version_id")]
        public uint VersionID { get; set; }

        [BlobField(FieldKey = CDRAppVersionFields.eFieldIsNotAvailable, Depth = 1)]
        [SqlColumn("is_not_available")]
        public bool IsNotAvailable { get; set; }

        [BlobField(FieldKey = CDRAppVersionFields.eFieldLaunchOptionIdsRecord, Depth = 1)]
        [SqlColumn("launch_option_ids")]
        public List<int> LaunchOptionIDs { get; set; }

        [BlobField(FieldKey = CDRAppVersionFields.eFieldDepotEncryptionKey, Depth = 1)]
        [SqlColumn("depot_key")]
        public string DepotEncryptionKey { get; set; }

        [BlobField(FieldKey = CDRAppVersionFields.eFieldIsEncryptionKeyAvailable, Depth = 1)]
        [SqlColumn("is_encryption_key_available")]
        public bool IsEncryptionKeyAvailable { get; set; }

        [BlobField(FieldKey = CDRAppVersionFields.eFieldIsRebased, Depth = 1)]
        [SqlColumn("is_rebased")]
        public bool IsRebased { get; set; }

        [BlobField(FieldKey = CDRAppVersionFields.eFieldIsLongVersionRoll, Depth = 1)]
        [SqlColumn("is_long_version_roll")]
        public bool IsLongVersionRoll { get; set; }
    }

    public class AppFilesystem
    {
        [BlobField(FieldKey = CDRAppFilesystemFields.eFieldAppId, Depth = 1)]
        [SqlColumn("app_id_filesystem")]
        public int AppID { get; set; }

        [BlobField(FieldKey = CDRAppFilesystemFields.eFieldMountName, Depth = 1)]
        [SqlColumn("mount_name")]
        public string MountName { get; set; }

        [BlobField(FieldKey = CDRAppFilesystemFields.eFieldIsOptional, Depth = 1)]
        [SqlColumn("is_optional")]
        public bool IsOptional { get; set; }

        [BlobField(FieldKey = CDRAppFilesystemFields.eFieldPlatform, Depth = 1)]
        [SqlColumn("platform")]
        public string Platform { get; set; }
    }

    public class App
    {
        [BlobField(FieldKey = CDRAppRecordFields.eFieldAppId, Depth = 1)]
        [SqlColumn("app_id")]
        public uint AppID { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldName, Depth = 1)]
        [SqlColumn("name")]
        public string Name { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldInstallDirName, Depth = 1)]
        [SqlColumn("install_dir")]
        public string InstallDirName { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldMinCacheFileSizeMB, Depth = 1)]
        [SqlColumn("min_cache_size")]
        public uint MinCacheFileSizeMB { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldMaxCacheFileSizeMB, Depth = 1)]
        [SqlColumn("max_cache_size")]
        public uint MaxCacheFileSizeMB { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldLaunchOptionsRecord, Complex = true, Depth = 1)]
        [SqlColumn("launch_options")]
        public List<AppLaunchOption> LaunchOptions { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldOnFirstLaunch, Depth = 1)]
        [SqlColumn("on_first_launch")]
        public int OnFirstLaunch { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldIsBandwidthGreedy, Depth = 1)]
        [SqlColumn("is_bandwidth_greedy")]
        public bool IsBandwidthGreedy { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldVersionsRecord, Complex = true, Depth = 1)]
        public List<AppVersion> Versions { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldCurrentVersionId, Depth = 1)]
        [SqlColumn("current_version_id")]
        public uint CurrentVersionID { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldFilesystemsRecord, Complex = true, Depth = 1)]
        public List<AppFilesystem> Filesystems { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldTrickleVersionId, Depth = 1)]
        [SqlColumn("trickle_version_id")]
        public int TrickleVersionID { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldUserDefinedRecord, Depth = 1)]
        [SqlColumn("user_defined")]
        public Dictionary<string, string> UserDefined { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldBetaVersionPassword, Depth = 1)]
        [SqlColumn("beta_version_password")]
        public string BetaVersionPassword { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldBetaVersionId, Depth = 1)]
        [SqlColumn("beta_version_id")]
        public int BetaVersionID { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldLegacyInstallDirName, Depth = 1)]
        [SqlColumn("legacy_install_dir")]
        public string LegacyInstallDirName { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldSkipMFPOverwrite, Depth = 1)]
        [SqlColumn("skip_mfp_overwrite")]
        public bool SkipMFPOverwrite { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldUseFilesystemDvr, Depth = 1)]
        [SqlColumn("use_filesystem_dvr")]
        public bool UseFilesystemDvr { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldManifestOnlyApp, Depth = 1)]
        [SqlColumn("manifest_only")]
        public bool ManifestOnlyApp { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldAppOfManifestOnlyCache, Depth = 1)]
        [SqlColumn("app_of_manifest_only")]
        public uint AppOfManifestOnlyCache { get; set; }

        [SqlColumn("sub_count")]
        public int virtual_sub_count { get; set; }
    }

    public class SubRateLimit
    {
        [BlobField(FieldKey = CDRSubRateLimitFields.eFieldLimit, Depth = 1)]
        public uint Limit { get; set; }

        [BlobField(FieldKey = CDRSubRateLimitFields.eFieldPeriodInMinutes, Depth = 1)]
        public uint PeriodInMinutes { get; set; }
    }

    public class SubDiscount
    {
        [BlobField(FieldKey = CDRSubDiscountFields.eFieldName, Depth = 1)]
        public string Name { get; set; }

        [BlobField(FieldKey = CDRSubDiscountFields.eFieldDiscountInCents, Depth = 1)]
        public uint DiscountInCents { get; set; }

        [BlobField(FieldKey = CDRSubDiscountFields.eFieldDiscountQualifiersRecord, Complex = true, Depth = 1)]
        public List<SubDiscountQualifier> DiscountQualifiers { get; set; }
    }

    public class SubDiscountQualifier
    {
        [BlobField(FieldKey = CDRSubDiscountQualifierFields.eFieldName, Depth = 1)]
        public string Name { get; set; }

        [BlobField(FieldKey = CDRSubDiscountQualifierFields.eFieldSubscriptionRequired, Depth = 1)]
        public uint SubscriptionRequired { get; set; }

        [BlobField(FieldKey = CDRSubDiscountQualifierFields.eFieldIsDisqualifier, Depth = 1)]
        public bool IsDisqualifier { get; set; }
    }
    
    public class Sub 
    {
        [BlobField( FieldKey = CDRSubRecordFields.eFieldSubId )]
        [SqlColumn("sub_id")]
        public uint SubID { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldName )]
        [SqlColumn("name")]
        public string Name { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldBillingType )]
        [SqlColumn("billing_type")]
        public ESubscriptionBillingType BillingType { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldCostInCents )]
        [SqlColumn("cost_in_cents")]
        public uint CostInCents { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldPeriodInMinutes )]
        [SqlColumn("period_in_minutes")]
        public int PeriodInMinutes { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldAppIdsRecord )]
        public List<uint> AppIDs { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldOnSubscribeRunAppId )]
        [SqlColumn("on_subscribe_run_app_id")]
        public int OnSubscribeRunAppID { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldOnSubscribeRunLaunchOptionIndex )]
        [SqlColumn("on_subscribe_run_launch_option_index")]
        public int OnSubscribeRunLaunchOptionIndex { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldOptionalRateLimitRecord, Complex = true)]
        [SqlColumn("rate_limits")]
        public List<SubRateLimit> RateLimits { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldDiscountsRecord, Complex = true )]
        [SqlColumn("discounts")]
        public List<SubDiscount> Discounts { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldIsPreorder )]
        [SqlColumn("is_preorder")]
        public bool IsPreorder { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldRequiresShippingAddress )]
        [SqlColumn("requires_shipping_address")]
        public bool RequiresShippingAddress { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldDomesticCostInCents )]
        [SqlColumn("domestic_cost_in_cents")]
        public uint DomesticCostInCents { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldInternationalCostInCents )]
        [SqlColumn("international_cost_in_cents")]
        public uint InternationalCostInCents { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldRequiredKeyType )]
        [SqlColumn("required_key_type")]
        public uint RequiredKeyType { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldIsCyberCafe )]
        [SqlColumn("is_cyber_cafe")]
        public bool IsCyberCafe { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldGameCode )]
        [SqlColumn("game_code")]
        public int GameCode { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldGameCodeDescription )]
        [SqlColumn("game_code_description")]
        public string GameCodeDescription { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldIsDisabled )]
        [SqlColumn("is_disabled")]
        public bool IsDisabled { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldRequiresCD )]
        [SqlColumn("requires_cd")]
        public bool RequiresCD { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldTerritoryCode )]
        [SqlColumn("territory_code")]
        public uint TerritoryCode { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldIsSteam3Subscription )]
        [SqlColumn("is_steam3_subscription")]
        public bool IsSteam3Subscription { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldExtendedInfoRecord )]
        [SqlColumn("extended_info")]
        public Dictionary<string, string> ExtendedInfo { get; set; }

        [SqlColumn("app_count")]
        public int virtual_app_count { get; set; }
    }

    public class CDR
    {
        [BlobField(FieldKey = CDRFields.eFieldVersionNum, Depth = 1)]
        public ushort VersionNum { get; set; }

        [BlobField(FieldKey = CDRFields.eFieldApplicationsRecord, Depth = 1, Complex = true)]
        public List<App> Apps { get; set; }

        [BlobField(FieldKey = CDRFields.eFieldSubscriptionsRecord, Depth = 1, Complex = true)]
        public List<Sub> Subs { get; set; }

        [BlobField(FieldKey = CDRFields.eFieldLastChangedExistingAppOrSubscriptionTime, Depth = 1)]
        public MicroTime LastChangedExistingAppOrSubscriptionTime { get; set; }


    }
}
