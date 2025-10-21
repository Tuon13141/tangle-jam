using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elements
{
    public class LockElement : MonoBehaviour
    {
        [ReadOnly] public StageData.CellData cellData;
        [ReadOnly] public LockElement lockElement;
        public GridElement gridElement;
        [SerializeField] private TMPro.TMP_Text m_ValueText;
        public TMPro.TMP_Text ValueText => m_ValueText;
        [SerializeField] private AnimationSupport m_Animation;
        public void Setup(StageData.CellData data, GridElement grid, Matrix<GridElement> map)
        {
            cellData = data;
            gridElement = grid;
            var gridTarget = map[gridElement.indexGrid.x, gridElement.indexGrid.y];
            if (gridTarget != null) gridTarget.SetLockElement(this);
            UpdateValueText();
        }
        
        public void UpdateValueText()
        {
            m_ValueText.text = $"{cellData.Value}";
        }

        public void UnlockAnim()
        {
            m_Animation.Play("Lock Handle", 1f);
        }
        
        public void DisableLock()
        {
            if (!gameObject.activeSelf) return;

            //Disable lock
            //Percas.ActionEvent.OnReleasePin?.Invoke();
            gridElement.CheckTubeElenentTarget();
            gridElement.cellType = StageData.CellType.Empty;
            gridElement.lockElements = null;
            gameObject.SetActive(false);
        }
    }
}