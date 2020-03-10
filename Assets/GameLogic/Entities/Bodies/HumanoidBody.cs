//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using Entities.Bodies.Damages;
//using Entities.Bodies.Injuries;
//using Entities.Bodies.Health;
//using Entities.Materials;

//namespace Entities.Bodies
//{
//    public class HumanoidBody : Body
//    {
//        public readonly static string HumanoidBodyName = "Humanoid Body";

//        public HumanoidBody() : base()
//        {
//            this.bodyPartBase = new HumanoidBodyBase();
//        }

//        override public string Name { get { return HumanoidBodyName; } }

//        override public EInjuryState GetInjuryState()
//        {
//            return bodyPartBase.GetInjuryState();
//        }
//    }

//    public abstract class HumanoidBodyPartBase : BodyPartBase
//    {
//        public readonly static string HumanoidBodyBaseName = "Humanoid";
//        override public string Name { get { return HumanoidBodyBaseName; } }
//        public HumanoidBodyPartBase(HPSystem hpSystem) : base(hpSystem) { }
//    }

//    public class HumanoidBodyBase : HumanoidBodyPartBase
//    {
//        public readonly static int HumanoidBodyBaseHP = 200;

//        Torso torso; // legs, arms, upper body
//        Head head; // neck, skull

//        public HumanoidBodyBase() : base(HPSystemFactory.GetHPSystem(HumanoidBodyBaseHP, EEntityMaterial.Flesh))
//        {
//            torso = (Torso)AddBodyPart(new Torso());
//            head = (Head)AddBodyPart(new Head());
//        }

//        override public EInjuryState GetInjuryState(List<BodyPart> bodyParts)
//        {
//            // TODO go over body parts
//            return EInjuryState.None;
//        }
//    }

//    public abstract class HumanoidBodyPart : BodyPart
//    {
//        public readonly static string HumanoidBodyPartName = "Humanoid Body";
//        override public string Name { get { return HumanoidBodyPartName; } }

//        public HumanoidBodyPart(HPSystem hpSystem) : base(hpSystem) { }
//    }

//    public class Torso : HumanoidBodyPartBase
//    {
//        public readonly static int TorsoHP = 200;

//        public readonly static string TorsoName = "Torso";
//        override public string Name { get { return TorsoName; } }

//        Spine spine;
//        Leg leftLeg, rightLeg;
//        Hand leftHand, rightHand;

//        public Torso() : base(HPSystemFactory.GetHPSystem(TorsoHP, EEntityMaterial.Flesh))
//        {
//            spine = (Spine)AddBodyPart(new Spine());
//            leftLeg = (Leg)AddBodyPart(new Leg());
//            rightLeg = (Leg)AddBodyPart(new Leg());
//            leftHand = (Hand)AddBodyPart(new Hand());
//            rightHand = (Hand)AddBodyPart(new Hand());
//        }

//        override public EInjuryState GetInjuryState(List<BodyPart> bodyParts)
//        {
//            // TODO go over body parts
//            return EInjuryState.None;
//        }
//    }

//    public class Head : HumanoidBodyPartBase
//    {
//        public readonly static int HeadHP = 50;

//        public readonly static string HeadName = "Head";
//        override public string Name { get { return HeadName; } }

//        Brain brain;
//        Nose nose;
//        Eye leftEye, rightEye;
//        Mouth mouth;
//        Ear leftEar, rightEar;

//        public Head() : base(HPSystemFactory.GetHPSystem(HeadHP, EEntityMaterial.Bone))
//        {
//            brain = (Brain)AddBodyPart(new Brain());
//            nose = (Nose)AddBodyPart(new Nose());
//            leftEye = (Eye)AddBodyPart(new Eye());
//            rightEye = (Eye)AddBodyPart(new Eye());
//            mouth = (Mouth)AddBodyPart(new Mouth());
//            leftEar = (Ear)AddBodyPart(new Ear());
//            rightEar = (Ear)AddBodyPart(new Ear());
//        }

//        override public EInjuryState GetInjuryState(List<BodyPart> bodyParts)
//        {
//            // TODO go over body parts
//            return EInjuryState.None;
//        }
//    }

//    public class Leg : HumanoidBodyPart
//    {
//        public readonly static int LegHP = 35;

//        public readonly static string LegName = "Leg";
//        override public string Name { get { return LegName; } }

//        public Leg() : base(HPSystemFactory.GetHPSystem(LegHP, EEntityMaterial.Bone)) { }
//    }

//    public class Hand : HumanoidBodyPart
//    {
//        public readonly static int HandHP = 20;

//        public readonly static string HandName = "Hand";
//        override public string Name { get { return HandName; } }
//        public Hand() : base(HPSystemFactory.GetHPSystem(HandHP, EEntityMaterial.Bone)) { }
//    }

//    public class Spine : HumanoidBodyPart
//    {
//        public readonly static int SpineHP = 40;

//        public readonly static string SpineName = "Spine";
//        override public string Name { get { return SpineName; } }
//        public Spine() : base(HPSystemFactory.GetHPSystem(SpineHP, EEntityMaterial.Bone)) { }
//    }

//    public class Nose : HumanoidBodyPart
//    {
//        public readonly static int NoseHP = 5;

//        public readonly static string NoseName = "Nose";
//        override public string Name { get { return NoseName; } }
//        public Nose() : base(HPSystemFactory.GetHPSystem(NoseHP, EEntityMaterial.Bone)) { }
//    }

//    public class Eye : HumanoidBodyPart
//    {
//        public readonly static int EyeHP = 2;

//        public readonly static string EyeName = "Eye";
//        override public string Name { get { return EyeName; } }
//        public Eye() : base(HPSystemFactory.GetHPSystem(EyeHP, EEntityMaterial.Flesh)) { }
//    }

//    public class Ear : HumanoidBodyPart
//    {
//        public readonly static int EarHP = 5;

//        public readonly static string EarName = "Ear";
//        override public string Name { get { return EarName; } }
//        public Ear() : base(HPSystemFactory.GetHPSystem(EarHP, EEntityMaterial.Flesh)) { }

//    }

//    public class Mouth : HumanoidBodyPart
//    {
//        public readonly static int MouthHP = 5;

//        public readonly static string MouthName = "Mouth";
//        override public string Name { get { return MouthName; } }
//        public Mouth() : base(HPSystemFactory.GetHPSystem(MouthHP, EEntityMaterial.Flesh)) { }
//    }

//    public class Brain : HumanoidBodyPart
//    {
//        public readonly static int BrainHP = 5;

//        public readonly static string BrainName = "Brain";
//        override public string Name { get { return BrainName; } }
//        public Brain() : base(HPSystemFactory.GetHPSystem(BrainHP, EEntityMaterial.Flesh)) { }
//    }
//}
