using UnityEngine;

namespace SDE.UI
{
    public abstract class Progress : MonoBehaviour
    {
        // ___________________________________________________________
        // @ Static Decleration
        private delegate float DelProgressPercentage(float value, float maxValue);
        private readonly static DelProgressPercentage[] PROGRESS_METHODS = { GetPercentage0To100, GetPercentage100To0 };

        public enum EBarProgressDir { From0To100 = 0, From100To0 };

        // ____________________________________________________________
        // @ Inspector
        public Progress Child;
        public EBarProgressDir ProgressDirection = EBarProgressDir.From0To100;

        // ____________________________________________________________
        // @ Controls
        public void UpdateProgress(float value, float maxValue)
        {
            float percent = PROGRESS_METHODS[(int)ProgressDirection](value, maxValue);
            UpdateProgress(percent);
        }
        public void UpdateProgress(float percentage)
        {
            OnUpdateProgress(percentage);
            if (Child)
                Child.UpdateProgress(percentage);
        }

        // ____________________________________________________________
        // @ Abstraction
        protected abstract void OnUpdateProgress(float percentage);

        // ____________________________________________________________
        // @ Static Methods
        private static float GetPercentage0To100(float value, float maxValue)
        {
            return value / maxValue;
        }
        private static float GetPercentage100To0(float value, float maxValue)
        {
            return (maxValue - value) / maxValue;
        }
    }
}