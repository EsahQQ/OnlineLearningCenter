using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineLearningCenter.Web.Controllers;
using OnlineLearningCenter.Web.ViewModels; 
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace OnlineLearningCenter.Web.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
    private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
    private readonly UsersController _controller;
    private readonly List<IdentityUser> _users;

    public UsersControllerTests()
    {
        _users = new List<IdentityUser>
        {
            new IdentityUser { Id = "1", UserName = "admin@test.com" },
            new IdentityUser { Id = "2", UserName = "user@test.com" }
        };

        _mockUserManager = MockHelper.MockUserManager(_users);

        var roles = new List<IdentityRole> { new IdentityRole("Admin"), new IdentityRole("User") };
        _mockRoleManager = MockHelper.MockRoleManager(roles);

        _controller = new UsersController(_mockUserManager.Object, _mockRoleManager.Object);
    }

    [Fact]
    public void Index_ShouldReturnViewWithListOfUsers() 
    {
        // Arrange

        // Act
        var result = _controller.Index(); 

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        var model = viewResult.Model.Should().BeAssignableTo<IEnumerable<IdentityUser>>().Subject;
        model.Should().HaveCount(2);
    }

    [Fact]
    public async Task Delete_Post_ShouldCallDeleteAsyncAndRedirect()
    {
        // Arrange
        var userIdToDelete = "2";
        var userToDelete = _users.Find(u => u.Id == userIdToDelete);

        _mockUserManager.Setup(m => m.GetUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns("1");

        // Act
        var result = await _controller.DeleteConfirmed(userIdToDelete);

        // Assert
        _mockUserManager.Verify(x => x.DeleteAsync(userToDelete), Times.Once);
        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
    }

    [Fact]
    public async Task ManageRoles_Post_ShouldUpdateUserRolesAndRedirect() 
    {
        // Arrange
        var userIdToUpdate = "2";
        var userToUpdate = _users.Find(u => u.Id == userIdToUpdate);
        var currentRoles = new List<string> { "User" };

        var viewModel = new ManageUserRolesViewModel
        {
            UserId = userIdToUpdate,
            UserName = userToUpdate.UserName,
            Roles = new List<RoleViewModel>
            {
                new RoleViewModel { RoleName = "Admin", IsSelected = true },
                new RoleViewModel { RoleName = "User", IsSelected = false }
            }
        };

        _mockUserManager.Setup(m => m.FindByIdAsync(userIdToUpdate)).ReturnsAsync(userToUpdate);
        _mockUserManager.Setup(m => m.GetRolesAsync(userToUpdate)).ReturnsAsync(currentRoles);

        // Act
        var result = await _controller.ManageRoles(viewModel);

        // Assert
        _mockUserManager.Verify(m => m.RemoveFromRolesAsync(userToUpdate, currentRoles), Times.Once);
        _mockUserManager.Verify(m => m.AddToRolesAsync(userToUpdate, It.Is<IEnumerable<string>>(roles => roles.Count() == 1 && roles.Contains("Admin"))), Times.Once);

        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject; 
        redirectResult.ActionName.Should().Be("Index");
    }
}