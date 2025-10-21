using System;
using UnityEngine;

namespace Percas
{
    public class UILevelLabel_PhaseItem : MonoBehaviour
    {
        [SerializeField] int phase;
        [SerializeField] GameObject completed, inProcess, toDo;

        public static Action<int> OnShow;

        private void Awake()
        {
            OnShow += Show;
        }

        private void OnDestroy()
        {
            OnShow -= Show;
        }

        private void Show(int currentPhase)
        {
            completed.SetActive(currentPhase > phase);
            inProcess.SetActive(currentPhase == phase);
            toDo.SetActive(currentPhase < phase);
        }
    }
}
