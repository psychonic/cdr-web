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
    }

    public class AppLaunchOption
    {
        [BlobField(FieldKey = CDRAppLaunchOptionFields.eFieldDescription)]
        public string Description { get; set; }

        [BlobField(FieldKey = CDRAppLaunchOptionFields.eFieldCommandLine)]
        public string CommandLine { get; set; }

        [BlobField(FieldKey = CDRAppLaunchOptionFields.eFieldIconIndex)]
        public int IconIndex { get; set; }

        [BlobField(FieldKey = CDRAppLaunchOptionFields.eFieldNoDesktopShortcut)]
        public bool NoDesktopShortcut { get; set; }

        [BlobField(FieldKey = CDRAppLaunchOptionFields.eFieldNoStartMenuShortcut)]
        public bool NoStartMenuShortcut { get; set; }

        [BlobField(FieldKey = CDRAppLaunchOptionFields.eFieldLongRunningUnattended)]
        public bool LongRunningUnattended { get; set; }
    }

    public class AppVersion
    {
        [BlobField(FieldKey = CDRAppVersionFields.eFieldDescription)]
        public string Description { get; set; }

        [BlobField(FieldKey = CDRAppVersionFields.eFieldVersionId)]
        public uint VersionID { get; set; }

        [BlobField(FieldKey = CDRAppVersionFields.eFieldIsNotAvailable)]
        public bool IsNotAvailable { get; set; }

        [BlobField(FieldKey = CDRAppVersionFields.eFieldLaunchOptionIdsRecord)]
        public List<int> LaunchOptionIDs { get; set; }

        [BlobField(FieldKey = CDRAppVersionFields.eFieldDepotEncryptionKey)]
        public string DepotEncryptionKey { get; set; }

        [BlobField(FieldKey = CDRAppVersionFields.eFieldIsEncryptionKeyAvailable)]
        public bool IsEncryptionKeyAvailable { get; set; }

        [BlobField(FieldKey = CDRAppVersionFields.eFieldIsRebased)]
        public bool IsRebased { get; set; }

        [BlobField(FieldKey = CDRAppVersionFields.eFieldIsLongVersionRoll)]
        public bool IsLongVersionRoll { get; set; }
    }

    public class AppFilesystem
    {
        [BlobField(FieldKey = CDRAppFilesystemFields.eFieldAppId)]
        public uint AppID { get; set; }

        [BlobField(FieldKey = CDRAppFilesystemFields.eFieldMountName)]
        public string MountName { get; set; }

        [BlobField(FieldKey = CDRAppFilesystemFields.eFieldIsOptional)]
        public bool IsOptional { get; set; }
    }

    public class App
    {
        [BlobField(FieldKey = CDRAppRecordFields.eFieldAppId, Depth = 1)]
        public uint AppID { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldName, Depth = 1)]
        public string Name { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldInstallDirName, Depth = 1)]
        public string InstallDirName { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldMinCacheFileSizeMB, Depth = 1)]
        public uint MinCacheFileSizeMB { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldMaxCacheFileSizeMB, Depth = 1)]
        public uint MaxCacheFileSizeMB { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldLaunchOptionsRecord, Complex = true, Depth = 1)]
        public List<AppLaunchOption> LaunchOptions { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldOnFirstLaunch, Depth = 1)]
        public int OnFirstLaunch { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldIsBandwidthGreedy, Depth = 1)]
        public bool IsBandwidthGreedy { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldVersionsRecord, Complex = true, Depth = 1)]
        public List<AppVersion> Versions { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldCurrentVersionId, Depth = 1)]
        public uint CurrentVersionID { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldFilesystemsRecord, Complex = true, Depth = 1)]
        public List<AppFilesystem> Filesystems { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldTrickleVersionId, Depth = 1)]
        public int TrickleVersionID { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldUserDefinedRecord, Depth = 1)]
        public Dictionary<string, string> UserDefined { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldBetaVersionPassword, Depth = 1)]
        public string BetaVersionPassword { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldBetaVersionId, Depth = 1)]
        public int BetaVersionID { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldLegacyInstallDirName, Depth = 1)]
        public string LegacyInstallDirName { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldSkipMFPOverwrite, Depth = 1)]
        public bool SkipMFPOverwrite { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldUseFilesystemDvr, Depth = 1)]
        public bool UseFilesystemDvr { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldManifestOnlyApp, Depth = 1)]
        public bool ManifestOnlyApp { get; set; }

        [BlobField(FieldKey = CDRAppRecordFields.eFieldAppOfManifestOnlyCache, Depth = 1)]
        public uint AppOfManifestOnlyCache { get; set; }
    }

    public class SubRateLimit
    {
        [BlobField( FieldKey = CDRSubRateLimitFields.eFieldLimit ) ]
        public uint Limit { get; set; }

        [BlobField( FieldKey = CDRSubRateLimitFields.eFieldPeriodInMinutes )]
        public uint PeriodInMinutes { get; set; }
    }

    public class SubDiscount
    {
        [BlobField( FieldKey = CDRSubDiscountFields.eFieldName )]
        public string Name { get; set; }

        [BlobField( FieldKey = CDRSubDiscountFields.eFieldDiscountInCents )]
        public uint DiscountInCents { get; set; }

        [BlobField( FieldKey = CDRSubDiscountFields.eFieldDiscountQualifiersRecord, Complex = true )]
        public List<SubDiscountQualifier> DiscountQualifiers { get; set; }
    }

    public class SubDiscountQualifier
    {
        [BlobField( FieldKey = CDRSubDiscountQualifierFields.eFieldName )]
        public string Name { get; set; }

        [BlobField( FieldKey = CDRSubDiscountQualifierFields.eFieldSubscriptionRequired )]
        public uint SubscriptionRequired { get; set; }

        [BlobField( FieldKey = CDRSubDiscountQualifierFields.eFieldIsDisqualifier )]
        public bool IsDisqualifier { get; set; }
    }
    
    public class Sub 
    {
        [BlobField( FieldKey = CDRSubRecordFields.eFieldSubId )]
        public uint SubID { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldName )]
        public string Name { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldBillingType )]
        public ESubscriptionBillingType BillingType { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldCostInCents )]
        public uint CostInCents { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldPeriodInMinutes )]
        public int PeriodInMinutes { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldAppIdsRecord )]
        public List<uint> AppIDs { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldOnSubscribeRunAppId )]
        public int OnSubscribeRunAppID { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldOnSubscribeRunLaunchOptionIndex )]
        public int OnSubscribeRunLaunchOptionIndex { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldOptionalRateLimitRecord, Complex = true)]
        public List<SubRateLimit> RateLimits { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldDiscountsRecord, Complex = true )]
        public List<SubDiscount> Discounts { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldIsPreorder )]
        public bool IsPreorder { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldRequiresShippingAddress )]
        public bool RequiresShippingAddress { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldDomesticCostInCents )]
        public uint DomesticCostInCents { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldInternationalCostInCents )]
        public uint InternationalCostInCents { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldRequiredKeyType )]
        public uint RequiredKeyType { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldIsCyberCafe )]
        public bool IsCyberCafe { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldGameCode )]
        public int GameCode { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldGameCodeDescription )]
        public string GameCodeDescription { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldIsDisabled )]
        public bool IsDisabled { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldRequiresCD )]
        public bool RequiresCD { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldTerritoryCode )]
        public uint TerritoryCode { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldIsSteam3Subscription )]
        public bool IsSteam3Subscription { get; set; }

        [BlobField( FieldKey = CDRSubRecordFields.eFieldExtendedInfoRecord )]
        public Dictionary<string, string> ExtendedInfo { get; set; }
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
