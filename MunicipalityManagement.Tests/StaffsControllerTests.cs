using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using MunicipalityManagement.Controllers;
using MunicipalityManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class StaffsControllerTests
{
    private readonly Mock<MunicipalityDbContext> _mockContext;
    private readonly StaffsController _controller;
    public StaffsControllerTests()
    {
        // Create DbContextOptions for the mock
        var options = new DbContextOptionsBuilder<MunicipalityDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase") // Use in-memory for simplicity
            .Options;

        // Setup the mock with the options
        _mockContext = new Mock<MunicipalityDbContext>(options);
        _controller = new StaffsController(_mockContext.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewResult_WithStaffList()
    {
        // Arrange
        var staff = new List<Staff>
        {
            new Staff { StaffID = 1, FullName = "Alice Smith" },
            new Staff { StaffID = 2, FullName = "Bob Johnson" }
        }.AsQueryable();

        var mockSet = new Mock<DbSet<Staff>>();
        mockSet.As<IQueryable<Staff>>().Setup(m => m.Provider).Returns(staff.Provider);
        mockSet.As<IQueryable<Staff>>().Setup(m => m.Expression).Returns(staff.Expression);
        mockSet.As<IQueryable<Staff>>().Setup(m => m.ElementType).Returns(staff.ElementType);
        mockSet.As<IQueryable<Staff>>().Setup(m => m.GetEnumerator()).Returns(staff.GetEnumerator());

        _mockContext.Setup(c => c.Staff).Returns(mockSet.Object);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Staff>>(viewResult.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public async Task Create_ValidStaff_RedirectsToIndex()
    {
        // Arrange
        var staff = new Staff { StaffID = 1, FullName = "Alice Smith" };
        _controller.ModelState.Clear(); // Simulate valid state
        _mockContext.Setup(c => c.Add(staff));
        _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _controller.Create(staff);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task DeleteConfirmed_RemovesStaff_RedirectsToIndex()
    {
        // Arrange
        var staff = new Staff { StaffID = 1, FullName = "Alice Smith" };
        _mockContext.Setup(c => c.Staff.FindAsync(1)).ReturnsAsync(staff);
        _mockContext.Setup(c => c.Staff.Remove(staff));
        _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _controller.DeleteConfirmed(1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }
}