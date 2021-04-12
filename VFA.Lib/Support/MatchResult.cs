using System.Collections.Generic;

namespace VFA.Lib.Support
{
    public sealed class MatchResult
    {
        public int HomeGoals { get; set; }

        public int AwayGoals { get; set; }
        
        public List<GoalScorers> HomeScorers { get; set; }

        public List<GoalScorers> AwayScorers { get; set; }

        public MatchResult()
        {
            this.HomeScorers = new List<GoalScorers>();
            this.AwayScorers = new List<GoalScorers>();
        }
    }

    public sealed class GoalScorers
    {
        public string Name { get; set; }

        public int Minute { get; set; }
    }
}
