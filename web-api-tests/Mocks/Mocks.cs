using Moq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using WebApi.Models;
using WebApi.DAL;

namespace WebApiTests.Mocks;

public class MockUtil
{
    public static Mock<TaskViewContext> CreateDbWithTransaction()
    {
        var mockContext = new Mock<TaskViewContext>();
        var mockDatabase = new Mock<DatabaseFacade>(mockContext.Object);
        var mockTransaction = new Mock<IDbContextTransaction>();

        mockDatabase
            .Setup(db => db.BeginTransactionAsync(default))
            .ReturnsAsync(mockTransaction.Object);

        mockContext.SetupGet(ctx => ctx.Database).Returns(mockDatabase.Object);

        return mockContext;
    }

    public static Mock<UserManager<User>> CreateUserManager()
    {
        var store = new Mock<IUserStore<User>>();

        // The null values should not cause any trouble, since this will only be used in (unit) tests.
        return new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }
}
