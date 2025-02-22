/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;

public class CoreStory
{
    // [Can Change]
    // True = Bot only does its smart checks on quests with Once: True 
    // False = Bot does it's smart checks on all quest
    // Recommended: false
    // Used for testing bots, dont toggle this as a user
    public bool TestBot { get; set; } = false;

    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.RunCore();
    }

    /// <summary>
    /// Kills a monster for a Quest, and turns in the quest if possible. Automatically checks if the next quest is unlocked. If it is, it will skip this one.
    /// </summary>
    /// <param name="QuestID">ID of the quest</param>
    /// <param name="MapName">Map where the <paramref name="MonsterName"/> are</param>
    /// <param name="MonsterName">Monster to kill</param>
    /// <param name="GetReward">Whether or not the <paramref name="Reward"/> should be added with AddDrop</param>
    /// <param name="Reward">What item should be added with AddDrop</param>
    /// <param name="AutoCompleteQuest">If the method should turn in the quest for you when the quest can be completed</param>
    public void KillQuest(int QuestID, string MapName, string MonsterName, bool GetReward = true, string Reward = "All", bool AutoCompleteQuest = true)
    {
        Quest QuestData = Core.EnsureLoad(QuestID);
        if (QuestProgression(QuestID, GetReward, Reward))
            return;

        SmartKillMonster(QuestID, MapName, MonsterName);
        if (AutoCompleteQuest)
            foreach (ItemBase item in QuestData.Requirements)
                Bot.Wait.ForPickup(item.ID);
        TryComplete(QuestData, AutoCompleteQuest);

        void SmartKillMonster(int questID, string map, string monster)
        {
            Core.EnsureAccept(questID);
            _AddRequirement(questID);
            Core.Join(map);
            _SmartKill(monster, 20);
            CurrentRequirements.Clear();
        }
    }

    /// <summary>
    /// Kills an array of monsters for a Quest, and turns in the quest if possible. Automatically checks if the next quest is unlocked. If it is, it will skip this one.
    /// </summary>
    /// <param name="QuestID">ID of the quest</param>
    /// <param name="MapName">Map where the <paramref name="MonsterName"/> are</param>
    /// <param name="MonsterName">Monster to kill</param>
    /// <param name="GetReward">Whether or not the <paramref name="Reward"/> should be added with AddDrop</param>
    /// <param name="Reward">What item should be added with AddDrop</param>
    /// <param name="AutoCompleteQuest">If the method should turn in the quest for you when the quest can be completed</param>
    public void KillQuest(int QuestID, string MapName, string[] MonsterNames, bool GetReward = true, string Reward = "All", bool AutoCompleteQuest = true)
    {
        Quest QuestData = Core.EnsureLoad(QuestID);
        if (QuestProgression(QuestID, GetReward, Reward))
            return;

        SmartKillMonster(QuestID, MapName, MonsterNames);
        if (AutoCompleteQuest)
            foreach (ItemBase item in QuestData.Requirements)
                Bot.Wait.ForPickup(item.ID);
        TryComplete(QuestData, AutoCompleteQuest);

        void SmartKillMonster(int questID, string map, string[] monsters)
        {
            Core.EnsureAccept(questID);
            _AddRequirement(questID);
            Core.Join(map);
            foreach (string monster in monsters)
                _SmartKill(monster, 20);
            CurrentRequirements.Clear();
        }
    }

    /// <summary>
    /// Gets a MapItem X times for a Quest, and turns in the quest if possible. Automatically checks if the next quest is unlocked. If it is, it will skip this one.
    /// </summary>
    /// <param name="QuestID">ID of the quest</param>
    /// <param name="MapName">Map where the <paramref name="MonsterName"/> are</param>
    /// <param name="MapItemID">ID of the item</param>
    /// <param name="Amount">The amount of <paramref name="MapItemID"/> it grabs</param>
    /// <param name="GetReward">Whether or not the <paramref name="Reward"/> should be added with AddDrop</param>
    /// <param name="Reward">What item should be added with AddDrop</param>
    /// <param name="AutoCompleteQuest">If the method should turn in the quest for you when the quest can be completed</param>
    public void MapItemQuest(int QuestID, string MapName, int MapItemID, int Amount = 1, bool GetReward = true, string Reward = "All", bool AutoCompleteQuest = true)
    {
        Quest QuestData = Core.EnsureLoad(QuestID);

        if (QuestProgression(QuestID, GetReward, Reward))
            return;

        Core.EnsureAccept(QuestID);
        Core.GetMapItem(MapItemID, Amount, MapName);
        TryComplete(QuestData, AutoCompleteQuest);
    }

    /// <summary>
    /// Gets a MapItem X times for a Quest, and turns in the quest if possible. Automatically checks if the next quest is unlocked. If it is, it will skip this one.
    /// </summary>
    /// <param name="QuestID">ID of the quest</param>
    /// <param name="MapName">Map where the <paramref name="MonsterName"/> are</param>
    /// <param name="MapItemIDs">ID of the item</param>
    /// <param name="Amount">The amount of <paramref name="MapItemID"/> it grabs</param>
    /// <param name="GetReward">Whether or not the <paramref name="Reward"/> should be added with AddDrop</param>
    /// <param name="Reward">What item should be added with AddDrop</param>
    /// <param name="AutoCompleteQuest">If the method should turn in the quest for you when the quest can be completed</param>
    public void MapItemQuest(int QuestID, string MapName, int[] MapItemIDs, int Amount = 1, bool GetReward = true, string Reward = "All", bool AutoCompleteQuest = true)
    {
        Quest QuestData = Core.EnsureLoad(QuestID);

        if (QuestProgression(QuestID, GetReward, Reward))
            return;

        Core.EnsureAccept(QuestID);
        foreach (int MapItemID in MapItemIDs)
            Core.GetMapItem(MapItemID, Amount, MapName);
        TryComplete(QuestData, AutoCompleteQuest);
    }

    /// <summary>
    /// Gets a MapItem X times for a Quest, and turns in the quest if possible. Automatically checks if the next quest is unlocked. If it is, it will skip this one.
    /// </summary>
    /// <param name="QuestID">ID of the quest</param>
    /// <param name="MapName">Map where the <paramref name="MonsterName"/> are</param>
    /// <param name="ItemName">Name of the item</param>
    /// <param name="Amount">The amount of <paramref name="ItemName"/> to buy</param>
    /// <param name="GetReward">Whether or not the <paramref name="Reward"/> should be added with AddDrop</param>
    /// <param name="Reward">What item should be added with AddDrop</param>
    /// <param name="AutoCompleteQuest">If the method should turn in the quest for you when the quest can be completed</param>
    public void BuyQuest(int QuestID, string MapName, int ShopID, string ItemName, int Amount = 1, bool GetReward = true, string Reward = "All", bool AutoCompleteQuest = true)
    {
        Quest QuestData = Core.EnsureLoad(QuestID);

        if (QuestProgression(QuestID, GetReward, Reward))
            return;

        Core.EnsureAccept(QuestID);
        Core.BuyItem(MapName, ShopID, ItemName, Amount);
        TryComplete(QuestData, AutoCompleteQuest);
    }

    /// <summary>
    /// Accepts a quest and then turns it in again
    /// </summary>
    /// <param name="QuestID">ID of the quest</param>
    /// <param name="GetReward">Whether or not the <paramref name="Reward"/> should be added with AddDrop</param>
    /// <param name="Reward">What item should be added with AddDrop</param>
    /// <param name="AutoCompleteQuest">If the method should turn in the quest for you when the quest can be completed</param>
    public void ChainQuest(int QuestID, bool GetReward = true, string Reward = "All", bool AutoCompleteQuest = true)
    {
        Quest QuestData = Core.EnsureLoad(QuestID);

        if (QuestProgression(QuestID, GetReward, Reward))
            return;

        Core.Sleep();
        if (AutoCompleteQuest)
            Core.ChainComplete(QuestID);
        else
        {
            Core.EnsureAccept(QuestID);
        }
        Bot.Wait.ForQuestComplete(QuestID);
        Core.Logger($"Completed \"{QuestData.Name}\" [{QuestID}]");
        Core.Sleep();
    }

    public void QuestComplete(int questID) => TryComplete(Core.EnsureLoad(questID), true);

    private void TryComplete(Quest QuestData, bool AutoCompleteQuest)
    {
        if (!Bot.Quests.CanComplete(QuestData.ID))
            return;

        Core.Sleep();
        if (AutoCompleteQuest)
            Core.EnsureComplete(QuestData.ID);
        Bot.Wait.ForQuestComplete(QuestData.ID);
        Core.Logger($"Completed Quest: [{QuestData.ID}] - \"{QuestData.Name}\"", "QuestProgression");
        Core.Sleep(1500);
    }

    /// <summary>
    /// Skeleton of KillQuest, MapItemQuest, BuyQuest and ChainQuest. Only needs to be used inside a script if the quest spans across multiple maps
    /// </summary>
    /// <param name="QuestID">ID of the quest</param>
    /// <param name="GetReward">Whether or not the <paramref name="Reward"/> should be added with AddDrop</param>
    /// <param name="Reward">What item should be added with AddDrop</param>
    public bool QuestProgression(int QuestID, bool GetReward = true, string Reward = "All")
    {
        if (QuestID != 0 && PreviousQuestID == QuestID)
            return PreviousQuestState;
        PreviousQuestID = QuestID;

        if (!CBO_Checked)
        {
            if (Core.CBOBool("BCO_Story_TestBot", out bool _TestBot))
                TestBot = _TestBot;
            CBO_Checked = true;
        }

        Quest QuestData = Core.EnsureLoad(QuestID);

        int timeout = 0;
        while (!Bot.Quests.IsUnlocked(QuestID))
        {
            Core.Sleep(1000);
            timeout++;

            if (timeout > 15)
            {
                int currentValue = Bot.Flash.CallGameFunction<int>("world.getQuestValue", QuestData.Slot);
                Quest? prevQuest = Bot.Quests.Tree.Find(q => q.Slot == QuestData.Slot && q.Value == (currentValue + 1));

                prevQuestReq ??=
                    prevQuest == null || prevQuest.Requirements.All(r => Core.CheckInventory(r.ID, r.Quantity)) ?
                        null :
                        String.Join(',', prevQuest.Requirements.Where(r => !Core.CheckInventory(r.ID, r.Quantity)).Select(i => i.Name));
                prevQuestAReq ??=
                    prevQuest == null || prevQuest.AcceptRequirements.All(r => Core.CheckInventory(r.ID, r.Quantity)) ?
                        null :
                        String.Join(',', prevQuest.Requirements.Where(r => !Core.CheckInventory(r.ID, r.Quantity)).Select(i => i.Name));
                prevQuestExplain ??=
                    prevQuest == null ?
                        String.Empty :
                        $"Quest \"{prevQuest.Name}\" [{prevQuest.ID}] appears to have failed to turn in somehow.|" +
                        (prevQuestReq == null ?
                            String.Empty :
                            $"Missing QuestItems: {prevQuestReq}|") +
                        (prevQuestAReq == null ?
                            String.Empty :
                            $"Missing AcceptRequirements: {prevQuestAReq}|");

                if (lastFailedQuestID != QuestData.ID)
                {
                    if (prevQuest != null && prevQuest.Status == "c")
                    {
                        TryComplete(prevQuest, true);
                        timeout = 0;
                    }
                    else if (QuestData.Value - currentValue <= 2)
                    {
                        Core.Logger("A server/client desync happened (common) for your quest progress, the bot will now restart");
                        lastFailedQuestID = QuestData.ID;
                        timeout = 0;
                        Core.Relogin();
                    }
                }
                else
                {
                    string message2 = $"Quest \"{QuestData.Name}\" [{QuestID}] is not unlocked.|" +
                                     $"Expected value = [{QuestData.Value - 1}/{QuestData.Slot}], recieved = [{currentValue}/{QuestData.Slot}]|" +
                                      prevQuestExplain +
                                      "Please fill in the Skua Scripts Form to report this.|" +
                                      "Do you wish to be brought to the form?";
                    Core.Logger(message2.Replace("|", " "));
                    if (Bot.ShowMessageBox(message2.Replace("|", "\n"), "Quest not unlocked", true) == true)
                    {
                        string url =
                            $"\"https://docs.google.com/forms/d/e/1FAIpQLSeI_S99Q7BSKoUCY2O6o04KXF1Yh2uZtLp0ykVKsFD1bwAXUg/viewform?usp=pp_url&" +

                            "entry.209396189=Skua&" +
                            "entry.2118425091=Bug+Report&" +
                            $"entry.290078150={Core.loadedBot}&" +
                            "entry.1803231651=I+got+a+popup+saying+a+quest+was+not+unlocked&" +

                            $"entry.1918245848={QuestData.ID}&" +
                            $"entry.1809007115={QuestData.Value - 1}/{QuestData.Slot}&" +
                            $"entry.493943632={currentValue}/{QuestData.Slot}&" +
                            $"entry.148016785={QuestData.Name}";

                        if (prevQuest != null)
                            url +=
                                $"&entry.77289389={prevQuest.ID}&" +
                                $"entry.2130921787={prevQuest.Name}&" +
                                $"entry.1966808403={prevQuestReq ?? String.Empty}&" +
                                $"entry.914792808={prevQuestAReq ?? String.Empty}";
                        url += "\"";

                        Process p = new();
                        p.StartInfo.FileName = "rundll32";
                        p.StartInfo.Arguments = "url,OpenURL " + url;
                        p.StartInfo.WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System).Split('\\').First() + "\\";
                        p.Start();
                    }
                    Bot.Stop(true);
                }
            }
        }

        if (Core.isCompletedBefore(QuestID) && (TestBot ? QuestData.Once : true))
        {
            if (TestBot)
                Core.Logger($"Skipped (Once = true): [{QuestID}] - \"{QuestData.Name}\"");
            else Core.Logger($"Already Completed: [{QuestID}] - \"{QuestData.Name}\"");
            PreviousQuestState = true;
            return true;
        }

        if (GetReward)
        {
            if (Reward != "All")
            {
                if (Core.CheckInventory(Reward))
                {
                    Core.Logger($"You already have {Reward}, skipping quest");
                    PreviousQuestState = true;
                    return true;
                }
                Core.AddDrop(Reward);
            }
            else Core.AddDrop(Core.QuestRewards(QuestID));
        }

        Core.Logger($"Doing Quest: [{QuestID}] - \"{QuestData.Name}\"");
        Core.EquipClass(ClassType.Solo);
        PreviousQuestState = false;
        return false;
    }
    private bool CBO_Checked = false;
    private int lastFailedQuestID = 0;
    private string? prevQuestExplain;
    private string? prevQuestReq;
    private string? prevQuestAReq;

    public void LegacyQuestManager(Action questLogic, params int[] questIDs)
    {
        List<Quest> questData = Core.EnsureLoad(questIDs);
        List<LegacyQuestObject> whereToGet = new List<LegacyQuestObject>();

        //Core.DL_Enable();
        Core.DebugLogger(this, "-------------\t");
        foreach (Quest quest in questData)
        {
            List<ItemBase> desiredQuestReward = quest.Rewards.Where(r => questData.Any(q => q.AcceptRequirements.Any(a => a.ID == r.ID || a.Name == r.Name))).ToList();
            int requiredQuestID = questData.Find((q => q.Rewards.Any(r => quest.AcceptRequirements != null && quest.AcceptRequirements.Any(a => a.ID == r.ID || a.Name == r.Name))))?.ID ?? 0;
            List<ItemBase>? requiredQuestReward = quest.AcceptRequirements?.Where(r => questData.Any(q => q.Rewards.Any(a => a.ID == r.ID || a.Name == r.Name)))?.ToList();

            Core.DebugLogger(this, $"{quest.ID}\t\t");
            Core.DebugLogger(this, $"{desiredQuestReward.FirstOrDefault()?.Name}\t");
            Core.DebugLogger(this, $"{requiredQuestID}\t\t");
            Core.DebugLogger(this, $"{requiredQuestReward?.FirstOrDefault()?.Name}\t");
            Core.DebugLogger(this, "-------------\t");

            if (requiredQuestReward?.Count() == 0 && quest.AcceptRequirements?.Count() > 0)
            {
                Core.Logger("The managed failed to find the location of \"" +
                String.Join("\" + \"", quest.AcceptRequirements.Select(a => a.Name)) +
                $"\" for Quest ID {quest.ID}, is the function missing a Quest ID?",
                messageBox: true);
                return;
            }

            whereToGet.Add(new(quest.ID, desiredQuestReward, requiredQuestID, requiredQuestReward));
        }

        if (whereToGet.All(x => x.desiredQuestReward.Count == 0) || whereToGet.All(x => x.requiredQuestReward?.Count == 0))
        {
            Core.Logger("None of Quest IDs filled in are supposed to be used in the LegacyQuestManager, " +
                        "please report to the bot makers that they must make this story line in the normal way.",
                        messageBox: true);
            return;
        }

        var finalItemQuest = whereToGet.Find(x => x.desiredQuestReward.Count == 0);
        if (finalItemQuest == null || finalItemQuest.desiredQuestID <= 0)
        {
            Core.Logger("Could not find the Quest ID of the last quest in the item chain");
            return;
        }

        Core.Logger($"Final quest in Legacy Quest Chain: [{finalItemQuest.desiredQuestID}] \"{Core.EnsureLoad(finalItemQuest.desiredQuestID).Name}\"");

        runQuest(finalItemQuest.desiredQuestID);

        foreach (LegacyQuestObject l in whereToGet)
            if (l.requiredQuestReward != null)
                Core.ToBank(l.requiredQuestReward.Select(i => i.ID).ToArray());

        void runQuest(int questID)
        {
            LegacyQuestObject? runQuestData = whereToGet.Find(d => d.desiredQuestID == questID);

            Core.DebugLogger(this);
            if (runQuestData == null)
            {
                Core.Logger("runQuestData is NULL");
                return;
            }
            Quest questData = Core.EnsureLoad(questID);
            Core.DebugLogger(this);

            int[] requiredReward = runQuestData.requiredQuestReward!.Select(i => i.ID).ToArray();
            Core.DebugLogger(this);
            if (runQuestData.desiredQuestReward.Count == 0 && questID != finalItemQuest.desiredQuestID)
            {
                if (!Core.CheckInventory(requiredReward))
                    runQuest(runQuestData.requiredQuestID);
                return;
            }

            Core.DebugLogger(this);
            int[] desiredReward = runQuestData.desiredQuestReward.Select(i => i.ID).ToArray();
            if (questID != finalItemQuest.desiredQuestID ? Core.CheckInventory(desiredReward) : Core.CheckInventory(Core.EnsureLoad(finalItemQuest.desiredQuestID).Rewards.Select(x => x.ID).ToArray()))
            {
                Core.Logger($"Already Completed: [{questID}] - \"{questData.Name}\"", "QuestProgression");
                return;
            }
            Core.DebugLogger(this);

            if (!Core.CheckInventory(requiredReward))
                runQuest(runQuestData.requiredQuestID);

            if (_LegacyQuestStop)
                return;
            Core.DebugLogger(this);

            Core.Logger($"Doing Quest: [{questID}] - \"{questData.Name}\"", "QuestProgression");
            Core.EnsureAccept(questID);
            Core.AddDrop(desiredReward);

            Core.DebugLogger(this);
            LegacyQuestID = questID;
            questLogic();
            Core.DebugLogger(this);

            TryComplete(questData, LegacyQuestAutoComplete);
            Core.DebugLogger(this);
            foreach (int i in desiredReward)
                Bot.Wait.ForPickup(i);
            Core.DebugLogger(this);
            if (questID == finalItemQuest.desiredQuestID)
                Bot.Drops.Pickup(Core.EnsureLoad(finalItemQuest.desiredQuestID).Rewards.Select(x => x.ID).ToArray());
            Core.DebugLogger(this);

            LegacyQuestAutoComplete = true;
        }
    }
    private class LegacyQuestObject
    {
        public int desiredQuestID { get; set; } // In order to do ....
        public List<ItemBase> desiredQuestReward { get; set; } // And obtain ...
        public int requiredQuestID { get; set; } // You must do ...
        public List<ItemBase>? requiredQuestReward { get; set; } // And obtain ...

        public LegacyQuestObject(int desiredQuestID, List<ItemBase> desiredQuestReward, int requiredQuestID, List<ItemBase>? requiredQuestReward)
        {
            this.desiredQuestID = desiredQuestID;
            this.desiredQuestReward = desiredQuestReward;
            this.requiredQuestID = requiredQuestID;
            this.requiredQuestReward = requiredQuestReward;
        }
    }
    public int LegacyQuestID = -1;
    public bool LegacyQuestAutoComplete = true;
    private bool _LegacyQuestStop = false;
    public void LegacyQuestStop() => _LegacyQuestStop = true;

    /// <summary>
    /// Put this at the start of your story script so that the bot will load all quests that are used in the bot. This will speed up any progression checks tremendiously.
    /// </summary>
    public void PreLoad(Object _this, [CallerMemberName] string caller = "")
    {
        List<int> QuestIDs = new();
        string[] ScriptSlice = Core.CompiledScript();
        if (ScriptSlice.Count() == 0)
        {
            Core.Logger("PreLoad failed, cannot read Compiled Script. You might not be on the latest version of Skua");
            return;
        }

        int classStartIndex = Array.IndexOf(ScriptSlice, $"public class {_this}");
        int classEndIndex = Array.IndexOf(ScriptSlice[(classStartIndex)..], "}") + classStartIndex + 1;
        ScriptSlice = ScriptSlice[(classStartIndex)..classEndIndex];

        int methodStartIndex = -1;
        foreach (string p in new string[] { "public", "private" })
        {
            foreach (string s in new string[] { "void", "bool", "string", "int" })
            {
                methodStartIndex = Array.FindIndex(ScriptSlice, l => l.Contains($"{p} {s} {caller}"));
                if (methodStartIndex > -1)
                    break;
            }
            if (methodStartIndex > -1)
                break;
        }
        if (methodStartIndex == -1)
        {
            Core.Logger("Failed to parse methodStartIndex, no quests will be pre-loaded");
            return;
        }

        int methodIndentCount = ScriptSlice[methodStartIndex + 1].IndexOf('{');
        string indent = "";
        for (int i = 0; i < methodIndentCount; i++)
            indent += " ";
        int methodEndIndex = Array.FindIndex(ScriptSlice, methodStartIndex, l => l == indent + "}") + 1;

        ScriptSlice = ScriptSlice[methodStartIndex..methodEndIndex];

        string[] SearchParam = {
            "Story.KillQuest",
            "Story.MapItemQuest",
            "Story.BuyQuest",
            "Story.ChainQuest",
            "Story.QuestProgression",
            "Core.EnsureAccept",
            "Core.EnsureComplete",
            "Core.EnsureCompleteChoose",
            "Core.ChainComplete"
        };

        foreach (string Line in ScriptSlice)
        {
            if (!Line.Any(char.IsDigit))
                continue;

            string EdittedLine = Line
                                    .Replace(" ", "")
                                    .Replace("!", "")
                                    .Replace("(", "")
                                    .Replace("if", "")
                                    .Replace("else", "");

            if (!SearchParam.Any(x => EdittedLine.StartsWith(x)))
                continue;

            char[] digits = Line.SkipWhile(c => !Char.IsDigit(c)).TakeWhile(Char.IsDigit).ToArray();
            string sQuestID = new string(digits);
            int QuestID = int.Parse(sQuestID);

            if (!QuestIDs.Contains(QuestID) && !Bot.Quests.Tree.Exists(x => x.ID == QuestID))
                QuestIDs.Add(QuestID);
        }

        if (QuestIDs.Count() + Bot.Quests.Tree.Count() > Core.LoadedQuestLimit
            && QuestIDs.Count < Core.LoadedQuestLimit)
        {
            Bot.Flash.SetGameObject("world.questTree", new ExpandoObject());
        }
        else if (QuestIDs.Count > (Core.LoadedQuestLimit - Bot.Quests.Tree.Count()))
        {
            Core.Logger($"Found {QuestIDs.Count} Quests, this exceeds the max amount of loaded quests ({Core.LoadedQuestLimit}). No quests will be loaded.");
            return;
        }

        Core.Logger($"Loading {QuestIDs.Count} Quests.");
        if (QuestIDs.Count > 30)
            Core.Logger($"Estimated Loading Time: {Convert.ToInt32(QuestIDs.Count / 30 * 1.6)}s");

        for (int i = 0; i < QuestIDs.Count; i = i + 30)
        {
            Bot.Quests.Load(QuestIDs.ToArray()[i..(QuestIDs.Count > i ? QuestIDs.Count : i + 30)]);
            Core.Sleep(1500);
        }
    }
    private int PreviousQuestID = 0;
    private bool PreviousQuestState = false;

    private void _SmartKill(string monster, int iterations = 20)
    {
        if (monster == null)
        {
            Core.Logger("ERROR: monster is null, please report", stopBot: true);
            return;
        }

        bool repeat = true;
        for (int j = 0; j < iterations; j++)
        {
            if (CurrentRequirements.Count == 0)
                break;
            if (CurrentRequirements.Count == 1)
            {
                if (_RepeatCheck(ref repeat, 0))
                    break;
                _MonsterHunt(ref repeat, monster, CurrentRequirements[0].Name, CurrentRequirements[0].Quantity, CurrentRequirements[0].Temp, 0);
                break;
            }
            else
            {
                for (int i = CurrentRequirements.Count - 1; i >= 0; i--)
                {
                    if (j == 0 && (Core.CheckInventory(CurrentRequirements[i].Name, CurrentRequirements[i].Quantity)))
                    {
                        CurrentRequirements.RemoveAt(i);
                        continue;
                    }
                    if (j != 0 && Core.CheckInventory(CurrentRequirements[i].Name))
                    {
                        if (_RepeatCheck(ref repeat, i))
                            break;
                        _MonsterHunt(ref repeat, monster, CurrentRequirements[i].Name, CurrentRequirements[i].Quantity, CurrentRequirements[i].Temp, i);
                        break;
                    }
                }
            }
            if (!repeat)
                break;

            Bot.Hunt.Monster(monster);
            Bot.Drops.Pickup(CurrentRequirements.Where(item => !item.Temp).Select(item => item.Name).ToArray());
            Core.Sleep();
        }
    }
    private List<ItemBase> CurrentRequirements = new();
    private void _MonsterHunt(ref bool shouldRepeat, string monster, string itemName, int quantity, bool isTemp, int index)
    {
        Bot.Hunt.ForItem(monster, itemName, quantity, isTemp);
        CurrentRequirements.RemoveAt(index);
        shouldRepeat = false;
    }
    private bool _RepeatCheck(ref bool shouldRepeat, int index)
    {
        if (Core.CheckInventory(CurrentRequirements[index].Name, CurrentRequirements[index].Quantity))
        {
            CurrentRequirements.RemoveAt(index);
            shouldRepeat = false;
            return true;
        }
        return false;
    }
    private int lastQuestID;
    private void _AddRequirement(int questID)
    {
        if (questID > 0 && questID != lastQuestID)
        {
            lastQuestID = questID;
            Quest quest = Core.EnsureLoad(questID);

            List<string> reqItems = new();
            quest.AcceptRequirements.ForEach(item => reqItems.Add(item.Name));
            quest.Requirements.ForEach(item =>
            {
                if (!CurrentRequirements.Where(i => i.Name == item.Name).Any())
                {
                    if (!item.Temp)
                        reqItems.Add(item.Name);
                    CurrentRequirements.Add(item);
                }
            });
            Core.AddDrop(reqItems.ToArray());
        }
    }
}
