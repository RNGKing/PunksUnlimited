using Assets.DataModel.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PunksUnlimited.DataModel
{
    public class CharacterData : MonoBehaviour
    {
        public MapController mapController;

        Vector2 CurrentGridLocation;
        Guid CharacterGuid;
        Color playercolor = Color.white;

        public int PlayerNumber;

        public int PlayerId;

        public float ActivityEnergy = 100f;
        public float RechargeRate = 10f;
        public float MaxActivityEnergy = 100f;

        public float CurrentResources = 0f;
        public float GatherAmount = 4f;

        Color CharacterColor;

        ResourceContainer resources;

        #region Events

        public delegate void FailedMove();
        public event FailedMove OnFailedMove;

        public delegate void EnergyUpdate(float CurrentValue);
        public event EnergyUpdate OnEnergyUpdate;

        public delegate void ResourceUpdate(float CurrentValue);
        public event ResourceUpdate OnResourceUpdate;

        #endregion

        public bool Active = false;

        void Awake()
        {
            resources = new ResourceContainer();
            resources.OnResource += OnHarvest;
            mapController = GameObject.Find("MapControl").GetComponent<MapController>();
            CharacterGuid = Guid.NewGuid();
        }

        private void OnHarvest(object sender, float e)
        {
            CurrentResources += e;
            OnResourceUpdate?.Invoke(CurrentResources);
        }

        public void Initialize(Vector2 initPosition, int PlayerId, int PlayerNumber)
        {   
            CurrentGridLocation = initPosition;
            this.PlayerId = PlayerId;
            this.PlayerNumber = PlayerNumber;
            var components = GetComponents<TimeUpdate>();
            List<TimeUpdate> updates = new List<TimeUpdate>();

            CharacterColor = GenerateRandomColor();

            GetComponentInChildren<Renderer>().material.color = CharacterColor;

            foreach (var comp in components)
            {
                updates.Add(comp);
            }
            updates.Find(x => x.Name.Equals("EnergyTimer")).OnTick += EnergyTimer_OnTick;
        }

        private Color GenerateRandomColor()
        {
            var r = UnityEngine.Random.Range(0f, 1.0f);
            var g = UnityEngine.Random.Range(0f, 1.0f);
            var b = UnityEngine.Random.Range(0f, 1.0f);
            return new Color(r, g, b);
        }
        
        private void EnergyTimer_OnTick(object sender, float e)
        {
            ActivityEnergy += RechargeRate;
            if(ActivityEnergy >= MaxActivityEnergy)
            {
                ActivityEnergy = MaxActivityEnergy;
            }
            OnEnergyUpdate?.Invoke(ActivityEnergy);
        }

        private void ConsumeEnergy(float value)
        {
            ActivityEnergy -= value;
            if(ActivityEnergy <= 0f)
            {
                ActivityEnergy = 0f;
            }
            OnEnergyUpdate?.Invoke(ActivityEnergy);
        }

        public void ExecuteCommand(CommandData command)
        {
            if(command is MoveData moveData)
            {
                MoveCharacter(moveData.Move);
            }
            else if (command is BuildData buildData)
            {
                BuildOnTile(buildData);
            }
        }

        private void BuildOnTile(BuildData buildData)
        {
            if(ActivityEnergy >= 50f)
            {
                ConsumeEnergy(50f);
                var provider = mapController.BuildOnTile(CurrentGridLocation, CharacterGuid, CharacterColor);
                resources.Add(provider);
            }
        }

        private void MoveCharacter(Vector2 move)
        {
            var newTransform = CurrentGridLocation + move;
            if (mapController.CheckValid(newTransform, CharacterGuid))
            {
                var cost = mapController.CheckCost(newTransform);
                if (ActivityEnergy >= cost)
                {
                    ConsumeEnergy(cost);
                    mapController.Changeloc(CurrentGridLocation, newTransform);
                    transform.position = new Vector3(newTransform.x, 0, newTransform.y);
                    CurrentGridLocation = new Vector2(newTransform.x, newTransform.y);
                }
            }
            else
            {
                OnFailedMove?.Invoke();
            }
        }
    }
}
