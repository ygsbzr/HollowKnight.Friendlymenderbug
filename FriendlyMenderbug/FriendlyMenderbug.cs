using Modding;
using UnityEngine;
using Satchel;
using HutongGames.PlayMaker.Actions;
using HutongGames.PlayMaker;
using GlobalEnums;
using Language;
namespace FriendlyMenderbug
{
    public class FriendlyMenderbug:Mod
    {
        public override string GetVersion()
        {
            return "1.0";
        }
        public static readonly string CurrentLang = Language.Language.CurrentLanguage().ToString().ToLower();
        public override void Initialize()
        {
            On.GameManager.BeginScene += FindMender;
            On.HealthManager.TakeDamage += powerfulbug;
            ModHooks.LanguageGetHook += AddDream;
        }

        private string AddDream(string key, string sheetTitle, string orig)
        {
            string result;
            switch (key)
            {
                case "MENDERBUG_DREAM_1":
                    if(CurrentLang.Equals("zh"))
                    {
                        result = "我热爱我的工作!";
                    }
                    else
                    {
                        result = "I love my job!";
                    }
                    break;
                case "MENDERBUG_DREAM_2":
                    if (CurrentLang.Equals("zh"))
                    {
                        result = "别打我!";
                    }
                    else
                    {
                        result = "Dont hurt me!";
                    }
                    break;
                default:
                    result = orig;
                    break;

            }
            return result;
        }

        private void powerfulbug(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            if(self.gameObject.name.Equals("Mender Bug"))
            {
                hitInstance.DamageDealt = 0;
                HeroController.instance.TakeDamage(HeroController.instance.gameObject, CollisionSide.other, 9999, 2);//punishment
            }
            orig(self, hitInstance);
        }

        private void FindMender(On.GameManager.orig_BeginScene orig, GameManager self)
        {
            orig(self);
            if (!self.sceneName.Equals("Crossroads_01"))
                return;
            GameObject menderbug = UnityEngine.Object.FindObjectsOfType<GameObject>().FirstOrDefault(x => x.name.Equals("Mender Bug"));
            if(menderbug != null)
            {
                ModifyMenderbug(menderbug);
            }
        }

        private void ModifyMenderbug(GameObject menderbug)
        {
            PlayMakerFSM menderfsm=menderbug.GetComponent<PlayMakerFSM>();
            if(menderfsm != null)
            {
                menderfsm.RemoveAction("Chance", 0);
                menderfsm.GetAction<IntCompare>("Chance", 1).lessThan = FsmEvent.Finished; //make menderbug will appear every time
                menderfsm.RemoveTransition("Idle", "HERO ENTER");//just work
            }
            Log("Modify menderbug fsm");
            DamageHero menderbugcol=menderbug.GetAddComponent<DamageHero>();
            menderbugcol.damageDealt = 9999;
            menderbugcol.hazardType = 2;
            menderbugcol.shadowDashHazard = true;
            Log("It become powerful");
            EnemyDreamnailReaction menderdream = menderbug.GetAddComponent<EnemyDreamnailReaction>();
            menderdream.SetConvoTitle("MENDERBUG_DREAM");
            ReflectionHelper.SetField(menderdream, "convoAmount", 2);
            Log("Add mender dream");
        }
    }
}
