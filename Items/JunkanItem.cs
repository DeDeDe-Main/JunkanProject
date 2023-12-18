using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using RiskOfOptions;
using JunkanProject.Utils;
using static JunkanProject.Main;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;
using BepInEx;

namespace JunkanProject.Items
{

    [BepInDependency("com.rune580.riskofoptions")]
    [BepInDependency("com.TeamMoonstorm.Starstorm2", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.EnforcerGang.Enforcer",BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rob.Paladin", BepInDependency.DependencyFlags.SoftDependency)]

    public class JunkanItem : ItemBase<JunkanItem>
    {
        public static ConfigEntry<int> WhiteValue;
        public static ConfigEntry<int> GreenValue;
        public static ConfigEntry<bool> RegenScrapCounts;
        public static ConfigEntry<int> RedValue;
        public static ConfigEntry<int> YellowValue;
        public static ConfigEntry<bool> ShouldStack;

        public override string ItemName => "Ser Junkan";

        public override string ItemLangTokenName => "SER_JUNKAN";

        public override string ItemPickupDesc => "Loves junk.";

        public override string ItemFullDescription => "Loves junk a lot. Gain stats for each scrap you have.  Higher rarity scraps are worth more!  <style=cstack>(Double the Junkan, double the <i>additive</i> value.)</style>";

        public override string ItemLore => "{Beginning audio playback...}\n\"It appeared from a rift in time and space.  Without proper legs it could not move far.  With little need for restraints, the staff began introducing the unknown lifeform to various devices and paraphernalia, but the creature clung to one item rather quickly.  Upon reaching a standard-issue scrapper, it refused to leave the vicinity despite our use of excessive force.  We will monitor it closely for the next few days...\"\n{End of recording.}\n" + "{Beginning audio playback...}\n\"Some of the engineers have taken to tossing forks into the scrapper to watch this little.. thing gather up the resulting scrap.  I'll admit I never expected something with so little mobility to be so creative.  It seems to be building something right now even... D'aww.  It's a little helmet!  Oh?  What's that you've got behind your back?\"\n" + "[Multiple voices shouting for security as subject has produced a sword.]\n{End of recording.}";


        public override ItemTier Tier => ItemTier.Tier3;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("JunkanPickup.prefab");

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("JunkanIcon.png");

        public static GameObject ItemBodyModelPrefab;

        public Inventory inventory { get; private set; }

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            ModSettingsManager.SetModDescription("A little buddy that loves junk.  We don't have any, so scrap will have to do!");

            Sprite icon = ItemIcon;

            ModSettingsManager.SetModIcon(icon);

            WhiteValue = config.Bind<int>("Scrap Value", "Value of White Scrap:", 1, "What should each white scrap be worth to Junkan?");
            RiskOfOptions.ModSettingsManager.AddOption(new IntSliderOption(WhiteValue));
            GreenValue = config.Bind<int>("Scrap Value", "Value of Green Scrap:", 2, "What should each Green scrap be worth to Junkan?");
            RiskOfOptions.ModSettingsManager.AddOption(new IntSliderOption(GreenValue));
            RedValue = config.Bind<int>("Scrap Value", "Value of Red Scrap:", 3, "What should each Red scrap be worth to Junkan?");
            RiskOfOptions.ModSettingsManager.AddOption(new IntSliderOption(RedValue));
            YellowValue = config.Bind<int>("Scrap Value", "Value of Yellow Scrap:", 4, "What should each Yellow scrap be worth to Junkan?");
            RiskOfOptions.ModSettingsManager.AddOption(new IntSliderOption(YellowValue));
            
