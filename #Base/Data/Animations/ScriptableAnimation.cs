using UnityEngine;

namespace SDE
{
	[CreateAssetMenu(fileName = "Scriptable Animation", menuName = "SDE/Data/Animation/Scriptable Animation Trigger", order = 0)]
	public class ScriptableAnimation : ScriptableObject 
	{
		public string VariableName;

		public virtual void SetValue(Animator animator)
		{
			animator.SetTrigger(VariableName);
		}
	}
}


