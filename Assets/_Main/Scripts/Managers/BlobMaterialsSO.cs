using AYellowpaper.SerializedCollections;
using LevelEditor;
using UnityEngine;

namespace Managers
{
	[CreateAssetMenu(fileName = "Blob Materials", menuName = "Blob Line/BlobMaterials", order = 11)]
	public class BlobMaterialsSO : ScriptableObject
	{
		public SerializedDictionary<CellType, Material> BlobMaterials = new SerializedDictionary<CellType, Material>();
	}
}