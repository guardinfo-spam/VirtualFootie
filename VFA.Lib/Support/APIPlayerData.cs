namespace VFA.Lib.Support
{
    public sealed class APIPlayerData
    {
        
        public int PlayerID { get; set; }
        
        public int? id { get; set; }
        public string name { get; set; }

        public string first_name { get; set; }

        public string last_name { get; set; }

        public int? resource_id { get; set; }
        public int resource_base_id { get; set; }

        public int? fut_bin_id { get; set; }

        public int? fut_wiz_id { get; set; }

        public int? league { get; set; }

        public int? height { get; set; }

        public int? nation { get; set; }
        public int? club { get; set; }

        public int? rating { get; set; }

        public int? rating_average { get; set; }

        public int? pace { get; set; }

        public int? shooting { get; set; }

        public int? passing { get; set; }

        public int? dribbling { get; set; }

        public int? defending { get; set; }

        public int? physicality { get; set; }

        public int? rarity { get; set; }

        public string common_name { get; set; }

        public string position { get; set; }

        public string foot { get; set; }

        public string attack_work_rate { get; set; }

        public string defense_work_rate { get; set; }

        public int? skill_moves { get; set; }

        public int? weak_foot { get; set; }

        
        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(common_name))
                return common_name;

            if (!string.IsNullOrWhiteSpace(name))
                return name;

            return $"{first_name} {last_name}";
        }
    }
}
