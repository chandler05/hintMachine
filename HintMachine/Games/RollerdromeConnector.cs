namespace HintMachine.Games
{
    public class RollerdromeConnector : IGameConnector
    {
        private readonly HintQuestCumulative _scoreQuest = new HintQuestCumulative
        {
            Name = "Score",
            GoalValue = 750000,
        };

        private readonly HintQuestCumulative _killQuest = new HintQuestCumulative
        {
            Name = "Kills",
            GoalValue = 50,
            MaxIncrease = 5,
        };

        private ProcessRamWatcher _ram = null;

        public RollerdromeConnector()
        {
            Name = "Rollerdrome";
            Description = "Rollerdrome is a BAFTA award-winning third person action shooter that seamlessly blends high octane combat with fluid motion to create an action experience like no other. Dominate with style in cinematic, visceral combat where kills net you health and pulling off tricks and grinds provide you ammunition, in this adrenaline-pumping action shooter.";
            Platform = "PC";
            SupportedVersions.Add("Steam");
            CoverFilename = "rollerdrome.png";
            Author = "Chandler";

            Quests.Add(_scoreQuest);
            Quests.Add(_killQuest);
        }

        public override bool Connect()
        {
            _ram = new ProcessRamWatcher("ROLLERDROME", "GameAssembly.dll");
            return _ram.TryConnect();
        }

        public override void Disconnect()
        {
            _ram = null;
        }

        public override bool Poll()
        {
            if (_ram.TestProcess() == false) { return false; }

            // Note: RAM does not seem to be consistent between rounds. Pointers exist but seem to change between individual rounds within a single run of the game.
            try {
                long scoreAddress = _ram.ResolvePointerPath64(_ram.BaseAddress + 0x207D9F8, new int[] { 0x40, 0xB8, 0x0, 0xC8, 0x50, 0x168 });
                if (scoreAddress == 0) {
                    scoreAddress = _ram.ResolvePointerPath64(_ram.BaseAddress + 0x2122080, new int[] { 0xB8, 0x0, 0x108, 0x30, 0x240, 0x120, 0x48 });
                }
                _scoreQuest.UpdateValue(_ram.ReadUint32(scoreAddress));
            }
            catch
            { }

            try {
                long killAddress = _ram.ResolvePointerPath64(_ram.BaseAddress + 0x2122080, new int[] { 0xB8, 0x0, 0x128, 0x58, 0x68, 0x118, 0x18 });
                _killQuest.UpdateValue(_ram.ReadUint32(killAddress));
            }
            catch
            { }
            
            return true;
        }
    }
}
