﻿using GenshinPray.Common;
using GenshinPray.Models;
using GenshinPray.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinPray.Service
{
    public class RolePrayService : BasePrayService
    {
        /// <summary>
        /// 根据名称随机实际补给项目
        /// </summary>
        /// <param name="prayRecord"></param>
        /// <param name="ySUpItem"></param>
        /// <param name="floor180Surplus"></param>
        /// <param name="floor20Surplus"></param>
        /// <returns></returns>
        protected override YSPrayRecord getPrayRecord(YSPrayRecord prayRecord, YSUpItem ySUpItem, int floor180Surplus, int floor20Surplus)
        {
            List<YSGoodsItem> Star5UpList = ySUpItem == null ? SiteConfig.RoleStar5UpList : ySUpItem.Star5UpList;
            List<YSGoodsItem> Star5NonUpList = ySUpItem == null ? SiteConfig.RoleStar5NonUpList : ySUpItem.Star5NonUpList;
            List<YSGoodsItem> Star4UpList = ySUpItem == null ? SiteConfig.RoleStar4UpList : ySUpItem.Star4UpList;
            List<YSGoodsItem> Star4NonUpList = ySUpItem == null ? SiteConfig.RoleStar4NonUpList : ySUpItem.Star4NonUpList;
            if (prayRecord.GoodsItem.GoodsName == "5星物品")
            {
                bool isGetUp = floor180Surplus < 90 ? true : RandomHelper.getRandomBetween(1, 100) > 50;
                return isGetUp ? getRandomGoodsInList(Star5UpList) : getRandomGoodsInList(Star5NonUpList);
            }
            if (prayRecord.GoodsItem.GoodsName == "4星物品")
            {
                bool isGetUp = floor20Surplus < 10 ? true : RandomHelper.getRandomBetween(1, 100) > 50;
                return isGetUp ? getRandomGoodsInList(Star4UpList) : getRandomGoodsInList(Star4NonUpList);
            }
            if (prayRecord.GoodsItem.GoodsName == "3星物品")
            {
                return getRandomGoodsInList(SiteConfig.ArmStar3PermList);
            }
            return prayRecord;
        }

        /// <summary>
        /// 判断一个项目是否up项目
        /// </summary>
        /// <param name="goodsItem"></param>
        /// <returns></returns>
        protected override bool isUpItem(YSGoodsItem goodsItem)
        {
            if (SiteConfig.RoleStar5UpList.Where(m => m.GoodsName == goodsItem.GoodsName).Count() > 0) return true;
            if (SiteConfig.RoleStar4UpList.Where(m => m.GoodsName == goodsItem.GoodsName).Count() > 0) return true;
            return false;
        }

    }
}
