using YGOSharp.OCGWrapper.Enums;
using System.Collections.Generic;
using WindBot;
using WindBot.Game;
using WindBot.Game.AI;
using System.Linq;
using System;
using YGOSharp.OCGWrapper;


namespace WindBot.Game.AI.Decks
{
    [Deck("Voiceless", "AI_Voiceless")]
    public class VoicelessExecutor : DefaultExecutor
    {
        public class CardId
        {
            public const int Saffira = 51296484;
            public const int Lo = 25801745;
            public const int VVSauravis = 88284599;
            public const int VVSkullGuard = 10774240;
            public const int VVPrayer = 52472775;
            public const int VVBarrier = 98477480;
            public const int VVRadiance = 86310763;
            public const int VVBlessing = 39114494;
            public const int VVSaffira = 10804018;

            public const int AshBlossom = 14558127;
            public const int EffectVeiler = 97268402;
            public const int InfiniteImperm = 10045474;
            
            public const int PrePrep = 13048472;
            public const int Diviner = 92919429;
            public const int Trias = 26866984;
            public const int CalledBy = 24224830;
            public const int Sauravis = 4810828;
            public const int SolemnStrike = 40605147;

            public const int Ntss = 80532587;
            public const int Herald = 79606837;
            public const int DynaMondo = 73898890;//?

            //dump extra

            //problem list
            public const int ChaosAngel = 22850702;
            public const int LittleKnight = 29301450;
            public const int UnderworldGoddess = 98127546;
            public const int Droll = 94145021;
            public const int GhostMournerMoonlitChill = 52038441;
            public const int NaturalExterio = 99916754;
            public const int NaturalBeast = 33198837;
            public const int ImperialOrder = 61740673;
            public const int SwordsmanLV7 = 37267041;
            public const int RoyalDecree = 51452091;
            public const int Number41BagooskatheTerriblyTiredTapir = 90590303;
            public const int InspectorBoarder = 15397015;
            public const int DimensionShifter = 91800273;
        }

        //bool KagariSummoned = false;
        //bool ShizukuSummoned = false;
        //bool HayateSummoned = false;
        const int SetcodeTimeLord = 0x4a;
        const int SetcodeOrcust = 0x11b;
        const int SetcodeVV = 0x1a6;

        bool DivinerCheck = false; // In case of Trias in Hand, add either spell/ritual or board break

        public VoicelessExecutor(GameAI ai, Duel duel)
            : base(ai, duel)
        {
            AddExecutor(ExecutorType.Activate, CardId.AshBlossom, DefaultAshBlossomAndJoyousSpring);
            AddExecutor(ExecutorType.Activate, CardId.EffectVeiler, DefaultBreakthroughSkill);
            AddExecutor(ExecutorType.Activate, CardId.InfiniteImperm, DefaultBreakthroughSkill);
            AddExecutor(ExecutorType.Activate, CardId.SolemnStrike, DefaultSolemnStrike);
            AddExecutor(ExecutorType.Activate, CardId.CalledBy, CalledByEffect);
            AddExecutor(ExecutorType.Activate, CardId.Sauravis);
            AddExecutor(ExecutorType.Activate, CardId.Ntss, NtssActivate);

            AddExecutor(ExecutorType.Activate, _CardId.PotOfExtravagance, PotofExtravaganceActivate);
            AddExecutor(ExecutorType.Activate, CardId.VVBarrier, BarrierFirst);
            AddExecutor(ExecutorType.Activate, CardId.Saffira, SaffEffect);
            AddExecutor(ExecutorType.Activate, CardId.Saffira, SaffiraRitual);
            AddExecutor(ExecutorType.Activate, CardId.VVSaffira);
            AddExecutor(ExecutorType.SpSummon, CardId.VVSauravis, VVSauravisSS);
            AddExecutor(ExecutorType.Activate, CardId.VVSauravis);
            AddExecutor(ExecutorType.Summon, CardId.Diviner);
            AddExecutor(ExecutorType.Activate, CardId.Diviner, DivinerEffect);
            AddExecutor(ExecutorType.Activate, CardId.Trias, TriasEffect);

            AddExecutor(ExecutorType.SpSummon, CardId.Lo, LoEffect);
            AddExecutor(ExecutorType.Summon, CardId.Lo, LoSummon);
            AddExecutor(ExecutorType.Activate, CardId.Lo, LoPlace);
            AddExecutor(ExecutorType.Activate, CardId.VVSkullGuard, SkullSearch);
            AddExecutor(ExecutorType.Activate, CardId.VVSkullGuard, SkullCounter);
            AddExecutor(ExecutorType.Activate, CardId.PrePrep, PrePrepSearch);
            AddExecutor(ExecutorType.Activate, CardId.Herald, HeraldSearch);
            AddExecutor(ExecutorType.Activate, CardId.VVRadiance);
            AddExecutor(ExecutorType.Activate, CardId.VVBlessing);
            AddExecutor(ExecutorType.Activate, CardId.VVPrayer);


            AddExecutor(ExecutorType.Repos, MonsterRepos);
            AddExecutor(ExecutorType.SpellSet, SpellSetCheck);

        }
        List<int> notToNegateIdList = new List<int>{
                58699500, 20343502
            };
        List<int> notToBeTrapTargetList = new List<int>{
                72144675, 86188410, 41589166, 11443677, 72566043, 1688285, 59071624, 6511113, 48183890, 952523, 22423493, 73639099
            };
        List<int> targetNegateIdList = new List<int> {
                _CardId.EffectVeiler, _CardId.InfiniteImpermanence, CardId.GhostMournerMoonlitChill, _CardId.BreakthroughSkill, 74003290, 67037924,
                9753964, 66192538, 23204029, 73445448, 35103106, 30286474, 45002991, 5795980, 38511382, 53742162, 30430448
            };
        List<int> notToDestroySpellTrap = new List<int> { 50005218, 6767771 };

