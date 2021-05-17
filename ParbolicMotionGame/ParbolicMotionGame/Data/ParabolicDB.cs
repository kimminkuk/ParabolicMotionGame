using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ParbolicMotionGame.Data
{
    public class ParabolicDB
    {
        [JsonProperty("IdParabolic")]
        public int IdParabolic_class { get; set; }

        [JsonProperty("LevelParabolic")]
        public int LevelPrabolic_class { get; set; }
        [JsonProperty("ScoreParabolic")]
        public int ScoreParabolic_class { get; set; }

        [JsonProperty("NameParabolic")]
        public string NameParabolic_class { get; set; }

        [JsonProperty("DateParabolic")]
        public DateTime DateParabolic_class { get; set; }
    }
}
