#if ENABLE_ANYTHINK
using System;
using System.Collections.Generic;
using AnyThinkAds.Api;
using PluginSet.Core;
using UnityEngine;
using Logger = PluginSet.Core.Logger;

namespace PluginSet.AnyThink
{
    [PluginRegister]
    public class PluginAnyThink : PluginBase, IPrivacyAuthorizationCallback, IBannerAdPlugin, IOpenAdPlugin, IRewardAdPlugin, IInterstitialAdPlugin, ATSDKInitListener
    {
        private static readonly Logger Logger = LoggerManager.GetLogger("AnyThink");
        private static readonly Dictionary<string, object> EmptyJsonMap = new Dictionary<string, object>();
        public override string Name => "AnyThink";

        public int StartOrder => PluginsStartOrder.SdkDefault;
        public bool IsRunning { get; private set; }

        private bool _inited = false;

        private string _appId;
        private string _appKey;
        private string _bannerAdUnitId;
        private string _rewardAdUnitId;
        private string _interstitialAdUnitId;
        private string _openAdUnitId;
        
        private Action _onRewardAdLoadedSuccess;
        private Action<int> _onRewardAdLoadedFail;
        private Action<bool, int> _onRewardCallback;
        private AdInfo _loadedRewardAdInfo;
        
        private bool _isLoadingRewardAd;
        private bool _isShowingRewardAd;
        
        private Action _onInterstitialAdLoadedSuccess;
        private Action<int> _onInterstitialAdLoadedFail;
        private Action<bool, int> _onInterstitialCallback;
        private AdInfo _loadedInterstitialAdInfo;
        
        private bool _isLoadingInterstitialAd;
        private bool _isShowingInterstitialAd;
        
        
        private bool _isLoadingOpenAd;
        private bool _isShowingOpenAd;

        private Action _onOpenAdLoadedSuccess;
        private Action<int> _onOpenAdLoadedFail;
        private Action<bool, int> _onOpenAdCallback;
        private AdInfo _loadedOpenAdInfo;
        
        protected override void Init(PluginSetConfig config)
        {
            var cfg = config.Get<PluginAnyThinkConfig>();
            _appId = cfg.AppId;
            _appKey = cfg.AppKey;
            _bannerAdUnitId = cfg.BannerAdUnitId;
            _rewardAdUnitId = cfg.RewardAdUnitId;
            _interstitialAdUnitId = cfg.InterstitialAdUnitId;
            
            _openAdUnitId = cfg.OpenAdUnitId;
        }

        public void OnPrivacyAuthorization()
        {
            ATSDKAPI.setChannel(PluginsManager.Instance.GetChannelName());
            ATSDKAPI.setSubChannel(PluginsManager.Instance.GetChannelId().ToString());
            
            ATSDKAPI.setLogDebug(true);
            
            ATSDKAPI.initSDK(_appId, _appKey, this);
        }

