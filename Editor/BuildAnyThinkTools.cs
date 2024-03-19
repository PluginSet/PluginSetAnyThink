using PluginSet.Core;
using PluginSet.Core.Editor;
using UnityEditor;

namespace PluginSet.AnyThink.Editor
{
    [BuildTools]
    public static class BuildAnyThinkTools
    {
        [OnSyncEditorSetting]
        public static void OnSyncEditorSetting(BuildProcessorContext context)
        {
            if (!context.BuildTarget.Equals(BuildTarget.Android) && !context.BuildTarget.Equals(BuildTarget.iOS))
                return;
            
            var buildParams = context.BuildChannels.Get<BuildAnyThinkParams>();
            if (!buildParams.Enable)
                return;
            
            context.Symbols.Add("ENABLE_ANYTHINK");
            context.AddLinkAssembly(".AnyThink");
            
            var pluginConfig = context.Get<PluginSetConfig>("pluginsConfig");
            var config = pluginConfig.AddConfig<PluginAnyThinkConfig>("AnyThink");
            config.AppId = buildParams.AppId;
            config.AppKey = buildParams.AppKey;
            config.BannerAdUnitId = buildParams.BannerAdUnitId;
            config.RewardAdUnitId = buildParams.RewardAdUnitId;
            config.InterstitialAdUnitId = buildParams.InterstitialAdUnitId;
            config.OpenAdUnitId = buildParams.AppOpenAdUnitId;
            
            Global.CopyDependenciesInLib("com.pluginset.anythink", "Dependencies", delegate(string name)
            {
                if (name.StartsWith("adcolony/"))
                    return !buildParams.IncludeAdColony;
                
                if (name.StartsWith("admob/"))
                    return !buildParams.IncludeAdMob;
                
                if (name.StartsWith("anythink/"))
                    return false;
                
                if (name.StartsWith("applovin/"))
                    return !buildParams.IncludeAppLovin;
                
                if (name.StartsWith("baidu/"))
                    return !buildParams.IncludeBaidu;
                
                if (name.StartsWith("bigo/"))
                    return !buildParams.IncludeBigo;
                
                if (name.StartsWith("chartboost/"))
                    return !buildParams.IncludeChartBoost;
                
                if (name.StartsWith("facebook/"))
                    return !buildParams.IncludeFacebook;
                
                if (name.StartsWith("fyber/"))
                    return !buildParams.IncludeFyber;
                
                if (name.StartsWith("gdt/"))
                    return !buildParams.IncludeGDT;
                
                if (name.StartsWith("huawei/"))
                    return !buildParams.IncludeHuawei;
                
                if (name.StartsWith("inmobi/"))
                    return !buildParams.IncludeInMobi;
                
                if (name.StartsWith("ironsource/"))
                    return !buildParams.IncludeIronSource;
                
                if (name.StartsWith("kidoz/"))
                    return !buildParams.IncludeKidoz;
                
                if (name.StartsWith("klevin/"))
                    return !buildParams.IncludeKlevin;
                
                if (name.StartsWith("kuaishou/"))
                    return !buildParams.IncludeKuaishou;
                
                if (name.StartsWith("maio/"))
                    return !buildParams.IncludeMaio;
                
                if (name.StartsWith("mintegral/"))
                    return !buildParams.IncludeMintegral;
                
                if (name.StartsWith("mytarget/"))
                    return !buildParams.IncludeMyTarget;
                
                if (name.StartsWith("nend/"))
                    return !buildParams.IncludeNend;

                if (name.StartsWith("ogury/"))
                    return !buildParams.IncludeOgury;
                
                if (name.StartsWith("pangle/"))
                    return !buildParams.IncludePangle;
                
                if (name.StartsWith("pubnative/"))
                    return !buildParams.IncludePubNative;
                
                if (name.StartsWith("sigmob/"))
                    return !buildParams.IncludeSigmob;
                
                if (name.StartsWith("startapp/"))
                    return !buildParams.IncludeStartApp;
                
                if (name.StartsWith("tapjoy/"))
                    return !buildParams.IncludeTapjoy;
                
                if (name.StartsWith("unityads/"))
                    return !buildParams.IncludeUnityAds;
                
                if (name.StartsWith("vungle/"))
                    return !buildParams.IncludeVungle;
                
                if (name.StartsWith("yandex/"))
                    return !buildParams.IncludeYandex;
                
                return false;
            });
        }

