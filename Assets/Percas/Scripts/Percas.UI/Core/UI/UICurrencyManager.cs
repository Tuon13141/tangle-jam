using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using Percas.Data;

namespace Percas.UI
{
    public class UICurrencyManager : MonoBehaviour
    {
        [SerializeField] CanvasGroup m_overlay;

        [SerializeField] RectTransform rtBalances;
        [SerializeField] RectTransform balanceCoin, balanceLive;
        [SerializeField] Vector2 balanceCoinPos, balanceLivePos;
        [SerializeField] RectTransform rtTargetPosCoin, rtTargetPosButtonPlay, rtTargetPosButtonFill, rtTargetPosLive, rtTargetPosPin, rtTargetPosPictureWithAds, rtTargetPosPictureNoAds, rtRewardGain, rtRewardValue;
        [SerializeField] ButtonBase buttonCheat;
        [SerializeField] GameObject blockMask;
        [Header("Reward Gain Prefabs")]
        [SerializeField] GameObject coinGainPrefab;
        [SerializeField] GameObject coilGainPrefab;
        [SerializeField] GameObject liveGainPrefab;
        [SerializeField] GameObject pinGainPrefab;
        [SerializeField] GameObject pictureGainPrefab;
        [SerializeField] GameObject boosterUndoGainPrefab, boosterAddSlotsGainPrefab, boosterClearGainPrefab;
        [SerializeField] GameObject rewardValuePrefab;

        public static Action<bool> OnShowBlockMask;
        public static Action<float, Action> OnShowMaskWithAutoClose;
        //public static Action<bool, bool, bool> OnShowBalance;
        public static Action<int> OnShowCoinGain;
        public static Action<int> OnShowCoilGain;
        public static Action<int> OnShowPinGain;
        public static Action<BoosterType, int> OnShowBoosterGain;
        public static Action<bool, int> OnShowLiveGain;
        public static Action<bool, int, Action> OnShowPictureGain;
        public static Action<int, Vector3> OnShowValue;

        public static bool BlockBalanceButton { get; set; }

        private readonly List<Vector2> CoinPositions = new()
        {
            new Vector2(1,0),
            new Vector2(0,1),
            new Vector2(-1,0),
            new Vector2(1,1),
            new Vector2(1,-1),
            new Vector2(0,-1),
            new Vector2(-1,-1),
            new Vector2(-1,1)
        };

        private void Awake()
        {
            OnShowBlockMask += ShowBlockMask;
            OnShowMaskWithAutoClose += ShowMaskWithAutoClose;
            //OnShowBalance += ShowBalance;
            OnShowCoinGain += ShowCoinGain;
            OnShowCoilGain += ShowCoilGain;
            OnShowPinGain += ShowPinGain;
            OnShowBoosterGain += ShowBoosterGain;
            OnShowLiveGain += ShowLiveGain;
            OnShowPictureGain += ShowPictureGain;
            OnShowValue += SpawnRewardValue;

            HandleButtonCheat();
        }

        private void OnDestroy()
        {
            OnShowBlockMask -= ShowBlockMask;
            OnShowMaskWithAutoClose -= ShowMaskWithAutoClose;
            //OnShowBalance -= ShowBalance;
            OnShowCoinGain -= ShowCoinGain;
            OnShowCoilGain -= ShowCoilGain;
            OnShowPinGain -= ShowPinGain;
            OnShowBoosterGain -= ShowBoosterGain;
            OnShowLiveGain -= ShowLiveGain;
            OnShowPictureGain -= ShowPictureGain;
            OnShowValue -= SpawnRewardValue;
        }

        private void Start()
        {
            float bannerGap = GameLogic.IsNoAds ? 176f : 0f;
            rtTargetPosButtonPlay.anchoredPosition = new Vector2(rtTargetPosButtonPlay.anchoredPosition.x, rtTargetPosButtonPlay.anchoredPosition.y - bannerGap);
        }

        private void HandleButtonCheat()
        {
            if (!GameConfig.Instance.CheatOn)
            {
                buttonCheat.gameObject.SetActive(false);
            }
            else
            {
                buttonCheat.gameObject.SetActive(true);
                buttonCheat.SetPointerClickEvent(OpenCheat);
            }
        }

        private void OpenCheat()
        {
            ServiceLocator.PopupScene.ShowPopup(PopupName.Cheat);
        }

