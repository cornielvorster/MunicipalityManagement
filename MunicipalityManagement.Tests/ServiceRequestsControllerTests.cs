using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using MunicipalityManagement.Controllers;
using MunicipalityManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class ServiceRequestsControllerTests
{
    private readonly Mock<MunicipalityDbContext> _mockContext;
    private readonly ServiceRequestsController _controller;
    public ServiceRequestsControllerTests()
    {
        // Create DbContextOptions for the mock
        var options = new DbContextOptionsBuilder<MunicipalityDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase") // Use in-memory for simplicity
            .Options;

        // Setup the mock with the options
        _mockContext = new Mock<MunicipalityDbContext>(options);
        _controller = new ServiceRequestsController(_mockContext.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewResult_WithRequests()
    {
        // Arrange
        var requests = new List<ServiceRequest>
        {
            new ServiceRequest { RequestID = 1, ServiceType = "Water" },
            new ServiceRequest { RequestID = 2, ServiceType = "Road" }
        }.AsQueryable();

        var mockSet = new Mock<DbSet<ServiceRequest>>();
        mockSet.As<IQueryable<ServiceRequest>>().Setup(m => m.Provider).Returns(requests.Provider);
        mockSet.As<IQueryable<ServiceRequest>>().Setup(m => m.Expression).Returns(requests.Expression);
        mockSet.As<IQueryable<ServiceRequest>>().Setup(m => m.ElementType).Returns(requests.ElementType);
        mockSet.As<IQueryable<ServiceRequest>>().Setup(m => m.GetEnumerator()).Returns(requests.GetEnumerator());

        _mockContext.Setup(c => c.ServiceRequests).Returns(mockSet.Object);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<ServiceRequest>>(viewResult.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public async Task Details_NonExistentId_ReturnsNotFound()
    {
        // Arrange
        _mockContext.Setup(c => c.ServiceRequests.FindAsync(1)).ReturnsAsync((ServiceRequest)null);

        // Act
        var result = await _controller.Details(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Edit_ValidData_UpdatesAndRedirects()
    {
        // Arrange
        var request = new ServiceRequest { RequestID = 1, ServiceType = "Water" };
        _mockContext.Setup(c => c.Update(request));
        _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);
        _controller.ModelState.Clear(); // Simulate valid state

        // Act
        var result = await _controller.Edit(1, request);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }
}