        bool summoned = false; //Some unused variables since code was borrowed from Louse's lab
        List<int> activatedCardIdList = new List<int>();
        List<ClientCard> currentNegateMonsterList = new List<ClientCard>();
        List<ClientCard> currentDestroyCardList = new List<ClientCard>();
        List<ClientCard> setTrapThisTurn = new List<ClientCard>();
        List<ClientCard> summonThisTurn = new List<ClientCard>();
        List<ClientCard> enemySetThisTurn = new List<ClientCard>();
        List<ClientCard> escapeTargetList = new List<ClientCard>();
        List<ClientCard> summonInChainList = new List<ClientCard>();
        List<int> infiniteImpermanenceList = new List<int>();
        List<int> currentNegatingIdList = new List<int>();
        int banSpSummonExceptFiendCount = 0;
        int enemySpSummonFromExLastTurn = 0;
        int enemySpSummonFromExThisTurn = 0;
        List<int> chainSummoningIdList = new List<int>(3);
        bool enemyActivateMaxxC = false;
        bool enemyActivateLockBird = false;
        bool enemy_activate_DimensionShifter = false;
        bool enemyMoveGrave = false;
        bool paxCallToField = false;
        bool potActivate = false;

        private bool MonsterRepos()
        {
            if (Card.IsFacedown())
                return true;
            if (Card.Id == CardId.Lo)
                return Card.IsDefense();
            if (Card.Id == CardId.Diviner)
                return Card.IsDefense();
            if (Card.Id == CardId.Trias)
                return Card.IsDefense();
            return DefaultMonsterRepos();
        }
        public bool SpellSetCheck()
        {
            if (Duel.Phase == DuelPhase.Main1 && Bot.HasAttackingMonster() && Duel.Turn > 1) return false;
            
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
        public List<T> ShuffleList<T>(List<T> list)
        {
            List<T> result = list;
            int n = result.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(result.Count);
                int nextIndex = (index + Program.Rand.Next(result.Count - 1)) % result.Count;
                T tempCard = result[index];
                result[index] = result[nextIndex];
                result[nextIndex] = tempCard;
            }
            return result;
        }

        public bool NtssActivate()
        {
            List<ClientCard> problemCardList = GetProblematicEnemyCardList(true, selfType: CardType.Monster);
            problemCardList.AddRange(GetNormalEnemyTargetList(true, true, CardType.Monster));
            if (GetProblematicEnemyCardList(true, selfType: CardType.Monster).Count() == 0) //If there's no cards on opponent's field
                return false;
            else
            {
                AI.SelectCard(problemCardList);
                activatedCardIdList.Add(Card.Id);
                return true;
            }
        }
        
