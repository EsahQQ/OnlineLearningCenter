using Microsoft.AspNetCore.Identity;
using Moq;
using System.Collections.Generic;
using System.Linq;

namespace OnlineLearningCenter.Web.Tests;

public static class MockHelper
{
    public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : IdentityUser
    {
        var store = new Mock<IUserStore<TUser>>();
        var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
        mgr.Object.UserValidators.Add(new UserValidator<TUser>());
        mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

        mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
        mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
        mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
        mgr.Setup(x => x.Users).Returns(ls.AsQueryable());

        mgr.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((string id) => ls.FirstOrDefault(u => u.Id == id));
        mgr.Setup(x => x.GetRolesAsync(It.IsAny<TUser>())).ReturnsAsync(new List<string>());
        mgr.Setup(x => x.RemoveFromRolesAsync(It.IsAny<TUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);
        mgr.Setup(x => x.AddToRoleAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

        return mgr;
    }

    public static Mock<RoleManager<TRole>> MockRoleManager<TRole>(List<TRole> ls) where TRole : IdentityRole
    {
        var store = new Mock<IRoleStore<TRole>>();
        var mgr = new Mock<RoleManager<TRole>>(store.Object, null, null, null, null);
        mgr.Setup(x => x.Roles).Returns(ls.AsQueryable());
        return mgr;
    }
}