using System;
using System.Collections.Generic;
using Board;
using Interaction;
using UnityEngine;
using Space = Board.Space;

namespace Core
{
    // Reformat: one class for creation, another for extraction
    // The class that extracts info will have a reference to this class (creation)
    public class BoardManager : MonoBehaviour
    {
        [Header("Board")]
        [SerializeField, Range(0, 10)] private byte columns = 7;
        [SerializeField, Range(0, 10)] private byte rows = 6;
        [Tooltip("Space between discs"), SerializeField, Range(0, 1)] private float spaceAddition = 0.25f;
        private ushort _capacity; //< columns * rows => 7 * 6 = 42
        
        [Header("Prefabs")]
        [SerializeField] private GameObject spacePrefab;
        [SerializeField] private GameObject discPrefab;
        [SerializeField] private GameObject backGroundPrefab;
        [SerializeField] private GameObject columnPrefab;
        
        // Space radius in Unity units
        private float _spaceRadius;
        
        private List<Space> _spaces = null;
        private List<Disc> _disc = null;
        private List<Column> _columns = null;
    
        public void SetupScene()
        {
            _capacity = (ushort)(rows * columns);
            _spaces = new List<Space>(_capacity);
            _disc = new List<Disc>(_capacity);
            _columns = new List<Column>(columns);

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
            for (byte columnInt = 0; columnInt < columns; ++columnInt)
            {
                GameObject columnGameObject = Instantiate(columnPrefab, Vector3.zero, Quaternion.identity);
                columnGameObject.TryGetComponent(out Column column);
                
                Vector2 columnPosition = new Vector2(columnInt * (_spaceRadius * 2 + spaceAddition), 0);
                columnPosition.x += Offset().x;
                column.SetPosition(columnPosition);

                float yColumnSize = rows * (2 * (_spaceRadius + spaceAddition / 2));
                Vector2 columnSize = new Vector2(_spaceRadius * 2 + spaceAddition, yColumnSize);
                column.SetColliderSize(columnSize);

                _columns.Add(column);
            }
        }

        private void BackGroundCreation()
        {
            GameObject backGroundGameObject = Instantiate(backGroundPrefab, Vector3.zero, Quaternion.identity);
            float boardBoxSize = 2 * (_spaceRadius + spaceAddition / 2);
            backGroundGameObject.transform.localScale = new Vector2(columns * boardBoxSize, rows * boardBoxSize);
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
            for (ushort i = 0; i < _capacity; ++i)
            {
                // Table position
                ushort row    = (ushort)(i / rows);
                ushort column = (ushort)(i % rows);
                    
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
                _spaces.Add(space);
                
                disc.SetPosition(spacePosition);
                disc.SetGamePosition(gamePosition);
                disc.SetVisibility(false);           //< Also the visibility
                _disc.Add(disc);
            }
        }
        
        private Vector2 Offset()
        {
            // Middle Position
            Vector2 offset = Vector2.zero;
            offset.x -= (columns-1) * (_spaceRadius + spaceAddition / 2);
            offset.y -=   (rows-1)  * (_spaceRadius + spaceAddition / 2);

            return offset;
        }

        public void ForEachSpace(ushort startColumn, ushort endColumn, Action<int, int> method)
        {
            startColumn *= rows;
            endColumn *= rows;
            
            for (ushort i = startColumn; i < endColumn; ++i)
            {
                ushort row    = (ushort)(i / rows);
                ushort column = (ushort)(i % rows);
                method(row, column);
                
                Debug.Log($"{row}, {column}");
            }
        }
        
        public void ForEachSpace(ushort startColumn, ushort endColumn, Action<Space> method)
        {
            startColumn *= rows;
            endColumn *= rows;
            
            for (ushort i = startColumn; i < endColumn; ++i)
            {
                ushort row    = (ushort)(i / rows);
                ushort column = (ushort)(i % rows);
                method(_spaces[i]);
                
                Debug.Log($"{row}, {column}");
            }
        }
        
        /// Debug, no use Actually
        public void ForEachSpaceInColumn(Column column)
        {
            ushort index = (ushort)_columns.FindIndex( element => element == column);
            
            ushort startColumn = (ushort)(rows * index);
            ushort endColumn = (ushort)(rows * (index + 1));
            
            for (ushort i = startColumn; i < endColumn; ++i)
            {
                ushort iRow    = (ushort)(i / rows);
                ushort iColumn = (ushort)(i % rows);
                Debug.Log($"{iRow}, {iColumn}");
            }
        }

        public void ForEachColumn(Column.Interaction method)
        {
            for (byte columnInt = 0; columnInt < columns; ++columnInt)
            {
                _columns[columnInt].OnInteraction += method;
            }
        }
        
        public void ForEachColumnRemove(Column.Interaction method)
        {
            for (byte columnInt = 0; columnInt < columns; ++columnInt)
            {
                _columns[columnInt].OnInteraction -= method;
            }
        }
        
        /// List of free positions
        public List<Space> ListOfSpacesInColumn(Column column)
        {
            List<Space> spacesForColumn = new List<Space>();
            ushort index = (ushort)_columns.FindIndex( element => element == column);
            
            ushort startColumn = (ushort)(rows * index);
            ushort endColumn = (ushort)(rows * (index + 1));
            
            for (ushort i = startColumn; i < endColumn; ++i)
            {
                // ushort srow    = (ushort)(i / rows);
                // ushort scolumn = (ushort)(i % rows);
                // Debug.Log($"{srow}, {scolumn}");
                spacesForColumn.Add(_spaces[i]);
            }
            return spacesForColumn;
        }
        
        public Space FirstSpaceInColumn(Column column)
        {
            ushort index = (ushort)_columns.FindIndex( element => element == column);
            ushort startColumn = (ushort)(rows * index);
            ushort endColumn = (ushort)(rows * (index + 1));
            
            for (ushort i = startColumn; i < endColumn; ++i)
            {
                if(_spaces[i].Free) return _spaces[i];
            }
            return null;
        }
        
        /// Temporal
        public Disc FirstDiscInColumn(Column column)
        {
            ushort index = (ushort)_columns.FindIndex( element => element == column);
            ushort startColumn = (ushort)(rows * index);
            ushort endColumn = (ushort)(rows * (index + 1));
            
            for (ushort i = startColumn; i < endColumn; ++i)
            {
                if (!_disc[i].Visibility)
                {
                    _disc[i].SetVisibility(true);
                    return _disc[i];
                }
            }
            return null;
        }
    }
}