using Moq;
using RSSReader.Data.Repositories;

namespace RSSReader.UnitTests.Wrappers.Repositories
{
    class MockUOW : Mock<IUnitOfWork>
    {

        public MockReaderRepository ReaderRepo { get; }
        public MockBlogRepository BlogRepo { get; }
        public MockGroupRepository GroupRepo { get; }
        public MockPostRepository PostRepo { get; }
        public MockSubscriptionRepository SubscriptionRepo { get; }
        public MockUserPostDataRepository UserPostDataRepo { get; }
        public MockUserRepository UserRepo { get; }

        public MockUOW()
        {
            ReaderRepo = new MockReaderRepository();
            Setup(x => x.ReaderRepo).Returns(ReaderRepo.Object);

            BlogRepo = new MockBlogRepository();
            Setup(x => x.BlogRepo).Returns(BlogRepo.Object);

            GroupRepo = new MockGroupRepository();
            Setup(x => x.GroupRepo).Returns(GroupRepo.Object);

            PostRepo = new MockPostRepository();
            Setup(x => x.PostRepo).Returns(PostRepo.Object);

            SubscriptionRepo = new MockSubscriptionRepository();
            Setup(x => x.SubscriptionRepo).Returns(SubscriptionRepo.Object);

            UserPostDataRepo = new MockUserPostDataRepository();
            Setup(x => x.UserPostDataRepo).Returns(UserPostDataRepo.Object);

            UserRepo = new MockUserRepository();
            Setup(x => x.UserRepo).Returns(UserRepo.Object);
        }
    }
}
