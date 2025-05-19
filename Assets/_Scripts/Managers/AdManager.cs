using System;
using UnityEngine;
using YG;

namespace Managers
{
    public sealed class AdManager : MonoBehaviour
    {
        public void Interstitial() => YG2.InterstitialAdvShow();
        public void Rewarded(string key, Action callback) => YG2.RewardedAdvShow(key, callback);
        public void SetStickyAds(bool enabled) => YG2.StickyAdActivity(enabled);
    }
}
