namespace JobCandidateHub.Tests
{
    [CollectionDefinition("Sequential-Tests", DisableParallelization = true)]
    public class SequentialTestsCollection : ICollectionFixture<SequentialTestsFixture>
    {
    }

    public class SequentialTestsFixture
    {
    }
}
