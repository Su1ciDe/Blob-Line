using System.Collections.Generic;
using GamePlay.Blobs;
using Interfaces;
using UnityEngine;

namespace HolderSystem
{
	public class Holder : MonoBehaviour, ISlot
	{
		public List<Blob> Blobs { get;  set; } = new List<Blob>();
		public int Index { get;  set; }

		[SerializeField] private float size;
		public float Size => size;

		public static float OFFSET = .25f;

		public void SetBlob(Blob blob)
		{
			Blobs.Add(blob);
		}

		public Transform GetTransform() => transform;
	}
}