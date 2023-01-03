using DynamicData;
using FluentAssertions;
using Moq;
using ReferenceTimer.Logic;
using ReferenceTimer.Model;

namespace ReferenceTimer.Test.Logic.ReferenceContainerIteratorTests
{
    public abstract class ReferenceContainerIteratorTests : TestBase
    {
        protected Mock<IReference> CreateReferenceMock(string path)
        {
            var referenceMock = new Mock<IReference>(MockBehavior.Strict);
            referenceMock.Setup(mock => mock.Path).Returns(path);

            return referenceMock;
        }

        protected Mock<ISourceList<IReference>> CreateReferenceSourceListMock(
            IEnumerable<IReference> references)
        {
            var listMock = new Mock<ISourceList<IReference>>(MockBehavior.Strict);
            listMock.Setup(mock => mock.Items).Returns(references);

            return listMock;
        }
    }

    [TestClass]
    public class WhenGetNextPathIsCalledAndReferenceContainerDoesNotContainGivenPath
        : ReferenceContainerIteratorTests
    {
        private readonly Mock<IReferenceContainer> _referenceContainerMock;
        private readonly string _currentPath = "this is the current path";

        private readonly ReferenceContainerIterator _target = new ReferenceContainerIterator();

        private string? _result;

        public WhenGetNextPathIsCalledAndReferenceContainerDoesNotContainGivenPath()
        {
            var sourceMock = CreateReferenceSourceListMock(new List<IReference>
            {
                CreateReferenceMock("Some reference").Object,
                CreateReferenceMock("Some other reference").Object,
                CreateReferenceMock("Some thrid reference").Object,
                CreateReferenceMock("Some fourth reference").Object,
            });

            _referenceContainerMock = new Mock<IReferenceContainer>(MockBehavior.Strict);
            _referenceContainerMock.Setup(mock => mock.References).Returns(sourceMock.Object);
        }

        protected override void BecauseOf()
        {
            _result = _target.GetNextPath(_referenceContainerMock.Object, _currentPath);
        }

        [TestMethod]
        public void Then_result_should_be_nextPath()
        {
            _result.Should().BeEquivalentTo(_currentPath);
        }
    }

    [TestClass]
    public class WhenGetNextPathIsCalledAndCurrentReferenceIsAtTheBeginningOfTheList
        : ReferenceContainerIteratorTests
    {
        private readonly Mock<IReferenceContainer> _referenceContainerMock;
        private readonly string _currentPath = "this is the current path";
        private readonly string _nextPath = "this is the next path";

        private readonly ReferenceContainerIterator _target = new ReferenceContainerIterator();

        private string? _result;

        public WhenGetNextPathIsCalledAndCurrentReferenceIsAtTheBeginningOfTheList()
        {
            var sourceMock = CreateReferenceSourceListMock(new List<IReference>
            {
                CreateReferenceMock(_currentPath).Object,
                CreateReferenceMock(_nextPath).Object,
                CreateReferenceMock("Some thrid reference").Object,
                CreateReferenceMock("Some fourth reference").Object,
            });

            _referenceContainerMock = new Mock<IReferenceContainer>(MockBehavior.Strict);
            _referenceContainerMock.Setup(mock => mock.References).Returns(sourceMock.Object);
        }

        protected override void BecauseOf()
        {
            _result = _target.GetNextPath(_referenceContainerMock.Object, _currentPath);
        }

        [TestMethod]
        public void Then_result_should_be_nextPath()
        {
            _result.Should().BeEquivalentTo(_nextPath);
        }
    }

    [TestClass]
    public class WhenGetNextPathIsCalledAndCurrentReferenceIsAtTheEndOfTheList
        : ReferenceContainerIteratorTests
    {
        private readonly Mock<IReferenceContainer> _referenceContainerMock;
        private readonly string _currentPath = "this is the current path";
        private readonly string _nextPath = "this is the next path";

        private readonly ReferenceContainerIterator _target = new ReferenceContainerIterator();

        private string? _result;

        public WhenGetNextPathIsCalledAndCurrentReferenceIsAtTheEndOfTheList()
        {
            var sourceMock = CreateReferenceSourceListMock(new List<IReference>
            {
                CreateReferenceMock(_nextPath).Object,
                CreateReferenceMock("Some other reference").Object,
                CreateReferenceMock("Some thrid reference").Object,
                CreateReferenceMock(_currentPath).Object,
            });

            _referenceContainerMock = new Mock<IReferenceContainer>(MockBehavior.Strict);
            _referenceContainerMock.Setup(mock => mock.References).Returns(sourceMock.Object);
        }

        protected override void BecauseOf()
        {
            _result = _target.GetNextPath(_referenceContainerMock.Object, _currentPath);
        }

        [TestMethod]
        public void Then_result_should_be_nextPath()
        {
            _result.Should().BeEquivalentTo(_nextPath);
        }
    }

