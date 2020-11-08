using UnityEngine;

namespace Assets.LevelGenerator.Models
{
    public class LevelExit
    {
        public Vector2 Position { get; set; }

        public int Index { get; set; }

        public int IndexTo { get; set; }

        public string Scene { get; set; }
    }
}
