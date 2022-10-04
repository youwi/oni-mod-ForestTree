using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [HarmonyPatch("CreatePrefab")]
        public static GameObject Postfix(GameObject __result)
        {
            GameObject gameObject = __result;
            EntityTemplates.CreateAndRegisterPreviewForPlant(
                EntityTemplates.CreateAndRegisterSeedForPlant(gameObject,
                SeedProducer.ProductionType.Harvest, "ForestTreeSeed",
                STRINGS.CREATURES.SPECIES.SEEDS.WOOD_TREE.NAME, STRINGS.CREATURES.SPECIES.SEEDS.WOOD_TREE.DESC,
                Assets.GetAnim("seed_tree_kanim"), "object", 1,
                new List<Tag>
                {
                    GameTags.CropSeed
                }, SingleEntityReceptacle.ReceptacleDirection.Top, default(Tag), 4, STRINGS.CREATURES.SPECIES.WOOD_TREE.DOMESTICATEDDESC, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, null, "", false), "ForestTree_preview", Assets.GetAnim("tree_kanim"), "place", 3, 3);
            //SeedProducer.ProductionType.Harvest//收获掉落


           // var mutations=gameObject.GetComponent<MutantPlant>();//获取变异属性
            //生成的树枝需要从上级继承才行.
          //  gameObject.GetComponentInParent<MutantPlant>();//获取上级的变异属性.

            KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
            MutantPlant seed= gameObject.GetComponent<MutantPlant>();
            if (seed != null)
            {
                Debug.LogWarning("树枝生成时：CreatePrefab上级变异信息为：" + seed.MutationIDs);
            }
          

            if (component2 != null)
            {
                gameObject.AddOrGet<MutantPlant>().SpeciesID = component2.PrefabTag;//上级如果是种子的形态.
                Debug.LogWarning("树枝生成时：PrefabTag：" + seed.MutationIDs);
                // gameObject.AddOrGet<MutantPlant>().SpeciesID = component2.SpeciesID;上级如果是有ID,是树枝的静态.
            }
            return gameObject;
        }

    }
}
