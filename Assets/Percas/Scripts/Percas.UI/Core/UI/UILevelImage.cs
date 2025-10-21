using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Percas.UI
{
    public class UILevelImage : MonoBehaviour, IActivatable
    {
        [SerializeField] Image eventBackground, image;
        [SerializeField] ButtonBase button;
        [SerializeField] RectTransform rectImage;
        [SerializeField] CanvasGroup canvasGroupImage;
        [SerializeField] GameObject m_grid, m_classicBackground, m_eventBackground;
        [SerializeField] List<PopupHiddenPicture_Piece> picturePieces;

        public static Action<Sprite> OnUpdateImage;

        private Tween shakeTween;
        private Tween fadeTween;

        private void Awake()
        {
            OnUpdateImage += UpdateImage;
        }

        private void OnDestroy()
        {
            OnUpdateImage -= UpdateImage;
            shakeTween?.Kill();
            fadeTween?.Kill();
        }

        public void Activate()
        {
            button.SetPointerClickEvent(ShakeImage);
            SetupUI();
        }

        public void Deactivate()
        {
            shakeTween?.Kill();
            fadeTween?.Kill();
        }

        private void SetupUI()
        {
            m_grid.SetActive(false);
            image.gameObject.SetActive(true);
            m_classicBackground.SetActive(true);
            m_eventBackground.SetActive(false);
        }

        private void UpdateImage(Sprite sprite)
        {
            image.sprite = sprite;
        }

        private void ShakeImage()
        {
            if (shakeTween != null) return;
            shakeTween = rectImage.DOShakePosition(0.2f, 5).OnComplete(() => shakeTween = null);
        }

        private IEnumerator SetupPictureGrid(HiddenPictureDataSO data)
        {
            List<LevelAsset> levelAssets = data.LevelDatas;
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < levelAssets.Count; i++)
            {
                //picturePieces[i].UpdateUI(Static.GetSpriteHiddenPicture(levelAssets[i]), HiddenPictureManager.Data.IsUnlocked(i));
                picturePieces[i].UpdateUI(DataManager.Instance.GetCurrentHiddenPicturePiece(i), HiddenPictureManager.Data.IsUnlocked(i));
                yield return null;
            }
        }

        public void ReloadHiddenPicture(int eventID, Action onLoading, Action onCompleted)
        {
            onLoading?.Invoke();

            if (eventID < 0)
            {
                onCompleted?.Invoke();
                return;
            }

            HiddenPictureDataSO data = DataManager.Instance.GetHiddenPictureDataByIndex(eventID);

            m_classicBackground.SetActive(false);
            m_eventBackground.SetActive(true);
            eventBackground.sprite = data.HiddenBackground;

            Sprite sprite = data.HiddenPicture;
            UpdateImage(sprite);

            onCompleted?.Invoke();
            if (!HiddenPictureManager.Data.IsCollectedEvent(eventID))
            {
                image.gameObject.SetActive(false);
                m_grid.SetActive(true);
                StartCoroutine(SetupPictureGrid(data));
            }
            else
            {
                image.gameObject.SetActive(true);
                m_grid.SetActive(false);
            }
        }

        public void ReloadClassic(int level, Action onLoading, Action onCompleted)
        {
            onLoading?.Invoke();
            m_classicBackground.SetActive(true);
            m_eventBackground.SetActive(false);

            Sprite sprite = Static.GetSpriteLevelPicture(level);
            UpdateImage(sprite);

            onCompleted?.Invoke();
            image.gameObject.SetActive(true);
            m_grid.SetActive(false);
        }

        public void DisplayPicture(bool value)
        {
            if (!value)
            {
                canvasGroupImage.alpha = 0;
            }
            else
            {
                canvasGroupImage.alpha = 0;
                fadeTween = canvasGroupImage.DOFade(1, 1.25f);
            }
        }
    }
}
