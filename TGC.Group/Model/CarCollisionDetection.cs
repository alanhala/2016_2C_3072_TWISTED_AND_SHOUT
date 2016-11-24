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
        private List<TgcMesh> meshes;

        public CarCollisionDetection(Car car, List<TgcMesh> meshes)
        {
            this.car = car;
            this.meshes = meshes;
        }

        public Boolean hasCollisioned()
        {
            foreach (var mesh in meshes)
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
