using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.DataModel
{
    public interface IResourceProvider
    {
        event EventHandler<float> OnResource;

        event Action<IResourceProvider> OnDeath;
    }
}
