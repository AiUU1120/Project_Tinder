/*
* @Author: AiUU
* @Description: SkillMaster 编辑器配置
* @AkanyaTech.SkillMaster
*/

namespace AkanyaTools.SkillMaster.Editor.EditorWindow
{
    public sealed class SkillMasterEditorConfig
    {
        /// <summary>
        /// 帧单位宽度
        /// </summary>
        public int frameUnitWidth = 10;

        /// <summary>
        /// 默认帧单位宽度
        /// </summary>
        public const int standard_frame_unit_width = 10;

        /// <summary>
        /// 最大缩放级别
        /// </summary>
        public const int max_frame_width_level = 10;

        /// <summary>
        /// 最小缩放级别
        /// </summary>
        public const float min_frame_width_level = 0.5f;

        /// <summary>
        /// 默认帧率
        /// </summary>
        public const float default_frame_rate = 30;
    }
}