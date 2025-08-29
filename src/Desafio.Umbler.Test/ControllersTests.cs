using Desafio.Umbler.Controllers;
using Desafio.Umbler.Models;
using Desafio.Umbler.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using Desafio.Umbler.Dtos;

namespace Desafio.Umbler.Test
{
    [TestClass]
    public class ControllersTest
    {
        [TestMethod]
        public void Home_Index_returns_View()
        {
            var controller = new HomeController();
            var response = controller.Index();
            var result = response as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Home_Error_returns_View_With_Model()
        {
            var controller = new HomeController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var response = controller.Error();
            var result = response as ViewResult;
            var model = result.Model as ErrorViewModel;

            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public async Task Domain_In_Database()
        {
            var domain = new Domain
            {
                Id = 1,
                Ip = "192.168.0.1",
                Name = "test.com",
                UpdatedAt = DateTime.Now,
                HostedAt = "umbler.corp",
                Ttl = 60,
                WhoIs = "Ns.umbler.com"
            };

            var serviceMock = new Mock<IDomainService>();
            serviceMock.Setup(s => s.GetDomainInfoAsync("test.com"))
                       .ReturnsAsync(domain);

            var controller = new DomainController(serviceMock.Object);

            var response = await controller.Get("test.com");
            var result = response as OkObjectResult;
            var obj = result.Value as DomainResultDto;

            Assert.IsNotNull(result);
            Assert.IsNotNull(obj);
            Assert.AreEqual(domain.Name, obj.Domain);
            Assert.AreEqual(domain.Ip, obj.Ip);
            Assert.AreEqual(domain.HostedAt, obj.HostedAt);
        }

        [TestMethod]
        public async Task Domain_Not_In_Database()
        { 
            var serviceMock = new Mock<IDomainService>();
            serviceMock.Setup(s => s.GetDomainInfoAsync("unknown.com"))
                       .ReturnsAsync((Domain?)null);

            var controller = new DomainController(serviceMock.Object);

            var response = await controller.Get("unknown.com");
            var result = response as OkObjectResult;
            var obj = result?.Value as DomainResultDto;

            Assert.IsNotNull(result);
            Assert.IsNull(obj);
        }


        [TestMethod]
        public async Task Domain_Invalid_Domain()
        {
            var serviceMock = new Mock<IDomainService>();
            serviceMock.Setup(s => s.GetDomainInfoAsync("invalid"))
                       .ThrowsAsync(new ArgumentException("Domínio inválido"));

            var controller = new DomainController(serviceMock.Object);

            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await controller.Get("invalid");
            });
        }
    }
}
