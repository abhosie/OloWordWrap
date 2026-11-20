using OloWordWrap.Application.SampleItems;
using OloWordWrap.Domain.SampleItems;

namespace OloWordWrap.UnitTests.SampleItems;

public sealed class GetSampleItemsHandlerTests
{
    private readonly ISampleItemRepository _repository = Substitute.For<ISampleItemRepository>();

    [Fact]
    public async Task HandleAsync_WithItems_ReturnsThemOrderedByName()
    {
        var banana = new SampleItem(Guid.NewGuid(), "banana");
        var apple = new SampleItem(Guid.NewGuid(), "Apple");
        _repository.ListAsync(Arg.Any<CancellationToken>()).Returns([banana, apple]);
        var handler = new GetSampleItemsHandler(_repository);

        var result = await handler.HandleAsync(TestContext.Current.CancellationToken);

        result.Should().Equal(new SampleItemDto(apple.Id, "Apple"), new SampleItemDto(banana.Id, "banana"));
    }

    [Fact]
    public async Task HandleAsync_WithNoItems_ReturnsEmptyList()
    {
        _repository.ListAsync(Arg.Any<CancellationToken>()).Returns([]);
        var handler = new GetSampleItemsHandler(_repository);

        var result = await handler.HandleAsync(TestContext.Current.CancellationToken);

        result.Should().BeEmpty();
    }
}
