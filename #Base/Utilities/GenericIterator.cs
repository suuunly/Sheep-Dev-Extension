using UnityEngine;

namespace SDE
{
	public enum EEdgeReaction {
		Stop, Wrap
	}

	public class GenericIterator<T>
	{
		private delegate int DelEdgeReactionType(int value, int min, int max);
		private static readonly DelEdgeReactionType[] EDGE_REACTIONS = {Mathf.Clamp, SDEMath.Wrap};

		public T[] Objects;
		public EEdgeReaction EdgeReactionType;
		private int mCurrentIndex;

		public T Current => Objects[mCurrentIndex];
		public bool ReachedEnd => Current.Equals(Objects[Objects.Length - 1]);

		public void Reset() 
		{
			mCurrentIndex = 0;
		}

		public T Next() 
		{
			mCurrentIndex = HandleEdge(mCurrentIndex+1);
			return Current;
		}
		public T Prev() 
		{
			mCurrentIndex = HandleEdge(mCurrentIndex-1);
			return Current;
		}

		public void Set(int index)
		{
			mCurrentIndex = HandleEdge(index);
		}

		private int HandleEdge(int index) 
		{
			return EDGE_REACTIONS[(int)EdgeReactionType](index, 0, Objects.Length-1);
		}
	}
}
