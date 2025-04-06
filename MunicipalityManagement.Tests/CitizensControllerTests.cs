using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using MunicipalityManagement.Controllers;
using MunicipalityManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class CitizensControllerTests
{
    private readonly Mock<MunicipalityDbContext> _mockContext;
    private readonly CitizensController _controller;
    public CitizensControllerTests()
    {
        // Create DbContextOptions for the mock
        var options = new DbContextOptionsBuilder<MunicipalityDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase") // Use in-memory for simplicity
            .Options;

        // Setup the mock with the options
        _mockContext = new Mock<MunicipalityDbContext>(options);
        _controller = new CitizensController(_mockContext.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewResult_WithListOfCitizens()
    {
        // Arrange
        var citizens = new List<Citizen>
        {
            new Citizen { CitizenID = 1, FullName = "John Doe" },
            new Citizen { CitizenID = 2, FullName = "Jane Doe" }
        }.AsQueryable();

        var mockSet = new Mock<DbSet<Citizen>>();
        mockSet.As<IQueryable<Citizen>>().Setup(m => m.Provider).Returns(citizens.Provider);
        mockSet.As<IQueryable<Citizen>>().Setup(m => m.Expression).Returns(citizens.Expression);
        mockSet.As<IQueryable<Citizen>>().Setup(m => m.ElementType).Returns(citizens.ElementType);
        mockSet.As<IQueryable<Citizen>>().Setup(m => m.GetEnumerator()).Returns(citizens.GetEnumerator());

        _mockContext.Setup(c => c.Citizens).Returns(mockSet.Object);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Citizen>>(viewResult.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public async Task Details_NullId_ReturnsNotFound()
    {
        // Act
        var result = await _controller.Details(null);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_ValidModel_RedirectsToIndex()
    {
        // Arrange
        var citizen = new Citizen { CitizenID = 1, FullName = "John Doe" };
        _controller.ModelState.Clear(); // Simulate valid model state
        _mockContext.Setup(c => c.Add(citizen));
        _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _controller.Create(citizen);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task Edit_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var citizen = new Citizen { CitizenID = 1, FullName = "John Doe" };

        // Act
        var result = await _controller.Edit(2, citizen); // ID mismatch

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}