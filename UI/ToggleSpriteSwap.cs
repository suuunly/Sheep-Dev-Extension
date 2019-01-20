using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace SDE.UI
{
	[System.Serializable]
	public class ToggledEvent : UnityEvent<bool>{}
	
	[RequireComponent(typeof(Button))]
	public class ToggleSpriteSwap : MonoBehaviour
	{
		public bool IsOn;
		public Sprite On;
		
		public ToggledEvent OnToggled;
		
		private Button mButton;
		private Image mImage;
		private Sprite mOff;
		private bool mToggleState;

		public void SetToggle(bool value, bool invokeEvent = false)
		{
			mToggleState = value;
			SetSprite();
			if(invokeEvent)
				OnToggled.Invoke(mToggleState);
		}
		
		private void Awake()
		{
			mButton = GetComponent<Button>();
			mImage = mButton.image;
			mOff = mImage.sprite;
			
			mButton.onClick.AddListener(OnButtonPressed);	
			mToggleState = IsOn;

			SetSprite();
		}

		private void SetSprite()
		{
			mImage.sprite = (mToggleState) ? On : mOff;
		}

		private void OnValidate() {
			mToggleState = IsOn;
			
			if(mImage)
				SetSprite();
		}
		private void OnDestroy() => mButton.onClick.RemoveListener(OnButtonPressed);

		private void OnButtonPressed()
		{
			mToggleState = !mToggleState;
			
			SetSprite();
			OnToggled.Invoke(mToggleState);
		}
	}	

}

