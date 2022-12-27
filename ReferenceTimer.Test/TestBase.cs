namespace ReferenceTimer.Test
{
    public class TestBase
    {
        [TestInitialize]
        public void Initialize()
        {
            EstablishContext();
            BecauseOf();
        }

        protected virtual void EstablishContext() { }

        protected virtual void BecauseOf() { }
    }
}