            ShouldStack = config.Bind<bool>("Other", "Stackable:", true, "Should Junkan stack?");
            ModSettingsManager.AddOption(new CheckBoxOption(ShouldStack)); 
            RegenScrapCounts = config.Bind<bool>("Other", "Regen Scrap Count As Green:", true, "Should regen scrap be worthy of Junkan?");
            ModSettingsManager.AddOption(new CheckBoxOption(RegenScrapCounts));
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = MainAssets.LoadAsset<GameObject>("Junkan.prefab");
            var itemDisplay = ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            itemDisplay.rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict();
            rules.Add("mdlCommandoDualies", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0.00337F, 0.15134F, -0.33042F),
                    localAngles = new Vector3(353.238F, 179.3276F, 359.9428F),
                    localScale = new Vector3(0.00812F, 0.00851F, 0.00851F)
                }
            });
            rules.Add("mdlHuntress", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0.25419F, 0.10565F, 0.15022F),
                    localAngles = new Vector3(6.79089F, 77.10178F, 239.3109F),
                    localScale = new Vector3(0.00557F, 0.00584F, 0.00584F)
                }
            });
            rules.Add("mdlToolbot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0.0492F, 0.61058F, 1.29338F),
                    localAngles = new Vector3(301.3343F, 184.4681F, 357.3943F),
                    localScale = new Vector3(0.0427F, 0.04475F, 0.04475F)
                }
            });
            rules.Add("mdlEngi", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "CannonHeadL",
                    localPos = new Vector3(-0.09054F, 0.30338F, 0.18402F),
                    localAngles = new Vector3(274.0644F, 227.2362F, 312.6784F),
                    localScale = new Vector3(0.00516F, 0.00541F, 0.00541F)
                }
            });
            rules.Add("mdlMage", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0.00533F, 0.16144F, 0.07968F),
                    localAngles = new Vector3(0.98114F, 0.6852F, 2.58176F),
                    localScale = new Vector3(0.0024F, 0.00252F, 0.00252F)
                }
            });
            rules.Add("mdlMerc", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(-0.11023F, 0.28783F, 0.00428F),
                    localAngles = new Vector3(358.6989F, 275.1202F, 183.4945F),
                    localScale = new Vector3(0.00723F, 0.00653F, 0.00641F)
                }
            });
            rules.Add("mdlTreebot", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "FlowerBase",
                    localPos = new Vector3(0.01791F, -0.80604F, 0.63195F),
                    localAngles = new Vector3(357.696F, 0.27423F, 0.04861F),
                    localScale = new Vector3(0.00812F, 0.00851F, 0.00851F)
                }
            });
            rules.Add("mdlLoader", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0.1795F, 0.18152F, 0.2361F),
                    localAngles = new Vector3(4.00476F, 0.07026F, 91.1243F),
                    localScale = new Vector3(0.00678F, 0.0071F, 0.0071F)
                }
            });
            rules.Add("mdlCroco", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.04724F, 3.65092F, 2.76313F),
                    localAngles = new Vector3(308.9618F, 180.7798F, 1.69895F),
                    localScale = new Vector3(0.03813F, 0.03813F, 0.03813F)
                }
            });
            rules.Add("mdlCaptain", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.00055F, 0.14415F, 0.15859F),
                    localAngles = new Vector3(355.2004F, 0.81311F, 358.3623F),
                    localScale = new Vector3(0.00234F, 0.00245F, 0.00245F)
                }
            });
            rules.Add("mdlBandit2", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Hat",
                    localPos = new Vector3(0.1412F, 0.01422F, -0.05569F),
                    localAngles = new Vector3(342.6077F, 85.66895F, 58.89248F),
                    localScale = new Vector3(0.00239F, 0.00262F, 0.00262F)
                }
            });
            rules.Add("RobPaladinBody", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Sword",
                    localPos = new Vector3(-0.002F, -0.10324F, -0.00512F),
                    localAngles = new Vector3(3.01076F, 209.9027F, 179.9966F),
                    localScale = new Vector3(0.00423F, 0.00443F, 0.00435F)
                }
            });
            rules.Add("EnforcerBody", new RoR2.ItemDisplayRule[]
{
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Shield",
                    localPos = new Vector3(0.11234F, 0.22205F, 0.09062F),
                    localAngles = new Vector3(6.72028F, 119.8226F, 351.8811F),
                    localScale = new Vector3(0.00812F, 0.00851F, 0.00851F)
                }
});
            rules.Add("NemesisEnforcerBody", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.00119F, 0.00864F, 0.00005F),
                    localAngles = new Vector3(1.78746F, 270.657F, 179.7083F),
                    localScale = new Vector3(0.00008F, -0.00009F, 0.0001F)
                }
            });
            rules.Add("mdlNemCommando", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.09446F, 0.20658F, -1.84951F),
                    localAngles = new Vector3(10.03252F, 180.276F, 2.26978F),
                    localScale = new Vector3(0.02813F, 0.02948F, 0.02948F)
                }
            });
            rules.Add("mdlNemMerc", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Shotgun",
                    localPos = new Vector3(0.00686F, -0.36188F, -0.52113F),
                    localAngles = new Vector3(28.5421F, 181.8571F, 180.6936F),
                    localScale = new Vector3(0.00159F, 0.00166F, 0.00166F)
                }
            });
            rules.Add("mdlExecutioner2", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0.0209F, 0.04466F, -0.13211F),
                    localAngles = new Vector3(359.6969F, 181.014F, 179.8738F),
                    localScale = new Vector3(0.0025F, 0.00262F, 0.00262F)
                }
            });
            rules.Add("mdlRailGunner", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Backpack",
                    localPos = new Vector3(0.07453F, 0.01978F, -0.14893F),
                    localAngles = new Vector3(1.63474F, 182.5314F, 0.6099F),
                    localScale = new Vector3(0.00375F, 0.00393F, 0.00393F)
                }
            });
            rules.Add("mdlVoidSurvivor", new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.00736F, 0.19686F, -0.15444F),
                    localAngles = new Vector3(21.73323F, 185.0706F, 358.1841F),
                    localScale = new Vector3(0.004F, 0.00419F, 0.00419F)
                }
            });
            return rules;
        }

        public override void Hooks()
        {
            R2API.RecalculateStatsAPI.GetStatCoefficients += StatBuffs;
        }

        private void StatBuffs(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {  
            var inventory = sender.inventory;
            if (inventory)
            {
                    var whiteCount = inventory.GetItemCount(RoR2Content.Items.ScrapWhite);
                    var greenCount = (inventory.GetItemCount(RoR2Content.Items.ScrapGreen));
                    if(RegenScrapCounts.Value == true)
                    {
                    greenCount += inventory.GetItemCount(DLC1Content.Items.RegeneratingScrap);
                    }
                    var redCount = inventory.GetItemCount(RoR2Content.Items.ScrapRed);
                    var yellowCount = inventory.GetItemCount(RoR2Content.Items.ScrapYellow);

                    var stack = ((whiteCount * WhiteValue.Value) + (greenCount * GreenValue.Value) + (redCount * RedValue.Value) + (yellowCount * YellowValue.Value));
                    if (ShouldStack.Value == true)
                    {
                    stack *= inventory.GetItemCount(ItemDef);
                    }

                    if (stack > 0)
                    {
                    args.moveSpeedMultAdd += 0.01f * stack;
                    args.attackSpeedMultAdd += 0.01f * stack;
                    args.armorAdd += 1f * stack;
                    args.critAdd += 0.5f * stack;
                    args.healthMultAdd += 0.01f * stack;
                    args.damageMultAdd += 0.01f * stack;
                    } 
                



            }
        }
    }
}
