using UnityEngine;

namespace SDE
{
    [CreateAssetMenu(fileName = "Scriptable Animation Bool", menuName = "SDE/Data/Animation/Scriptable Animation Bool", order = 0)]
    public class ScriptableAnimationBool : ScriptableAnimation
    {
        public bool Value;
        public override void SetValue(Animator animator)
        {
            animator.SetBool(VariableName, Value);
        }
    }

}
