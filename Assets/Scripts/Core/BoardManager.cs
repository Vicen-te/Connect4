using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public class BoardManager : MonoBehaviour
    {
        [Header("Board")]
        [SerializeField, Range(0, 10)] private byte columns = 7;
        [SerializeField, Range(0, 10)] private byte rows = 6;
        [SerializeField, Range(0, 1)] private float spaceAddition = 0.25f;
        
        [Header("Prefabs")]
        [SerializeField] private GameObject discPrefab;
        [SerializeField] private GameObject backGroundPrefab;
        [SerializeField] private GameObject columnPrefab;
        
        private float _halfColumns = 3.5f; 
        private float _halfRows = 3f;  
        
        private List<Disc> _discs = null;
        
        // Diameter in Unity units = 1;
        private const float DiscRadius = 0.5f;

        private void Start()
        {
            SetupScene();
        }

        private void SetupScene()
        {
            ushort capacity = (ushort)(rows * columns);
            
            _halfColumns = (float) columns / 2;
            _halfRows =    (float) rows    / 2; 

            _discs = new List<Disc>(capacity);
            
            DiscCreation();
            BackGroundCreation();
            ClicksColumnsCreation();
        }

        private void ClicksColumnsCreation()
        {
            for (byte columnInt = 0; columnInt < columns; ++columnInt)
            {
                GameObject columnGameObject = Instantiate(columnPrefab, Vector3.zero, Quaternion.identity);
                columnGameObject.TryGetComponent(out Column column);
                
                Vector2 columnPosition = new Vector2(columnInt * (DiscRadius*2 + spaceAddition), 0);
                columnPosition.x += Offset().x;
                column.SetPosition(columnPosition);

                float yColumnSize = rows * (2 * (DiscRadius + spaceAddition / 2));
                Vector2 columnSize = new Vector2(DiscRadius*2 + spaceAddition, yColumnSize);
                column.SetColliderSize(columnSize);
            }
        }

        private void BackGroundCreation()
        {
            GameObject backGroundGameObject = Instantiate(backGroundPrefab, Vector3.zero, Quaternion.identity);
            float boardBoxSize = 2 * (DiscRadius + spaceAddition / 2);
            backGroundGameObject.transform.localScale = new Vector2(columns * boardBoxSize, rows * boardBoxSize);
        }
        
        private void DiscCreation()
        {
            for (byte column = 0; column < columns; ++column)
            {
                for (byte row = 0; row < rows; ++row)
                {
                    GameObject discGameObject = Instantiate(discPrefab, Vector3.zero, Quaternion.identity);
                    discGameObject.TryGetComponent(out Disc disc);
                    
                    Vector2 discPosition = new Vector2(column * (DiscRadius*2 + spaceAddition), 
                                                        row * (DiscRadius*2 + spaceAddition));
                    discPosition += Offset();
                    
                    disc.SetPosition(discPosition);
                    _discs.Add(disc);
                }
            }
        }

        private Vector2 Offset()
        {
            // Middle Position
            Vector2 offset = Vector2.zero;
            offset.x -= (columns-1) * (DiscRadius + spaceAddition / 2);
            offset.y -=   (rows-1)  * (DiscRadius + spaceAddition / 2);

            return offset;
        }
    }
}