using System;

namespace SWPatcher.Helpers.Steam
{
    [Flags]
    public enum AppState
    {
        StateInvalid = 0,
        StateUninstalled = 1 << 0,
        StateUpdateRequired = 1 << 1,
        StateFullyInstalled = 1 << 2,
        StateEncrypted = 1 << 3,
        StateLocked = 1 << 4,
        StateFilesMissing = 1 << 5,
        StateAppRunning = 1 << 6,
        StateFilesCorrupt = 1 << 7,
        StateUpdateRunning = 1 << 8,
        StateUpdatePaused = 1 << 9,
        StateUpdateStarted = 1 << 10,
        StateUninstalling = 1 << 11,
        StateBackupRunning = 1 << 12,
        Unknown1 = 1 << 13,
        Unknown2 = 1 << 14,
        Unknown3 = 1 << 15,
        StateReconfiguring = 1 << 16,
        StateValidating = 1 << 17,
        StateAddingFiles = 1 << 18,
        StatePreallocating = 1 << 19,
        StateDownloading = 1 << 20,
        StateStaging = 1 << 21,
        StateCommitting = 1 << 22,
        StateUpdateStopping = 1 << 23,
    }
}
