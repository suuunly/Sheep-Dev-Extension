using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SDE.UI
{
    public class ProgressText : Progress
    {
		public Text Text;
        protected override void OnUpdateProgress(float percentage)
        {
            Text.text = (percentage * 100.0f).ToString("00");
        }
    }
}
