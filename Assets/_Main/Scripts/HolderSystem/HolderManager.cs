using System.Collections.Generic;
using Fiber.Managers;
using Fiber.Utilities;
using TriInspector;
using UnityEngine;

namespace HolderSystem
{
	public class HolderManager : Singleton<HolderManager>
	{
		[SerializeField] private int holderCount = 5;

		[Title("References")]
		[SerializeField] private Holder holderPrefab;

		private List<Holder> holders = new List<Holder>();

		private void OnEnable()
		{
			LevelManager.OnLevelLoad += Setup;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelLoad -= Setup;
		}

		private void Setup()
		{
			var offset = holderCount * holderPrefab.Size / 2f - holderPrefab.Size / 2f;
			for (int i = 0; i < holderCount; i++)
			{
				var holder = Instantiate(holderPrefab, transform);
				holder.transform.localPosition = new Vector3(i * holderPrefab.Size - offset, 0, 0);
				holders.Add(holder);
			}
		}
	}
}