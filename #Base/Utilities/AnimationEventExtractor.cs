using UnityEngine;
using UnityEngine.Events;

namespace SDE
{
	[System.Serializable]
	public class UnityEventIterator : GenericIterator<UnityEvent> {}
	public class AnimationEventExtractor : MonoBehaviour
	{
		public Animator Animator;
		public UnityEventIterator KeyEvents;
		
		[SerializeField] public UnityEvent EndAnimationKeyEvent;

		private void Awake()
		{
			Animator = Animator.GetComponent<Animator>();
		}

		public void OnKeyEvent()
		{
			KeyEvents.Current.Invoke();
			KeyEvents.Next();
		}

		public void OnAnimationEndKeyEvent()
		{
			EndAnimationKeyEvent.Invoke();
		}
	}

}

