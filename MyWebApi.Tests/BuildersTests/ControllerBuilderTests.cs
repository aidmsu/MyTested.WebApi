﻿namespace MyWebApi.Tests.BuildersTests
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http.ModelBinding;
    using System.Web.Http.Results;

    using Builders.Contracts;
    using ControllerSetups;

    using NUnit.Framework;

    [TestFixture]
    public class ControllerBuilderTests
    {
        [Test]
        public void CallingShouldPopulateCorrectActionNameAndActionResultWithNormalActionCall()
        {
            var actionResultTestBuilder = MyWebApi
                .Controller<WebApiController>()
                .Calling(c => c.OkResultAction());

            this.CheckActionResultTestBuilder(actionResultTestBuilder, "OkResultAction");
        }

        [Test]
        public void CallingShouldPopulateCorrectActionNameAndActionResultWithAsyncActionCall()
        {
            var actionResultTestBuilder = MyWebApi
                .Controller<WebApiController>()
                .CallingAsync(c => c.AsyncOkResultAction());

            this.CheckActionResultTestBuilder(actionResultTestBuilder, "AsyncOkResultAction");
        }

        [Test]
        public void CallingShouldPopulateModelStateWhenThereAreModelErrors()
        {
            var requestBody = new RequestModel();

            var actionResultTestBuilder = MyWebApi
                .Controller<WebApiController>()
                .Calling(c => c.OkResultActionWithRequestBody(1, requestBody));

            var modelState = actionResultTestBuilder.Controller.ModelState;

            Assert.IsFalse(modelState.IsValid);
            Assert.AreEqual(1, modelState.Values.Count);
            Assert.AreEqual("Name", modelState.Keys.First());
        }

        [Test]
        public void CallingShouldHaveValidModelStateWhenThereAreNoModelErrors()
        {
            var requestBody = new RequestModel
            {
                Id = 1,
                Name = "Test"
            };

            var actionResultTestBuilder = MyWebApi
                .Controller<WebApiController>()
                .Calling(c => c.OkResultActionWithRequestBody(1, requestBody));

            var modelState = actionResultTestBuilder.Controller.ModelState;

            Assert.IsTrue(modelState.IsValid);
            Assert.AreEqual(0, modelState.Values.Count);
            Assert.AreEqual(0, modelState.Keys.Count);
        }

        private void CheckActionResultTestBuilder<TActionResult>(
            IActionResultTestBuilder<TActionResult> actionResultTestBuilder,
            string expectedActionName)
        {
            var actionName = actionResultTestBuilder.ActionName;
            var actionResult = actionResultTestBuilder.ActionResult;

            var testedActionResult = actionResult;
            if (actionResult is Task<TActionResult>)
            {
                testedActionResult = (actionResult as Task<TActionResult>).Result;
            }

            Assert.IsNotNullOrEmpty(actionName);
            Assert.IsNotNull(testedActionResult);

            Assert.AreEqual(expectedActionName, actionName);
            Assert.IsAssignableFrom<OkResult>(testedActionResult);
        }
    }
}
