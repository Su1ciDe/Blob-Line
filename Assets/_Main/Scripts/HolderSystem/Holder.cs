using System.Collections.Generic;
using GamePlay.Blobs;
using Interfaces;
using UnityEngine;

namespace HolderSystem
{
	public class Holder : MonoBehaviour, ISlot
	{
		public Stack<Blob> Blobs { get;  set; } = new Stack<Blob>();
		public int Index { get;  set; }

		[SerializeField] private float size;
		public float Size => size;

		public static float OFFSET = .25f;

		public void SetBlob(Blob blob)
		{
			Blobs.Push(blob);
		}

		public Transform GetTransform() => transform;
	}
}