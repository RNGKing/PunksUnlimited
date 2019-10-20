using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.DataModel.Resources
{
    public class ResourceContainer
    {
        private List<IResourceProvider> Providers;

        public EventHandler<float> OnResource;

        public int Length { get { return Providers.Count; } }

        public ResourceContainer() {
            Providers = new List<IResourceProvider>();
        }

        public void Add(IResourceProvider provider)
        {
            Remove(provider);
            provider.OnDeath += OnProviderDeath;
            provider.OnResource += OnProviderResource;
            Providers.Add(provider);
        }

        public void Remove(IResourceProvider provider)
        {
            try
            {
                provider.OnDeath -= OnProviderDeath;
                provider.OnResource -= OnProviderResource;
            }
            catch (Exception) { }
            Providers.Remove(provider);
        }

        private void OnProviderResource(object sender, float e)
        {
            OnResource?.Invoke(this,e);
        }

        private void OnProviderDeath(object sender)
        {
            if(sender is IResourceProvider provider)
            {
                Remove(provider);
            }
        }
    }
}
