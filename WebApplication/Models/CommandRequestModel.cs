using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using WebApplication.Models.Classes;

namespace WebApplication.Models
{
    public class CommandRequestModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Выберите команду")]
        public int CommandId { get; set; }
        public string CommandName { get; set; }

        [Required(ErrorMessage = "Выберите один или несколько терминалов")]
        public string TerminalId { get; set; }
        public int? ParameterValue1 { get; set; }
        public int? ParameterValue2 { get; set; }
        public int? ParameterValue3 { get; set; }
        public int? ParameterValue4 { get; set; }
        public string StrParameterValue1 { get; set; }
        public string StrParameterValue2 { get; set; }

        public List<CommandTypeModel> CommandTypeList { get; set; }

        public List<int> TerminalsList { get; set; }
    }
}