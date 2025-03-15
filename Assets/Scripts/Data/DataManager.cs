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
                skillLearnedDatas = new SkillLearnedDatas()
                {
                    skillPoint = 999,
                }
            };
            playerData.skillLearnedDatas.learnedSkillsDic.Dictionary.Add(0, new SkillLearnedData() { level = 1 });
            playerData.skillLearnedDatas.learnedSkillsDic.Dictionary.Add(1, new SkillLearnedData() { level = 2 });
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