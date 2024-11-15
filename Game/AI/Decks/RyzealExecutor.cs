using YGOSharp.OCGWrapper;
using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;
using System;

namespace WindBot.Game.AI.Decks
{
    [Deck("Ryzeal", "AI_Ryzeal")]
    class RyzealExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int RyzealEx = 34022970;
            public const int RyzealThode = 35844557;
            public const int RyzealIce = 8633261;
            public const int RyzealNode = 72238166;
            public const int RyzealPalma = 61116514;
            // _CardId.MaxxC = 23434538;
            // _CardId.AshBlossom = 14558127;
            public const int Fuwalos = 42141493;
            public const int BigJaw = 55697723;
            public const int DrakeShark = 81096431;

            public const int RyzealPlugin = 60394026;
            public const int RyzealCross = 6798031;
            public const int Seven = 7477101;
            public const int SevenA = 23153227;
            public const int Bonfire = 85106525;
            public const int TTT = 25311006;
            // _CardId.CalledByTheGrave = 24224830;

            public const int RyzealHole = 33787730;
            // _CardId.InfiniteImpermanence = 10045474;

            public const int RyzealDuo = 7511613;
            public const int RyzealDD = 34909328;
            public const int TimeThiefRedoer = 55285840;
            public const int No104 = 2061963;
            // _CardId.EvilswarmExcitonKnight = 46772449;
            public const int Number41BagooskatheTerriblyTiredTapir = 90590303;
            public const int LDignister = 61399402; //Light Dragon @iginster
            public const int TwinE = 45852939; //Twin Eclipse
            public const int No103 = 94380860; //ragnazero
            public const int No101 = 48739166;

