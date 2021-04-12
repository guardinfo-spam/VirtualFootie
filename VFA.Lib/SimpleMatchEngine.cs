using System;

namespace VFA.Lib
{
    public class SimpleMatchEngine : IMatchEngine
    {
        public int DetermineWinner(double team1Strength, double team2Strength)
        {
            var random = new Random();
            var team1Buff = random.Next(0, 2);
            var team2Buff = random.Next(0, 2);

            team1Strength = applyTeamBuff(team1Strength, team1Buff);
            team2Strength = applyTeamBuff(team2Strength, team2Buff);

            //0 means home team wins
            //2 means away team wins
            if (team1Strength > team2Strength) return 0;
            //if (team1Strength == team2Strength) return 1;
            if (team1Strength <= team2Strength) return 2;

            return 0;
        }

        private double applyTeamBuff(double stats, int buff)
        {
            if (buff == 0)
                return stats - stats * 15 / 100;

            if (buff == 2)
                return stats + stats * 15 / 100;

            return stats;
        }
    }
}
