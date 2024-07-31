using AYellowpaper.SerializedCollections;
using GamePlay.Obstacles;
using LevelEditor;
using UnityEngine;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "Obstacles", menuName = "Blob Line/Obstacles", order = 12)]
	public class ObstaclesSO : ScriptableObject
	{
		public SerializedDictionary<CellType, BaseObstacle> Obstacles = new SerializedDictionary<CellType, BaseObstacle>();
	}
}