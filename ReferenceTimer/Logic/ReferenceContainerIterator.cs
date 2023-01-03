using System;
using System.Linq;

using ReferenceTimer.Model;

namespace ReferenceTimer.Logic
{
    public interface IReferenceContainerIterator
    {
        string GetNextPath(IReferenceContainer referenceContainer, string currentPath);
        string GetPreviousPath(IReferenceContainer referenceContainer, string currentPath);
    }

    internal class ReferenceContainerIterator : IReferenceContainerIterator
    {
        public string GetNextPath(
            IReferenceContainer referenceContainer,
            string currentPath)
        {
            return ChangeReference(NextIndex, referenceContainer, currentPath);
        }

        public string GetPreviousPath(
            IReferenceContainer referenceContainer,
            string currentPath)
        {
            return ChangeReference(PreviousIndex, referenceContainer, currentPath);
        }

        private static int NextIndex(int listLength, int index)
        {
            return index + 1 == listLength
                ? 0
                : index + 1;
        }

        private static int PreviousIndex(int listLength, int index)
        {
            return index == 0
                ? listLength - 1
                : index - 1;
        }

        private string ChangeReference(
            Func<int, int, int> getNextIndex,
            IReferenceContainer referenceContainer,
            string currentPath)
        {
            var referenceList = referenceContainer.References.Items.ToList();

            var reference = referenceList
                .FirstOrDefault(item => item.Path.Equals(currentPath));
            if (reference is null)
                return currentPath;

            var index = referenceList.IndexOf(reference);
            int nextIndex = getNextIndex(referenceList.Count, index);

            return referenceList[nextIndex].Path;
        }
    }
}