        public void initSuccess()
        {
            _inited = true;

            ATBannerAd.Instance.client.onAdLoadEvent += OnBannerAdLoaded;

            ATRewardedVideo.Instance.client.onAdLoadEvent += OnRewardedAdLoadedEvent;
            ATRewardedVideo.Instance.client.onAdLoadFailureEvent += OnRewardedAdLoadFailedEvent;
            ATRewardedVideo.Instance.client.onAdVideoStartEvent += OnRewardedAdDisplayedEvent;
            // ATRewardedVideo.Instance.client.onAdVideoEndEvent += OnRewardedAdHiddenEvent;
            ATRewardedVideo.Instance.client.onAdVideoFailureEvent += OnRewardedAdFailedToDisplayEvent;
            ATRewardedVideo.Instance.client.onAdClickEvent += OnRewardedAdClickedEvent;
            ATRewardedVideo.Instance.client.onRewardEvent += OnRewardedAdReceivedRewardEvent;
            ATRewardedVideo.Instance.client.onAdVideoCloseEvent += OnRewardedAdHiddenEvent;

            ATInterstitialAd.Instance.client.onAdLoadEvent += OnInterstitialLoadedEvent;
            ATInterstitialAd.Instance.client.onAdLoadFailureEvent += OnInterstitialLoadFailedEvent;
            ATInterstitialAd.Instance.client.onAdShowEvent += OnInterstitialDisplayedEvent;
            ATInterstitialAd.Instance.client.onAdClickEvent += OnInterstitialClickedEvent;
            ATInterstitialAd.Instance.client.onAdCloseEvent += OnInterstitialHiddenEvent;
            ATInterstitialAd.Instance.client.onAdShowFailureEvent += OnInterstitialAdFailedToDisplayEvent;

            ATSplashAd.Instance.client.onAdLoadEvent += OnOpenAdLoadedEvent;
            ATSplashAd.Instance.client.onAdLoadFailureEvent += OnOpenAdLoadFailedEvent;
            ATSplashAd.Instance.client.onAdLoadTimeoutEvent += OnOpenAdLoadTimeoutEvent;
            ATSplashAd.Instance.client.onAdShowEvent += OnOpenAdDisplayedEvent;
            ATSplashAd.Instance.client.onAdCloseEvent += OnOpenAdHiddenEvent;
            ATSplashAd.Instance.client.onAdClickEvent += OnOpenAdClickedEvent;
            
            Logger.Debug($"SDK initialized");
        }

        public void initFail(string message)
        {
            Logger.Warn($"SDK Failed ::: {message}");
        }

#region Banner Ad
        public bool IsEnableShowBanner => _inited;
        
        private List<string> _showingBannerAdIds = new List<string>();

        public void ShowBannerAd(string adId, BannerPosition position = BannerPosition.BottomCenter, Dictionary<string, object> extensions = null)
        {
            if (string.IsNullOrEmpty(adId))
                adId = _bannerAdUnitId;

            if (string.IsNullOrEmpty(adId))
                return;

            if (_showingBannerAdIds.Contains(adId))
                return;
            
            _showingBannerAdIds.Add(adId);

            var map = new Dictionary<string, object>();
            ATSize bannerSize = new ATSize(Screen.width, 100, true);
            map.Add(ATBannerAdLoadingExtra.kATBannerAdLoadingExtraBannerAdSizeStruct, bannerSize);
            
            //v5.6.5新增，只针对Admob的自适应Banner
            map.Add(ATBannerAdLoadingExtra.kATBannerAdLoadingExtraInlineAdaptiveWidth, bannerSize.width);
            map.Add(ATBannerAdLoadingExtra.kATBannerAdLoadingExtraInlineAdaptiveOrientation, ATBannerAdLoadingExtra.kATBannerAdLoadingExtraInlineAdaptiveOrientationCurrent);

            ATBannerAd.Instance.loadBannerAd(adId, map);
        }

        private void OnBannerAdLoaded(object sender, ATAdEventArgs e)
        {
            foreach (var id in _showingBannerAdIds)
            {
                ATBannerAd.Instance.showBannerAd(id, ATBannerAdLoadingExtra.kATBannerAdShowingPisitionBottom);
            }
        }

        public void HideBannerAd(string adId)
        {
            if (string.IsNullOrEmpty(adId))
                adId = _bannerAdUnitId;
            
            if (string.IsNullOrEmpty(adId))
                return;
            
            ATBannerAd.Instance.cleanBannerAd(adId);
            
            if (_showingBannerAdIds.Contains(adId))
                _showingBannerAdIds.Remove(adId);
        }

