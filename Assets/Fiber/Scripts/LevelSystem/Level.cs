using Fiber.Managers;
using LevelEditor;
using UnityEngine;

namespace Fiber.LevelSystem
{
	public class Level : MonoBehaviour
	{
		public LevelDataSO LevelDataSO { get; private set; }
		public int Money => LevelManager.Instance.LevelNo * 2 + 4;

		public virtual void Load(LevelDataSO levelDataSO)
		{
			LevelDataSO = levelDataSO;
			gameObject.SetActive(true);

			// Grid.Instance.Setup(LevelDataSO.Grid);
		}

		public virtual void Play()
		{
		}
	}
}