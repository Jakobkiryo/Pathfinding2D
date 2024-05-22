using UnityEngine;

namespace Grids
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GridCell : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;
        public bool walkable;

        void Start()
        {
            OnValidate();
        }

        public override string ToString()
        {
            return $"{nameof(GridCell)} ({Mathf.FloorToInt(transform.position.x)}|{Mathf.FloorToInt(transform.position.y)})";
        }

        private void OnValidate()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = walkable ? Color.white : Color.black;
        }
    }
}