        public void HideAllBanners()
        {
            foreach (var id in _showingBannerAdIds)
            {
                ATBannerAd.Instance.cleanBannerAd(id);
            }
            _showingBannerAdIds.Clear();
        }
#endregion

#region Reward Ad
        public bool IsEnableShowRewardAd => _inited;
        public bool IsReadyToShowRewardAd => _inited && ATRewardedVideo.Instance.hasAdReady(_rewardAdUnitId);
        public void LoadRewardAd(Action success = null, Action<int> fail = null)
        {
            if (!_inited)
            {
                fail?.Invoke(PluginConstants.InvalidCode);
                return;
            }
            
            if (IsReadyToShowRewardAd)
            {
                success?.Invoke();
                return;
            }
            
            if (_isLoadingRewardAd)
            {
                fail?.Invoke((int)AdErrorCode.IsLoading);
                return;
            }
            
            if (_isShowingRewardAd)
            {
                fail?.Invoke((int)AdErrorCode.IsShowing);
                return;
            }

            if (string.IsNullOrEmpty(_rewardAdUnitId))
            {
                fail?.Invoke((int)AdErrorCode.NotLoaded);
                return;
            }
            
            _onRewardAdLoadedSuccess = success;
            _onRewardAdLoadedFail = fail;
            _isLoadingRewardAd = true;
            
            Logger.Debug($"Load Reward Ad {_rewardAdUnitId}");
            
            Dictionary<string,string> jsonmap = new Dictionary<string,string>();
            //如果需要通过开发者的服务器进行奖励的下发（部分广告平台支持此服务器激励），则需要传递下面两个key
            //ATConst.USERID_KEY必传，用于标识每个用户;ATConst.USER_EXTRA_DATA为可选参数，传入后将透传到开发者的服务器
            // jsonmap.Add(ATConst.USERID_KEY, "test_user_id");
            // jsonmap.Add(ATConst.USER_EXTRA_DATA, "test_user_extra_data");

            ATRewardedVideo.Instance.loadVideoAd(_rewardAdUnitId,jsonmap);
        }

        public void ShowRewardAd(Action<bool, int> dismiss = null)
        {
            if (!_inited)
            {
                dismiss?.Invoke(false, PluginConstants.InvalidCode);
                return;
            }
            
            if (!IsReadyToShowRewardAd)
            {
                dismiss?.Invoke(false, (int)AdErrorCode.NotLoaded);
                return;
            }
            
            if (_isShowingRewardAd)
            {
                dismiss?.Invoke(false, (int)AdErrorCode.IsShowing);
                return;
            }
            
            _isShowingRewardAd = true;
            _onRewardCallback = dismiss;
            
            Logger.Debug($"Show Reward Ad {_rewardAdUnitId}");
            ATRewardedVideo.Instance.showAd(_rewardAdUnitId);
        }

        public AdInfo GetLoadedRewardAdInfo()
        {
            return _loadedRewardAdInfo;
        }

        #endregion

#region Interstitial Ad
        public bool IsEnableShowInterstitialAd => _inited;
        public bool IsReadyToShowInterstitialAd => _inited && ATInterstitialAd.Instance.client.hasInterstitialAdReady(_interstitialAdUnitId);
        public void LoadInterstitialAd(Action success = null, Action<int> fail = null)
        {
            if (!_inited)
            {
                fail?.Invoke(PluginConstants.InvalidCode);
                return;
            }
            
            if (IsReadyToShowInterstitialAd)
            {
                success?.Invoke();
                return;
            }
            
            if (_isLoadingInterstitialAd)
            {
                fail?.Invoke((int)AdErrorCode.IsLoading);
                return;
            }
            
            if (_isShowingInterstitialAd)
            {
                fail?.Invoke((int)AdErrorCode.IsShowing);
                return;
            }

            if (string.IsNullOrEmpty(_interstitialAdUnitId))
            {
                fail?.Invoke((int)AdErrorCode.NotLoaded);
                return;
            }
            
            _onInterstitialAdLoadedSuccess = success;
            _onInterstitialAdLoadedFail = fail;
            _isLoadingInterstitialAd = true;
            
            Logger.Debug($"Load Interstitial Ad {_interstitialAdUnitId}");
            
            ATInterstitialAd.Instance.loadInterstitialAd(_interstitialAdUnitId, EmptyJsonMap);
        }

