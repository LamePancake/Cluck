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
        private float borderSize;
        private Vector4 lineColor;
        private float ambientIntensity;
        private Vector4 ambientColor;

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
            borderSize = 0.2f;
            lineColor = new Vector4(0, 0, 0, 1);
            ambientIntensity = 0.1f;
            ambientColor = new Vector4(1, 1, 1, 1);
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
            borderSize = 0.2f;
            lineColor = new Vector4(0, 0, 0, 1);
            ambientIntensity = 0.1f;
            ambientColor = new Vector4(1, 1, 1, 1);
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
            borderSize = 0.2f;
            lineColor = new Vector4(0, 0, 0, 1);
            ambientIntensity = 0.1f;
            ambientColor = new Vector4(1, 1, 1, 1);
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

        public void SetBoundingBox(BoundingBox bb)
        {
            entityBox = bb;
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

        public void SetAnimationPlayer(AnimationPlayer amp)
        {
            animationPlayer = amp;
        }

        public void SetAnimationPlayer()
        {
            animationPlayer = null;
        }

        public Effect GetEffect()
        {
            return customEffect;
        }

        public void SetBorderSize(float f)
        {
            borderSize = f;
        }

        public float GetBorderSize()
        {
            return borderSize;
        }

        public Vector4 GetLineColor()
        {
            return lineColor;
        }

        public void SetLineColor(Vector4 c)
        {
            lineColor = c;
        }

        public void SetAmbientIntensity(float f)
        {
            ambientIntensity = f;
        }

        public float GetAmbientIntensity()
        {
            return ambientIntensity;
        }

        public Vector4 GetAmbientColor()
        {
            return ambientColor;
        }

        public void SetAmbientColor(Vector4 c)
        {
            ambientColor = c;
        }

        public void SetEffect(Effect fx)
        {
            customEffect = fx;
        }
    }
}
