using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using Percas.IAR;
using Percas.Data;

namespace Percas
{
    public class LuckyWheel : MonoBehaviour, IActivatable
    {
        [SerializeField] int testRewardIndex = -1;
        [SerializeField] List<Reward> _rewards_A;
        [SerializeField] List<Reward> _rewards_B;
        [SerializeField] List<PopupLuckyWheel_RewardItem> rewardItems_A;
        [SerializeField] List<PopupLuckyWheel_RewardItem> rewardItems_B;
        [SerializeField] RectTransform _wheelSpin, _wheelPointer;

        private Vector3 _lastPosition;

        private float _initialSpinSpeed = 800f; // Initial rotation speed
        private float _decelerationSpinSpeed = 200f; // Rate at which to slow down
        private float _currentSpinSpeed; // Current rotation speed
        private float _targetRewardPos;
        private float _snapSpeed = 100f; // Speed at which to snap to target angle
        private float _decelerationSnapSpeed = 50f;

        private int _targetRewardIndex = 0;
        private int _liveLastRewardIndex = 0;
        private int _liveCurrentRewardIndex = 0;

        private bool _onPointer;
        private bool _isSpinning;
        private bool _isStopPointer;

        private Action OnSpinning;
        private Action OnSpinStopped;
        private Action OnCompleted;

        private Reward spinReward;

        private readonly float[] wheelZRotation = new float[] { 90, 135, 270, 315 };

        private readonly int[] rewardTier1 = new int[] { 1, 5 };
        private readonly int[] rewardTier2 = new int[] { 3, 7 };
        private readonly int[] rewardTier3 = new int[] { 0, 2, 4, 6 };

        private List<Reward> _rewards;
        private List<PopupLuckyWheel_RewardItem> rewardItems;

        public void Activate()
        {
            SetupRewardOption();
            SetupRewards();
            // reset the wheel & pointer position
            _wheelSpin.localEulerAngles = new Vector3(0, 0, wheelZRotation[Random.Range(0, wheelZRotation.Length)]);
            _wheelPointer.localEulerAngles = new Vector3(0, 0, 0);
        }

        public void Deactivate() { }

        private void SetupRewardOption()
        {
            if (PlayerDataManager.PlayerData.IntroToLuxuryBasket)
            {
                _rewards = _rewards_B;
                rewardItems = rewardItems_B;
                rewardItems_A[2].gameObject.SetActive(false);
                rewardItems_B[2].gameObject.SetActive(true);
            }
            else
            {
                _rewards = _rewards_A;
                rewardItems = rewardItems_A;
                rewardItems_A[2].gameObject.SetActive(true);
                rewardItems_B[2].gameObject.SetActive(false);
            }
        }

        private void SetupRewards()
        {
            int index = -1;
            foreach (Reward reward in _rewards)
            {
                index += 1;
                rewardItems[index].Init(reward);
            }
        }

        private void UpdateResult(float zPos)
        {
            _liveCurrentRewardIndex = GetRewardIndex(zPos);
            if (_liveCurrentRewardIndex != _liveLastRewardIndex)
            {
                _liveLastRewardIndex = _liveCurrentRewardIndex;
                OnStartPointerPunchRotation();
            }
            Reward reward = GetReward(_liveCurrentRewardIndex);
            spinReward = reward;
        }

        private IEnumerator SpinTheWheel()
        {
            while (_currentSpinSpeed > _snapSpeed)
            {
                _wheelSpin.Rotate(Vector3.forward, _currentSpinSpeed * Time.deltaTime);
                _currentSpinSpeed -= _decelerationSpinSpeed * Time.deltaTime;
                UpdateResult(_wheelSpin.localEulerAngles.z);
                yield return null;
            }
            // After deceleration, start snapping to the target angle smoothly
            if (_currentSpinSpeed <= _snapSpeed)
            {
                _snapSpeed = (int)_currentSpinSpeed;
                _lastPosition = _wheelSpin.localEulerAngles;
                if (!_isStopPointer) OnStopToSpinPointer();
                StartCoroutine(SnapToTargetReward(_targetRewardPos));
            }
        }

