using System.Collections.Generic;
using Interaction;
using UnityEngine;

namespace Board
{
    /// <summary>
    /// Class responsible for loading and initializing the board for the game.
    /// It manages the creation of spaces, discs, columns, and background elements.
    /// </summary>
    public class BoardLoader : MonoBehaviour
    {
        /// <summary>
        /// Number of columns in the board (ranges from 0 to 10).
        /// </summary>
        [Header("Board"), SerializeField, Range(0, 10)] private byte numColumns = 7;
        
        /// <summary>
        /// Number of rows in the board (ranges from 0 to 10).
        /// </summary>
        [SerializeField, Range(0, 10)] private byte numRows = 6;
        
        /// <summary>
        /// Space between discs on the board (ranging from 0 to 1)
        /// .</summary>
        [Tooltip("Space between discs"), SerializeField, Range(0, 1)] private float spaceAddition = 0.25f;
        
        /// <summary>
        /// Prefab used to create each space on the board
        /// .</summary>
        [Header("Prefabs"), SerializeField] private GameObject spacePrefab;
        
        /// <summary>
        /// Prefab used to create each disc on the board.
        /// </summary>
        [SerializeField] private GameObject discPrefab;
        
        /// <summary>
        /// Prefab for the background element of the board.
        /// </summary>
        [SerializeField] private GameObject backGroundPrefab;
        
        /// <summary>
        /// Prefab used to create the columns on the board.
        /// </summary>
        [SerializeField] private GameObject columnPrefab;
        
        // The radius of each space in Unity units.
        private float _spaceRadius;

        // Getters for properties
        public ushort Capacity { get; private set; } //< Number of spaces on the board (columns/7 * rows/6 = 42).
        public byte NumColumns => numColumns;
        public byte NumRows => numRows;
        public List<Disc> Discs { get; private set; }
        public List<Column> Columns { get; private set; }

        
        /// <summary>
        /// Initializes the board by calculating capacity and creating all the necessary components.
        /// </summary>
        public void BuildBoard()
        {
            // Calculate the total number of spaces on the board (columns * rows).
            Capacity = (ushort)(numRows * numColumns);
            Discs = new List<Disc>(Capacity);
            Columns = new List<Column>(numColumns);

            // Create the space radius by instantiating a sample space object.
            GameObject spaceRadiusGo = Instantiate(spacePrefab);
            spaceRadiusGo.TryGetComponent(out Space space);
            spaceRadiusGo.name = "spaceRadius";
            _spaceRadius = space.Radius;

            // Check the scale of the space to adjust the radius if necessary.
            Vector3 spaceScale = spaceRadiusGo.transform.localScale;
            float multiplier = spaceScale.x;
            if (spaceScale.x < spaceScale.y) 
                multiplier = spaceScale.y;
                
            _spaceRadius *= multiplier;
            spaceRadiusGo.SetActive(false); //< Hide the radius object after use.

            // Create the rest of the board components (spaces, discs, background, columns).
            SpaceAndDiscCreation();
            BackGroundCreation();
            ColumnsCreation();
        }

        /// <summary>
        /// Creates columns and sets their position and collider size based on the grid dimensions.
        /// </summary>
        private void ColumnsCreation()
        {
            // Loop through each column and instantiate a column object at the appropriate position.
            for (byte columnInt = 0; columnInt < numColumns; ++columnInt)
            {
                GameObject columnGameObject = Instantiate(columnPrefab, Vector3.zero, Quaternion.identity);
                columnGameObject.TryGetComponent(out Column column);
                
                // Calculate the position of the column based on its index.
                Vector2 columnPosition = new Vector2(columnInt * (_spaceRadius * 2 + spaceAddition), 0);
                columnPosition.x += Offset().x; //< Adjust for offset.
                column.SetPosition(columnPosition);

                // Set the size of the column (height based on the number of rows).
                float yColumnSize = numRows * (2 * (_spaceRadius + spaceAddition / 2));
                Vector2 columnSize = new Vector2(_spaceRadius * 2 + spaceAddition, yColumnSize);
                column.SetColliderSize(columnSize);

                Columns.Add(column);
            }
        }

        /// <summary>
        /// Creates the background of the board and sets its size.
        /// </summary>
        private void BackGroundCreation()
        {
            // Instantiate the background object and set its size based on the board's dimensions.
            GameObject backGroundGameObject = Instantiate(backGroundPrefab, Vector3.zero, Quaternion.identity);
            float boxSpacing = _spaceRadius + spaceAddition / 2; //< Define spacing for the box size calculation.
            float boardBoxSize = 2 * boxSpacing;
            backGroundGameObject.transform.localScale = new Vector2(numColumns * boardBoxSize, numRows * boardBoxSize);
        }
        
        /// <summary>
        /// Creates the spaces and discs on the board in a grid pattern.
        /// </summary>
        /// <remarks>
        /// Example of a 3x3 grid:
        ///      2 5 8
        ///      1 4 7
        ///      0 3 6
        /// </remarks>
        private void SpaceAndDiscCreation()
        {
            // Loop through each space and create both space and disc objects.
            for (ushort i = 0; i < Capacity; ++i)
            {
                // Calculate the row and column for this space based on its index.
                ushort rowIndex    = (ushort)(i / numRows);
                ushort columnIndex = (ushort)(i % numRows);
                    
                // Create the space object and set its position on the board.
                GameObject spaceGameObject = Instantiate(spacePrefab, Vector3.zero, Quaternion.identity);
                spaceGameObject.name = $"Space [{rowIndex },{columnIndex }]";
                spaceGameObject.TryGetComponent(out Space space);
                
                // Create the disc object and set its initial position (hidden).
                GameObject discGameObject = Instantiate(discPrefab, Vector3.zero, Quaternion.identity);
                discGameObject.name = $"Disc [{rowIndex },{columnIndex }]";
                discGameObject.TryGetComponent(out Disc disc);
                
                // Calculate the position of the space, applying any offsets.
                Vector2 spacePosition = new Vector2(rowIndex  * (_spaceRadius * 2 + spaceAddition), 
                                                    columnIndex  * (_spaceRadius * 2 + spaceAddition));
                spacePosition += Offset();    
                space.SetPosition(spacePosition);
                
                // Position the disc in the same location as the space and set it to be invisible initially.
                disc.SetPosition(spacePosition);
                disc.SetVisibility(false); //< Set disc to be invisible
                disc.SetStartPositionForAnimation((numColumns * (_spaceRadius * 2 + spaceAddition))/2);
                disc.SetLastPositionForAnimation(spacePosition.y);
                
                Discs.Add(disc); //< Add the disc to the list of discs.
            }
        }
        
        /// <summary>
        /// Calculates an offset to position the board correctly in the center.
        /// </summary>
        /// <returns>A vector that represents the offset for proper board positioning.</returns>
        private Vector2 Offset()
        {
            // Calculate the offset needed to center the board based on the number of columns and rows.
            Vector2 offset = Vector2.zero;
            offset.x -= (numColumns-1) * (_spaceRadius + spaceAddition / 2);
            offset.y -=   (numRows-1)  * (_spaceRadius + spaceAddition / 2);

            return offset;
        }

        /// <summary>
        /// Resets the visibility and color of all discs on the board (e.g., for a new round).
        /// </summary>
        public void ResetDiscs()
        {
            // Iterate through each disc and reset its visibility and color.
            foreach (var disc in Discs)
            {
                disc.SetVisibility(false);
                disc.SetColor(Color.white); //< Reset to the default color (white).
            }
        }
    }
}