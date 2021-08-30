﻿using GenshinPray.Common;
using GenshinPray.Dao;
using GenshinPray.Exceptions;
using GenshinPray.Models;
using GenshinPray.Models.Dto;
using GenshinPray.Models.PO;
using GenshinPray.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenshinPray.Service
{
    public class GoodsService
    {
        private GoodsDao goodsDao;

        public GoodsService()
        {
            this.goodsDao = new GoodsDao();
        }

        /// <summary>
        /// 加载蛋池数据到内存
        /// </summary>
        public void loadYSPrayItem()
        {
            SiteConfig.ArmStar3PermList = changeToYSGoodsItem(goodsDao.getPermGoods(YSGoodsType.武器, YSRareType.三星));//三星常驻武器
            SiteConfig.ArmStar4PermList = changeToYSGoodsItem(goodsDao.getPermGoods(YSGoodsType.武器, YSRareType.四星));//四星常驻武器
            SiteConfig.RoleStar4PermList = changeToYSGoodsItem(goodsDao.getPermGoods(YSGoodsType.角色, YSRareType.四星));//四星常驻角色
            SiteConfig.RoleStar5PermList = changeToYSGoodsItem(goodsDao.getPermGoods(YSGoodsType.角色, YSRareType.五星));//五星常驻角色

            List<GoodsPO> rolePondList = goodsDao.getByPondType((int)YSPondType.角色);
            List<YSGoodsItem> roleItemList = changeToYSGoodsItem(rolePondList);
            List<YSGoodsItem> roleStar5UpList = roleItemList.Where(m => m.RareType == YSRareType.五星).ToList();
            List<YSGoodsItem> roleStar4UpList = roleItemList.Where(m => m.RareType == YSRareType.四星).ToList();
            List<YSGoodsItem> roleStar5NonUpList = getStar5NonUpList(SiteConfig.RoleStar5PermList, roleStar5UpList);
            List<YSGoodsItem> roleStar4NonUpList = getStar4NonUpList(SiteConfig.RoleStar4PermList, roleStar4UpList);
            SiteConfig.RoleStar5UpList = roleStar5UpList;
            SiteConfig.RoleStar4UpList = roleStar4UpList;
            SiteConfig.RoleStar5NonUpList = roleStar5NonUpList;
            SiteConfig.RoleStar4NonUpList = roleStar4NonUpList;
        }

        public YSUpItem GetUpItem(CustomUpDto customUp)
        {
            List<YSGoodsItem> Star5UpList = new List<YSGoodsItem>();
            foreach (var goodsName in customUp.Star5UpItems)
            {
                if (string.IsNullOrWhiteSpace(goodsName)) throw new GoodsNotFoundException($"UP项目不能指定为null或空字符串");
                GoodsPO goodsPO = goodsDao.getPermGoods(goodsName);
                if (goodsPO == null) throw new GoodsNotFoundException($"找不到与{goodsName}对应的物品");
                if (goodsPO.RareType != YSRareType.五星) throw new ParamException($"{goodsName}不是{Enum.GetName(typeof(YSRareType), YSRareType.五星)}物品");
                Star5UpList.Add(changeToYSGoodsItem(goodsPO));
            }

            List<YSGoodsItem> Star4UpList = new List<YSGoodsItem>();
            foreach (var goodsName in customUp.Star4UpItems)
            {
                if (string.IsNullOrWhiteSpace(goodsName)) throw new GoodsNotFoundException($"UP项目不能指定为null或空字符串");
                GoodsPO goodsPO = goodsDao.getPermGoods(goodsName);
                if (goodsPO == null) throw new GoodsNotFoundException($"找不到与{goodsName}对应的物品");
                if (goodsPO.RareType != YSRareType.四星) throw new ParamException($"{goodsName}不是{Enum.GetName(typeof(YSRareType), YSRareType.四星)}物品");
                Star4UpList.Add(changeToYSGoodsItem(goodsPO));
            }

            YSUpItem ySUpItem = new YSUpItem();
            ySUpItem.Star5UpList = Star5UpList;
            ySUpItem.Star5NonUpList = getStar5NonUpList(SiteConfig.RoleStar5PermList, Star5UpList);
            ySUpItem.Star4UpList = Star4UpList;
            ySUpItem.Star4NonUpList = getStar4NonUpList(SiteConfig.RoleStar4PermList, Star4UpList);
            return ySUpItem;
        }

        /// <summary>
        /// 获取5星非up列表
        /// </summary>
        /// <returns></returns>
        private List<YSGoodsItem> getStar5NonUpList(List<YSGoodsItem> star5List, List<YSGoodsItem> star5UpList)
        {
            List<YSGoodsItem> star5NonUpList = new List<YSGoodsItem>();
            foreach (YSGoodsItem goodsItem in star5List)
            {
                if (star5UpList.Where(m => m.GoodsName == goodsItem.GoodsName).Count() > 0) continue;
                star5NonUpList.Add(goodsItem);
            }
            return star5NonUpList;
        }

        /// <summary>
        /// 获取4星非up列表
        /// </summary>
        /// <returns></returns>
        private List<YSGoodsItem> getStar4NonUpList(List<YSGoodsItem> star4List, List<YSGoodsItem> star4UpList)
        {
            List<YSGoodsItem> star4NonUpList = new List<YSGoodsItem>();
            foreach (YSGoodsItem goodsItem in star4List)
            {
                if (star4UpList.Where(m => m.GoodsName == goodsItem.GoodsName).Count() > 0) continue;
                star4NonUpList.Add(goodsItem);
            }
            return star4NonUpList;
        }

        /// <summary>
        /// 将GoodsPO转化为YSGoodsItem
        /// </summary>
        /// <param name="goodsPO"></param>
        /// <returns></returns>
        private YSGoodsItem changeToYSGoodsItem(GoodsPO goodsPO)
        {
            YSGoodsItem goodsItem = new YSGoodsItem();
            goodsItem.Probability = SiteConfig.DefaultPR;
            goodsItem.GoodsName = goodsPO.GoodsName;
            goodsItem.RareType = goodsPO.RareType;
            goodsItem.GoodsType = goodsPO.GoodsType;
            goodsItem.GoodsSubType = goodsPO.GoodsSubType;
            return goodsItem;
        }

        /// <summary>
        /// 将GoodsPO转化为YSGoodsItem
        /// </summary>
        /// <param name="poList"></param>
        /// <returns></returns>
        private List<YSGoodsItem> changeToYSGoodsItem(List<GoodsPO> poList)
        {
            List<YSGoodsItem> goodsItemList = new List<YSGoodsItem>();
            foreach (GoodsPO goodsPO in poList)
            {
                YSGoodsItem goodsItem = changeToYSGoodsItem(goodsPO);
                goodsItemList.Add(goodsItem);
            }
            return goodsItemList;
        }

        /// <summary>
        /// 连接两个集合,返回无重复部分
        /// </summary>
        /// <param name="listA"></param>
        /// <param name="listB"></param>
        /// <returns></returns>
        private List<YSGoodsItem> concatList(List<YSGoodsItem> listA, List<YSGoodsItem> listB)
        {
            List<YSGoodsItem> returnList = new List<YSGoodsItem>();
            foreach (var item in listA)
            {
                if (returnList.Where(m => m.GoodsName == item.GoodsName).Count() > 0) continue;
                returnList.Add(item);
            }
            foreach (var item in listB)
            {
                if (returnList.Where(m => m.GoodsName == item.GoodsName).Count() > 0) continue;
                returnList.Add(item);
            }
            return returnList;
        }




    }
}
