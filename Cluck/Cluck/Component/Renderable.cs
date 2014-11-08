using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SkinnedModel;

namespace Cluck
{
    public class Renderable : Component
    {
        private Model entityModel;
        private Matrix worldMatrix;
        private Texture2D entityTexture;
        private BoundingSphere entitySphere;
        private BoundingBox entityBox;
        private Effect customEffect;
        private Boolean borderMutable;
        private Boolean inRange;

        private AnimationPlayer animationPlayer;

        public Renderable(Model model, Texture2D texture, BoundingSphere sphere, AnimationPlayer a, Effect effect)
            : base((int)component_flags.renderable)
        {
            entityModel = model;
            worldMatrix = Matrix.Identity;
            entityTexture = texture;
            entitySphere = sphere;
            animationPlayer = a;
            customEffect = effect;
            borderMutable = false;
            inRange = false;
        }

        public Renderable(Model model, Texture2D texture, BoundingSphere sphere, Effect effect)
            : base((int)component_flags.renderable)
        {
            entityModel = model;
            worldMatrix = Matrix.Identity;
            entityTexture = texture;
            entitySphere = sphere;
            animationPlayer = null;
            customEffect = effect;
            borderMutable = false;
            inRange = false;
        }

        public Renderable(Model model, Texture2D texture, BoundingBox box, Effect effect)
            : base((int)component_flags.renderable)
        {
            entityModel = model;
            worldMatrix = Matrix.Identity;
            entityTexture = texture;
            entityBox = box;
            animationPlayer = null;
            customEffect = effect;
            borderMutable = false;
            inRange = false;
        }

        public Model GetModel()
        {
            return entityModel;
        }

        public void SetModel(Model model)
        {
            entityModel = model;
        }

        public Matrix GetMatrix()
        {
            return worldMatrix;
        }

        public void SetMatrix(Matrix mat)
        {
            worldMatrix = mat;
        }

        public Texture2D GetTexture()
        {
            return entityTexture;
        }

        public void SetTexture(Texture2D texture)
        {
            entityTexture = texture;
        }

        public BoundingSphere GetBoundingSphere()
        {
            return entitySphere;
        }

        public void SetBoundingSphere(BoundingSphere boundsphere)
        {
            entitySphere = boundsphere;
        }

        public BoundingBox GetBoundingBox()
        {
            return entityBox;
        }

        public void SetBoundingSphere(BoundingBox boundbox)
        {
            entityBox = boundbox;
        }

        public AnimationPlayer GetAnimationPlayer()
        {
            return animationPlayer;
        }

        public Effect GetEffect()
        {
            return customEffect;
        }

        public void SetBorderMutable(Boolean bm)
        {
            borderMutable = bm;
        }

        public Boolean GetBorderMutable()
        {
            return borderMutable;
        }

        public void SetInRange(Boolean ir)
        {
            inRange = ir;
        }

        public Boolean GetInRange()
        {
            return inRange;
        }
    }
}
