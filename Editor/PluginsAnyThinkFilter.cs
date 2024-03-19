using System;
using PluginSet.Core.Editor;
using UnityEditor;

namespace PluginSet.AnyThink.Editor
{
    [InitializeOnLoad]
    public static class PluginsAnyThinkFilter
    {
        static PluginsAnyThinkFilter()
        {
            var fileter = PluginFilter.IsBuildParamsEnable<BuildAnyThinkParams>();
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/iOS", fileter);
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android", fileter);
            
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/adcolony", Filter("IncludeAdColony"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/admob", Filter("IncludeAdMob"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/applovin", Filter("IncludeAppLovin"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/baidu", Filter("IncludeBaidu"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/bigo", Filter("IncludeBigo"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/chartboost", Filter("IncludeChartBoost"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/facebook", Filter("IncludeFacebook"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/fyber", Filter("IncludeFyber"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/gdt", Filter("IncludeGDT"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/huawei", Filter("IncludeHuawei"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/inmoby", Filter("IncludeInMobi"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/ironsource", Filter("IncludeIronSource"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/kidoz", Filter("IncludeKidoz"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/klevin", Filter("IncludKlevin"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/kuaishou", Filter("IncludeKuaishou"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/maio", Filter("IncludeMaio"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/mintegral", Filter("IncludeMintegral"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/mytarget", Filter("IncludeMyTarget"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/nend", Filter("IncludeNend"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/ogury", Filter("IncludeOgury"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/pangle", Filter("IncludePangle"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/pubnative", Filter("IncludePubNative"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/sigmob", Filter("IncludeSigmob"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/tapjoy", Filter("IncludeTapjoy"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/startapp", Filter("IncludeStartApp"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/unityads", Filter("IncludeUnityAds"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/vungle", Filter("IncludeVungle"));
            PluginFilter.RegisterFilter("com.pluginset.anythink/Plugins/Android/yandex", Filter("IncludeYandex"));
        }

        private static Func<string, BuildProcessorContext, bool> Filter(string name)
        {
            return delegate(string s, BuildProcessorContext context)
            {
                
                var buildParams = context.BuildChannels.Get<BuildAnyThinkParams>();
                if (!buildParams.Enable)
                    return true;

                var prop = buildParams.GetType().GetField(name);
                if (prop == null || !(bool)prop.GetValue(buildParams))
                    return true;

                return false;
            };
        }
    }
}