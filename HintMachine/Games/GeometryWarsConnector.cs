﻿namespace HintMachine.Games
{
    public class GeometryWarsConnector : IGameConnector
    {
        private ProcessRamWatcher _ram = null;

        private readonly HintQuestCumulative _scoreQuest = new HintQuestCumulative
        {
            Name = "Score",
            GoalValue = 50000
        };

        public GeometryWarsConnector()
        {
            Name = "Geometry Wars: Retro Evolved";
            Description = "Kill geometric ennemies in this box shaped area twin stick shooter.";
            SupportedVersions = "Tested on up-to-date Steam version.";
            Author = "CalDrac";

            Quests.Add(_scoreQuest);
        }

        public override bool Connect()
        {
            _ram = new ProcessRamWatcher("GeometryWars");
            return _ram.TryConnect();
        }

        public override void Disconnect()
        {
            _ram = null;
        }

        public override bool Poll()
        {
            long scoreAddress = 0x63C890;
            _scoreQuest.UpdateValue(_ram.ReadUint32(scoreAddress));

            return true;
        }
    }
}