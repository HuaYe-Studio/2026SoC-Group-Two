using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace awotr
{
    public class AwotrObj
    {
        public string id { get; private set; }
        public string name { get; private set; }
        public string description { get; private set; }
        public string info { get; private set; }
        public Image icon;
        public float cost { get; private set; }
        public float price { get; private set; }

        public AwotrObj(string id, string name, string description = "", string info = "",
                        Image icon = null, float cost = 0f, float price = 0f)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.info = info;
            this.icon = icon;
            this.cost = cost;
            this.price = price;
        }
    }

    public static class InitObj
    {
        private static readonly Dictionary<string, AwotrObj> _obj = new Dictionary<string, AwotrObj>();

        static InitObj()
        {
            InitAllObj();
        }

        private static void InitAllObj()
        {
            void Add(string id, string name,string des="",string info="",Image icon=null,float cost=0f,float price=0f)
            {
                var item = new AwotrObj(id, name, des, info, icon, cost, price);
                _obj[id] = item;
            }
            //下面是AI
            // ---------- 1. 衣物 ----------
            Add("tx", "T恤套装");
            Add("shirt", "卫衣套装");
            Add("coat", "棉袄套装");

            // ---------- 2. 医疗 ----------
            Add("firstaid", "急救包");
            Add("oralLiquid", "口服液");
            Add("pill", "药瓶");
            Add("_84", "消毒水");
            Add("bandaid", "创可贴");
            Add("bandage", "绷带");
            Add("plaster", "石膏");

            // ---------- 3. 日用品 ----------
            Add("detergent", "洗衣液");
            Add("soap", "肥皂");
            Add("energyDrink", "能量饮料");

            // ---------- 4. 食物 ----------
            Add("wheat", "小麦");
            Add("meat", "生肉");
            Add("egg", "鸡蛋");
            Add("milk", "牛奶");
            Add("fish", "鱼");
            Add("lettuce", "生菜");
            Add("been", "豆子");
            Add("ham", "火腿");
            Add("mealBox", "便当");
            Add("can", "罐头");
            Add("chips", "薯片");
            Add("watermelon", "西瓜");
            Add("apple", "苹果");
            Add("patota", "发霉的马铃薯");

            // ---------- 5. 饮料与酒 ----------
            Add("water", "水");
            Add("ice", "冰块");
            Add("juice", "果汁");
            Add("soda", "汽水");
            Add("rum", "朗姆酒");
            Add("vodka", "伏特加");
            Add("whiskey", "威士忌");
            Add("tequila", "龙舌兰");
            Add("brandy", "白兰地");
            Add("gin", "金酒");
            Add("fruitSlices", "水果切片");
            Add("ingredient", "辅料");
            Add("shaker", "调酒设施");
            Add("coffee", "咖啡");

            // ---------- 6. 原材料 ----------
            Add("coal", "菌炭");
            Add("fungus", "真菌");
            Add("petroleum", "进口石油");
            Add("bottle", "瓶瓶罐罐");
            Add("stone", "石头");
            Add("flower", "花卉");
            Add("wood", "木头");
            Add("seed", "种子");
            Add("cuprum", "铜");
            Add("iron", "铁");
            Add("jade", "玉胚");
            Add("fur", "毛皮");
            Add("bone", "骨头");
            Add("ivory", "象牙");
            Add("phoneComponent", "手机组件");

            // ---------- 7. 工艺品 ----------
            Add("minkCoat", "貂皮大衣");
            Add("woollenSweater", "羊毛衫");
            Add("blanket", "毛毯");
            Add("ivoryCarving", "象牙雕");
            Add("jadeBracelet", "玉镯子");
            Add("jadeWare", "古玩玉器");
            Add("china", "古玩瓷器");
            Add("miniPot", "迷你盆景");

            // ---------- 8. 工具 ----------
            Add("pickaxe", "镐");
            Add("axe", "斧");
            Add("shovel", "铲");
            Add("bow", "弓");
            Add("slingshot", "弹弓");
            Add("canOpener", "开罐器");
            Add("swissArmyKnife", "瑞士军刀");

            // ---------- 9. 货物 ----------
            Add("iceSculpture", "冰雕");
            Add("soil", "土壤");
            Add("shell", "贝壳");
            Add("sliver", "白银");
            Add("stock", "股票");
            Add("woodCarving", "珍贵木雕");

            // ---------- 10. 特殊物品 ----------
            Add("key", "钥匙");
            Add("map", "藏宝图");
            Add("idCard", "身份证");
            Add("ticket", "车票");
            Add("newspaper", "报纸");
            Add("phone", "手机");
            Add("letter", "家书");

            // ---------- 11. 垃圾 ----------
            Add("fakeJade", "玉器");
            Add("pickedTicket", "捡到的车票");
            Add("scratchTicket", "刮刮乐");
            Add("oldPhone", "旧手机");
        }

        public static AwotrObj GetObj(string id)
        {
            _obj.TryGetValue(id, out var obj);
            return obj;
        }
    }


}