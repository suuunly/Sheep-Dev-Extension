using UnityEngine;

public class BasicRotatorOscillation : MonoBehaviour
{
	public Vector3 RotateTo;
	public float Speed;

	private Quaternion mStartRot;
	private Quaternion mEndRot;

	private Vector3 mVelocity;
	private float mTime;
	
	private void Start()
	{
		mStartRot = transform.rotation;
		mEndRot = Quaternion.Euler(RotateTo);
	}
	
	private void Update()
	{
		mTime += Time.deltaTime;
		float t = (Mathf.Sin (mTime * Speed * Mathf.PI * 2.0f) + 1.0f) / 2.0f;
		
		transform.rotation = Quaternion.Slerp(mStartRot, mEndRot, t);
	}
}
