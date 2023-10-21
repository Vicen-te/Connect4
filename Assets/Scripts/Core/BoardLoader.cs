using System.Collections.Generic;
using Board;
using Interaction;
using UnityEngine;
using Space = Board.Space;

namespace Core
{
    public class BoardLoader : MonoBehaviour
    {
        [Header("Board")]
        [SerializeField, Range(0, 10)] public byte columnsInt = 7;
        [SerializeField, Range(0, 10)] public byte rowsInt = 6;
        [Tooltip("Space between discs"), SerializeField, Range(0, 1)] private float spaceAddition = 0.25f;
        public ushort Capacity { get; private set; } //< columns * rows => 7 * 6 = 42
        
        [Header("Prefabs")]
        [SerializeField] private GameObject spacePrefab;
        [SerializeField] private GameObject discPrefab;
        [SerializeField] private GameObject backGroundPrefab;
        [SerializeField] private GameObject columnPrefab;
        
        // Space radius in Unity units
        private float _spaceRadius;
        
        public List<Space> Spaces { get; private set; }
        public List<Disc> Discs { get; private set; }
        public List<Column> Columns { get; private set; }

        public void SetupScene()
        {
            Capacity = (ushort)(rowsInt * columnsInt);
            Spaces = new List<Space>(Capacity);
            Discs = new List<Disc>(Capacity);
            Columns = new List<Column>(columnsInt);

            // Space Radius
            GameObject spaceRadiusGo = Instantiate(spacePrefab);
            spaceRadiusGo.transform.TryGetComponent(out SpriteRenderer spriteRenderer);
            spaceRadiusGo.name = "spaceRadius";
            _spaceRadius = Circle.GetRadius(spriteRenderer);

            // Check Scale
            Vector3 spaceScale = spaceRadiusGo.transform.localScale;
            float multiplier = spaceScale.x;
            if (spaceScale.x < spaceScale.y) 
                multiplier = spaceScale.y;
                
            _spaceRadius *= multiplier;
            spaceRadiusGo.SetActive(false);

            // Create Board
            SpaceAndDiscCreation();
            BackGroundCreation();
            ClicksColumnsCreation();
            // ForEachSpace((ushort)(columns-1), columns, (x,y) => {});
        }

        private void ClicksColumnsCreation()
        {
            for (byte columnInt = 0; columnInt < columnsInt; ++columnInt)
            {
                GameObject columnGameObject = Instantiate(columnPrefab, Vector3.zero, Quaternion.identity);
                columnGameObject.TryGetComponent(out Column column);
                
                Vector2 columnPosition = new Vector2(columnInt * (_spaceRadius * 2 + spaceAddition), 0);
                columnPosition.x += Offset().x;
                column.SetPosition(columnPosition);

                float yColumnSize = rowsInt * (2 * (_spaceRadius + spaceAddition / 2));
                Vector2 columnSize = new Vector2(_spaceRadius * 2 + spaceAddition, yColumnSize);
                column.SetColliderSize(columnSize);

                Columns.Add(column);
            }
        }

        private void BackGroundCreation()
        {
            GameObject backGroundGameObject = Instantiate(backGroundPrefab, Vector3.zero, Quaternion.identity);
            float boardBoxSize = 2 * (_spaceRadius + spaceAddition / 2);
            backGroundGameObject.transform.localScale = new Vector2(columnsInt * boardBoxSize, rowsInt * boardBoxSize);
        }
        
        /**
         * Generation Example
         *  3x3:
         *      2 5 8
         *      1 4 7
         *      0 3 6
         */
        private void SpaceAndDiscCreation()
        {
            for (ushort i = 0; i < Capacity; ++i)
            {
                // Table position
                ushort row    = (ushort)(i / rowsInt);
                ushort column = (ushort)(i % rowsInt);
                    
                // Space Creation
                GameObject spaceGameObject = Instantiate(spacePrefab, Vector3.zero, Quaternion.identity);
                spaceGameObject.name = $"Space [{row},{column}]";
                spaceGameObject.TryGetComponent(out Space space);
                
                // Disc Creation
                GameObject discGameObject = Instantiate(discPrefab, Vector3.zero, Quaternion.identity);
                discGameObject.name = $"Disc [{row},{column}]";
                discGameObject.TryGetComponent(out Disc disc);
                
                // Margin space
                Vector2 spacePosition = new Vector2(row * (_spaceRadius * 2 + spaceAddition), 
                                                    column * (_spaceRadius * 2 + spaceAddition));
                spacePosition += Offset();
                
                // Change Space and Disc Position and Game Position
                KeyValuePair<int, int> gamePosition = new KeyValuePair<int, int>(row, column);                
                
                space.SetPosition(spacePosition);
                space.SetGamePosition(gamePosition);
                Spaces.Add(space);
                
                disc.SetPosition(spacePosition);
                disc.SetGamePosition(gamePosition);
                disc.SetVisibility(false);           //< Also the visibility
                disc.SetStartPosition((columnsInt * (_spaceRadius * 2 + spaceAddition))/2);
                disc.SetLastPosition(spacePosition.y);
                Discs.Add(disc);
            }
        }
        
        private Vector2 Offset()
        {
            // Middle Position
            Vector2 offset = Vector2.zero;
            offset.x -= (columnsInt-1) * (_spaceRadius + spaceAddition / 2);
            offset.y -=   (rowsInt-1)  * (_spaceRadius + spaceAddition / 2);

            return offset;
        }
    }
}