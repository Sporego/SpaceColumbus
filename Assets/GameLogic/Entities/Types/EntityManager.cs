using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public static class EntityManager
    {
        private static Dictionary<int, Entity> Entities;

        public static void RegisterEntity(Entity entity)
        {
            int id = entity.GetId();
            if (!Entities.ContainsKey(id))
                Entities.Add(id, entity);
        }
        
        public static void UnregisterEntity(Entity entity)
        {
            int id = entity.GetId();
            if (Entities.ContainsKey(id))
                Entities.Remove(id);
        }

        public static void Initialize()
        {
            Entities = new Dictionary<int, Entity>();
        }

        public static void Update()
        {
            // TODO: cluster entities
        }



    }
}
