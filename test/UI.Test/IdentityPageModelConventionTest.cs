using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Microsoft.AspNetCore.Identity.UI.Test
{
    public class IdentityPageModelConventionTest
    {
        [Fact]
        public void IdentityUser_Type_SetsPageModelType_AsExpected()
        {
            // Arrange
            var pageApplicationModel = new PageApplicationModel(
                new PageActionDescriptor(),
                typeof(TestPageHandler).GetTypeInfo(),
                new List<object>());
            pageApplicationModel.ModelType = typeof(TestPageModel).GetTypeInfo();
            var identityPageModelConvention = new IdentityPageModelConvention<IdentityUser>();

            // Act
            identityPageModelConvention.Apply(pageApplicationModel);

            // Assert
            Assert.Equal(typeof(TestModel<IdentityUser, string>), pageApplicationModel.ModelType);
        }

        [Fact]
        public void DeriveFrom_IdentityUser_Type_SetsPageModelType_AsExpected()
        {
            // Arrange
            var pageApplicationModel = new PageApplicationModel(
                new PageActionDescriptor(),
                typeof(TestPageHandler).GetTypeInfo(),
                new List<object>());
            pageApplicationModel.ModelType = typeof(TestPageModel).GetTypeInfo();
            var identityPageModelConvention = new IdentityPageModelConvention<ApplicationUser>();

            // Act
            identityPageModelConvention.Apply(pageApplicationModel);

            // Assert
            Assert.Equal(typeof(TestModel<ApplicationUser, string>), pageApplicationModel.ModelType);
        }

        public static TheoryData<IPageApplicationModelConvention, Type> DeriveFromIdentityUserOfTData
        {
            get
            {
                return new TheoryData<IPageApplicationModelConvention, Type>()
                {
                    {
                        new IdentityPageModelConvention<GuidApplicationUser>(),
                        typeof(TestModel<GuidApplicationUser, Guid>)
                    },
                    {
                        new IdentityPageModelConvention<InheritedGuidApplicationUser>(),
                        typeof(TestModel<InheritedGuidApplicationUser, Guid>)
                    }
                };
            }
        }

        [Theory]
        [MemberData(nameof(DeriveFromIdentityUserOfTData))]
        public void DeriveFrom_IdentityUserOfT_Type_SetsPageModelType_AsExpected(
            IPageApplicationModelConvention identityPageModelConvention,
            Type expectedIdentityUserType)
        {
            // Arrange
            var pageApplicationModel = new PageApplicationModel(
                new PageActionDescriptor(),
                typeof(TestPageHandler).GetTypeInfo(),
                new List<object>());
            pageApplicationModel.ModelType = typeof(TestPageModel).GetTypeInfo();

            // Act
            identityPageModelConvention.Apply(pageApplicationModel);

            // Assert
            Assert.Equal(expectedIdentityUserType, pageApplicationModel.ModelType);
        }

        [Fact]
        public void ThrowsException_ForNonIdentityUser()
        {
            // Arrange
            var pageApplicationModel = new PageApplicationModel(
                new PageActionDescriptor(),
                typeof(TestPageHandler).GetTypeInfo(),
                new List<object>());
            pageApplicationModel.ModelType = typeof(TestPageModel).GetTypeInfo();
            var identityPageModelConvention = new IdentityPageModelConvention<NonIdentityUser>();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(
                () => identityPageModelConvention.Apply(pageApplicationModel));
            Assert.Equal(
                $"The type '{typeof(NonIdentityUser)}' must derive from '{typeof(IdentityUser<>).FullName}'.",
                exception.Message);
        }

        private class TestPageHandler { }

        private class NonIdentityUser { }

        private class ApplicationUser : IdentityUser { }

        private class GuidApplicationUser : IdentityUser<Guid> { }

        private class InheritedGuidApplicationUser : GuidApplicationUser { }

        [IdentityDefaultUI(typeof(TestModel<,>))]
        private abstract class TestPageModel { }

        private class TestModel<TUser, TKey> { }
    }
}
