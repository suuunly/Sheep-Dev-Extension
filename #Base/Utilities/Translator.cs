using UnityEngine;
namespace SDE {
	public class Translator : MonoBehaviour {

		public Vector3 Speed;
		private void Update () => transform.Translate(Speed * Time.deltaTime);
	}
}