        public List<ClientCard> GetNormalEnemyTargetList(bool canBeTarget = true, bool ignoreCurrentDestroy = false, CardType selfType = 0)
        {
            List<ClientCard> targetList = GetProblematicEnemyCardList(canBeTarget, selfType: selfType);
            List<ClientCard> enemyMonster = Enemy.GetMonsters().Where(card => card.IsFaceup() && !targetList.Contains(card)
                && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card))).ToList();
            enemyMonster.Sort(CardContainer.CompareCardAttack);
            enemyMonster.Reverse();
            targetList.AddRange(enemyMonster);
            targetList.AddRange(ShuffleList(Enemy.GetSpells().Where(card =>
                (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card)) && enemySetThisTurn.Contains(card)).ToList()));
            targetList.AddRange(ShuffleList(Enemy.GetSpells().Where(card =>
                (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card)) && !enemySetThisTurn.Contains(card)).ToList()));
            targetList.AddRange(ShuffleList(Enemy.GetMonsters().Where(card => card.IsFacedown() && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card))).ToList()));
            return targetList;
        }

        public ClientCard GetProblematicEnemyMonster(int attack = 0, bool canBeTarget = false, bool ignoreCurrentDestroy = false, CardType selfType = 0)
        {
            List<ClientCard> dangerList = Enemy.MonsterZone.Where(c => c?.Data != null &&
                c.IsMonsterDangerous() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)
                && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))).OrderByDescending(card => card.Attack).ToList();
            if (dangerList.Count() > 0) return dangerList[0];

            List<ClientCard> invincibleList = Enemy.MonsterZone.Where(c => c?.Data != null &&
                c.IsMonsterInvincible() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)
                && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))).OrderByDescending(card => card.Attack).ToList();
            if (invincibleList.Count() > 0) return invincibleList[0];

            List<ClientCard> equippedList = Enemy.MonsterZone.Where(c => c?.Data != null &&
                c.EquipCards.Count() > 0 && CheckCanBeTargeted(c, canBeTarget, selfType)
                && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(c))).OrderByDescending(card => card.Attack).ToList();
            if (equippedList.Count() > 0) return equippedList[0];

            List<ClientCard> enemyMonsters = Enemy.GetMonsters().OrderByDescending(card => card.Attack).ToList();
            if (enemyMonsters.Count() > 0)
            {
                foreach (ClientCard target in enemyMonsters)
                {
                    if ((target.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz)
                            || (target.HasType(CardType.Link) && target.LinkCount >= 2))
                        && CheckCanBeTargeted(target, canBeTarget, selfType) && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(target))
                    ) return target;
                }
            }

            if (attack >= 0)
            {
                if (attack == 0)
                    attack = Util.GetBestAttack(Bot);
                List<ClientCard> betterList = Enemy.MonsterZone.GetMonsters()
                    .Where(card => card.GetDefensePower() >= attack && card.GetDefensePower() > 0 && card.IsAttack() && CheckCanBeTargeted(card, canBeTarget, selfType)
                    && (!ignoreCurrentDestroy || !currentDestroyCardList.Contains(card))).OrderByDescending(card => card.Attack).ToList();
                if (betterList.Count() > 0) return betterList[0];
            }
            return null;
        }

        public List<ClientCard> GetProblematicEnemyCardList(bool canBeTarget = false, bool ignoreSpells = false, CardType selfType = 0)
        {
            List<ClientCard> resultList = new List<ClientCard>();
            
            List<ClientCard> problemEnemySpellList = Enemy.SpellZone.Where(c => c?.Data != null && !resultList.Contains(c) && !currentDestroyCardList.Contains(c)
                && c.IsFloodgate() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).ToList();
            if (problemEnemySpellList.Count() > 0) resultList.AddRange(ShuffleList(problemEnemySpellList));

            List<ClientCard> dangerList = Enemy.MonsterZone.Where(c => c?.Data != null && !resultList.Contains(c) && !currentDestroyCardList.Contains(c)
                && c.IsMonsterDangerous() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).ToList();
            if (dangerList.Count() > 0
                && (Duel.Player == 0 || (Duel.Phase > DuelPhase.Main1 && Duel.Phase < DuelPhase.Main2))) resultList.AddRange(dangerList);

            List<ClientCard> invincibleList = Enemy.MonsterZone.Where(c => c?.Data != null && !resultList.Contains(c) && !currentDestroyCardList.Contains(c)
                && c.IsMonsterInvincible() && c.IsFaceup() && CheckCanBeTargeted(c, canBeTarget, selfType)).OrderByDescending(card => card.Attack).ToList();
            if (invincibleList.Count() > 0) resultList.AddRange(invincibleList);

            List<ClientCard> enemyMonsters = Enemy.GetMonsters().Where(c => !currentDestroyCardList.Contains(c)).OrderByDescending(card => card.Attack).ToList();
            if (enemyMonsters.Count() > 0)
            {
                foreach(ClientCard target in enemyMonsters)
                {
                    if ( (target.HasType(CardType.Fusion | CardType.Ritual | CardType.Synchro | CardType.Xyz)
                            || (target.HasType(CardType.Link) && target.LinkCount >= 2))
                        && !resultList.Contains(target) && CheckCanBeTargeted(target, canBeTarget, selfType)
                        )
                    {
                        resultList.Add(target);
                    }
                }
            }

            List<ClientCard> spells = Enemy.GetSpells().Where(c => c.IsFaceup() && !currentDestroyCardList.Contains(c)
                && c.HasType(CardType.Equip | CardType.Pendulum | CardType.Field | CardType.Continuous) && CheckCanBeTargeted(c, canBeTarget, selfType)
                && !notToDestroySpellTrap.Contains(c.Id)).ToList();
            if (spells.Count() > 0 && !ignoreSpells) resultList.AddRange(ShuffleList(spells));

            return resultList;
        }

        public bool CheckCanBeTargeted(ClientCard card, bool canBeTarget, CardType selfType)
        {
            if (card == null) return true;
            if (canBeTarget)
            {
                if (card.IsShouldNotBeTarget()) return false;
                if (((int)selfType & (int)CardType.Monster) > 0 && card.IsShouldNotBeMonsterTarget()) return false;
                if (((int)selfType & (int)CardType.Spell) > 0 && card.IsShouldNotBeSpellTrapTarget()) return false;
                if (((int)selfType & (int)CardType.Trap) > 0
                    && (card.IsShouldNotBeSpellTrapTarget() || !card.IsDisabled())) return false;
            }
            return true;
        }
       
        public int GetMaterialAttack(List<ClientCard> materials)
        {
            if (Util.IsTurn1OrMain2()) return 0;
            int result = 0;
            foreach (ClientCard material in materials)
            {
                if (material.IsAttack() || !summonThisTurn.Contains(material)) result += material.Attack;
            }
            return result;
        }
        public List<ClientCard> GetDangerousCardinEnemyGrave(bool onlyMonster = false)
        {
            List<ClientCard> result = Enemy.Graveyard.GetMatchingCards(card => 
                (!onlyMonster || card.IsMonster())).ToList(); //some specific matchup stuff removed
            List<int> dangerMonsterIdList = new List<int>{
                99937011, 63542003, 9411399, 28954097, 30680659
            };
            result.AddRange(Enemy.Graveyard.GetMatchingCards(card => dangerMonsterIdList.Contains(card.Id)));
            return result;
        }

        public ClientCard GetBestEnemyMonster(bool onlyFaceup = false, bool canBeTarget = false, bool ignoreCurrentDestroy = false, CardType selfType = 0)
        {
            ClientCard card = GetProblematicEnemyMonster(0, canBeTarget, ignoreCurrentDestroy, selfType);
            if (card != null)
                return card;

            card = Enemy.MonsterZone.Where(c => c?.Data != null && c.HasType(CardType.Monster) && c.IsFaceup()
                && CheckCanBeTargeted(c, canBeTarget, selfType) && (!ignoreCurrentDestroy || currentDestroyCardList.Contains(c)))
                .OrderByDescending(c => c.Attack).FirstOrDefault();
            if (card != null)
                return card;

            List<ClientCard> monsters = Enemy.GetMonsters().Where(c => !ignoreCurrentDestroy || currentDestroyCardList.Contains(c)).ToList();

            // after GetHighestAttackMonster, the left monsters must be face-down.
            if (monsters.Count() > 0 && !onlyFaceup)
                return ShuffleList(monsters)[0];

            return null;
        }
        
        public int CompareUsableAttack(ClientCard cardA, ClientCard cardB)
        {
            if (cardA == null && cardB == null)
                return 0;
            if (cardA == null)
                return -1;
            if (cardB == null)
                return 1;
            int powerA = (cardA.IsDefense() && summonThisTurn.Contains(cardA)) ? 0 : cardA.Attack;
            int powerB = (cardB.IsDefense() && summonThisTurn.Contains(cardB)) ? 0 : cardB.Attack;
            if (powerA < powerB)
                return -1;
            if (powerA == powerB)
                return CardContainer.CompareCardLevel(cardA, cardB);
            return 1;
        }
        private bool SkullCounter()
        {
            if (ActivateDescription == Util.GetStringId(CardId.VVSkullGuard, 1) && Duel.LastChainPlayer == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private bool HeraldSearch()
        {
            if (Bot.HasInHand(CardId.VVPrayer))
                AI.SelectCard(CardId.VVSkullGuard, CardId.Sauravis);
            else
                AI.SelectCard(CardId.VVPrayer);
            return true;
        }

        private bool TriasEffect()
        {
            if (Bot.HasInHand(CardId.Trias) && Util.ChainContainPlayer(0)) // Might need to change to when diviner is targetted by opponent; but don't know how to do that...
            {//maybe chaintargets?
                return false;
            }
            AI.SelectCard(CardId.Diviner);
            return true;
        }
        public void SelectSTPlace(ClientCard card = null, bool avoid_Impermanence = false, List<int> avoid_list = null)
        {
            List<int> list = new List<int> { 0, 1, 2, 3, 4 };
            int n = list.Count;
            while (n-- > 1)
            {
                int index = Program.Rand.Next(n + 1);
                int temp = list[index];
                list[index] = list[n];
                list[n] = temp;
            }
            foreach (int seq in list)
            {
                int zone = (int)System.Math.Pow(2, seq);
                if (Bot.SpellZone[seq] == null)
                {
                    if (card != null && card.Location == CardLocation.Hand && avoid_Impermanence && infiniteImpermanenceList.Contains(seq)) continue;
                    if (avoid_list != null && avoid_list.Contains(seq)) continue;
                    AI.SelectPlace(zone);
                    return;
                };
            }
            AI.SelectPlace(0);
        }

        // check whether negate maxxc and InfiniteImpermanence
        public void CheckDeactiveFlag()
        {
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().Id == _CardId.MaxxC && Duel.LastChainPlayer == 1)
            {
                enemyActivateMaxxC = false;
            }
            if (Util.GetLastChainCard() != null && Util.GetLastChainCard().Id == CardId.DimensionShifter && Duel.LastChainPlayer == 1)
            {
                enemy_activate_DimensionShifter = false;
            }
        }
        public bool NegatedCheck(bool disablecheck = true)
        {
            if (Card.IsSpell() || Card.IsTrap())
            {
                if (SpellNegatable()) return true;
            }
            if (CheckCalledbytheGrave(Card.Id) > 0 /*|| CrossoutDesignatorCheck(LastChainCard,Card.Id,3)*/)
            {
                return true;
            }
            if (Card.IsMonster() && Card.Location == CardLocation.MonsterZone && Card.IsDefense())
            {
                if (Enemy.MonsterZone.GetFirstMatchingFaceupCard(card => card.Id == CardId.Number41BagooskatheTerriblyTiredTapir && card.IsDefense() && !card.IsDisabled()) != null
                    || Bot.MonsterZone.GetFirstMatchingFaceupCard(card => card.Id == CardId.Number41BagooskatheTerriblyTiredTapir && card.IsDefense() && !card.IsDisabled()) != null)
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
        public int CheckCalledbytheGrave(int id)
        {
            if (currentNegatingIdList.Contains(id)) return 1;
            if (DefaultCheckWhetherCardIdIsNegated(id)) return 1;
            return 0;
        }
        // activate of CalledbytheGrave
        public bool CalledByEffect()
        {
            if (NegatedCheck(true)) return false;
            if (Duel.LastChainPlayer == 1)
            {
                // negate
                if (Util.GetLastChainCard().IsMonster())
                {
                    int code = Util.GetLastChainCard().Id;
                    if (code == 0) return false;
                    if (CheckCalledbytheGrave(code) > 0 /*|| CrossoutDesignatorTarget == code*/) return false;
                    if (Enemy.Graveyard.GetFirstMatchingCard(card => card.IsMonster() && card.IsOriginalCode(code)) != null)
                    {
                        if (!(Card.Location == CardLocation.SpellZone))
                        {
                            SelectSTPlace(null, true);
                        }
                        AI.SelectCard(code);
                        //CalledbytheGraveCount(code) = 2;
                        CheckDeactiveFlag();
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
                        //CalledbytheGraveCount[code] = 2;
                        return true;
                    }
                }

                // become targets
                if (Duel.ChainTargets.Contains(Card))
                {
                    List<ClientCard> enemy_monsters = Enemy.Graveyard.GetMatchingCards(card => card.IsMonster()).ToList();
                    if (enemy_monsters.Count > 0)
                    {
                        enemy_monsters.Sort(CardContainer.CompareCardAttack);
                        enemy_monsters.Reverse();
                        int code = enemy_monsters[0].Id;
                        AI.SelectCard(code);
                        //CalledbytheGraveCount[code] = 2;
                        return true;
                    }
                }
            }

            // avoid danger monster in grave
            if (Duel.LastChainPlayer == 1) return false;
            List<ClientCard> targets = CheckDangerousCardinEnemyGrave(true);
            if (targets.Count() > 0)
            {
                int code = targets[0].Id;
                if (!(Card.Location == CardLocation.SpellZone))
                {
                    SelectSTPlace(null, true);
                }
                AI.SelectCard(code);
                //CalledbytheGraveCount[code] = 2;
                return true;
            }

            return false;
        }
        public List<ClientCard> CheckDangerousCardinEnemyGrave(bool onlyMonster = false)
        {
            List<ClientCard> result = Enemy.Graveyard.GetMatchingCards(card =>
            (!onlyMonster || card.IsMonster()) && card.HasSetcode(SetcodeOrcust)).ToList();
            return result;
        }

        public override bool OnSelectHand()
        {
            //go first
            return true;
        }

        private bool BarrierFirst()
        {
            if (ActivateDescription == Util.GetStringId(CardId.VVBarrier, 0))
            {
                int target = GetCardToSearch();
                if (target != 0)
                { 
                    AI.SelectCard(target);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (Bot.HasInSpellZone(CardId.VVBarrier, true))
            { return false; }
            else { return true; }
        }

        private bool DivinerEffect()
        {
            if (!Bot.HasInHand(CardId.Trias) && !Bot.HasInGraveyard(CardId.Trias))
                AI.SelectCard(CardId.Trias);
            else if (Util.GetProblematicEnemyMonster() != null && Bot.GetRemainingCount(CardId.Trias,1) == 0)
                AI.SelectCard(CardId.Herald);
            else
                AI.SelectCard(CardId.Ntss);
            return true;
        }

        private bool SkullSearch()
        {
            if (ActivateDescription == Util.GetStringId(CardId.VVSkullGuard, 0))
            {
                if (Bot.HasInHand(CardId.Sauravis))
                {    
                    AI.SelectCard(CardId.VVSauravis);
                    return true; 
                }
                else if (!Bot.HasInHand(CardId.Sauravis))
                {
                    AI.SelectCard(CardId.Sauravis);
                    return true;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        private bool SaffEffect()
        {
            AI.SelectCard(CardId.VVPrayer);
            if (GetCardToSearch() > 0)
            {
                AI.SelectCard(GetCardToSearch());
                return true; 
            }
            else
            {    //AI.SelectCard(CardId.VVSkullGuard, CardId.Sauravis);
                return false;
            }
        }

        private bool SaffiraRitual()
        {
            if (ActivateDescription != Util.GetStringId(CardId.Saffira, 1))
                return false;
            if (Bot.HasInHand(CardId.VVSkullGuard) && !Bot.HasInMonstersZone(CardId.VVSkullGuard))
                AI.SelectCard(CardId.VVSkullGuard);
            else if (Bot.HasInHand(CardId.Sauravis))
                AI.SelectCard(CardId.Sauravis);

            // select sacrifice
            if (Bot.HasInHand(CardId.Trias) || Bot.HasInMonstersZone(CardId.Trias))
                AI.SelectCard(CardId.Trias);
            //else
                return true;
        }

        public override bool OnSelectYesNo(int desc)
        {
            if (desc == Util.GetStringId(CardId.Saffira, 2)) // search ritual monster?
                return OnSelectYesNo(2);
            if (desc == Util.GetStringId(CardId.VVPrayer, 0)) // reinforce ritual monsters?
                return OnSelectYesNo(0);
            return base.OnSelectYesNo(desc);
        }

        private bool NoLo()
        {
            return !Bot.HasInMonstersZoneOrInGraveyard(CardId.Lo);
        }

        private bool LoSummon()
        {
            if (!Bot.HasInHand(CardId.Diviner) && !Bot.HasInHand(CardId.PrePrep) && !Bot.HasInMonstersZone(CardId.Lo))
            { return true; }
            else
            { return false; }
        }

        private bool LoPlace()
        {
            if (Bot.GetRemainingCount(CardId.VVBarrier, 1) > 0)
            {   
                AI.SelectCard(CardId.VVBarrier);
                return true;
            }
            else if (Bot.GetRemainingCount(CardId.VVRadiance, 1) > 0)
            {   
                AI.SelectCard(CardId.VVRadiance);
                return true; 
            }
            else if (Bot.GetRemainingCount(CardId.VVBlessing,1) > 0)
            {
                AI.SelectCard(CardId.VVBlessing);
                return true;
            }
            else { return false; }
        }

        private bool PrePrepSearch()
        {
            if (GetCardToSearch() != 0)
            {
                AI.SelectCard(GetCardToSearch());
                return true;
            }
            else
            //    AI.SelectCard(CardId.VVSkullGuard, CardId.Sauravis, CardId.VVPrayer);
            { return false; }
        }

        private int GetCardToSearch()
        {
            if (NoLo() && !Bot.HasInHand(CardId.Lo) && !Bot.HasInHand(CardId.Diviner) && Bot.GetRemainingCount(CardId.Lo, 3) > 0)
            {
                return CardId.Lo;
            }
            else if (!Bot.HasInHand(CardId.Saffira) && Bot.GetRemainingCount(CardId.Saffira, 3) > 0 && Bot.GetRemainingCount(CardId.VVPrayer, 2) > 0)
            {
                return CardId.Saffira;
            }
            else if (Bot.GetMonsterCount() == 0 && !Bot.HasInHand(CardId.VVSkullGuard) && Bot.GetRemainingCount(CardId.VVSkullGuard, 2) > 0)
            {
                return CardId.VVSkullGuard;
            }
            else if (Bot.GetRemainingCount(CardId.VVSauravis, 1) > 0 && Bot.GetGraveyardSpells().Count > 1) //if there's >= 2 spells in grave
            {
                return CardId.VVSauravis;
            }
            else if (Bot.GetRemainingCount(CardId.VVRadiance, 1) > 0)
            {
                return CardId.VVRadiance;
            }
            else if (Bot.GetRemainingCount(CardId.VVBlessing, 1) > 0)
            {
                return CardId.VVRadiance;
            }
            return 0;
        }

        private bool LoEffect()
        {
            AI.SelectPosition(CardPosition.FaceUpDefence);
            return true;
        }

        public override IList<ClientCard> OnSelectCard(IList<ClientCard> cards, int min, int max, int hint, bool cancelable)
        {
            ClientCard currentSolvingChain = Duel.GetCurrentSolvingChainCard();
            if (currentSolvingChain != null)
            {
                if (hint == HintMsg.Release && currentSolvingChain.Id != CardId.Trias)
                {
                    List<ClientCard> result = cards.Where(card => card.IsCode(CardId.Lo) && card.Location == CardLocation.MonsterZone && card.Controller == 0).ToList();
                    List<ClientCard> result2 = cards.Where(card => card.IsCode(CardId.Lo) && card.Location == CardLocation.Hand && card.Controller == 0).ToList();
                    List<ClientCard> Trias = cards.Where(card => card.IsCode(CardId.Trias) && card.Location == CardLocation.MonsterZone && card.Controller == 0).ToList();
                    List<ClientCard> Trias2 = cards.Where(card => card.IsCode(CardId.Trias) && card.Location == CardLocation.Hand && card.Controller == 0).ToList();
                    if (result.Count > 0)
                    {
                        return Util.CheckSelectCount(result, cards, min, max);
                    }
                    else if (result2.Count > 0)
                    {
                        return Util.CheckSelectCount(result2, cards, min, max);
                    }
                    else if (Trias.Count > 0)
                    {
                        return Util.CheckSelectCount(Trias, cards, min, max);
                    }
                    else if (Trias2.Count > 0)
                    {
                        return Util.CheckSelectCount(Trias2, cards, min, max);
                    }
                }
                if (currentSolvingChain.Id == CardId.VVBlessing && hint == HintMsg.ReturnToHand && (cards[0].Location == CardLocation.Grave || cards[0].Location == CardLocation.Removed))
                {
                    List<ClientCard> result = new List<ClientCard>();
                    foreach (ClientCard listCard in cards)
                    {
                        if (listCard.Controller == 0)
                        {
                            if (listCard.IsCode(CardId.VVRadiance))
                            {
                                result.Add(listCard);
                            }
                            else if (listCard.IsCode(CardId.Saffira) && (Bot.HasInHand(CardId.VVSkullGuard) || Bot.HasInHand(CardId.VVSaffira)))
                            {
                                result.Add(listCard);
                            }
                            else if (listCard.IsCode(CardId.VVSauravis))
                            {
                                result.Add(listCard);
                            }
                            else if (listCard.IsCode(CardId.VVSkullGuard))
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
                if (currentSolvingChain.Id == CardId.VVSauravis && hint == HintMsg.ToDeck && cards[0].HasType(CardType.Spell) && (cards[0].Location == CardLocation.Grave || cards[0].Location == CardLocation.Hand))
                {
                    List<ClientCard> result = new List<ClientCard>();
                    foreach (ClientCard listCard in cards)
                    {
                        if (listCard.Controller == 0)
                        {
                            if (listCard.Id == CardId.VVPrayer && listCard.Location == CardLocation.Grave)
                            {
                                result.Add(listCard);
                            }
                            else if (listCard.Id == CardId.VVBarrier && listCard.Location == CardLocation.Grave)
                            {
                                result.Add(listCard);
                            }
                            else if (listCard.Id == CardId.VVBarrier && listCard.Location == CardLocation.Hand)
                            {
                                result.Add(listCard);
                            }
                            else if (listCard.HasSetcode(SetcodeVV) && listCard.Location == CardLocation.Grave)
                            {
                                result.Add(listCard);
                            }
                            else if (listCard.Location == CardLocation.Grave)
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
            }
            return base.OnSelectCard(cards, min, max, hint, cancelable);
        }

        public override CardPosition OnSelectPosition(int cardId, IList<CardPosition> positions)
        {
            if (cardId == CardId.Lo)
            {
                return CardPosition.FaceUpDefence;
            }
            if (cardId == CardId.Trias)
            {
                return CardPosition.FaceUpDefence;
            }
            return base.OnSelectPosition(cardId, positions);
        }
        public bool PotofExtravaganceActivate()
        {
            if (CheckWhetherNegated())
            {
                return false;
            }
            if (SpellNegatable()) return false;
            potActivate = true;
            SelectSTPlace(Card, true);
            AI.SelectOption(1);
            return true;
        }
        public override void OnNewTurn()
        {
            enemyActivateMaxxC = false;
            enemyActivateLockBird = false;
            enemy_activate_DimensionShifter = false;
            enemyMoveGrave = false;
            paxCallToField = false;
            potActivate = false;

            base.OnNewTurn();
        }
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
        public bool VVSauravisSS()
        {
            if (Bot.GetGraveyardSpells().Count > 1)
            {
                return true;
            }
            return false;
        }
    }
}