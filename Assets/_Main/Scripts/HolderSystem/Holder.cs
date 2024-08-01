using GamePlay.Blobs;
using Interfaces;
using UnityEngine;

namespace HolderSystem
{
	public class Holder : MonoBehaviour, ISlot
	{
		public Blob Blob { get; set; }
		public int Index { get; set; }

		[SerializeField] private float size;
		public float Size => size;

		public void SetBlob(Blob blob, bool setPosition = true)
		{
		}

		public Transform GetTransform() => transform;
	}
}