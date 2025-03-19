/*
 * @Author: AiUU
 * @Description: 数据管理器
 * @AkanyaTech.Tinder
 */

using Data.GameCore;
using Data.GameCore.Save;
using JKFrame;

namespace Data
{
    public static class DataManager
    {
        static DataManager()
        {
            LoadSaveData();
        }

        public static PlayerData playerData { get; private set; }

        public static bool isHaveSaveData { get; private set; }

        private static void LoadSaveData()
        {
            var saveItem = SaveSystem.GetSaveItem(0);
            isHaveSaveData = saveItem != null;
        }

        /// <summary>
        /// 创建存档数据
        /// </summary>
        public static void CreateSaveData()
        {
            if (isHaveSaveData)
            {
                SaveSystem.DeleteAllSaveItem();
            }
            SaveSystem.CreateSaveItem();
            InitPlayerData();
            SavePlayerData();
        }

        /// <summary>
        /// 加载当前存档数据
        /// </summary>
        public static void LoadCurrentSaveData()
        {
        }

        /// <summary>
        /// 初始化玩家数据
        /// </summary>
        public static void InitPlayerData()
        {
            playerData = new PlayerData
            {
                curWeaponId = "Katana_Normal",
                skillPoint = 1000,
                skillLearnedDatasDic = new Serialized_Dic<string, SkillLearnedDatas>(),
                shortcutSkillDataDic = new Serialized_Dic<string, ShortcutSkillData>(),
            };
            // 初始化技能学习数据
            playerData.skillLearnedDatasDic.Dictionary.Add("Katana_Normal", new SkillLearnedDatas());
            playerData.skillLearnedDatasDic.Dictionary.Add("GreatSword_Normal", new SkillLearnedDatas());
            // 初始化快捷栏技能数据
            playerData.shortcutSkillDataDic.Dictionary.Add("Katana_Normal", new ShortcutSkillData());
            playerData.shortcutSkillDataDic.Dictionary.Add("GreatSword_Normal", new ShortcutSkillData());
        }

        /// <summary>
        /// 保存玩家数据
        /// </summary>
        public static void SavePlayerData()
        {
            SaveSystem.SaveObject(playerData);
        }
    }
}