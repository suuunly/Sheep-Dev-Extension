using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDE
{
	public static class SDEMath
	{
		#region Wrapping
		public static int Wrap(int value, int min, int max) 
		{ 
			return (value < min) ? max : (value > max) ? min : value; 
		}
		public static float Wrap(float value, float min, float max)
		{
			return (value < min) ? max : (value > max) ? min : value; 
		}
		public static double Wrap(double value, double min, double max)
		{
			return (value < min) ? max : (value > max) ? min : value; 
		}
		#endregion


		public static Vector3 Random0Max(Vector3 maxRandom)
		{
			return new Vector3(
				Random.Range(0.0f, maxRandom.x),
				Random.Range(0.0f, maxRandom.y),
				Random.Range(0.0f, maxRandom.z)
				);
		}

		public static float Diff(float a, float b)
		{
			return Mathf.Abs(a - b);
		}
	}
}
