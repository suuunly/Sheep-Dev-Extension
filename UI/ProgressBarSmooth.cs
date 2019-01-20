using System.Collections;
using UnityEngine;

namespace SDE.UI
{
    public class ProgressBarSmooth : ProgressBar
    {
        public const float NEAR_ENOUGH_FUDGE_FACTOR = 0.4f;
        public float LerpSpeed;

        private Coroutine mRoutine;

        // ________________________________________________________ Controls
        protected override void SetBarSize(Vector2 targetSize)
        {
            // Run a coroutine that updates the bar over time
            if (mRoutine != null) StopCoroutine(mRoutine);
            mRoutine = StartCoroutine(BarRoutine(targetSize));
        }
        // ________________________________________________________ Methods
        IEnumerator BarRoutine(Vector2 targetSize)
        {
            Vector2 currPos = Vector2.zero;
            do
            {
                // constantly move towards the targeted size, until the size is close enough 
                currPos = Bar.sizeDelta;
                Bar.sizeDelta = Vector2.Lerp(currPos, targetSize, LerpSpeed * Time.deltaTime);
                yield return null;
                // NOTE: using a near enough value, rather than the exact value due to floating point inaccuracy
            } while ((targetSize - currPos).magnitude > NEAR_ENOUGH_FUDGE_FACTOR);
        }
    }
}