using SampSharp.GameMode;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.Definitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.Item
{
    public class ItemType : SampSharp.GameMode.Pools.IdentifiedPool<ItemType>
    {
        string  __name;
        int     __model; 
        Vector3 __defaultRot;
        Vector3 __attachPos;
        Vector3 __attachRot;
        Vector3 __scale;
        float   __zoffset;
        bool    __useCarryAnim;
        Bone     __attachBone;

        public ItemType(string name, int model, Vector3 defaultRot = new Vector3(), float zoffset = 0.0f, Vector3 attachPos = new Vector3(), Vector3 attachRot = new Vector3(), Vector3 scale = new Vector3(), bool usecarryanim = false, Bone bone = Bone.RightHand)
        {
            __name = name;
            __model = model;
            __defaultRot = defaultRot;
            __zoffset = zoffset;
            __attachPos = attachPos;
            __attachRot = attachRot;
            __attachBone = bone;

            __scale = scale.IsEmpty ? new Vector3(1.0, 1.0, 1.0) : scale;
            __useCarryAnim = usecarryanim;
        }

        public string Name { get => __name; set => __name = value; }
        public int Model { get => __model; set => __model = value; }
        public Vector3 DefaultRot { get => __defaultRot; set => __defaultRot = value; }
        public Vector3 AttachPos { get => __attachPos; set => __attachPos = value; }
        public Vector3 AttachRot { get => __attachRot; set => __attachRot = value; }
        public Vector3 Scale { get => __scale; set => __scale = value; }
        public float Zoffset { get => __zoffset; set => __zoffset = value; }
        public bool UseCarryAnim { get => __useCarryAnim; set => __useCarryAnim = value; }
        public Bone AttachBone { get => __attachBone; set => __attachBone = value; }
    }
}
