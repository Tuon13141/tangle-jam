using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Elements
{
    public class KeyElement : MonoBehaviour
    {
        [ReadOnly] public StageData.CellData cellData;
        [ReadOnly] public LockElement lockElement;
        public GridElement gridElement;
        [SerializeField] private TMPro.TMP_Text m_ValueText;
        public TMPro.TMP_Text ValueText => m_ValueText;
        [ReadOnly] public CoilStatus coilStatus;
        [SerializeField] private Transform m_KeyModel;
        [SerializeField] private Transform m_KeyView;
        [SerializeField] private AnimationSupport m_Animation;
        public void Setup(StageData.CellData data, GridElement grid, Matrix<GridElement> map)
        {
            cellData = data;
            gridElement = grid;
            var gridTarget = map[gridElement.indexGrid.x, gridElement.indexGrid.y];
            if (gridTarget != null) gridTarget.SetKeyElement(this);
            UpdateValueText();
            m_Animation.Play("Key Idle");
        }
        public void UpdateValueText()
        {
            m_ValueText.text = $"{cellData.Value}";
        }
        
        public async void DisableKey()
        {
            if (!gameObject.activeSelf) return;
            //Disable lock
            //Percas.ActionEvent.OnReleasePin?.Invoke();
            gridElement.CheckTubeElenentTarget();
            gridElement.cellType = StageData.CellType.Empty;
            gridElement.keyElements = null;
            m_ValueText.gameObject.SetActive(false);
            await MoveToLockPos();
            lockElement.DisableLock();
            gameObject.SetActive(false);
        }

        private async UniTask MoveToLockPos()
        {
            transform.parent = null;
            transform.DOMoveY(transform.position.y + 5f, 0.5f).SetEase(Ease.OutBack);
            var lockPos = lockElement.transform.position;
            lockPos.y += 5f;
            lockPos.z -= 0.5f;
            lockElement.transform.DOMoveY(lockElement.transform.position.y + 3f, 1.5f).SetEase(Ease.OutBack);
            m_Animation.Play("Key Fly", 1.5f);
            await UniTask.Delay(500);
            transform.DOScale(new Vector3(2, 2, 2), 0.4f).OnComplete(() =>
            {
                transform.DOScale(new Vector3(1, 1, 1), 0.2f);
            });
            Static.MoveCurved(transform, lockPos, transform.position, 0.9f, true, new Vector3(0, 10, 0)).Forget();
            m_KeyView.DOLocalMove(Vector3.zero, 0.5f);
            await UniTask.Delay(910);
            m_KeyModel.DOLocalRotate(new Vector3(0, -45, 0), 0.01f);
            transform.DOMove(new Vector3(lockPos.x + 0.1f, lockPos.y - 0.8f, lockPos.z - 0.1f) , 0.01f);
            m_Animation.Play("Key Unlock", 1f);
            lockElement.UnlockAnim();
            await UniTask.Delay(1000);
        }
    }
}