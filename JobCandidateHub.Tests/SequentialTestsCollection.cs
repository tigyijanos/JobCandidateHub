namespace JobCandidateHub.Tests
{
    /// <summary>
    /// This class is needed for defining a test collection to ensure tests run sequentially.
    /// </summary>
    [CollectionDefinition("Sequential-Tests", DisableParallelization = true)]
    public class SequentialTestsCollection : ICollectionFixture<SequentialTestsFixture>
    {
    }
}
