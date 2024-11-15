using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using Test.MVCApp.Models;
using Test.MVCApp.Models.Interface;
using System.Text.Json;
using System.Data;
using Microsoft.AspNetCore.Authorization;

namespace Test.MVCApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            IApiClient iApiClient = new ApiClient();


            IList<JsTreeModel> nodes = new List<JsTreeModel>();

            foreach (var item in iApiClient.GetEndPointsAsync().Result.EndPoints)
            {
                nodes.Add(new JsTreeModel
                {
                    id =item.Id.ToString(),
                    parent = item.ParentId.ToString(),
                    text = item.EndPointName
                });  
            }
            //Serialize to JSON string.
            ViewBag.Json = JsonSerializer.Serialize(nodes);
            ViewData["nodes"] = JsonSerializer.Serialize(nodes);

            return View(iApiClient);
        }

        public IList<JsTreeModel> GetEndPoints()
        {
            IApiClient iApiClient = new ApiClient();
            IList<JsTreeModel> nodes = new List<JsTreeModel>();
            foreach (var item in iApiClient.GetEndPointsAsync().Result.EndPoints)
            {
                nodes.Add(new JsTreeModel
                {
                    id = item.Id.ToString(),
                    parent = item.ParentId.ToString(),
                    text = item.EndPointName
                });
            }
            //Serialize to JSON string.
            //ViewBag.Json = JsonSerializer.Serialize(nodes);
            return nodes;
        }
        [HttpGet]
        //[Authorize]
        public async Task<JsonResult> Nodes()
        {
            IApiClient iApiClient = new ApiClient();
            var nodes = new List<JsTreeModel2>();
            var result =await iApiClient.GetEndPointsAsync();
            foreach (var item in result.EndPoints)
            {
                nodes.Add(new JsTreeModel2
                {
                    id = item.ParentId.ToString()+"-"+ item.Id.ToString(),
                    parent = item.ParentId== item.Id ? "#": item.ParentId.ToString()+"-"+( item.SubParentId>0? item.SubParentId.ToString(): item.ParentId.ToString()), // item.ParentId.ToString(), // (item.ParentId == 0 ) ? "#" : item.ParentId.ToString(),
                    text = item.EndPointName
                });
            }
            var re__ = Json(nodes);
            return Json(nodes);

            var nodes1 = new List<JsTreeModel2>();
            nodes1.Add(new JsTreeModel2() { id = "101", parent = "#", text = "Kunal" });
            nodes1.Add(new JsTreeModel2() { id = "102", parent = "#", text = "Root node 1" });
            nodes1.Add(new JsTreeModel2() { id = "103", parent = "102", text = "Child 1" });
            nodes1.Add(new JsTreeModel2() { id = "104", parent = "102", text = "Child 2" });

            var re1__ = Json(nodes1);

            return Json(nodes1);
            
        }


        public JsonResult GetChildItems()
        {
            List<JsTreeModel1> load = new List<JsTreeModel1>();
            load.Add(new JsTreeModel1 { id = 2, parentId = 1, name = "Folder 1" });
            load.Add(new JsTreeModel1 { id = 3, parentId = 1, name = "Folder 2" });
            load.Add(new JsTreeModel1 { id = 4, parentId = 1, name = "Folder 3", hasChild = true });
            load.Add(new JsTreeModel1 { id = 6, parentId = 4, name = "File 1" });
            load.Add(new JsTreeModel1 { id = 7, parentId = 4, name = "File 2" });

            load.Add(new JsTreeModel1 { id = 9, parentId = 2, name = "Folder 4", hasChild = true });
            load.Add(new JsTreeModel1 { id = 10, parentId = 9, name = "File 4" });
            load.Add(new JsTreeModel1 { id = 11, parentId = 9, name = "File 5" });
            load.Add(new JsTreeModel1 { id = 13, parentId = 9, name = "File 6" });

            load.Add(new JsTreeModel1 { id = 16, parentId = 3, name = "Folder 7" });
            load.Add(new JsTreeModel1 { id = 17, parentId = 3, name = "File 7" });
            load.Add(new JsTreeModel1 { id = 18, parentId = 3, name = "File 8" });
            load.Add(new JsTreeModel1 { id = 19, parentId = 3, name = "File 9" });
            //var childData = load.Where(t => t.parentId == id);
            var childData = load;
            return Json(childData);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}