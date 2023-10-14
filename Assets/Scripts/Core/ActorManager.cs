using System.Collections.Generic;
using Board;
using UnityEngine;

namespace Core
{
    public class ActorManager : MonoBehaviour
    {
        [SerializeField] private Color color;
        private List<Disc> _discs;

        // Reformat:
        //      remove actorManager, PlayerManager.
        //      move evaluatewincondition to the gamemanager
        //      create a struct for actor (id, color)
        //      AI inherit as an actor (id, color, evaluation function - script).
    

        public bool EvaluateWinCondition()
        {
            // Horizontal Check 
            for(int i = 0; i < _discs.Count; ++i)
            {
                bool find1 = _discs.Find(disc => 
                    disc.GamePosition.Value == _discs[i].GamePosition.Value + 1 && 
                    disc.GamePosition.Key == _discs[i].GamePosition.Key);
            
                bool find2 = _discs.Find(disc => 
                    disc.GamePosition.Value == _discs[i].GamePosition.Value + 2 && 
                    disc.GamePosition.Key == _discs[i].GamePosition.Key);
            
                bool find3 = _discs.Find(disc => 
                    disc.GamePosition.Value == _discs[i].GamePosition.Value + 3 && 
                    disc.GamePosition.Key == _discs[i].GamePosition.Key);

                if (find1 && find2 && find3) return true;
            }
        
            // Vertical Check 
            for(int i = 0; i < _discs.Count; ++i)
            {
                bool find1 = _discs.Find(disc => 
                    disc.GamePosition.Value == _discs[i].GamePosition.Value && 
                    disc.GamePosition.Key == _discs[i].GamePosition.Key + 1);
            
                bool find2 = _discs.Find(disc => 
                    disc.GamePosition.Value == _discs[i].GamePosition.Value && 
                    disc.GamePosition.Key == _discs[i].GamePosition.Key + 2);
            
                bool find3 = _discs.Find(disc => 
                    disc.GamePosition.Value == _discs[i].GamePosition.Value && 
                    disc.GamePosition.Key == _discs[i].GamePosition.Key + 3);

                if (find1 && find2 && find3) return true;
            }
        
            // Ascending Diagonal Check 
            for(int i = 0; i < _discs.Count; ++i)
            {
                bool find1 = _discs.Find(disc => 
                    disc.GamePosition.Value == _discs[i].GamePosition.Value + 1 && 
                    disc.GamePosition.Key == _discs[i].GamePosition.Key - 1);
            
                bool find2 = _discs.Find(disc => 
                    disc.GamePosition.Value == _discs[i].GamePosition.Value + 2 && 
                    disc.GamePosition.Key == _discs[i].GamePosition.Key - 2);
            
                bool find3 = _discs.Find(disc => 
                    disc.GamePosition.Value == _discs[i].GamePosition.Value + 3 && 
                    disc.GamePosition.Key == _discs[i].GamePosition.Key - 3);

                if (find1 && find2 && find3) return true;
            }
        
        
            // Descending Diagonal Check
            for(int i = 0; i < _discs.Count; ++i)
            {
                bool find1 = _discs.Find(disc => 
                    disc.GamePosition.Value == _discs[i].GamePosition.Value - 1 && 
                    disc.GamePosition.Key == _discs[i].GamePosition.Key - 1);
            
                bool find2 = _discs.Find(disc => 
                    disc.GamePosition.Value == _discs[i].GamePosition.Value - 2 && 
                    disc.GamePosition.Key == _discs[i].GamePosition.Key - 2);
            
                bool find3 = _discs.Find(disc => 
                    disc.GamePosition.Value == _discs[i].GamePosition.Value - 3 && 
                    disc.GamePosition.Key == _discs[i].GamePosition.Key - 3);

                if (find1 && find2 && find3) return true;
            }
         
            return false;
        }

        public void AddDiscToActor(Disc disc)
        {
            // Change the disc color 
            disc.SetColor(color);
            _discs.Add(disc);
        }
    
        // Start is called before the first frame update
        void Start()
        {
            _discs = new List<Disc>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public virtual void OnGameTurnChange()
        {
            // throw new System.NotImplementedException();
        
            // Turn column interaction on and off if it is the player/AI's turn
            Debug.Log(gameObject.name);
        }
    }
}