        //private void ShowBalance(bool value, bool showCoin, bool showLive)
        //{
        //    rtBalances.gameObject.SetActive(value);
        //    balanceCoin.gameObject.SetActive(showCoin);
        //    if (showCoin) balanceCoin.anchoredPosition = balanceCoinPos;
        //    balanceLive.gameObject.SetActive(showLive && GameLogic.CurrentLevel >= GameLogic.LevelUnlockHome);
        //    if (showLive) balanceLive.anchoredPosition = balanceLivePos;
        //}

        private void ShowBlockMask(bool value)
        {
            blockMask.SetActive(value);
        }

        private void ShowMaskWithAutoClose(float timeToClose, Action onCompleted)
        {
            blockMask.SetActive(true);
            StartCoroutine(OnCloseMaskWithDelay(timeToClose, onCompleted));
        }

        private IEnumerator OnCloseMaskWithDelay(float timeToClose, Action onCompleted)
        {
            yield return new WaitForSeconds(timeToClose);
            blockMask.SetActive(false);
            onCompleted?.Invoke();
        }

        #region EFFECTS
        bool isFading = false;
        private void ShowOverlay()
        {
            if (isFading) return;
            m_overlay.gameObject.SetActive(true);
        }

        private void HideOverlay()
        {
            isFading = true;
            m_overlay.DOFade(0, 0.5f).OnComplete(() =>
            {
                m_overlay.gameObject.SetActive(false);
                m_overlay.alpha = 1;
                isFading = false;
            });
        }

        private void SpawnRewardValue(int amount, Vector3 target)
        {
            GameObject valueObject = SimplePool.Spawn(rewardValuePrefab, Vector3.zero, Quaternion.identity);
            valueObject.transform.SetParent(rtRewardValue);
            UIRewardValue rewardValue = valueObject.GetComponent<UIRewardValue>();
            rewardValue.Show();
            rewardValue.SetPosition(target);
            rewardValue.Display(amount);
        }
        #endregion

        #region GAIN COINS
        private float Distance => Random.Range(60f, 80f);

