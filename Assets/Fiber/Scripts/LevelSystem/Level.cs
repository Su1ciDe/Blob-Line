using LevelEditor;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace Fiber.LevelSystem
{
	public class Level : MonoBehaviour
	{
		public LevelDataSO LevelDataSO { get; private set; }

		public virtual void Load(LevelDataSO levelDataSO)
		{
			LevelDataSO = levelDataSO;
			gameObject.SetActive(true);

			Grid.Instance.Setup(LevelDataSO.Grid);
		}

		public virtual void Play()
		{
		}
	}
}