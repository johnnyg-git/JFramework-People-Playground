using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JFramework
{
    public class Modification
    {
		/// <summary>
		/// The item to modify
		/// </summary>
		public SpawnableAsset OriginalItem;
		/// <summary>
		/// What to rename the item to
		/// </summary>
		public string NameOverride;
		/// <summary>
		/// What to change the description of the item to
		/// </summary>
		public string DescriptionOverride;
		/// <summary>
		/// What name to order the item by
		/// </summary>
		public string NameToOrderByOverride;
		/// <summary>
		/// What category to put the item in
		/// </summary>
		public Category CategoryOverride;
		/// <summary>
		/// What the items thumbnail should be
		/// </summary>
		public Sprite ThumbnailOverride;
		/// <summary>
		/// Ran after the item is spawned, passes the item GameObject
		/// </summary>
		public Action<GameObject> AfterSpawn;
	}
}
