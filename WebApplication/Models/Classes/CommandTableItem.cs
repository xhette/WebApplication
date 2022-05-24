using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models.Classes
{
    public class CommandTableItem
    {

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("terminal_id")]
        public int TerminalId { get; set; }

        [JsonProperty("command_id")]
        public int CommandId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("parameter1")]
        public int Parameter1 { get; set; }

        [JsonProperty("paramete2")]
        public int Parameter2 { get; set; }

        [JsonProperty("parameter3")]
        public int Parameter3 { get; set; }

        [JsonProperty("parameter4")]
        public int Parameter4 { get; set; }

        [JsonProperty("str_parameter1")]
        public string StrParameter1 { get; set; }

        [JsonProperty("str_parameter2")]
        public string StrParameter2 { get; set; }

        [JsonProperty("state")]
        public int State { get; set; }

        [JsonProperty("state_name")]
        public string StateName { get; set; }

        [JsonProperty("time_created")]
        public DateTime TimeCreated { get; set; }

        [JsonProperty("time_delivered")]
        public DateTime TimeDelivered { get; set; }

    }
}