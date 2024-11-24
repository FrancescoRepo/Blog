using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Services;
using LinkDotNet.Blog.Web.Features.ShowBlogPost.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NCronJob;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost.Components;
public class BlogPostPreviewDialogTests : BunitContext
{
    public BlogPostPreviewDialogTests()
    {
        ComponentFactories.AddStub<SimilarBlogPostSection>();
        JSInterop.Mode = JSRuntimeMode.Loose;
        var shortCodeRepository = Substitute.For<IRepository<ShortCode>>();
        shortCodeRepository.GetAllAsync().Returns(PagedList<ShortCode>.Empty);
        Services.AddScoped(_ => shortCodeRepository);
        Services.AddScoped(_ => Substitute.For<IUserRecordService>());
        Services.AddScoped(_ => Substitute.For<IInstantJobRegistry>());
        Services.AddScoped(_ => Substitute.For<IToastService>());
        Services.AddScoped(_ => Options.Create(new ApplicationConfigurationBuilder().Build()));
        AddAuthorization();
        ComponentFactories.AddStub<PageTitle>();
        ComponentFactories.AddStub<Like>();
        ComponentFactories.AddStub<CommentSection>();
    }

    [Fact]
    public void ShouldRenderBlogPostModalPreview()
    {
        var repositoryMock = Substitute.For<IRepository<BlogPost>>();
        Services.AddScoped(_ => repositoryMock);
        var blogPost = new BlogPostBuilder()
            .WithTitle("sample")
            .WithContent("# Content Title")
            .Build();

        var cut = Render<BlogPostPreviewDialog>(
            p => p.Add(s => s.BlogPost, blogPost));
        var blogPostDetail = cut.FindComponent<BlogPostDetail>();
        if (blogPostDetail != null)
        {
            blogPostDetail.Instance.IsPreview.ShouldBeTrue();
            var pageTitleStub = blogPostDetail.FindComponent<PageTitleStub>();
            var pageTitle = Render(pageTitleStub.Instance.ChildContent!);
            pageTitle.Markup.ShouldBe("sample");
        }

    }
}
