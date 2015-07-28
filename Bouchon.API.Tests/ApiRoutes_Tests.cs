using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace Bouchon.API.Tests
{
    [TestClass]
    public class ApiRoutes_Tests
    {
        private const string ROOT_URL = "http://localhost:52653/";
        private HttpConfiguration _config;

        public ApiRoutes_Tests()
        {
            _config = new HttpConfiguration();
            WebApiConfig.Register(_config);
            _config.EnsureInitialized(); //exec route attributes mapping
        }

        [TestMethod]
        public void ApiRoutesTests()
        {
            var routesToTest = new List<RouteToTest>
            {
                // Shares
                //new RouteToTest(HttpMethod.Get, "api/share", "GET: api/share", typeof(ShareApiController), "Get"),
                //new RouteToTest(HttpMethod.Get, "api/share/5", "GET: api/share/{id:int}", typeof(ShareApiController), "Get"),
                //new RouteToTest(HttpMethod.Post, "api/share", "POST: api/share", typeof(ShareApiController), "Post"),
                //new RouteToTest(HttpMethod.Put, "api/share", "PUT: api/share", typeof(ShareApiController), "Put"),
                //new RouteToTest(HttpMethod.Delete, "api/share/5", "DELETE: api/share/{id:int}", typeof(ShareApiController), "Delete"),
            };

            foreach (var route in routesToTest)
                _TestRoute(route);
        }

        private void _TestRoute(RouteToTest route)
        {
            var req = new HttpRequestMessage(route.Method, ROOT_URL + route.Route);
            var tester = new ApiRouteTester(_config, req);

            var ctrlTypeErrMsg = "Issue with route: " + route.Name + " (controller type)";
            var actionNameErrMsg = "Issue with route: " + route.Name + " (action name)";

            Type ctrlType = null;
            try
            {
                ctrlType = tester.GetControllerType();
            }
            catch (Exception e)
            {
                Assert.Fail(ctrlTypeErrMsg);
            }

            string actionName = String.Empty;
            try
            {
                actionName = tester.GetActionName();
            }
            catch (Exception e)
            {
                Assert.Fail(actionNameErrMsg);
            }

            Assert.AreEqual(route.ControllerType, ctrlType);
            Assert.AreEqual(route.MethodName, actionName);
        }
    }

    public class RouteToTest
    {
        public RouteToTest(HttpMethod method, string route, string name, Type ctrlType, string methodName)
        {
            Method = method;
            Route = route;
            Name = name;
            ControllerType = ctrlType;
            MethodName = methodName;
        }

        public HttpMethod Method { get; set; }

        public string Route { get; set; }

        public string Name { get; set; }

        public Type ControllerType { get; set; }

        public string MethodName { get; set; }
    }

    public class ApiRouteTester
    {
        private HttpConfiguration _config;
        private HttpRequestMessage _request;
        private IHttpRouteData _routeData;
        private IHttpControllerSelector _controllerSelector;
        private HttpControllerContext _controllerContext;

        public ApiRouteTester(HttpConfiguration conf, HttpRequestMessage req)
        {
            _config = conf;
            _request = req;
            _routeData = _config.Routes.GetRouteData(_request);
            _request.Properties[HttpPropertyKeys.HttpRouteDataKey] = _routeData;
            _controllerSelector = new DefaultHttpControllerSelector(_config);
            _controllerContext = new HttpControllerContext(_config, _routeData, _request);
        }

        public string GetActionName()
        {
            if (_controllerContext.ControllerDescriptor == null)
                GetControllerType();

            var actionSelector = new ApiControllerActionSelector();
            var descriptor = actionSelector.SelectAction(_controllerContext);

            return descriptor.ActionName;
        }

        public Type GetControllerType()
        {
            var descriptor = _controllerSelector.SelectController(_request);
            _controllerContext.ControllerDescriptor = descriptor;

            return descriptor.ControllerType;
        }
    }
}