using System.Collections.Generic;
using UnityEngine;
namespace NeonHighway
{
    public class RoadGenerator : MonoBehaviour
    {
        public PoolManager pools;
        public RunnerSettings settings;
        public PlayerCar player;
        readonly Queue<GameObject> segments=new();
        float nextSegment,nextRow;
        int rowIndex;
        System.Random random;
        public int ActiveSegments=>segments.Count;
        public void ResetRoad()
        {
            pools.ReturnAll();
            segments.Clear();
            nextSegment=-settings.segmentLength;
            nextRow=85;
            rowIndex=0;
            random=new System.Random(2026);
            for(int i=0;i<9;i++)AddSegment();
        }
        void AddSegment()
        {
            segments.Enqueue(pools.Rent("Road",new Vector3(0,0,nextSegment)));
            nextSegment+=settings.segmentLength;
        }
        void Update()
        {
            var game=GameManager.Instance;
            if(!game||game.State!=GameState.Playing)return;
            float z=player.transform.position.z;
            while(segments.Count>0&&segments.Peek().transform.position.z+settings.segmentLength<z-25)
            {
                segments.Dequeue().GetComponent<PooledObject>().Release();
                AddSegment();
            }
            while(nextRow<z+340)
            {
                SpawnRow(game);
                nextRow+=settings.RowSpacing(game.Elapsed);
            }
            if(z>5000)
            {
                const float shift=4800;
                pools.ShiftOrigin(shift);
                player.ShiftOrigin(shift);
                nextRow-=shift;
                nextSegment-=shift;
                player.cameraRig.Snap();
                foreach(var p in game.effects.GetComponentsInChildren<ParticleSystem>())p.Clear();
            }
        }
        void SpawnRow(GameManager game)
        {
            int safeLane = random.Next(-1, 2);
            int obstacles = random.NextDouble() < settings.DoubleObstacleChance(game.Elapsed) ? 2 : 1;

            // Fill the remaining lanes from left to right, keeping the pickup lane open.
            bool moving = rowIndex % 3 == 1;
            for (int lane = -1; lane <= 1 && obstacles > 0; lane++)
            {
                if (lane == safeLane)
                    continue;

                string kind = moving ? "Moving" : "Stationary";
                pools.Rent(kind, new Vector3(lane * settings.laneWidth, .05f, nextRow));
                obstacles--;
                moving = false;
            }

            float pickupX = safeLane * settings.laneWidth;
            if (rowIndex % 3 == 2)
            {
                pools.Rent("Fuel", new Vector3(pickupX, 1.1f, nextRow + 12));
            }
            else
            {
                for (int coin = 0; coin < 4; coin++)
                    pools.Rent("Coin", new Vector3(pickupX, 1.15f, nextRow + coin * 4));
            }
            rowIndex++;
        }
    }
}
