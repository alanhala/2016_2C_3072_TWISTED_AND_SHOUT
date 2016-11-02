using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Collision;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class CarCollisionDetection
    {
        private Car car;
        private TgcScene scene;

        public CarCollisionDetection(Car car, TgcScene scene)
        {
            this.car = car;
            this.scene = scene;
        }

        public Boolean hasCollisioned()
        {
            foreach (var mesh in scene.Meshes)
            {
                var sceneMeshBoundingBox = mesh.BoundingBox;

                if (TgcCollisionUtils.testObbAABB(car.boundingBox, sceneMeshBoundingBox))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