        public void ShowInterstitialAd(Action<bool, int> dismiss = null)
        {
            if (!_inited)
            {
                dismiss?.Invoke(false, PluginConstants.InvalidCode);
                return;
            }
            
            if (!IsReadyToShowInterstitialAd)
            {
                dismiss?.Invoke(false, (int)AdErrorCode.NotLoaded);
                return;
            }
            
            if (_isShowingInterstitialAd)
            {
                dismiss?.Invoke(false, (int)AdErrorCode.IsShowing);
                return;
            }
            
            _isShowingInterstitialAd = true;
            _onInterstitialCallback = dismiss;
            
            Logger.Debug($"Show Interstitial Ad {_interstitialAdUnitId}");
            ATInterstitialAd.Instance.client.showInterstitialAd(_interstitialAdUnitId, null);
        }

        public AdInfo GetLoadedInterstitialAdInfo()
        {
            return _loadedInterstitialAdInfo;
        }

        #endregion

#region open ad

        public bool IsEnableShowOpenAd => _inited;
        public bool IsReadyToShowOpenAd => ATSplashAd.Instance.client.hasSplashAdReady(_openAdUnitId);
        public void LoadOpenAd(Action success = null, Action<int> fail = null, Dictionary<string, object> extParams = null)
        {
            if (!_inited)
            {
                fail?.Invoke(PluginConstants.InvalidCode);
                return;
            }
            
            if (IsReadyToShowOpenAd)
            {
                success?.Invoke();
                return;
            }
            
            if (_isLoadingOpenAd)
            {
                fail?.Invoke((int)AdErrorCode.IsLoading);
                return;
            }
            
            if (_isShowingOpenAd)
            {
                fail?.Invoke((int)AdErrorCode.IsShowing);
                return;
            }

            if (string.IsNullOrEmpty(_openAdUnitId))
            {
                fail?.Invoke((int)AdErrorCode.NotLoaded);
                return;
            }
            
            _onOpenAdLoadedSuccess = success;
            _onOpenAdLoadedFail = fail;
            _isLoadingOpenAd = true;
            
            Logger.Debug($"Load open Ad {_openAdUnitId}");
            
            ATSplashAd.Instance.loadSplashAd(_openAdUnitId, EmptyJsonMap);
        }

        public void ShowOpenAd(Action<bool, int> dismiss = null)
        {
            if (!_inited)
            {
                dismiss?.Invoke(false, PluginConstants.InvalidCode);
                return;
            }
            
            if (!IsReadyToShowOpenAd)
            {
                dismiss?.Invoke(false, (int)AdErrorCode.NotLoaded);
                return;
            }
            
            if (_isShowingOpenAd)
            {
                dismiss?.Invoke(false, (int)AdErrorCode.IsShowing);
                return;
            }
            
            _isShowingOpenAd = true;
            _onOpenAdCallback = dismiss;
            
            Logger.Debug($"Show AppOpen Ad {_openAdUnitId}");
            ATSplashAd.Instance.showSplashAd(_openAdUnitId, null);
        }

        public AdInfo GetLoadedOpenAdInfo()
        {
            return _loadedOpenAdInfo;
        }

        #endregion

        private void OnRewardedAdLoadedEvent(object sender, ATAdEventArgs atAdEventArgs)
        {
            Logger.Debug($"[REWARDED AD] Loaded, adUnitId = {atAdEventArgs.placementId}");

            _loadedRewardAdInfo = new AdInfo()
            {
                AdUnitId = atAdEventArgs.placementId,
                Plugin = Name,
                Revenue = atAdEventArgs.callbackInfo.publisher_revenue,
                RevenuePrecision = atAdEventArgs.callbackInfo.currency
            };
            
            _isLoadingRewardAd = false;
            var callback = _onRewardAdLoadedSuccess;
            _onRewardAdLoadedSuccess = null;
            _onRewardAdLoadedFail = null;
            callback?.Invoke();
        }