    [TestClass]
    public class WhenGetPreviousPathIsCalledAndReferenceContainerDoesNotContainGivenPath
        : ReferenceContainerIteratorTests
    {
        private readonly Mock<IReferenceContainer> _referenceContainerMock;
        private readonly string _currentPath = "this is the current path";

        private readonly ReferenceContainerIterator _target = new ReferenceContainerIterator();

        private string? _result;

        public WhenGetPreviousPathIsCalledAndReferenceContainerDoesNotContainGivenPath()
        {
            var sourceMock = CreateReferenceSourceListMock(new List<IReference>
            {
                CreateReferenceMock("Some reference").Object,
                CreateReferenceMock("Some other reference").Object,
                CreateReferenceMock("Some thrid reference").Object,
                CreateReferenceMock("Some fourth reference").Object,
            });

            _referenceContainerMock = new Mock<IReferenceContainer>(MockBehavior.Strict);
            _referenceContainerMock.Setup(mock => mock.References).Returns(sourceMock.Object);
        }

        protected override void BecauseOf()
        {
            _result = _target.GetPreviousPath(_referenceContainerMock.Object, _currentPath);
        }

        [TestMethod]
        public void Then_result_should_be_nextPath()
        {
            _result.Should().BeEquivalentTo(_currentPath);
        }
    }

    [TestClass]
    public class WhenGetPreviousPathIsCalledAndCurrentReferenceIsAtTheBeginningOfTheList
        : ReferenceContainerIteratorTests
    {
        private readonly Mock<IReferenceContainer> _referenceContainerMock;
        private readonly string _currentPath = "this is the current path";
        private readonly string _nextPath = "this is the next path";

        private readonly ReferenceContainerIterator _target = new ReferenceContainerIterator();

        private string? _result;

        public WhenGetPreviousPathIsCalledAndCurrentReferenceIsAtTheBeginningOfTheList()
        {
            var sourceMock = CreateReferenceSourceListMock(new List<IReference>
            {
                CreateReferenceMock(_currentPath).Object,
                CreateReferenceMock("Some other reference").Object,
                CreateReferenceMock("Some thrid reference").Object,
                CreateReferenceMock(_nextPath).Object,
            });

            _referenceContainerMock = new Mock<IReferenceContainer>(MockBehavior.Strict);
            _referenceContainerMock.Setup(mock => mock.References).Returns(sourceMock.Object);
        }

        protected override void BecauseOf()
        {
            _result = _target.GetPreviousPath(_referenceContainerMock.Object, _currentPath);
        }

        [TestMethod]
        public void Then_result_should_be_nextPath()
        {
            _result.Should().BeEquivalentTo(_nextPath);
        }
    }

    [TestClass]
    public class WhenGetPreviousPathIsCalledAndCurrentReferenceIsAtTheEndOfTheList
        : ReferenceContainerIteratorTests
    {
        private readonly Mock<IReferenceContainer> _referenceContainerMock;
        private readonly string _currentPath = "this is the current path";
        private readonly string _nextPath = "this is the next path";

        private readonly ReferenceContainerIterator _target = new ReferenceContainerIterator();

        private string? _result;

        public WhenGetPreviousPathIsCalledAndCurrentReferenceIsAtTheEndOfTheList()
        {
            var sourceMock = CreateReferenceSourceListMock(new List<IReference>
            {
                CreateReferenceMock("Some reference").Object,
                CreateReferenceMock("Some other reference").Object,
                CreateReferenceMock(_nextPath).Object,
                CreateReferenceMock(_currentPath).Object,
            });

            _referenceContainerMock = new Mock<IReferenceContainer>(MockBehavior.Strict);
            _referenceContainerMock.Setup(mock => mock.References).Returns(sourceMock.Object);
        }

        protected override void BecauseOf()
        {
            _result = _target.GetPreviousPath(_referenceContainerMock.Object, _currentPath);
        }

        [TestMethod]
        public void Then_result_should_be_nextPath()
        {
            _result.Should().BeEquivalentTo(_nextPath);
        }
    }
}
