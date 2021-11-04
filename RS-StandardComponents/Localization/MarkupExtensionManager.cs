using System.Collections.Generic;

namespace RS_StandardComponents
{

    public class MarkupExtensionManager
    {
        private int _cleanupCount;
        private int _cleanupInterval = 40;
        public MarkupExtensionManager(int cleanupInterval)
        {
            _cleanupInterval = cleanupInterval;
        }
        public virtual void UpdateAllTargets()
        {
            List<ManagedMarkupExtension> copy = new List<ManagedMarkupExtension>(ActiveExtensions);
            foreach (ManagedMarkupExtension extension in copy)
            {
                extension.UpdateTargets();
            }
        }

        public List<ManagedMarkupExtension> ActiveExtensions { get; private set; } = new List<ManagedMarkupExtension>();
        
        public void CleanupInactiveExtensions()
        {
            List<ManagedMarkupExtension> newExtensions = new List<ManagedMarkupExtension>(ActiveExtensions.Count);
            foreach (ManagedMarkupExtension ext in ActiveExtensions)
            {
                if (ext.IsTargetsAlive)
                {
                    newExtensions.Add(ext);
                }
            }
            ActiveExtensions = newExtensions;
        }
        internal void RegisterExtension(ManagedMarkupExtension extension)
        {
            if (_cleanupCount > _cleanupInterval)
            {
                CleanupInactiveExtensions();
                _cleanupCount = 0;
            }
            ActiveExtensions.Add(extension);
            _cleanupCount++;
        }

       

    }
}