        [AndroidProjectModify]
        public static void OnAndroidProjectModify(BuildProcessorContext context, AndroidProjectManager projectManager)
        {
            var buildParams = context.BuildChannels.Get<BuildAnyThinkParams>();
            if (!buildParams.Enable)
                return;

            if (buildParams.IncludeAdMob)
            {
                projectManager.LibraryManifest.SetMetaData("com.google.android.gms.ads.APPLICATION_ID", buildParams.AdMobAndroidApplicationId);
            }
            if (buildParams.IncludeChartBoost)
            {
                var gradle1 = projectManager.LibraryGradle;
                var node1 = gradle1.ROOT.GetOrCreateNode("android/packagingOptions");
                node1.AppendContentNode("exclude 'META-INF/Chartboost-9.1.1_productionRelease.kotlin_module'");
                
                var gradle2 = projectManager.LauncherGradle;
                var node2 = gradle2.ROOT.GetOrCreateNode("android/packagingOptions");
                node2.AppendContentNode("exclude 'META-INF/Chartboost-9.1.1_productionRelease.kotlin_module'");
            }

            if (buildParams.IncludeVungle)
            {
                var gradle1 = projectManager.LibraryGradle;
                var node1 = gradle1.ROOT.GetOrCreateNode("android/packagingOptions");
                node1.AppendContentNode("exclude 'META-INF/kotlinx-serialization-json.kotlin_module'");
                node1.AppendContentNode("exclude 'META-INF/kotlinx-serialization-core.kotlin_module'");
                node1.AppendContentNode("exclude 'META-INF/okio.kotlin_module'");
                
                var gradle2 = projectManager.LauncherGradle;
                var node2 = gradle2.ROOT.GetOrCreateNode("android/packagingOptions");
                node2.AppendContentNode("exclude 'META-INF/kotlinx-serialization-json.kotlin_module'");
                node2.AppendContentNode("exclude 'META-INF/kotlinx-serialization-core.kotlin_module'");
                node2.AppendContentNode("exclude 'META-INF/okio.kotlin_module'");
            }
        }

        [iOSXCodeProjectModify(int.MaxValue)]
        public static void OnIOSXCodeProjectModify(BuildProcessorContext context, PBXProjectManager projectManager)
        {
            var buildParams = context.BuildChannels.Get<BuildAnyThinkParams>();
            if (!buildParams.Enable)
                return;

            if (buildParams.IncludeAdMob)
            {
                projectManager.PlistDocument.SetPlistValue("GADApplicationIdentifier", buildParams.AdMobIOSApplicationId);
            }
            
            var target = projectManager.UnityFramework;
            
            projectManager.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
            projectManager.SetBuildProperty(target, "GCC_ENABLE_OBJC_EXCEPTIONS", "YES");
            projectManager.SetBuildProperty(target, "GCC_C_LANGUAGE_STANDARD", "gnu99");

            projectManager.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");
            projectManager.AddBuildProperty(target, "OTHER_LDFLAGS", "-fobjc-arc");
            
#if UNITY_IOS_API
            var pbxProject = projectManager.Project;
            pbxProject.AddFileToBuild(target, pbxProject.AddFile("usr/lib/libxml2.tbd", "Libraries/libxml2.tbd", UnityEditor.iOS.Xcode.PBXSourceTree.Sdk));
            pbxProject.AddFileToBuild(target, pbxProject.AddFile("usr/lib/libresolv.9.tbd", "Libraries/libresolv.9.tbd", UnityEditor.iOS.Xcode.PBXSourceTree.Sdk));
#if UNITY_2019_3_OR_NEWER
            pbxProject.AddFileToBuild(target, pbxProject.AddFile("Frameworks/AnyThinkAds/Plugins/iOS/Core/AnyThinkSDK.bundle", "Frameworks/AnyThinkAds/Plugins/iOS/Core/AnyThinkSDK.bundle", UnityEditor.iOS.Xcode.PBXSourceTree.Sdk));
#else
#endif
#endif
        }
    }
}