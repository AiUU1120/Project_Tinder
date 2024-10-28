/*
* @Author: AiUU
* @Description: SkillMaster 判定帧事件
* @AkanyaTech.SkillMaster
*/

namespace AkanyaTools.SkillMaster.Runtime.Data.Event
{
    public sealed class SkillDetectionFrameEvent : SkillFrameEventBase
    {
#if UNITY_EDITOR
        public string trackName = "Detection Track";
#endif

        public int frameIndex = 0;

        public int durationFrame = 10;

        public DetectionDataBase detectionData;

#if UNITY_EDITOR
        public DetectionType detectionType
        {
            get
            {
                switch (detectionData)
                {
                    case null:
                        return DetectionType.None;
                    case WeaponDetectionData:
                        return DetectionType.Weapon;
                    case BoxDetectionData:
                        return DetectionType.Box;
                    case SphereDetectionData:
                        return DetectionType.Sphere;
                    default:
                        return DetectionType.None;
                }
            }
            set
            {
                if (value == detectionType)
                {
                    return;
                }
                switch (value)
                {
                    case DetectionType.None:
                        detectionData = null;
                        break;
                    case DetectionType.Weapon:
                        detectionData = new WeaponDetectionData();
                        break;
                    case DetectionType.Box:
                        detectionData = new BoxDetectionData();
                        break;
                    case DetectionType.Sphere:
                        detectionData = new SphereDetectionData();
                        break;
                }
            }
        }
    }
#endif

    public enum DetectionType
    {
        None,
        Weapon,
        Box,
        Sphere
    }

    /// <summary>
    /// 检测基类
    /// </summary>
    public abstract class DetectionDataBase
    {
    }

    /// <summary>
    /// 形状检测基类
    /// </summary>
    public abstract class ShapeDetectionDataBase : DetectionDataBase
    {
    }

    /// <summary>
    /// 武器检测
    /// </summary>
    public sealed class WeaponDetectionData : DetectionDataBase
    {
    }

    /// <summary>
    /// 盒型检测
    /// </summary>
    public sealed class BoxDetectionData : ShapeDetectionDataBase
    {
    }

    /// <summary>
    /// 球形检测
    /// </summary>
    public sealed class SphereDetectionData : ShapeDetectionDataBase
    {
    }
}