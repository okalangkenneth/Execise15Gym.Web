﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Gym.Tests.Externsions
{
    public static class ControllerExtensions
    {
        public static void SetUserIsAuthenticated(this Microsoft.AspNetCore.Mvc.Controller controller, bool isAuthenticated)
        {
            var mockContext = new Mock<HttpContext>();
            mockContext.SetupGet(context => context.User.Identity.IsAuthenticated).Returns(isAuthenticated);

            controller.ControllerContext = new ControllerContext { HttpContext = mockContext.Object };

        }

        public static void SetAjaxRequest(this Microsoft.AspNetCore.Mvc.Controller controller, bool isAjax)
        {
            var mockContext = new Mock<HttpContext>();
            if(isAjax)
                mockContext.SetupGet(c => c.Request.Headers["X-Requested-With"]).Returns("XMLHttpRequest");
            else
                mockContext.SetupGet(c => c.Request.Headers["X-Requested-With"]).Returns("");

            controller.ControllerContext = new ControllerContext { HttpContext = mockContext.Object };
        }
    }
}
