using Assets.DataModel;
using Assets.DataModel.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    public Vector2 latlong;
    public int xPos;
    public int yPos;

    public bool occupied = false;
    public bool pathable;

    public float travelCost;

    public Tile(Vector2 latlong, bool pathable, float travelCost)
    {
        this.latlong = latlong;
        this.pathable = pathable;
        this.travelCost = travelCost;
    }


    public virtual void Tick(MapController controller)
    {
    }
}
[System.Serializable]
public class Resource : Tile
{
    public float resources;
    public float replinishrate;

    public Resource(Vector2 latlong, bool pathable, float travelCost, float resources, float replinishrate) :
        base(latlong, pathable, travelCost)
    {
        this.resources = resources;
        this.replinishrate = replinishrate;
    }

    public override void Tick(MapController controller)
    {
        base.Tick(controller);
    }
}
[System.Serializable]
public class City : Tile
{
    public Guid Owner;

    public float Population;
    public float Consumption;

    public City(Vector2 latlong, bool pathable, float travelCost, float population, float consumptionrate, Guid owner) :
        base(latlong, pathable, travelCost)
    {
        this.Population = population;
        this.Consumption = consumptionrate;
        this.Owner = owner;
    }

    public override void Tick(MapController controller)
    {
        base.Tick(controller);
    }
}

[System.Serializable]
public class TileController : MonoBehaviour, IResourceProvider
{
    ResourceContainer providers;

    public MapController controller;

    public Vector2 LatLong;

    public List<GameObject> Visuals;
    public List<GameObject> ResourceVisuals;

	public float Rate = 0.5f;
    public Color OriginalCityColor;

    // current resource value
    private float _resources = 100;
    public float Resources {
        get
        {
            return _resources;
        }
        set
        {
            if(_resources > 0)
                _resources = value;
            if (_resources > 100)
                _resources = 100;
        }
    }

    // resource states
    public enum ResourceState
    {
        Dead,
        Low,
        Medium,
        High
    };

    public enum TileType
    {
        Resource,
        Mountain,
        Ocean,
        City
    };

    public ResourceState PreviousResources;

    // get ResourceState from current resource
    // value
    public ResourceState CurrentResources
    {
        get
        {
            if (Resources < 10) return ResourceState.Dead;
            if (Resources < 30) return ResourceState.Low;
            if (Resources < 60) return ResourceState.Medium;
            return ResourceState.High;
        }
    }

    public TileType CurrentType = TileType.Resource;

    public Tile myTile;

    public event EventHandler<float> OnResource;

    public event Action<IResourceProvider> OnDeath;

    private void Start()
    {
        PreviousResources = ResourceState.High;

        providers = new ResourceContainer();
        OnResource += OnProviderResource;
        if (GetComponent<TimeUpdate>() is TimeUpdate timer)
        {
            timer.OnTick += OnTick;
        };
    }

    private void OnProviderResource(object sender, float e)
    {
        Resources += e;
    }

    private void OnTick(object sender, float e)
    {
        if (CurrentType == TileType.Resource)
        {
            if (Resources < 100)
            {

                // regenerate some resources
                Resources += Rate;
                

                // get the number of harvesters and remove one for each
                // harvester.
                float harvesters = OnResource.GetInvocationList().Length;
                Resources -= (harvesters - 1);

                // send one resource to each harvester
                OnResource.Invoke(this, 1f);
            }
        };
        UpdateVisualMode();

        // if this is currently a city tile and there are less than
        // 4 providers, try to get more.
        if (myTile is City city)
        {
            OnResource.Invoke(this, 50f);
            

            if (providers.Length < 4)
            {
                var x = city.xPos;
                var y = city.yPos;

                var neighbors = controller.GetNeighbors(x, y, city.Owner);
                foreach (var neighbor in neighbors)
                {
                    // doesn't add if it is already in there
                    providers.Add(neighbor);
                }
            }

            Resources -= 2;

            if (Resources <= 0)
            {
                Change(TileType.Resource);
            }
        }
    }

    private void UpdateVisualMode()
    {
        if (PreviousResources != CurrentResources)
        {
            ResourceVisuals[(int)PreviousResources].SetActive(false);
        }

        if (CurrentType == TileType.Resource)
        {
            ResourceVisuals[(int)CurrentResources].SetActive(true);
        }
        else
        {
            Visuals[(int)CurrentType - 1].SetActive(true);
            //modelRef.SetActive(true);
        }

        if (Resources == 0)
        {
            OnDeath?.Invoke(this);
        }
        PreviousResources = CurrentResources;
    }

    public void ChangeCityColor(Color color)
    {
        var modelRef = Visuals[2];
        foreach(var renderer in modelRef.GetComponentsInChildren<Renderer>())
        {
            renderer.material.color = color;
        }

    }

    public void Change(TileType type)
    {
        if (CurrentType == TileType.Resource)
        {
            ResourceVisuals[(int)CurrentResources].SetActive(false);
        }
        else
        {
            Visuals[(int)CurrentType - 1].SetActive(false);
        }

        CurrentType = type;

        if (CurrentType == TileType.Resource)
        {
            ResourceVisuals[(int)CurrentResources].SetActive(true);
        }
        else
        {
            Visuals[(int)CurrentType - 1].SetActive(true);
        }
    }
}
