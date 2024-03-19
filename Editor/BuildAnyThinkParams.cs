using PluginSet.Core;
using PluginSet.Core.Editor;
using UnityEngine;

namespace PluginSet.AnyThink.Editor
{
    [BuildChannelsParams("AnyThink", "AnyThink SDK 配置")]
    public class BuildAnyThinkParams: ScriptableObject
    {
        [Tooltip("是否启用AnyThink")]
        public bool Enable;

        [VisibleCaseBoolValue("Enable", true)]
        [Tooltip("平台提供的APPID参数")]
        public string AppId;
        
        [VisibleCaseBoolValue("Enable", true)]
        [Tooltip("平台提供的APPKEY参数")]
        public string AppKey;
        
        [VisibleCaseBoolValue("Enable", true)]
        [Tooltip("Banner广告ID")]
        public string BannerAdUnitId;
        
        [VisibleCaseBoolValue("Enable", true)]
        [Tooltip("视频广告ID")]
        public string RewardAdUnitId;
        
        [VisibleCaseBoolValue("Enable", true)]
        [Tooltip("插屏广告ID")]
        public string InterstitialAdUnitId;
        
        [VisibleCaseBoolValue("Enable", true)]
        [Tooltip("开屏广告ID")]
        public string AppOpenAdUnitId;

        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeAdColony;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeAdMob;

        [VisibleCaseBoolValue("IncludeAdMob", true)]
        [Tooltip("Admob安卓平台ID")]
        public string AdMobAndroidApplicationId;
        [VisibleCaseBoolValue("IncludeAdMob", true)]
        [Tooltip("Admob iOS平台ID")]
        public string AdMobIOSApplicationId;

        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeAppLovin;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeBaidu;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeBigo;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeChartBoost;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeFacebook;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeFyber;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeGDT;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeHuawei;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeInMobi;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeIronSource;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeKidoz;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeKlevin;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeKuaishou;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeMaio;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeMintegral;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeMyTarget;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeNend;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeOgury;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludePangle;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludePubNative;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeSigmob;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeTapjoy;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeStartApp;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeUnityAds;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeVungle;
        
        [VisibleCaseBoolValue("Enable", true)]
        public bool IncludeYandex;
    }
}