        private bool isDynamicCoinBalance = false;
        private void ShowCoinGain(int valueToEarn)
        {
            // [HardCode]
            if (/*!rtBalances.gameObject.activeSelf
                    || (rtBalances.gameObject.activeSelf && !balanceCoin.gameObject.activeSelf)
                    || (rtBalances.gameObject.activeSelf && balanceCoin.gameObject.activeSelf && (balanceCoin.anchoredPosition.x == 380 || balanceCoin.anchoredPosition.y == 500))
                    || */GameLogic.IsInGame && GameLogic.CurrentLevel < 3)
            {
                balanceCoin.anchoredPosition = new Vector2(380, 0);
                balanceCoin.gameObject.SetActive(true);
                balanceCoin.DOAnchorPosX(-120, 0.5f).SetDelay(0).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    isDynamicCoinBalance = true;
                });
            }
            StartCoroutine(StartGainCoin(valueToEarn));
            AudioController.Instance.PlaySpawnCoins();
        }

        private Vector3 GetCoinPosition(int num)
        {
            Vector2 tmp = CoinPositions[num];
            return tmp.normalized * Distance;
        }

        private IEnumerator StartGainCoin(int valueToEarn)
        {
            ShowOverlay();

            List<UICoinGain> coinList = new();

            for (int i = 0; i < 8; i++)
            {
                GameObject coinObject = SimplePool.Spawn(coinGainPrefab, Vector3.zero, Quaternion.identity);
                UICoinGain coinGain = coinObject.GetComponent<UICoinGain>();
                coinList.Add(coinGain);
                coinGain.transform.SetParent(rtRewardGain);
                coinGain.Show();
                coinGain.SetPosition(Vector3.zero);
                coinGain.transform.DOMove(coinGain.transform.position + GetCoinPosition(i) * 1.2f, 0.2f);
            }

            SpawnRewardValue(valueToEarn, Vector3.zero);

            yield return new WaitForSeconds(0.2f);

            bool moveCompleted = false;
            int coinIndex = 0;
            foreach (UICoinGain coin in coinList)
            {
                coinIndex += 1;
                coin.Movement(rtTargetPosCoin.position, () =>
                {
                    coin.Hide();
                    AudioController.Instance.PlayEarnCoins();
                    AudioController.Instance.PlayVibration(HapticType.Light, 5);
                    if (coinIndex >= coinList.Count)
                    {
                        HideOverlay();
                        moveCompleted = true;
                        UICoinBalance.OnScaleLoop?.Invoke();
                        Helpers.ChangeValueInt(GameLogic.CurrentCoin - valueToEarn, GameLogic.CurrentCoin, 0.5f, 0.0f, (value) =>
                        {
                            UICoinBalance.OnUpdateUI?.Invoke(value);
                        });
                    }
                });
                yield return new WaitForSeconds(0.05f);
            }

            if (isDynamicCoinBalance)
            {
                yield return new WaitUntil(() => moveCompleted);
                balanceCoin.DOAnchorPosX(380, 0.5f).SetDelay(1.5f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    isDynamicCoinBalance = false;
                    balanceCoin.gameObject.SetActive(false);
                });
            }
        }
        #endregion

        #region GAIN COILS
        private void ShowCoilGain(int valueToEarn)
        {
            StartCoroutine(StartGainCoil(valueToEarn));
            AudioController.Instance.PlaySpawnCoins();
        }

        private IEnumerator StartGainCoil(int valueToEarn)
        {
            ShowOverlay();
            RectTransform targetPos = (GameLogic.OutOfRoom || GameLogic.TotalCoil <= 0) ? rtTargetPosButtonPlay : rtTargetPosButtonFill;
            GameObject coilObject = SimplePool.Spawn(coilGainPrefab, Vector3.zero, Quaternion.identity);
            UICoilGain coilGain = coilObject.GetComponent<UICoilGain>();
            coilGain.transform.SetParent(rtRewardGain);
            coilGain.Show();
            coilGain.SetPosition(new Vector3(0, 200, 0));
            coilGain.Movement(targetPos.position, () =>
            {
                HideOverlay();
                coilGain.Hide();
                GameLogic.CoilEarned = valueToEarn;
                ButtonPlay.OnScaleLoop?.Invoke();
                ButtonBuild.OnUpdateUI?.Invoke();
                AudioController.Instance.PlayEarnCoins();
                AudioController.Instance.PlayVibration(HapticType.Medium, 20);
            });
            SpawnRewardValue(valueToEarn, new Vector3(0, 200, 0));
            yield return new WaitForSeconds(0.05f);
            UIHomeController.OnDisplay?.Invoke(false, false);
        }
        #endregion

        #region GAIN PINS
        private void ShowPinGain(int valueToEarn)
        {
            StartCoroutine(StartGainPin(valueToEarn));
            AudioController.Instance.PlaySpawnCoins();
        }

        private IEnumerator StartGainPin(int valueToEarn)
        {
            ShowOverlay();
            GameObject pinObject = SimplePool.Spawn(pinGainPrefab, Vector3.zero, Quaternion.identity);
            UIPinGain pinGain = pinObject.GetComponent<UIPinGain>();
            pinGain.transform.SetParent(rtRewardGain);
            pinGain.Show();
            pinGain.SetPosition(Vector3.zero);
            pinGain.Movement(rtTargetPosPin.position, () =>
            {
                HideOverlay();
                pinGain.Hide();
                ButtonLuxuryBasket.OnScaleLoop?.Invoke();
                ButtonLuxuryBasket.OnUpdateNoti?.Invoke();
                ButtonLuxuryBasket.OnUpdateButtonText?.Invoke();
                AudioController.Instance.PlayEarnCoins();
                AudioController.Instance.PlayVibration(HapticType.Medium, 20);
            });
            SpawnRewardValue(valueToEarn, Vector3.zero);
            yield return null;
        }
        #endregion

        #region GAIN LIVE
        private void ShowLiveGain(bool isInfinite, int valueToEarn)
        {
            StartCoroutine(StartGainLive(isInfinite, valueToEarn));
            AudioController.Instance.PlaySpawnCoins();
        }

        private IEnumerator StartGainLive(bool isInfinite, int valueToEarn)
        {
            ShowOverlay();
            GameObject liveObject = SimplePool.Spawn(liveGainPrefab, Vector3.zero, Quaternion.identity);
            UILiveGain liveGain = liveObject.GetComponent<UILiveGain>();
            liveGain.transform.SetParent(rtRewardGain);
            if (isInfinite) liveGain.SetInfinite();
            liveGain.Init();
            liveGain.Show();
            liveGain.SetPosition(Vector3.zero);
            liveGain.Movement(rtTargetPosLive.position, () =>
            {
                HideOverlay();
                liveGain.Hide();
                UILiveBalance.OnScaleLoop?.Invoke();
                AudioController.Instance.PlayEarnCoins();
                AudioController.Instance.PlayVibration(HapticType.Medium, 20);
            });
            if (!isInfinite) SpawnRewardValue(valueToEarn, Vector3.zero);
            yield return new WaitForSeconds(0.05f);
        }
        #endregion

        #region GAIN BOOSTER
        private void ShowBoosterGain(BoosterType boosterType, int valueToEarn)
        {
            StartCoroutine(StartGainBooster(boosterType, valueToEarn));
            AudioController.Instance.PlaySpawnCoins();
        }

        private IEnumerator StartGainBooster(BoosterType boosterType, int valueToEarn)
        {
            ShowOverlay();
            GameObject boosterObject = boosterType switch
            {
                BoosterType.AddSlots => SimplePool.Spawn(boosterAddSlotsGainPrefab, Vector3.zero, Quaternion.identity),
                BoosterType.Clear => SimplePool.Spawn(boosterClearGainPrefab, Vector3.zero, Quaternion.identity),
                _ => SimplePool.Spawn(boosterUndoGainPrefab, Vector3.zero, Quaternion.identity),
            };
            UIBoosterGain boosterGain = boosterObject.GetComponent<UIBoosterGain>();
            boosterGain.transform.SetParent(rtRewardGain);
            boosterGain.Show();
            boosterGain.SetPosition(new Vector3(0, 200, 0));
            boosterGain.Movement(rtTargetPosButtonPlay.position, () =>
            {
                HideOverlay();
                boosterGain.Hide();
                ButtonPlay.OnScaleLoop?.Invoke();
                AudioController.Instance.PlayEarnCoins();
                AudioController.Instance.PlayVibration(HapticType.Medium, 20);
            });
            SpawnRewardValue(valueToEarn, new Vector3(0, 200, 0));
            yield return new WaitForSeconds(0.05f);
        }
        #endregion

        #region GAIN PICTURE
        private void ShowPictureGain(bool isHiddenPicture, int levelPicture, Action onCompleted)
        {
            StartCoroutine(StartGainPicture(isHiddenPicture, levelPicture, onCompleted));
        }

        private IEnumerator StartGainPicture(bool isHiddenPicture, int levelPicture, Action onCompleted)
        {
            Sprite sprite;
            if (isHiddenPicture)
            {
                Sprite piece = levelPicture == -1 ? DataManager.Instance.GetCurrentHiddenPicture() : DataManager.Instance.GetCurrentHiddenPicturePiece(levelPicture);
                sprite = piece != null ? piece : Static.GetSpriteLevelPicture(levelPicture);
            }
            else
            {
                sprite = Static.GetSpriteLevelPicture(levelPicture);
            }

            if (sprite == null) yield break;

            yield return new WaitForSeconds(0.5f);

            ShowOverlay();
            AudioController.Instance.PlaySpawnCoins();
            GameObject pictureObject = SimplePool.Spawn(pictureGainPrefab, Vector3.zero, Quaternion.identity);
            UIPictureGain pictureGain = pictureObject.GetComponent<UIPictureGain>();
            pictureGain.transform.SetParent(rtRewardGain);
            pictureGain.Show();
            pictureGain.SetPicture(sprite);
            pictureGain.SetPosition(new Vector3(0, 320, 0));
            pictureGain.Movement(GameLogic.IsNoAds ? rtTargetPosPictureNoAds.position : rtTargetPosPictureWithAds.position, () =>
            {
                HideOverlay();
                pictureGain.Hide();
                onCompleted?.Invoke();
                if (!PlayerDataManager.PlayerData.IntroToCollectionDemo)
                {
                    OnShowMaskWithAutoClose?.Invoke(0.25f, () =>
                    {
                        ServiceLocator.PopupScene.ShowPopup(PopupName.CollectionsDemo);
                    });
                }
                PlayerDataManager.AddPictureGain(levelPicture);
                UIHomeController.OnCollectionScale?.Invoke();
                AudioController.Instance.PlayEarnCoins();
                AudioController.Instance.PlayVibration(HapticType.Medium, 20);
            });
            yield return null;
        }
        #endregion
    }
}