        private void OnRewardedAdLoadFailedEvent(object sender, ATAdErrorEventArgs atAdErrorEventArgs)
        {
            Logger.Debug($"[REWARDED AD] Failed to load ({atAdErrorEventArgs.errorMessage}), adUnitId = {atAdErrorEventArgs.placementId}");
            
            _isLoadingRewardAd = false;
            var callback = _onRewardAdLoadedFail;
            _onRewardAdLoadedSuccess = null;
            _onRewardAdLoadedFail = null;
            callback?.Invoke((int)AdErrorCode.NotLoaded);
        }

        private void OnRewardedAdDisplayedEvent(object sender, ATAdEventArgs atAdEventArgs)
        {
            Logger.Debug($"[REWARDED AD] Displayed: {atAdEventArgs.placementId}");
        }

        private void OnRewardedAdClickedEvent(object sender, ATAdEventArgs atAdEventArgs)
        { }

        private void OnRewardedAdHiddenEvent(object sender, ATAdRewardEventArgs atAdRewardEventArgs)
        {
            Logger.Debug($"[REWARDED AD] Dismissed, adUnitId = {atAdRewardEventArgs.placementId}");
            
            _isShowingRewardAd = false;
            HandRewardCallback(false, (int)AdErrorCode.Dismissed);
        }

        private void OnRewardedAdFailedToDisplayEvent(object sender, ATAdErrorEventArgs atAdErrorEventArgs)
        {
            Logger.Debug($"[REWARDED AD] Failed to display, adUnitId = {atAdErrorEventArgs.placementId} :: {atAdErrorEventArgs.errorMessage}");
            
            _isShowingRewardAd = false;
            HandRewardCallback(false, (int)AdErrorCode.ShowFail);
        }

        private void OnRewardedAdReceivedRewardEvent(object sender, ATAdEventArgs atAdEventArgs)
        {
            Logger.Debug($"[REWARDED AD] Received reward, adUnitId = {atAdEventArgs.placementId}");
            HandRewardCallback(true, (int)AdErrorCode.Success);
        }
        
        private void HandRewardCallback(bool result, int code)
        {
            var callback = _onRewardCallback;
            _onRewardCallback = null;
            callback?.Invoke(result, code);
        }

        private void OnInterstitialLoadedEvent(object sender, ATAdEventArgs atAdEventArgs)
        {
            Logger.Debug($"[INTERSTITIAL AD] Loaded, adUnitId = {atAdEventArgs.placementId}");

            _loadedInterstitialAdInfo = new AdInfo()
            {
                AdUnitId = atAdEventArgs.placementId,
                Plugin = Name,
                Revenue = atAdEventArgs.callbackInfo.publisher_revenue,
                RevenuePrecision = atAdEventArgs.callbackInfo.currency
            };
            
            _isLoadingInterstitialAd = false;
            var callback = _onInterstitialAdLoadedSuccess;
            _onInterstitialAdLoadedSuccess = null;
            _onInterstitialAdLoadedFail = null;
            callback?.Invoke();
        }

        private void OnInterstitialLoadFailedEvent(object sender, ATAdErrorEventArgs atAdErrorEventArgs)
        {
            Logger.Debug($"[INTERSTITIAL AD] Failed to load, adUnitId = {atAdErrorEventArgs.placementId} ::: {atAdErrorEventArgs.errorMessage}");
            
            _isLoadingInterstitialAd = false;
            var callback = _onInterstitialAdLoadedFail;
            _onInterstitialAdLoadedSuccess = null;
            _onInterstitialAdLoadedFail = null;
            callback?.Invoke((int)AdErrorCode.NotLoaded);
        }

