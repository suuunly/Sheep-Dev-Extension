using UnityEngine;
namespace SDE.UI
{
    public class ProgressBar : Progress
    {
        // ________________________________________
        // @ Static Decleration
        [System.Flags] public enum EBarDir { Horizontal = 1, Vertical = 2 }

        // ________________________________________
        // @ Inspector
        public RectTransform Bar;
        public EBarDir Direction = EBarDir.Horizontal;

        // _______________________________________
        // @ Data
        protected Vector2 OriginalSize { get; private set; }

        // ________________________________________________________ 
        // @ Controls
        protected sealed override void OnUpdateProgress(float percentage)
        {
            Vector2 targetSize = OriginalSize;
            SetBarRect(ref targetSize, percentage);
            SetBarSize(targetSize);
        }
        
        protected void SetBarRect(ref Vector2 size, float value)
        {
            // update the appropriate size, depending on the binary x/y flags
            if ((Direction & EBarDir.Horizontal) == EBarDir.Horizontal) size.x *= value;
            if ((Direction & EBarDir.Vertical) == EBarDir.Vertical) size.y *= value;
        }
        
        protected virtual void SetBarSize(Vector2 targetSize)
        {
            Bar.sizeDelta = targetSize;
        }


        // ________________________________________________________ 
        // @ Methods
        private void Awake()
        {
            Vector2 size = Bar.sizeDelta;
            OriginalSize = size;

            // start by making the bar progress to be 0
            SetBarRect(ref size, 0.0f);
            Bar.sizeDelta = size;
        }
    }
}