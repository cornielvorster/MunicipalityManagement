using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using MunicipalityManagement.Controllers;
using MunicipalityManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class ReportsControllerTests
{
    private readonly Mock<MunicipalityDbContext> _mockContext;
    private readonly ReportsController _controller;
    public ReportsControllerTests()
    {
        // Create DbContextOptions for the mock
        var options = new DbContextOptionsBuilder<MunicipalityDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase") // Use in-memory for simplicity
            .Options;

        // Setup the mock with the options
        _mockContext = new Mock<MunicipalityDbContext>(options);
        _controller = new ReportsController(_mockContext.Object);
    }

    [Fact]
    public async Task Index_ReturnsViewResult_WithReports()
    {
        // Arrange
        var reports = new List<Report>
        {
            new Report { ReportID = 1, ReportType = "Complaint" },
            new Report { ReportID = 2, ReportType = "Suggestion" }
        }.AsQueryable();

        var mockSet = new Mock<DbSet<Report>>();
        mockSet.As<IQueryable<Report>>().Setup(m => m.Provider).Returns(reports.Provider);
        mockSet.As<IQueryable<Report>>().Setup(m => m.Expression).Returns(reports.Expression);
        mockSet.As<IQueryable<Report>>().Setup(m => m.ElementType).Returns(reports.ElementType);
        mockSet.As<IQueryable<Report>>().Setup(m => m.GetEnumerator()).Returns(reports.GetEnumerator());

        _mockContext.Setup(c => c.Reports).Returns(mockSet.Object);

        // Act
        var result = await _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Report>>(viewResult.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public async Task Create_InvalidModel_ReturnsView()
    {
        // Arrange
        var report = new Report { ReportID = 1, ReportType = "Complaint" };
        _controller.ModelState.AddModelError("ReportType", "Required"); // Simulate invalid state

        // Act
        var result = await _controller.Create(report);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(report, viewResult.Model);
    }

    [Fact]
    public async Task DeleteConfirmed_RemovesReport_RedirectsToIndex()
    {
        // Arrange
        var report = new Report { ReportID = 1, ReportType = "Complaint" };
        _mockContext.Setup(c => c.Reports.FindAsync(1)).ReturnsAsync(report);
        _mockContext.Setup(c => c.Reports.Remove(report));
        _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act
        var result = await _controller.DeleteConfirmed(1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }
}