        private void OnInterstitialDisplayedEvent(object sender, ATAdEventArgs atAdEventArgs)
        {
            Logger.Debug($"[INTERSTITIAL AD] Displayed {atAdEventArgs.placementId}");
        }

        private void OnInterstitialClickedEvent(object sender, ATAdEventArgs atAdEventArgs)
        { }

        private void OnInterstitialHiddenEvent(object sender, ATAdEventArgs atAdEventArgs)
        {
            Logger.Debug($"[INTERSTITIAL AD] Dismissed, adUnitId = {atAdEventArgs.placementId}");
            
            _isShowingInterstitialAd = false;
            if (_onInterstitialCallback == null) return;
            
            _onInterstitialCallback.Invoke(true, (int)AdErrorCode.Success);
            _onInterstitialCallback = null;
        }

        private void OnInterstitialAdFailedToDisplayEvent(object sender, ATAdErrorEventArgs atAdErrorEventArgs)
        {
            Logger.Debug($"[INTERSTITIAL AD] Failed to display, adUnitId = {atAdErrorEventArgs.placementId} ::: {atAdErrorEventArgs.errorMessage}");
            
            _isShowingInterstitialAd = false;
            if (_onInterstitialCallback == null) return;

            _onInterstitialCallback.Invoke(false, (int)AdErrorCode.ShowFail);
            _onInterstitialCallback = null;
        }

        private void OnOpenAdLoadedEvent(object sender, ATAdEventArgs atAdEventArgs)
        {
            Logger.Debug($"[OPEN AD] Loaded, adUnitId = {atAdEventArgs.placementId}");

            _loadedOpenAdInfo = new AdInfo()
            {
                AdUnitId = atAdEventArgs.placementId,
                Plugin = Name,
                Revenue = atAdEventArgs.callbackInfo.publisher_revenue,
                RevenuePrecision = atAdEventArgs.callbackInfo.currency,
            };
            _isLoadingOpenAd = false;
            var callback = _onOpenAdLoadedSuccess;
            _onOpenAdLoadedSuccess = null;
            _onOpenAdLoadedFail = null;
            callback?.Invoke();
        }

        private void OnOpenAdLoadFailedEvent(object sender, ATAdErrorEventArgs atAdErrorEventArgs)
        {
            Logger.Debug($"[OPEN AD] Failed to load, adUnitId = {atAdErrorEventArgs.placementId}::: {atAdErrorEventArgs.errorMessage}");
            
            _isLoadingOpenAd = false;
            var callback = _onOpenAdLoadedFail;
            _onOpenAdLoadedSuccess = null;
            _onOpenAdLoadedFail = null;
            callback?.Invoke((int)AdErrorCode.NotLoaded);
        }
        

        private void OnOpenAdLoadTimeoutEvent(object sender, ATAdEventArgs e)
        {
            Logger.Debug($"[OPEN AD] Failed to load, adUnitId = {e.placementId}::: time out");
            
            _isLoadingOpenAd = false;
            var callback = _onOpenAdLoadedFail;
            _onOpenAdLoadedSuccess = null;
            _onOpenAdLoadedFail = null;
            callback?.Invoke((int)AdErrorCode.NotLoaded);
        }

        private void OnOpenAdDisplayedEvent(object sender, ATAdEventArgs atAdEventArgs)
        {
            Logger.Debug($"[OPEN AD] Displayed {atAdEventArgs.placementId}");
        }

        private void OnOpenAdClickedEvent(object sender, ATAdEventArgs atAdEventArgs)
        {
        }

        private void OnOpenAdHiddenEvent(object sender, ATAdEventArgs atAdEventArgs)
        {
            Logger.Debug($"[OPEN AD] Dismissed, adUnitId = {atAdEventArgs.placementId}");
            
            _isShowingOpenAd = false;
            if (_onOpenAdCallback == null) return;
            
            _onOpenAdCallback.Invoke(true, (int)AdErrorCode.Success);
            _onOpenAdCallback = null;
        }
    }
}
#endif