            public const int NaturalExterio = 99916754;
            public const int NaturalBeast = 33198837;
            public const int ImperialOrder = 61740673;
            public const int SwordsmanLV7 = 37267041;
            public const int RoyalDecree = 51452091;
            public const int InspectorBoarder = 15397015;
            public const int DimensionShifter = 91800273;
            public const int DivineArsenalAAZEUS_SkyThunder = 90448279;

        }

        public RyzealExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            // Hand Trap
            AddExecutor(ExecutorType.Activate, _CardId.MaxxC, MaxxCActivate);
            AddExecutor(ExecutorType.Activate, CardId.Fuwalos, FuwaActivate);
            AddExecutor(ExecutorType.Activate, _CardId.InfiniteImpermanence, InfiniteImpermanenceActivate);
            AddExecutor(ExecutorType.Activate, _CardId.AshBlossom, AshBlossomActivate);
            AddExecutor(ExecutorType.Activate, _CardId.CalledByTheGrave, CalledbytheGraveActivate);
            AddExecutor(ExecutorType.Activate, CardId.TTT);

            AddExecutor(ExecutorType.Repos, CardId.Number41BagooskatheTerriblyTiredTapir, MonsterRepos);

            AddExecutor(ExecutorType.SpSummon, _CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightSummon);
            AddExecutor(ExecutorType.Activate, _CardId.EvilswarmExcitonKnight, DefaultEvilswarmExcitonKnightEffect);

            AddExecutor(ExecutorType.Activate, CardId.No103, DestroyEffect);
            AddExecutor(ExecutorType.Activate, CardId.RyzealHole, HoleEffect);
            AddExecutor(ExecutorType.Activate, CardId.LDignister);

            // search
            AddExecutor(ExecutorType.Activate, CardId.SevenA);
            AddExecutor(ExecutorType.Summon, CardId.RyzealIce);
            AddExecutor(ExecutorType.Activate, CardId.RyzealIce);
            AddExecutor(ExecutorType.SpSummon, CardId.RyzealEx, ExSS);
            AddExecutor(ExecutorType.Activate, CardId.RyzealEx, ExEffect);
            AddExecutor(ExecutorType.Activate, CardId.Bonfire, HandStarterEXCheck);
            AddExecutor(ExecutorType.Activate, CardId.Seven, SevenActivate);

            // addition monster summmon
            AddExecutor(ExecutorType.SpSummon, CardId.RyzealThode);
            AddExecutor(ExecutorType.Activate, CardId.RyzealThode, ThodeEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.RyzealNode);
            AddExecutor(ExecutorType.Activate, CardId.RyzealPlugin, PluginEffect);
            AddExecutor(ExecutorType.Activate, CardId.RyzealCross, CrossEffect);
            AddExecutor(ExecutorType.Activate, CardId.BigJaw);
            AddExecutor(ExecutorType.Activate, CardId.DrakeShark);
            AddExecutor(ExecutorType.SpSummon, CardId.RyzealPalma,PalmaSS);

            // xyz summon

            AddExecutor(ExecutorType.SpSummon, CardId.RyzealDuo, DuoSS);
            AddExecutor(ExecutorType.Activate, CardId.RyzealDuo);
            AddExecutor(ExecutorType.SpSummon, CardId.RyzealDD, DDSS);
            AddExecutor(ExecutorType.Activate, CardId.RyzealDD, DestroyEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.TimeThiefRedoer, CheckLessOperation);
            AddExecutor(ExecutorType.Activate, CardId.TimeThiefRedoer, RedoerEffect);
            AddExecutor(ExecutorType.SpSummon, CardId.LDignister, CheckLessOperation);
            AddExecutor(ExecutorType.SpSummon, CardId.No103, CheckLessOperation);
            //AddExecutor(ExecutorType.SpSummon, CardId.TwinE);
            AddExecutor(ExecutorType.Activate, CardId.TwinE);

            AddExecutor(ExecutorType.SpSummon, CardId.Number41BagooskatheTerriblyTiredTapir, Number41BagooskaTheTerriblyTiredTapirSummon);

            // normal summon for xyz
            AddExecutor(ExecutorType.Summon, CardId.RyzealThode);
            AddExecutor(ExecutorType.Summon, CardId.RyzealNode);
            AddExecutor(ExecutorType.Summon, Level4SummonCheck);

            // other
            AddExecutor(ExecutorType.Repos, MonsterRepos);
            AddExecutor(ExecutorType.SpellSet, SpellSetCheck);


        }

        const int SetcodeTimeLord = 0x4a;
        const int SetcodeOrcust = 0x11b;
        const int SetcodeRyzeal = 0x1be;

        Dictionary<int, List<int>> DeckCountTable = new Dictionary<int, List<int>>{
            {3, new List<int> { CardId.RyzealEx, CardId.RyzealThode,CardId.RyzealPalma, CardId.RyzealNode ,
                                _CardId.MaxxC, _CardId.AshBlossom, CardId.SevenA,
                                CardId.RyzealIce, CardId.Seven, _CardId.InfiniteImpermanence,
                                CardId.RyzealDuo,CardId.RyzealDD,CardId.Fuwalos } },
            {2, new List<int> {  _CardId.CalledByTheGrave}},
            {1, new List<int> { CardId.No101, CardId.No103, CardId.No104,CardId.TimeThiefRedoer,CardId.Bonfire, CardId.RyzealPlugin, CardId.TTT,
                                _CardId.EvilswarmExcitonKnight, CardId.RyzealCross ,CardId.RyzealHole, CardId.TwinE, CardId.LDignister } }
        };

        List<int> currentNegatingIdList = new List<int>();
        bool enemyActivateMaxxC = false;
        bool enemyActivateLockBird = false;
        List<int> infiniteImpermanenceList = new List<int>();
        List<ClientCard> removeChosenList = new List<ClientCard>();
        List<ClientCard> targetedDDList = new List<ClientCard>();
        List<ClientCard> activatedDDList = new List<ClientCard>();
        List<ClientCard> targetedHoleList = new List<ClientCard>();
        List<ClientCard> activatedHoleList = new List<ClientCard>();
        List<ClientCard> targetedNo103List = new List<ClientCard>();
        List<ClientCard> activatedNo103List = new List<ClientCard>();
        List<int> oncePerTurnEffectActivatedList = new List<int>();
        List<ClientCard> spSummonThisTurn = new List<ClientCard>();


        bool summoned = false;
        bool SearchEX = false;
        bool Cross = false;
        bool Fuwa = false;

        //GO FIRST
        public override bool OnSelectHand()
        {
            return true;
        }

        public List<ClientCard> ShuffleCardList(List<ClientCard> list)
        {
            List<ClientCard> result = list;
            int n = result.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(n + 1);
                ClientCard temp = result[index];
                result[index] = result[n];
                result[n] = temp;
            }
            return result;
        }

        //To fix for xyz summon
        public void SelectXyzMaterial(int num = 2, bool needRyzeal = false)
        {
            List<ClientCard> materialList = Bot.GetMonsters().Where(card => CheckAbleForXyz(card)).ToList();
            if (materialList?.Count() < num)
            {
                return;
            }
            if (needRyzeal && !materialList.Any(card => card.HasSetcode(SetcodeRyzeal)))
            {
                return;
            }
            List<ClientCard> selectedList = new List<ClientCard>();

            // if needed, select Ryzeal with less atk first
            if (needRyzeal)
            {
                List<ClientCard> RyzealList = materialList.Where(card => card.HasSetcode(SetcodeRyzeal)).ToList();
                RyzealList.Sort(CardContainer.CompareCardAttack);
                RyzealList.Reverse();
                ClientCard firstSelect = RyzealList[0];
                selectedList.Add(firstSelect);
                materialList.Remove(firstSelect);
            }

            List<ClientCard> sortMaterialList = materialList.Where(card =>
                (card?.Data != null && !card.HasSetcode(SetcodeRyzeal))).ToList();
            sortMaterialList.Sort(CardContainer.CompareCardAttack);
            foreach (ClientCard card in sortMaterialList)
            {
                selectedList.Add(card);
                if (selectedList.Count() >= num)
                {
                    AI.SelectMaterials(selectedList);
                    return;
                }
            }
        }

        // Check whether can be used for xyz summon.
        public bool CheckAbleForXyz(ClientCard card)
        {
            return card.IsFaceup() && !card.HasType(CardType.Xyz) && !card.HasType(CardType.Link) && !card.HasType(CardType.Token) && card.Level == 4;
        }

        // Check whether last chain card should be disabled.
        public bool CheckLastChainShouldNegated()
        {
            ClientCard lastcard = Util.GetLastChainCard();
            if (lastcard == null || lastcard.Controller != 1) return false;
            if (lastcard.IsMonster() && lastcard.HasSetcode(SetcodeTimeLord) && Duel.Phase == DuelPhase.Standby) return false;
            return true;
        }

        public ClientCard GetProblematicEnemyMonster(int attack = 0, bool canBeTarget = false)
        {
            List<ClientCard> floodagateList = Enemy.GetMonsters().Where(c => c?.Data != null &&
                c.IsFloodgate() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (floodagateList.Count > 0)
            {
                floodagateList.Sort(CardContainer.CompareCardAttack);
                floodagateList.Reverse();
                return floodagateList[0];
            }

            List<ClientCard> dangerList = Enemy.MonsterZone.Where(c => c?.Data != null &&
                c.IsMonsterDangerous() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (dangerList.Count > 0)
            {
                dangerList.Sort(CardContainer.CompareCardAttack);
                dangerList.Reverse();
                return dangerList[0];
            }

            List<ClientCard> invincibleList = Enemy.MonsterZone.Where(c => c?.Data != null &&
                c.IsMonsterInvincible() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (invincibleList.Count > 0)
            {
                invincibleList.Sort(CardContainer.CompareCardAttack);
                invincibleList.Reverse();
                return invincibleList[0];
            }

            if (attack == 0)
                attack = Util.GetBestAttack(Bot);
            List<ClientCard> betterList = Enemy.MonsterZone.GetMonsters()
                .Where(card => card.GetDefensePower() >= attack && card.IsAttack() && (!canBeTarget || !card.IsShouldNotBeTarget())).ToList();
            if (betterList.Count > 0)
            {
                betterList.Sort(CardContainer.CompareCardAttack);
                betterList.Reverse();
                return betterList[0];
            }
            return null;
        }

        public ClientCard GetProblematicEnemyCard(bool canBeTarget = false)
        {
            List<ClientCard> floodagateList = Enemy.MonsterZone.Where(c => c?.Data != null && !removeChosenList.Contains(c) &&
                c.IsFloodgate() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (floodagateList.Count > 0)
            {
                floodagateList.Sort(CardContainer.CompareCardAttack);
                floodagateList.Reverse();
                return floodagateList[0];
            }

            List<ClientCard> problemEnemySpellList = Enemy.SpellZone.Where(c => c?.Data != null && !removeChosenList.Contains(c)
                && c.IsFloodgate() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (problemEnemySpellList.Count > 0)
            {
                return ShuffleCardList(problemEnemySpellList)[0];
            }

            List<ClientCard> dangerList = Enemy.MonsterZone.Where(c => c?.Data != null && !removeChosenList.Contains(c)
                && c.IsMonsterDangerous() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (dangerList.Count > 0
                && (Duel.Player == 0 || (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)))
            {
                dangerList.Sort(CardContainer.CompareCardAttack);
                dangerList.Reverse();
                return dangerList[0];
            }

            List<ClientCard> invincibleList = Enemy.MonsterZone.Where(c => c?.Data != null && !removeChosenList.Contains(c)
                && c.IsMonsterInvincible() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (invincibleList.Count > 0)
            {
                invincibleList.Sort(CardContainer.CompareCardAttack);
                invincibleList.Reverse();
                return invincibleList[0];
            }

            List<ClientCard> enemyMonsters = Enemy.GetMonsters().Where(c => !removeChosenList.Contains(c)).ToList();
            if (enemyMonsters.Count > 0)
            {
                enemyMonsters.Sort(CardContainer.CompareCardAttack);
                enemyMonsters.Reverse();
                foreach (ClientCard target in enemyMonsters)
                {
                    if (target.HasType(CardType.Fusion) || target.HasType(CardType.Ritual) || target.HasType(CardType.Synchro) || target.HasType(CardType.Xyz) || (target.HasType(CardType.Link) && target.LinkCount >= 2))
                    {
                        if (!canBeTarget || !(target.IsShouldNotBeTarget() || target.IsShouldNotBeMonsterTarget())) return target;
                    }
                }
            }

            List<ClientCard> spells = Enemy.GetSpells().Where(c => c.IsFaceup() && !removeChosenList.Contains(c)
                && (c.HasType(CardType.Equip) || c.HasType(CardType.Pendulum) || c.HasType(CardType.Field) || c.HasType(CardType.Continuous)))
                .ToList();
            if (spells.Count > 0)
            {
                return ShuffleCardList(spells)[0];
            }

            return null;
        }

        public ClientCard GetBestEnemyMonster(bool onlyFaceup = false, bool canBeTarget = false)
        {
            ClientCard card = GetProblematicEnemyMonster(0, canBeTarget);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.GetHighestAttackMonster(canBeTarget);
            if (card != null)
                return card;

            List<ClientCard> monsters = Enemy.GetMonsters();

            // after GetHighestAttackMonster, the left monsters must be face-down.
            if (monsters.Count > 0 && !onlyFaceup)
                return ShuffleCardList(monsters)[0];

            return null;
        }

        public ClientCard GetBestEnemySpell(bool onlyFaceup = false, bool canBeTarget = false)
        {
            List<ClientCard> problemEnemySpellList = Enemy.SpellZone.Where(c => c?.Data != null
                && c.IsFloodgate() && c.IsFaceup() && (!canBeTarget || !c.IsShouldNotBeTarget())).ToList();
            if (problemEnemySpellList.Count > 0)
            {
                return ShuffleCardList(problemEnemySpellList)[0];
            }

            List<ClientCard> spells = Enemy.GetSpells().Where(card => !(card.IsFaceup() && card.IsCode(_CardId.EvenlyMatched))).ToList();

            List<ClientCard> faceUpList = spells.Where(ecard => ecard.IsFaceup() &&
                (ecard.HasType(CardType.Continuous) || ecard.HasType(CardType.Field) || ecard.HasType(CardType.Pendulum))).ToList();
            if (faceUpList.Count > 0)
            {
                return ShuffleCardList(faceUpList)[0];
            }

            if (spells.Count > 0 && !onlyFaceup)
            {
                return ShuffleCardList(spells)[0];
            }

            return null;
        }

        public ClientCard GetBestEnemyCard(bool onlyFaceup = false, bool canBeTarget = false, bool checkGrave = false)
        {
            ClientCard card = GetBestEnemyMonster(onlyFaceup, canBeTarget);
            if (card != null)
            {
                return card;
            }

            card = GetBestEnemySpell(onlyFaceup, canBeTarget);
            if (card != null)
            {
                return card;
            }

            if (checkGrave && Enemy.Graveyard.Count > 0)
            {
                List<ClientCard> graveMonsterList = Enemy.Graveyard.GetMatchingCards(c => c.IsMonster()).ToList();
                if (graveMonsterList.Count > 0)
                {
                    graveMonsterList.Sort(CardContainer.CompareCardAttack);
                    graveMonsterList.Reverse();
                    return graveMonsterList[0];
                }
                return ShuffleCardList(Enemy.Graveyard.ToList())[0];
            }

            return null;
        }

        public int CheckRemainInDeck(int id)
        {
            for (int count = 1; count < 4; ++count)
            {
                if (DeckCountTable[count].Contains(id))
                {
                    return Bot.GetRemainingCount(id, count);
                }
            }
            return 0;
        }

        public int CheckCalledbytheGrave(int id)
        {
            if (currentNegatingIdList.Contains(id)) return 1;
            if (DefaultCheckWhetherCardIdIsNegated(id)) return 1;
            return 0;
        }

        // Check whether opposite use Maxx-C, and thus make less operation.
        public bool CheckLessOperation()
        {
            if (enemyActivateMaxxC)
            {
                return true;
            }
            return CheckAtAdvantage();
        }

        public bool CheckAtAdvantage()
        {
            if (GetProblematicEnemyMonster() == null && Bot.GetMonsters().Any(card => card.IsFaceup()))
            {
                return true;
            }
            return false;
        }

        public bool CheckInDanger()
        {
            if (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2)
            {
                int totalAtk = 0;
                foreach (ClientCard m in Enemy.GetMonsters())
                {
                    if (m.IsAttack() && !m.Attacked) totalAtk += m.Attack;
                }
                if (totalAtk >= Bot.LifePoints) return true;
            }
            return false;
        }

        public void SelectDetachMaterial(ClientCard activateCard)
        {
            // TODO
            AI.SelectCard(0);
        }

        // check enemy's dangerous card in grave
        public List<ClientCard> CheckDangerousCardinEnemyGrave(bool onlyMonster = false)
        {
            List<ClientCard> result = Enemy.Graveyard.GetMatchingCards(card =>
            (!onlyMonster || card.IsMonster()) && card.HasSetcode(SetcodeOrcust)).ToList();
            return result;
        }

        // Whether spell or trap will be negate. If so, return true.
        public bool SpellNegatable(bool isCounter = false, ClientCard target = null)
        {
            // target default set
            if (target == null) target = Card;
            // won't negate if not on field
            if (target.Location != CardLocation.SpellZone && target.Location != CardLocation.Hand) return false;

            // negate judge
            if (Enemy.HasInMonstersZone(CardId.NaturalExterio, true) && !isCounter) return true;
            if (target.IsSpell())
            {
                if (Enemy.HasInMonstersZone(CardId.NaturalBeast, true)) return true;
                if (Enemy.HasInSpellZone(CardId.ImperialOrder, true) || Bot.HasInSpellZone(CardId.ImperialOrder, true)) return true;
                if (Enemy.HasInMonstersZone(CardId.SwordsmanLV7, true) || Bot.HasInMonstersZone(CardId.SwordsmanLV7, true)) return true;
            }
            if (target.IsTrap())
            {
                if (Enemy.HasInSpellZone(CardId.RoyalDecree, true) || Bot.HasInSpellZone(CardId.RoyalDecree, true)) return true;
            }
            if (target.Location == CardLocation.SpellZone && (target.IsSpell() || target.IsTrap()))
            {
                int selfSeq = -1;
                for (int i = 0; i < 5; ++i)
                {
                    if (Bot.SpellZone[i] == Card) selfSeq = i;
                }
                if (infiniteImpermanenceList.Contains(selfSeq))
                {
                    return true;
                }
            }
            // how to get here?
            return false;
        }

        //Check whether'll be negated
        public bool CheckWhetherNegated(bool disablecheck = true)
        {
            if (Card.IsSpell() || Card.IsTrap())
            {
                if (SpellNegatable()) return true;
            }
            if (CheckCalledbytheGrave(Card.Id) > 0)
            {
                return true;
            }
            if (Card.IsMonster() && Card.Location == CardLocation.MonsterZone && Card.IsDefense())
            {
                if (Enemy.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.Number41BagooskatheTerriblyTiredTapir) && card.IsDefense() && !card.IsDisabled()) != null
                    || Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.IsCode(CardId.Number41BagooskatheTerriblyTiredTapir) && card.IsDefense() && !card.IsDisabled()) != null)
                {
                    return true;
                }
            }
            if (disablecheck)
            {
                return Card.IsDisabled();
            }
            return false;
        }

        //To avoid InfiniteImpermanence
        public void SelectSTPlace(ClientCard card = null, bool avoidImpermanence = false, List<int> avoidList = null)
        {
            if (card == null) card = Card;
            List<int> list = new List<int>();
            for (int seq = 0; seq < 5; ++seq)
            {
                if (Bot.SpellZone[seq] == null)
                {
                    if (card != null && card.Location == CardLocation.Hand && avoidImpermanence && infiniteImpermanenceList.Contains(seq)) continue;
                    if (avoidList != null && avoidList.Contains(seq)) continue;
                    list.Add(seq);
                }
            }
            int n = list.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(list.Count);
                int nextIndex = (index + Program.Rand.Next(list.Count - 1)) % list.Count;
                int tempInt = list[index];
                list[index] = list[nextIndex];
                list[nextIndex] = tempInt;
            }
            if (avoidImpermanence && Bot.GetMonsters().Any(c => c.IsFaceup() && !c.IsDisabled()))
            {
                foreach (int seq in list)
                {
                    ClientCard enemySpell = Enemy.SpellZone[4 - seq];
                    if (enemySpell != null && enemySpell.IsFacedown()) continue;
                    int zone = (int)System.Math.Pow(2, seq);
                    AI.SelectPlace(zone);
                    return;
                }
            }
            foreach (int seq in list)
            {
                int zone = (int)System.Math.Pow(2, seq);
                AI.SelectPlace(zone);
                return;
            }
            AI.SelectPlace(0);
        }

        public override void OnChainSolved(int chainIndex)
        {
            ClientCard currentCard = Duel.GetCurrentSolvingChainCard();
            if (currentCard != null && !Duel.IsCurrentSolvingChainNegated() && currentCard.Controller == 1)
            {
                if (currentCard.IsCode(_CardId.MaxxC))
                    enemyActivateMaxxC = true;
                if (currentCard.IsCode(CardId.Fuwalos))
                    enemyActivateMaxxC = true;
                if (currentCard.IsCode(_CardId.LockBird))
                    enemyActivateLockBird = true;
                if (currentCard.IsCode(_CardId.InfiniteImpermanence))
                {
                    for (int i = 0; i < 5; ++i)
                    {
                        if (Enemy.SpellZone[i] == currentCard)
                        {
                            infiniteImpermanenceList.Add(4 - i);
                            break;
                        }
                    }
                }
            }
        }

        public override void OnChainEnd()
        {
            targetedNo103List.Clear();
            if (activatedNo103List.Count() > 0)
            {
                for (int idx = activatedNo103List.Count() - 1; idx >= 0; --idx)
                {
                    ClientCard checkTarget = activatedNo103List[idx];
                    if (checkTarget == null || checkTarget.IsFacedown() || checkTarget.Location != CardLocation.MonsterZone)
                    {
                        activatedNo103List.RemoveAt(idx);
                    }
                }
            }
            targetedDDList.Clear();
            if (activatedDDList.Count() > 0)
            {
                for (int idx = activatedDDList.Count() - 1; idx >= 0; --idx)
                {
                    ClientCard checkTarget = activatedDDList[idx];
                    if (checkTarget == null || checkTarget.IsFacedown() || checkTarget.Location != CardLocation.MonsterZone)
                    {
                        activatedDDList.RemoveAt(idx);
                    }
                }
            }
            targetedHoleList.Clear();
            if (activatedHoleList.Count() > 0)
            {
                for (int idx = activatedHoleList.Count() - 1; idx >= 0; --idx)
                {
                    ClientCard checkTarget = activatedHoleList[idx];
                    if (checkTarget == null || checkTarget.IsFacedown() || checkTarget.Location != CardLocation.MonsterZone)
                    {
                        activatedHoleList.RemoveAt(idx);
                    }
                }
            }
            if (spSummonThisTurn.Count() > 0)
            {
                for (int idx = spSummonThisTurn.Count() - 1; idx >= 0; --idx)
                {
                    ClientCard checkTarget = spSummonThisTurn[idx];
                    if (checkTarget == null || checkTarget.IsFacedown() || checkTarget.Location != CardLocation.MonsterZone)
                    {
                        spSummonThisTurn.RemoveAt(idx);
                    }
                }
            }
        }

        public override void OnNewTurn()
        {
            currentNegatingIdList = new List<int>();
            enemyActivateMaxxC = false;
            enemyActivateLockBird = false;
            infiniteImpermanenceList = new List<int>();
            removeChosenList = new List<ClientCard>();
            summoned = false;
            SearchEX = false;
            Cross = false;
            Fuwa = false;
            activatedNo103List.Clear();
            activatedDDList.Clear();
            activatedHoleList.Clear();
            spSummonThisTurn.Clear();
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            YGOSharp.OCGWrapper.NamedCard cardData = YGOSharp.OCGWrapper.NamedCard.Get(cardId);
            if (cardData != null)
            {
                if (Util.IsTurn1OrMain2())
                {
                    bool turnDefense = false;
                    if (cardId == CardId.DivineArsenalAAZEUS_SkyThunder)
                    {
                        turnDefense = true;
                    }
                    if (!cardData.HasType(CardType.Xyz))
                    {
                        turnDefense = true;
                    }
                    if (turnDefense)
                    {
                        return CardPosition.FaceUpDefence;
                    }
                }
                if (Duel.Player == 1)
                {
                    if (!cardData.HasType(CardType.Xyz) || cardData.Defense >= cardData.Attack || Util.IsOneEnemyBetterThanValue(cardData.Attack, true))
                    {
                        return CardPosition.FaceUpDefence;
                    }
                }
                int bestBotAttack = Math.Max(Util.GetBestAttack(Bot), cardData.Attack);
                if (Util.IsAllEnemyBetterThanValue(bestBotAttack, true))
                {
                    return CardPosition.FaceUpDefence;
                }
            }
            return base.OnSelectPosition(cardId, positions);
        }

        public bool AshBlossomActivate()
        {
            if (CheckCross())
            {
                return false;
            }
            if (CheckWhetherNegated(true) || !CheckLastChainShouldNegated()) return false;
            if (Duel.LastChainPlayer == 1 && Util.GetLastChainCard().IsCode(_CardId.MaxxC))
            {
                if (CheckAtAdvantage())
                {
                    return false;
                }
            }
            return DefaultAshBlossomAndJoyousSpring();
        }

        public bool MaxxCActivate()
        {
            if (CheckWhetherNegated(true) || Duel.LastChainPlayer == 0) return false;
            return DefaultMaxxC();
        }

        public bool InfiniteImpermanenceActivate()
        {
            if (CheckCross())
            {
                return false;
            }
            if (CheckWhetherNegated()) return false;
            // negate before effect used
            foreach (ClientCard m in Enemy.GetMonsters())
            {
                if (m.IsMonsterShouldBeDisabledBeforeItUseEffect() && !m.IsDisabled() && Duel.LastChainPlayer != 0)
                {
                    if (Card.Location == CardLocation.SpellZone)
                    {
                        for (int i = 0; i < 5; ++i)
                        {
                            if (Bot.SpellZone[i] == Card)
                            {
                                infiniteImpermanenceList.Add(i);
                                break;
                            }
                        }
                    }
                    if (Card.Location == CardLocation.Hand)
                    {
                        SelectSTPlace(Card, true);
                    }
                    AI.SelectCard(m);
                    return true;
                }
            }

            ClientCard LastChainCard = Util.GetLastChainCard();

            // negate spells
            if (Card.Location == CardLocation.SpellZone)
            {
                int this_seq = -1;
                int that_seq = -1;
                for (int i = 0; i < 5; ++i)
                {
                    if (Bot.SpellZone[i] == Card) this_seq = i;
                    if (LastChainCard != null
                        && LastChainCard.Controller == 1 && LastChainCard.Location == CardLocation.SpellZone && Enemy.SpellZone[i] == LastChainCard) that_seq = i;
                    else if (Duel.Player == 0 && Util.GetProblematicEnemySpell() != null
                        && Enemy.SpellZone[i] != null && Enemy.SpellZone[i].IsFloodgate()) that_seq = i;
                }
                if ((this_seq * that_seq >= 0 && this_seq + that_seq == 4)
                    || (Util.IsChainTarget(Card))
                    || (LastChainCard != null && LastChainCard.Controller == 1 && LastChainCard.IsCode(_CardId.HarpiesFeatherDuster)))
                {
                    ClientCard target = GetProblematicEnemyMonster(canBeTarget: true);
                    List<ClientCard> enemyMonsters = Enemy.GetMonsters();
                    AI.SelectCard(target);
                    infiniteImpermanenceList.Add(this_seq);
                    return true;
                }
            }
            if ((LastChainCard == null || LastChainCard.Controller != 1 || LastChainCard.Location != CardLocation.MonsterZone
                || LastChainCard.IsDisabled() || LastChainCard.IsShouldNotBeTarget() || LastChainCard.IsShouldNotBeSpellTrapTarget()))
                return false;
            // negate monsters
            if (Card.Location == CardLocation.SpellZone)
            {
                for (int i = 0; i < 5; ++i)
                {
                    if (Bot.SpellZone[i] == Card)
                    {
                        infiniteImpermanenceList.Add(i);
                        break;
                    }
                }
            }
            if (Card.Location == CardLocation.Hand)
            {
                SelectSTPlace(Card, true);
            }
            if (LastChainCard != null) AI.SelectCard(LastChainCard);
            else
            {
                List<ClientCard> enemyMonsters = Enemy.GetMonsters();
                enemyMonsters.Sort(CardContainer.CompareCardAttack);
                enemyMonsters.Reverse();
                foreach (ClientCard card in enemyMonsters)
                {
                    if (card.IsFaceup() && !card.IsShouldNotBeTarget() && !card.IsShouldNotBeSpellTrapTarget())
                    {
                        AI.SelectCard(card);
                        return true;
                    }
                }
            }
            return true;
        }

        public bool CalledbytheGraveActivate()
        {
            if (CheckCross())
            {
                return false;
            }
            if (CheckWhetherNegated(true)) return false;
            if (Duel.LastChainPlayer == 1)
            {
                // negate
                if (Util.GetLastChainCard().IsMonster())
                {
                    int code = Util.GetLastChainCard().GetOriginCode();
                    if (code == 0) return false;
                    if (CheckCalledbytheGrave(code) > 0) return false;
                    if (Util.GetLastChainCard().IsCode(_CardId.MaxxC) && CheckAtAdvantage())
                    {
                        return false;
                    }
                    if (code == CardId.DimensionShifter)
                    {
                        return false;
                    }
                    ClientCard targetCard = Enemy.Graveyard.GetFirstMatchingCard(card => card.IsMonster() && card.IsOriginalCode(code));
                    if (targetCard != null)
                    {
                        if (!(Card.Location == CardLocation.SpellZone))
                        {
                            SelectSTPlace(null, true);
                        }
                        AI.SelectCard(targetCard);
                        currentNegatingIdList.Add(code);
                        return true;
                    }
                }

                // banish target
                foreach (ClientCard cards in Enemy.Graveyard)
                {
                    if (Duel.ChainTargets.Contains(cards))
                    {
                        int code = cards.Id;
                        AI.SelectCard(cards);
                        currentNegatingIdList.Add(code);
                        return true;
                    }
                }

                // become targets
                if (Duel.ChainTargets.Contains(Card))
                {
                    List<ClientCard> enemyMonsters = Enemy.Graveyard.GetMatchingCards(card => card.IsMonster()).ToList();
                    if (enemyMonsters.Count > 0)
                    {
                        enemyMonsters.Sort(CardContainer.CompareCardAttack);
                        enemyMonsters.Reverse();
                        int code = enemyMonsters[0].Id;
                        AI.SelectCard(code);
                        currentNegatingIdList.Add(code);
                        return true;
                    }
                }
            }

            // avoid danger monster in grave
            if (Duel.LastChainPlayer == 1) return false;
            List<ClientCard> targets = CheckDangerousCardinEnemyGrave(true);
            if (targets.Count() > 0)
            {
                int code = targets[0].GetOriginCode();
                if (!(Card.Location == CardLocation.SpellZone))
                {
                    SelectSTPlace(null, true);
                }
                AI.SelectCard(targets);
                currentNegatingIdList.Add(code);
                return true;
            }

            return false;
        }

        public bool Level4SummonCheck()
        {
            // _CardId.MaxxC = 23434538;
            // _CardId.AshBlossom = 14558127;
            if ((Card.Id == 23434538) || (Card.Id == 14558127) || (Card.Id == CardId.RyzealEx))
            {
                return false;
            }
            if (Bot.GetMonsters().Count(card => CheckAbleForXyz(card)) == 1)
            {
                summoned = true;
                return true;
            }
            return false;
        }

        public bool SpellSetCheck()
        {
            if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1) return false;
            List<int> onlyOneSetList = new List<int>{
                CardId.RyzealPlugin, CardId.RyzealHole
            };
            if (onlyOneSetList.Contains(Card.Id) && Bot.HasInSpellZone(Card.Id))
            {
                return false;
            }

            // select place
            if ((Card.IsTrap() || Card.HasType(CardType.QuickPlay)))
            {
                List<int> avoid_list = new List<int>();
                int setFornfiniteImpermanence = 0;
                for (int i = 0; i < 5; ++i)
                {
                    if (Enemy.SpellZone[i] != null && Enemy.SpellZone[i].IsFaceup() && Bot.SpellZone[4 - i] == null)
                    {
                        avoid_list.Add(4 - i);
                        setFornfiniteImpermanence += (int)System.Math.Pow(2, 4 - i);
                    }
                }
                if (Bot.HasInHand(_CardId.InfiniteImpermanence))
                {
                    if (Card.IsCode(_CardId.InfiniteImpermanence))
                    {
                        AI.SelectPlace(setFornfiniteImpermanence);
                        return true;
                    }
                    else
                    {
                        SelectSTPlace(Card, false, avoid_list);
                        return true;
                    }
                }
                else
                {
                    SelectSTPlace();
                }
                return true;
            }

            return false;
        }

        protected override bool DefaultSetForDiabellze()
        {
            if (base.DefaultSetForDiabellze())
            {
                SelectSTPlace(null, true);
                return true;
            }
            return false;
        }

        private int _totalAttack;
        private int _totalBotAttack;
        private bool RedoerEffect()
        {

            List<ClientCard> enemy = Enemy.GetMonstersInMainZone();
            List<int> units = Card.Overlays;
            if (Duel.Phase == DuelPhase.Standby && (AI.Executor.Util.GetStringId(CardId.TimeThiefRedoer, 0) ==
                                                    ActivateDescription))
            {

                return true;
            }

            try
            {
                for (int i = 0; i < enemy.Count; i++)
                {
                    _totalAttack += enemy[i].Attack;
                }

                foreach (var t in Bot.GetMonsters())
                {
                    _totalBotAttack += t.Attack;
                }

                if (_totalAttack > Bot.LifePoints + _totalBotAttack)
                {
                    return false;
                }



                foreach (var t in enemy)
                {
                    if (t.Attack < 2400 || !t.IsAttack()) continue;
                    try
                    {
                        AI.SelectCard(t.Id);
                        AI.SelectCard(t.Id);
                    }
                    catch { }

                    return true;
                }
            }
            catch { }

            if (Bot.UnderAttack)
            {
                //AI.SelectCard(Util.GetBestEnemyMonster());
                return true;
            }

            return false;

        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            Logger.DebugWriteLine("=========================");
            Logger.DebugWriteLine("OnSelectCard " + cards.Count + " " + min + " " + max);
            for(int i = 0; i < cards.Count; i++)
            {
                Logger.DebugWriteLine("OnSelectCard Select " + cards[i].Name);
            }
            Logger.DebugWriteLine("=========================");
            //Ryzeal Duo Search effect
            if (max == 2 && cards[0].Location == CardLocation.Deck && hint == HintMsg.AddToHand)
            {
                Logger.DebugWriteLine("OnSelectCard Duo");
                List<ClientCard> result = new List<ClientCard>();
                foreach (ClientCard listCard in cards)
                {
                    if (listCard.HasSetcode(0x1be))
                    {
                        if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.RyzealThode) && listCard.IsCode(CardId.RyzealThode))
                        {
                            result.Add(listCard);
                        }
                        else if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.RyzealNode) && listCard.IsCode(CardId.RyzealNode))
                        {
                            result.Add(listCard);
                        }
                        else if (!Bot.HasInHandOrHasInMonstersZone(CardId.RyzealCross) && listCard.IsCode(CardId.RyzealCross))
                        {
                            result.Add(listCard);
                        }
                        else if (!Bot.HasInHandOrHasInMonstersZone(CardId.RyzealHole) && listCard.IsCode(CardId.RyzealHole))
                        {
                            result.Add(listCard);
                        }
                        else if (!Bot.HasInHandOrHasInMonstersZone(CardId.RyzealPlugin) && listCard.IsCode(CardId.RyzealPlugin))
                        {
                            result.Add(listCard);
                        }
                        else
                        {
                            result.Add(listCard);
                        }
                    }
                }
                return Util.CheckSelectCount(result, cards, min, max);
            }
            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        public override int OnSelectOption(IList<int> options)
        {
            ClientCard currentSolvingChain = Duel.GetCurrentSolvingChainCard();
            if (currentSolvingChain != null)
            {
                if (options.Count == 2 && options.Contains(1190))
                {
                    return options.IndexOf(1190);
                }
                if (options.Contains(Util.GetStringId(CardId.TTT, 1)))
                {
                    return 1;
                }
            }
            return base.OnSelectOption(options);
        }

        private bool Number41BagooskaTheTerriblyTiredTapirSummon()
        {
            if (!Util.IsTurn1OrMain2())
                return false;
            if (Bot.GetMonsterCount() > 3)
                return false;
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }
        private bool MonsterRepos()
        {
            if (Card.IsFacedown())
                return true;
            if (Card.IsCode(CardId.Number41BagooskatheTerriblyTiredTapir) && Card.IsDefense())
                return Card.Overlays.Count == 0;
            return DefaultMonsterRepos();
        }


        //=========================================================
        // Original Add
        //=========================================================

        public bool FuwaActivate()
        {
            if (Duel.Player != 1){ return false; }
            if (Fuwa) { return false; }
            if (Duel.Player == 1 && Duel.Phase == DuelPhase.Standby)
            {
                Fuwa = true;
                return DefaultMaxxC();
            }
            if (CheckWhetherNegated(true) || Duel.LastChainPlayer == 0) return false;
            Fuwa = true;
            return DefaultMaxxC();
        }
        public bool PalmaSS()
        {
            if (CheckLessOperation() == true) { return false; }
            if (Bot.GetMonsterCount() > 3 && Bot.GetHandCount() < 3)
            { return false; }
            List<ClientCard> ReturnList = new List<ClientCard>();
            foreach (ClientCard graveCard in Bot.Graveyard)
            {
                if (graveCard.HasSetcode(0x1be))
                {
                    if (graveCard.IsCode(CardId.RyzealPalma))
                    {
                        ReturnList.Add(graveCard);
                    }
                    else if (graveCard.HasType(CardType.Xyz) && graveCard.ProcCompleted == 0)
                    {
                        ReturnList.Add(graveCard);
                    }
                    else if (graveCard.HasType(CardType.Spell) || graveCard.HasType(CardType.Trap))
                    {
                        ReturnList.Add(graveCard);
                    }
                    else
                    {
                        ReturnList.Add(graveCard);
                    }
                }
            }
            if (ReturnList.Count > 0) { return true; }
            else { return true; }
        }
        public bool ExSS()
        {
            if (CheckLessOperation() == true) { return false; }
            List<ClientCard> ExCostList = ShuffleCardList(Bot.Graveyard.Where(card => card != null && card.IsMonster() && card.HasType(CardType.Xyz)).ToList());
            List<ClientCard> CompList = ShuffleCardList(Bot.Graveyard.Where(card => card != null && card.IsMonster() && card.HasType(CardType.Xyz) && card.ProcCompleted == 1).ToList());
            int tributeId = -1;
            if (Bot.HasInExtra(CardId.No103))
            { tributeId = CardId.No103; }
            if (Bot.HasInExtra(CardId.No104) && CheckRemainInDeck(CardId.Seven) == 0)
            { tributeId = CardId.No104; }
            if (Bot.HasInExtra(CardId.No101) && (CheckRemainInDeck(CardId.BigJaw) == 0 || CheckRemainInDeck(CardId.Seven) == 0))
            { tributeId = CardId.No101; }
            if (Bot.HasInExtra(CardId.RyzealDuo) && !Bot.HasInMonstersZoneOrInGraveyard(CardId.RyzealDuo) && !Bot.HasInBanished(CardId.RyzealDuo))
            { tributeId = CardId.RyzealDuo; }
            if (Bot.HasInExtra(CardId.RyzealDD) && !Bot.HasInMonstersZoneOrInGraveyard(CardId.RyzealDD) && !Bot.HasInBanished(CardId.RyzealDD))
            { tributeId = CardId.RyzealDD; }
            if ((Bot.HasInExtra(CardId.TwinE)) && ExCostList.Count > 0 && CompList.Count > 0 && Bot.GetMonsterCount() < 4)
            { tributeId = CardId.TwinE; }
            if (tributeId != -1)
            {
                SearchEX = true;
                AI.SelectCard(tributeId);
                AI.SelectPosition(CardPosition.FaceUpDefence);
                return true;
            }
            return false;
        }
        public bool ExEffect()
        {
            if (!Bot.HasInHandOrInMonstersZoneOrInGraveyard(CardId.RyzealThode))
            {
                AI.SelectCard(CardId.RyzealThode);
                return true;
            }
            else if (!Bot.HasInHand(CardId.RyzealNode))
            {
                AI.SelectCard(CardId.RyzealNode);
                return true;
            }
            else { return true; }
        }
        public bool ThodeEffect()
        {
            if (!Bot.HasInHand(CardId.RyzealIce))
            {
                AI.SelectCard(CardId.RyzealIce);
                return true;
            }
            else if (!Bot.HasInHand(CardId.RyzealEx))
            {
                AI.SelectCard(CardId.RyzealEx);
                return true;
            }
            else { return true; }
        }
        public bool DuoSS()
        {
            //if (CheckLessOperation() == true) { return false; }
            if (Bot.HasInMonstersZone(CardId.RyzealDuo))
            {
                return false;
            }
            SelectXyzMaterial();
            AI.SelectPosition(CardPosition.FaceUpAttack);
            return true;
        }
        public bool DDSS()
        {
            //if (CheckLessOperation() == true) { return false; }
            if (Bot.HasInMonstersZone(CardId.RyzealDD))
            {
                return false;
            }
            SelectXyzMaterial(2,true);
            AI.SelectPosition(CardPosition.FaceUpAttack);
            return true;
        }        
        public bool HandStarterEXCheck()
        {
            if (!Bot.HasInHandOrHasInMonstersZone(CardId.RyzealEx) && (SearchEX == false))
            {
                SelectSTPlace(Card, true);
                AI.SelectCard(CardId.RyzealEx);
                return true;
            }
            if (CheckRemainInDeck(CardId.RyzealIce) > 0)
            {
                SelectSTPlace(Card, true);
                AI.SelectCard(CardId.RyzealIce);
                return true;
            }
            return false;
        }
        public bool SevenActivate()
        {
            if (Bot.Hand.Count > 2) { return false; }
            if (!Bot.HasInHandOrHasInMonstersZone(CardId.RyzealEx) && (SearchEX == false))
            {
                SelectSTPlace(Card, true);
                AI.SelectCard(CardId.No104);
                AI.SelectNextCard(CardId.RyzealEx);
                return true;
            }
            else if (!Bot.HasInHandOrHasInMonstersZone(CardId.BigJaw) && CheckRemainInDeck(CardId.BigJaw) > 0)
            {
                SelectSTPlace(Card, true);
                AI.SelectCard(CardId.No101);
                AI.SelectNextCard(CardId.BigJaw);
                return true;
            }
            return false;
        }
        public bool PluginEffect()
        {
            SelectSTPlace(Card, true);
            return true;
        }
        public bool HoleEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.RyzealHole, 1))
            {
                return false;
            }
            if (ActivateDescription == Util.GetStringId(CardId.RyzealHole, 0))
            {
                if (CheckCross())
                {
                    return false;
                }
                if (CheckWhetherNegated())
                {
                    return false;
                }
                // Destroy problem card
                ClientCard target = GetProblematicEnemyCard();
                bool isProblemCard = false;
                if (target != null)
                {
                    isProblemCard = true;
                    Logger.DebugWriteLine("===Hole target 1: " + target?.Name);
                }
                // Destroy target
                List<ClientCard> currentTargetList = Duel.LastChainTargets.Where(card => card.Controller == 1 &&
                        (card.Location == CardLocation.MonsterZone || card.Location == CardLocation.SpellZone || card.Location == CardLocation.FieldZone)).ToList();

                if (Duel.LastChainPlayer == 1 && target == null)
                {
                    if (currentTargetList.Count() > 0)
                    {
                        ShuffleCardList(currentTargetList);
                        target = currentTargetList[0];
                        Logger.DebugWriteLine("===Hole target 2: " + target?.Name);
                    }
                }

                // dump Destroy
                if (target == null)
                {
                    target = GetBestEnemyCard(false, false);
                    bool check1 = !DefaultOnBecomeTarget() || Util.ChainContainsCard(_CardId.EvenlyMatched);
                    bool check2 = !targetedHoleList.Contains(Card);
                    bool check3 = !Bot.UnderAttack;
                    bool check4 = Duel.Phase != DuelPhase.End;
                    bool check5 = Duel.Player == 0 || Enemy.GetMonsterCount() < 2;
                    Logger.DebugWriteLine("===Hole check flag: " + check1 + " " + check2 + " " + check3 + " " + check4 + " " + check5);
                    if (check1 && check2 && check3 && check4 && check5)
                    {
                        target = null;
                    }
                }

                if (target != null && (Duel.LastChainPlayer != 0 || Util.GetLastChainCard() == Card))
                {
                    if (isProblemCard)
                    {
                        removeChosenList.Add(target);
                    }
                    Logger.DebugWriteLine("===Hole target final: " + target?.Name);
                    activatedHoleList.Add(Card);
                    AI.SelectCard();
                    if(currentTargetList.Count() > 1)
                    {
                        AI.SelectNextCard(currentTargetList);
                    }
                    else
                    {
                        AI.SelectNextCard(target);
                    }
                    
                    return true;
                }

                return false;
            }

            return false;
        }
        public bool DestroyEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.RyzealDD, 0))
            {
                Logger.DebugWriteLine("===DD Activate1: " + ActivateDescription);
                AI.SelectCard();
                return true;
            }
            // Destroy
            if ((ActivateDescription == Util.GetStringId(CardId.RyzealDD, 1)) || 
                (ActivateDescription == Util.GetStringId(CardId.No103, 0)))
            {
                Logger.DebugWriteLine("===DD Activate2: " + ActivateDescription);
                if (CheckCross())
                {
                    return false;
                }
                if (CheckWhetherNegated())
                {
                    return false;
                }
                // Destroy problem card
                ClientCard target = GetProblematicEnemyCard();
                bool isProblemCard = false;
                if (target != null)
                {
                    isProblemCard = true;
                    Logger.DebugWriteLine("===DD target 1: " + target?.Name);
                }
                // Destroy target
                if (Duel.LastChainPlayer == 1 && target == null)
                {
                    List<ClientCard> currentTargetList = Duel.LastChainTargets.Where(card => card.Controller == 1 &&
                        (card.Location == CardLocation.MonsterZone || card.Location == CardLocation.SpellZone || card.Location == CardLocation.FieldZone)).ToList();
                    if (currentTargetList.Count() > 0)
                    {
                        target = ShuffleCardList(currentTargetList)[0];
                        Logger.DebugWriteLine("===DD target 2: " + target?.Name);
                    }
                }

                // dump Destroy
                if (target == null)
                {
                    target = GetBestEnemyCard(false, false);
                    bool check1 = !DefaultOnBecomeTarget() || Util.ChainContainsCard(_CardId.EvenlyMatched);
                    bool check2 = !targetedDDList.Contains(Card);
                    bool check3 = !Bot.UnderAttack;
                    bool check4 = Duel.Phase != DuelPhase.End;
                    bool check5 = Duel.Player == 0 || Enemy.GetMonsterCount() < 2;
                    Logger.DebugWriteLine("===DD check flag: " + check1 + " " + check2 + " " + check3 + " " + check4 + " " + check5);
                    if (check1 && check2 && check3 && check4 && check5)
                    {
                        target = null;
                    }
                }

                if (target != null && (Duel.LastChainPlayer != 0 || Util.GetLastChainCard() == Card))
                {
                    if (isProblemCard)
                    {
                        removeChosenList.Add(target);
                    }
                    Logger.DebugWriteLine("===DD target final: " + target?.Name);
                    activatedDDList.Add(Card);
                    AI.SelectCard();
                    AI.SelectNextCard(target);
                    return true;
                }

                return false;
            }
            return false;
        }
        public bool CrossEffect()
        {
            if (ActivateDescription == Util.GetStringId(CardId.RyzealCross, 1))
            {
                List<ClientCard> ReturnList = new List<ClientCard>();
                //ShuffleCardList(Bot.Graveyard.Where(card => card != null && card.IsMonster() && card.HasType(CardType.Xyz) && card.ProcCompleted == 0).ToList());
                foreach (ClientCard graveCard in Bot.Graveyard)
                {
                    if (graveCard.HasSetcode(0x1be))
                    {
                        if (graveCard.HasType(CardType.Xyz) && graveCard.ProcCompleted == 0)
                        {
                            ReturnList.Add(graveCard);
                        }
                        else if (graveCard.HasType(CardType.Spell) || graveCard.HasType(CardType.Trap))
                        {
                            ReturnList.Add(graveCard);
                        }
                        else if (graveCard.IsCode(CardId.RyzealEx))
                        {
                            ReturnList.Add(graveCard);
                        }
                        else
                        {
                            ReturnList.Add(graveCard);
                        }
                    }
                }
                AI.SelectCard(ReturnList);
                return true;
            }
            if (Bot.HasInSpellZone(CardId.RyzealCross))
            {
                return false;
            }
            return true;
        }
        public bool CheckCross()
        {
            if (Bot.HasInSpellZone(CardId.RyzealCross) && 
                Cross == false &&
                (Bot.HasInMonstersZone(CardId.RyzealDD) && Card.HasXyzMaterial(CardId.RyzealDD) ||
                Bot.HasInMonstersZone(CardId.RyzealDuo) && Card.HasXyzMaterial(CardId.RyzealDuo))
               )
            {
                return true;
            }
            return false;
        }

    }
}
