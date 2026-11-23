using UnityEngine;
namespace NeonHighway
{
    public class PlayerCar : MonoBehaviour
    {
        public Transform visual;
        public FollowCamera cameraRig;
        public Light underglow;
        public Renderer glowRenderer;
        public ParticleSystem exhaust;
        Rigidbody body;
        int lane;
        float lateralVelocity;
        MaterialPropertyBlock block;
        static readonly int Emission=Shader.PropertyToID("_EmissionColor");
        public int Lane=>lane;
        public void Teleport(Vector3 position)
        {
            if(body)body.position=position;
            transform.position=position;
        }
        public void ShiftOrigin(float offset)
        {
            Teleport(body.position-Vector3.forward*offset);
        }
        public void ChangeLane(int direction)
        {
            lane=Mathf.Clamp(lane+direction,-1,1);
        }
        public void ResetCar()
        {
            lane=0;
            lateralVelocity=0;
            transform.position=new Vector3(0,.05f,0);
            if(body)body.position=transform.position;
            visual.localRotation=Quaternion.identity;
            cameraRig.Snap();
        }
        void Awake()
        {
            body=GetComponent<Rigidbody>();
            block=new MaterialPropertyBlock();
        }
        void Update()
        {
            var game=GameManager.Instance;
            if(!game)return;
            float ratio=Mathf.InverseLerp(game.settings.startSpeed,game.settings.maxSpeed,game.Speed);
            underglow.intensity=Mathf.Lerp(3,12,ratio);
            block.SetColor(Emission,new Color(.02f,.75f,1)*Mathf.Lerp(3,11,ratio));
            glowRenderer.SetPropertyBlock(block);
            if(game.State!=GameState.Playing)
            {
                if(exhaust.isPlaying)exhaust.Stop();
                return;
            }
            if(!exhaust.isPlaying)exhaust.Play();
        }
        void FixedUpdate()
        {
            var game=GameManager.Instance;
            if(!game||game.State!=GameState.Playing)return;
            var p=body.position;
            float old=p.x;
            p.x=Mathf.SmoothDamp(p.x,lane*game.settings.laneWidth,ref lateralVelocity,.12f,game.settings.laneChangeSpeed);
            p.z+=game.Speed*Time.deltaTime;
            body.MovePosition(p);
            float steer=Mathf.Clamp((p.x-old)/Mathf.Max(.001f,Time.deltaTime),-10,10);
            visual.localRotation=Quaternion.Slerp(visual.localRotation,Quaternion.Euler(0,steer*1.4f,-steer*.65f),Time.deltaTime*10);
        }
    }
}
