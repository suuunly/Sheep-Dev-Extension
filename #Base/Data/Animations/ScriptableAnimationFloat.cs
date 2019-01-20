using UnityEngine;

namespace SDE
{
    [CreateAssetMenu(fileName = "Scriptable Animation Float", menuName = "SDE/Data/Animation/Scriptable Animation Float", order = 0)]
    public class ScriptableAnimationFloat : ScriptableAnimation
    {
        public float Value;
        public override void SetValue(Animator animator)
        {
            animator.SetFloat(VariableName, Value);
        }
    }
}