        private IEnumerator SnapToTargetReward(float finalTargetAngle)
        {
            float currentAngle = _lastPosition.z + 360; // Get current local Z angle

            // Normalize angles to be in the range [0, 360]
            currentAngle %= 360;
            finalTargetAngle = finalTargetAngle % 360 > currentAngle ? finalTargetAngle % 360 : finalTargetAngle % 360 + 360;

            while (_isSpinning && finalTargetAngle > currentAngle) // Continue until close enough to target angle
            {
                // Calculate shortest direction to rotate towards target angle
                float angleDifference = finalTargetAngle - currentAngle;

                // Determine step size based on snap speed and delta time
                float step = _snapSpeed * Time.deltaTime;
                _snapSpeed -= _decelerationSnapSpeed * Time.deltaTime;
                _snapSpeed = _snapSpeed <= _decelerationSnapSpeed * 0.5f ? _decelerationSnapSpeed * 0.5f : _snapSpeed;

                // Move towards target without rotating back
                if (Mathf.Abs(angleDifference) < step)
                {
                    currentAngle = finalTargetAngle; // Snap directly if within step distance
                }
                else
                {
                    // Rotate in the correct direction only
                    currentAngle += Mathf.Sign(angleDifference) * step;
                }

                _wheelSpin.localEulerAngles = new Vector3(0, 0, currentAngle); // Update local Z rotation

                UpdateResult(currentAngle);

                yield return null; // Wait for the next frame
            }

            // Ensure it snaps exactly to the target angle without turning back
            _wheelSpin.localEulerAngles = new Vector3(0, 0, finalTargetAngle);

            _isSpinning = false; // Reset spinning state

            _lastPosition = _wheelSpin.localEulerAngles;

            OnStop();
        }

        private float GetRewardPos(int rewardIndex)
        {
            return (float)(360 / _rewards.Count) * (rewardIndex + 1) - (float)(180 / _rewards.Count);
        }

        private Reward GetReward(int index)
        {
            return _rewards[index];
        }

        private int GetRewardIndex(float zPos)
        {
            float z = zPos % 360;
            if (z >= 0 && z < 45) return 0;
            else if (z >= 45 && z < 90) return 1;
            else if (z >= 90 && z < 135) return 2;
            else if (z >= 135 && z < 180) return 3;
            else if (z >= 180 && z < 225) return 4;
            else if (z >= 225 && z < 270) return 5;
            else if (z >= 270 && z < 315) return 6;
            return 7;
        }

        private void OnStartPointerPunchRotation()
        {
            _wheelPointer.DOPunchRotation(new Vector3(0f, 0f, -5f), 0.5f, 8, 1);
        }

        private void OnStopToSpinPointer()
        {
            //Debug.LogError($"OnStopToSpinPointer");
            _isStopPointer = true;
            _wheelPointer.DORotateQuaternion(Quaternion.Euler(0, 0, 0f), _decelerationSpinSpeed / 100f);
        }

        private void OnSpinWheel()
        {
            OnSpinning?.Invoke();
            _isSpinning = true;
            _isStopPointer = false;
            _decelerationSpinSpeed = Random.Range(150, 300); // 0 if you want the wheel to run forever
            _currentSpinSpeed = _initialSpinSpeed; // Reset speed when starting
            DetermineResult();
            _targetRewardPos = GetRewardPos(_targetRewardIndex); // target angle for stopping
            StartCoroutine(SpinTheWheel());
        }

        private void DetermineResult()
        {
            //if (LuckyWheelManager.SpinCount >= 10)
            //{
            //    Debug.LogError($"100% Getting The Largest Reward Due To SpinCount >= 10");
            //    LuckyWheelManager.OnResetSpinCount?.Invoke();
            //    _targetRewardIndex = _rewards.Count - 1;
            //    return;
            //}

            // _targetRewardIndex = 6;

            if (testRewardIndex >= 0)
            {
                _targetRewardIndex = testRewardIndex;
            }
            else
            {
                int rand = Random.Range(1, 101);
                if (rand <= 5) // 5%
                {
                    _targetRewardIndex = rewardTier1[Random.Range(0, rewardTier1.Length)];
                }
                else if (rand > 5 && rand <= 25)
                {
                    _targetRewardIndex = rewardTier2[Random.Range(0, rewardTier2.Length)];
                }
                else
                {
                    _targetRewardIndex = rewardTier3[Random.Range(0, rewardTier3.Length)];
                }
            }
        }

