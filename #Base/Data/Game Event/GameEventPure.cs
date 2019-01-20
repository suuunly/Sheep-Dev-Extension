using System.Collections;
using System.Collections.Generic;
using SDE;
using UnityEngine;

[CreateAssetMenu(fileName = "Pure Game Event", menuName = "SDE/Data/Pure Game Event")]
public class GameEventPure : ScriptableObject
{
	public event System.Action<object> OnRaised;

	public void Raise(object param = null)
	{
		OnRaised.TryInvoke(param);
	}

	private void OnDisable()
	{
		OnRaised.RemoveAllListeners();
	}
}
