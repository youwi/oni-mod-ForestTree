using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace oni_ForestTree
{

    [HarmonyPatch(typeof(ForestTreeBranchConfig))]
    public class ForestTreeBranchConfigPatch
    {

        //ForestTreeBranchConfig
        //参考: https://bbs.3dmgame.com/thread-6333693-1-1.html
        //收获种子可以得到变异属性的种子.
        [HarmonyPatch("OnSpawn")]
        public static void Postfix(GameObject inst,GameObject __instance)
        {
            GameObject gameObject = inst;
            EntityTemplates.CreateAndRegisterPreviewForPlant(
                EntityTemplates.CreateAndRegisterSeedForPlant(gameObject,
                SeedProducer.ProductionType.Harvest, "ForestTreeSeed",
                STRINGS.CREATURES.SPECIES.SEEDS.WOOD_TREE.NAME,
                STRINGS.CREATURES.SPECIES.SEEDS.WOOD_TREE.DESC,
                Assets.GetAnim("seed_tree_kanim"), "object", 1,
                new List<Tag> { GameTags.CropSeed },
                SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 4,
                STRINGS.CREATURES.SPECIES.WOOD_TREE.DOMESTICATEDDESC,
                EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", false),
            "ForestTree_preview", Assets.GetAnim("tree_kanim"),
            "place", 3, 3);
            //SeedProducer.ProductionType.Harvest//收获掉落


            // var mutations=gameObject.GetComponent<MutantPlant>();//获取变异属性
            //生成的树枝需要从上级继承才行.
            //  gameObject.GetComponentInParent<MutantPlant>();//获取上级的变异属性.
            // Tree-->BuddingTrunk x6--TreeBudx6---TreeBranch
            //MutantPlant seedParent =  gameObject.GetComponentInParent<TreeBud>().GetComponentInParent<BuddingTrunk>().GetComponentInParent<MutantPlant>();
            //MutantPlant seedParent =  gameObject.FindComponent<TreeBud>().FindComponent<BuddingTrunk>().GetComponentInParent<MutantPlant>();
            MutantPlant seedParent = gameObject.GetComponentInParent<MutantPlant>();

            if (seedParent != null)
            {
                Console.WriteLine("ForestTreeBranchConfig树枝上级生成时：GetComponentInParent<MutantPlant>：" + seedParent.SpeciesID);
            }

            MutantPlant seed = gameObject.GetComponent<MutantPlant>();

            // seed.SpeciesID = component2.PrefabTag;//上级如果是种子的形态.

            // gameObject.AddOrGet<MutantPlant>().SpeciesID = component2.SpeciesID;上级如果是有ID,是树枝的静态.

            // seed.Mutate();//获取随机变异. 
/*
            seed.SpeciesID = new Tag("rottenHeaps");//强制修改为旺盛属性.
            seed.ApplyMutations();
            seed.AddTag(GameTags.MutatedSeed);
            seed.AddTag(new Tag("rottenHeaps"));//上面的办法改不了?


            Components.MutantPlants.Add(seed);
            List<string> list =  new List<string>();
            list.Add(Db.Get().PlantMutations.rottenHeaps.Id);
            seed.Analyze();
            seed.SetSubSpecies(list);
            seed.ApplyMutations();
            // PlantSubSpeciesCatalog.Instance.DiscoverSubSpecies(GetSubSpeciesInfo(), seed);
*/

            //Console.WriteLine("ForestTreeBranchConfig树枝生成时：seed.MutationIDs：" + seed.SpeciesID);
           // Console.WriteLine("ForestTreeBranchConfig树枝生成时：seed.IsOriginal：" + seed.IsOriginal);
            
          //  return gameObject;
        }

    }

    [HarmonyPatch(typeof(BuddingTrunk))]
    public class BuddingTrunkPatch
    {

        // Tree-->BuddingTrunk x6--TreeBudx6---TreeBranch
        // 树---对应6个节点,
        [HarmonyPatch("OnSpawn")]
        public static void Postfix(BuddingTrunk __instance)
        {
            var gameObject = __instance.GetComponentInParent<MutantPlant>();
            //Console.WriteLine("BuddingTrunk 测试变异信息：" + gameObject.SpeciesID); //结果是 ForestTree
            /*  __instance.AddTag(GameTags.MutatedSeed);
              __instance.FindOrAddComponent<MutantPlant>();*/
         
           
        }
    }
    [HarmonyPatch(typeof(TreeBud))]
    public class BTreeBudPatch
    {

        // Tree-->BuddingTrunk x6--TreeBudx6---TreeBranch
        // 树---对应6个节点
        // 从这里复制变异信息
        [HarmonyPatch("OnSpawn")]
        public static void Postfix(TreeBud __instance)
        {
            var mutantBranch = __instance.GetComponentInParent<MutantPlant>();
           // Console.WriteLine("BTreeBudPatch 测试变异信息：" + mutantBranch.SpeciesID);//结果是 ForestTreeBranch
            var mutantUp= __instance.buddingTrunk.Get().GetComponentInParent<MutantPlant>();

            mutantUp.CopyMutationsTo(mutantBranch);
            mutantBranch.ApplyMutations();
            // GetComponent<KSelectable>().SetName(GetSubSpeciesInfo().GetNameWithMutations(component.PrefabTag.ProperName(), flag, flag2));
            // 可以手动更新名字“乔木（旺盛）”需要手动更新信息。这里太复杂了,不做.
        }
    }
}