        private void OnStop()
        {
            // Handle what happens when the wheel stops (e.g., determine winning segment)
            UpdateResult(_lastPosition.z);
            OnSpinStopped?.Invoke();
            StartCoroutine(GetReward());
            //LuckyWheelManager.OnAddSpinCount?.Invoke();
        }

        private IEnumerator GetReward()
        {
            bool hasReward = false;
            switch (spinReward.RewardType)
            {
                case RewardType.Coin:
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoin(spinReward.RewardAmount, Vector3.zero, new LogCurrency("currency", "coin", "lucky_spin", "non_iap", "ads", "rwd_ads")));
                    hasReward = true;
                    break;

                case RewardType.Coil:
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainCoil(spinReward.RewardAmount, Vector3.zero, new LogCurrency("currency", "coil", "lucky_spin", "non_iap", "ads", "rwd_ads")));
                    hasReward = true;
                    break;

                case RewardType.InfiniteLive:
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainInfiniteLive(spinReward.RewardAmount, Vector3.zero, new LogCurrency("energy", "infinite_live", "lucky_spin", "non_iap", "ads", "rwd_ads")));
                    hasReward = true;
                    break;

                case RewardType.BoosterUndo:
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainBoosterUndo(spinReward.RewardAmount, Vector3.zero, new LogCurrency("booster", $"{BoosterType.Undo}", "lucky_spin", "non_iap", "ads", "rwd_ads")));
                    hasReward = true;
                    break;

                case RewardType.BoosterAddSlots:
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainBoosterAddSlots(spinReward.RewardAmount, Vector3.zero, new LogCurrency("booster", $"{BoosterType.AddSlots}", "lucky_spin", "non_iap", "ads", "rwd_ads")));
                    hasReward = true;
                    break;

                case RewardType.BoosterClear:
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainBoosterClear(spinReward.RewardAmount, Vector3.zero, new LogCurrency("booster", $"{BoosterType.Clear}", "lucky_spin", "non_iap", "ads", "rwd_ads")));
                    hasReward = true;
                    break;

                case RewardType.Pin:
                    RewardGainController.OnAddRewardGain?.Invoke(new RewardGainPin(spinReward.RewardAmount, Vector3.zero, new LogCurrency("currency", "pin", "lucky_spin", "non_iap", "ads", "rwd_ads")));
                    hasReward = true;
                    break;
            }
            if (hasReward)
            {
                yield return new WaitForSeconds(0.5f);
                OnCompleted?.Invoke();
                RewardGainController.OnStartGaining?.Invoke();
            }
        }

        private IEnumerator ImmediatelyStop()
        {
            _currentSpinSpeed = 50f;
            yield return new WaitForSeconds(0.75f);
            _isSpinning = false;
        }

        public void ClickOnPointer()
        {
            if (!_onPointer && !_isSpinning)
            {
                _onPointer = true;
                _wheelSpin.DOPunchRotation(new Vector3(0f, 0f, 3f), 0.5f, 8, 1);
                _wheelPointer.DOPunchRotation(new Vector3(0f, 0f, 10f), 0.5f, 8, 1).OnComplete(() => _onPointer = false);
            }
        }

        public void Spin(Action onSpinning, Action onSpinStopped, Action onCompleted)
        {
            if (_isSpinning) return;
            OnSpinning = onSpinning;
            OnSpinStopped = onSpinStopped;
            OnCompleted = onCompleted;
            // reset the wheel & pointer position
            _wheelSpin.localEulerAngles = new Vector3(0, 0, 270);
            _wheelPointer.localEulerAngles = new Vector3(0, 0, 0);
            // spin the wheel
            OnSpinWheel();
        }

        public void Stop()
        {
            StartCoroutine(ImmediatelyStop());
        }
    }
}
