using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using WebApplication.Models;
using WebApplication.Models.Classes;
using WebApplication.Models.Enums;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly string url;
        private readonly string login;
        private readonly string password;

        public HomeController()
        {
            url = ConfigurationManager.AppSettings["APIUrl"].ToString();
            login = ConfigurationManager.AppSettings["partLogin"].ToString();
            password = ConfigurationManager.AppSettings["partPassword"].ToString();

        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            string authorizationToken = "";
            CommandRequestModel model = new CommandRequestModel();

            if (Request.Cookies["APIAuthorizationToken"] != null)
            {
                authorizationToken = Request.Cookies["APIAuthorizationToken"].Value.ToString();
            }
            else
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage tokenResponse = await client.GetAsync($"/token?login={login}&password={password}");
                    if (tokenResponse.IsSuccessStatusCode)
                    {
                        var response = tokenResponse.Content.ReadAsStringAsync().Result;
                        Response.Cookies["APIAuthorizationToken"].Value = JObject.Parse(response)["token"].ToString();
                        authorizationToken = Request.Cookies["APIAuthorizationToken"].Value.ToString();
                    }
                }
            }

            if (!string.IsNullOrEmpty(authorizationToken))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync($"/commands/types?token={authorizationToken}");
                    if (response.IsSuccessStatusCode)
                    {
                        var commands = response.Content.ReadAsStringAsync().Result;

                        var commandsList = JObject.Parse(commands)["items"].ToString();

                        model.CommandTypeList = JsonConvert.DeserializeObject<List<CommandTypeModel>>(commandsList);
                    }

                    HttpResponseMessage response2 = await client.GetAsync($"/terminals?token={authorizationToken}");

                    if (response.IsSuccessStatusCode)
                    {
                        var terminals = response.Content.ReadAsStringAsync().Result;

                        var terminalList = (JContainer)JToken.Parse(terminals);
                        var list = terminalList.DescendantsAndSelf().OfType<JProperty>().Where(p => p.Name == "id").Select(p => p.Value.Value<int>()).ToList();

                        model.TerminalsList = list;
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Index(CommandRequestModel model)
        {
            string authorizationToken = "";

            if (Request.Cookies["APIAuthorizationToken"] != null)
            {
                authorizationToken = Request.Cookies["APIAuthorizationToken"].Value.ToString();
            }
            else
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage tokenResponse = await client.GetAsync($"/token?login={login}&password={password}");
                    if (tokenResponse.IsSuccessStatusCode)
                    {
                        var response = tokenResponse.Content.ReadAsStringAsync().Result;
                        Response.Cookies["APIAuthorizationToken"].Value = JObject.Parse(response)["token"].ToString();
                        authorizationToken = Request.Cookies["APIAuthorizationToken"].Value.ToString();
                    }
                }
            }

            if (ModelState.IsValid)
            {
                int[] terminals = model.TerminalId.Split(',').Select(n => Convert.ToInt32(n)).ToArray();

                if (terminals.Length == 1)
                {

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(url);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
                        var data = new Dictionary<String, String>();

                        data.Add("\"command_id\"", model.CommandId.ToString());


                        string postUrl = string.Format($"/terminals/{terminals[0]}/commands?token={authorizationToken}");
                        var jsonString = "{";
                        jsonString += string.Format($"\"command_id\": {model.CommandId}");

                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, postUrl);

                        if (model.ParameterValue1.HasValue)
                        {
                            jsonString += string.Format($", \"parameter1\": {model.ParameterValue1.Value}");
                        }

                        if (model.ParameterValue2.HasValue)
                        {
                            jsonString += string.Format($", \"parameter2\": {model.ParameterValue2.Value}");
                        }

                        if (model.ParameterValue3.HasValue)
                        {
                            jsonString += string.Format($", \"parameter3\": {model.ParameterValue3.Value}");
                        }

                        if (model.ParameterValue4.HasValue)
                        {
                            jsonString += string.Format($", \"parameter4\": {model.ParameterValue4.Value}");
                        }

                        if (model.ParameterValue4.HasValue)
                        {
                            jsonString += string.Format($", \"parameter4\": {model.ParameterValue4.Value}");
                        }

                        if (!string.IsNullOrEmpty(model.StrParameterValue1))
                        {
                            jsonString += string.Format($", \"str_parameter1\": \"{model.StrParameterValue1}\"");
                        }

                        if (!string.IsNullOrEmpty(model.StrParameterValue2))
                        {
                            jsonString += string.Format($", \"str_parameter2\": \"{model.StrParameterValue2}\"");
                        }

                        jsonString += "}";

                        request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = await client.SendAsync(request);

                        List<CommandTableItem> commandTableItems;

                        var session = HttpContext.Session;

                        if (session["CommandTableItems"] != null)
                        {
                            commandTableItems = JsonConvert.DeserializeObject<List<CommandTableItem>>(session["CommandTableItems"].ToString());
                        }
                        else
                        {
                            commandTableItems = new List<CommandTableItem>();
                        }

                        CommandTableItem item = new CommandTableItem
                        {
                            CommandId = model.CommandId,
                            Name = model.CommandName,
                            Parameter1 = model.ParameterValue1.HasValue ? model.ParameterValue1.Value : 0,
                            Parameter2 = model.ParameterValue2.HasValue ? model.ParameterValue2.Value : 0,
                            Parameter3 = model.ParameterValue3.HasValue ? model.ParameterValue3.Value : 0,
                            Parameter4 = model.ParameterValue4.HasValue ? model.ParameterValue4.Value : 0,
                            StrParameter1 = model.StrParameterValue1,
                            StrParameter2 = model.StrParameterValue2,
                            TimeCreated = DateTime.Now,
                            State = 0,
                            StateName = StatusEnumExtensions.GetDescruption(StatusEnum.NotDelivered),
                        };

                        if (commandTableItems.Count > 0)
                        {
                            int lastId = commandTableItems.Last().Id;
                            item.Id = lastId + 1;
                        }
                        else
                        {
                            item.Id = 1;
                        }

                        commandTableItems.Add(item);

                        session["CommandTableItems"] = JsonConvert.SerializeObject(commandTableItems);
                    }
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync($"/commands/types?token={authorizationToken}");
                    if (response.IsSuccessStatusCode)
                    {
                        var commands = response.Content.ReadAsStringAsync().Result;

                        var commandsList = JObject.Parse(commands)["items"].ToString();

                        model.CommandTypeList = JsonConvert.DeserializeObject<List<CommandTypeModel>>(commandsList);
                    }

                    HttpResponseMessage response2 = await client.GetAsync($"/terminals?token={authorizationToken}");

                    if (response.IsSuccessStatusCode)
                    {
                        var terminals = response.Content.ReadAsStringAsync().Result;

                        var terminalList = (JContainer)JToken.Parse(terminals);
                        var list = terminalList.DescendantsAndSelf().OfType<JProperty>().Where(p => p.Name == "id").Select(p => p.Value.Value<int>()).ToList();

                        model.TerminalsList = list;
                    }
                    return View(model);
                }
            }
        }


        public async Task<ActionResult> GetParametersInput(int selectedId)
        {
            string authorizationToken = Request.Cookies["APIAuthorizationToken"].Value.ToString();
            CommandTypeModel model = new CommandTypeModel();

            if (!string.IsNullOrEmpty(authorizationToken))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.GetAsync($"/commands/types?token={authorizationToken}");
                    if (response.IsSuccessStatusCode)
                    {
                        var commands = response.Content.ReadAsStringAsync().Result;

                        var commandsList = JObject.Parse(commands)["items"].ToString();

                        var commandTypeList = JsonConvert.DeserializeObject<List<CommandTypeModel>>(commandsList);
                        model = commandTypeList.Find(c => c.Id == selectedId);
                    }
                }

            }

            return PartialView("~/Views/Shared/_CommandsParametersInput.cshtml", model);
        }

        public ActionResult GetCommandsTable()
        {
            List<CommandTableItem> model;

            var session = HttpContext.Session;

            if (session["CommandTableItems"] != null)
            {
                model = JsonConvert.DeserializeObject<List<CommandTableItem>>(session["CommandTableItems"].ToString());
            }
            else
            {
                model = new List<CommandTableItem>();
            }

            return PartialView("~/Views/Shared/_CommandsTable.cshtml", model);
        }
    }
}