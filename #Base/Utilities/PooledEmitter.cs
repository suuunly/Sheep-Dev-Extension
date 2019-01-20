using SDE.Data;
using UnityEngine;

namespace SDE.GamePool
{
	public class PooledEmitter : MonoBehaviour
	{
		public RuntimeSet GamePoolSet;
		public GameObject EmittedPrefab;

		public GamePool.DataPool.EFetchType FetchType;
		public Vector3 MaxRandomOrientation;
	
		public void Emit()
		{
			GamePoolSet.TryApplyToFirst<GamePool>(pool => { 
				pool.Spawn(EmittedPrefab, transform.position, GetRandomOrientation(), FetchType); 
			});
		}

		private Quaternion GetRandomOrientation()
		{
			Vector3 v = SDEMath.Random0Max(MaxRandomOrientation);
			Debug.Log(v);
			return Quaternion.Euler(v);
		}
	}
}


