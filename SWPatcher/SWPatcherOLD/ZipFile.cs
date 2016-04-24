using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.AccessControl;
using Shell32;

namespace ZipFile
{
    /// <summary>
    /// Utility class to archive/unarchive files
    /// </summary>
    public class ArchiveManager
    {

        #region Private_Variables

        private static string _lastError = "";
        private const int SLEEP_DURATION = 100; // Miliseconds
        private const int TIMEOUT_VALUE = 5; // Seconds

        #endregion

        #region Private_Methods

        /// <summary>
        /// Does basic sanity check(Checks for file existence)
        /// Creates a new extraction path based on the archive name
        /// </summary>
        /// <param name="fileName">Archive file Name. Returns the reference as full path</param>
        /// <param name="extractionPath">[OUT]The extraction path for default extraction</param>
        /// <returns>true in case of success, false otherwise</returns>
        private static bool CheckAndGetExtractionPath(ref string fileName, out string extractionPath)
        {
            _lastError = "";    // Reset last error

            try
            {
                fileName = Path.GetFullPath(fileName);              // Ensure fileName has full path
                extractionPath = Path.GetDirectoryName(fileName);   // Extract just the Directory Name part of file name

                if (!File.Exists(fileName))                         // Do basic test to see if file exists
                {
                    _lastError = "ERROR: File " + fileName + " does NOT exist";
                    return false;
                }

                // Prepare the extraction path, based on file name
                extractionPath = Path.Combine(extractionPath, Path.GetFileNameWithoutExtension(fileName));
                if (!Directory.Exists(extractionPath))  // Create the default extraction path directory, if it does not exist
                {
                    Directory.CreateDirectory(extractionPath);
                }

                return true;
            }
            catch (Exception ex)
            {
                extractionPath = "";
                _lastError = "ERROR: Could not get/create the extraction path. Exception: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Checks if the extension of file is correct
        /// </summary>
        /// <param name="fileName">The filename to be checked</param>
        /// <returns>True if extension is correct, false otherwise</returns>
        private static bool VerifyExtension(string fileName)
        {
            try
            {
                _lastError = "";
                string extension = Path.GetExtension(fileName).ToUpper().Trim();
                if (extension != ".ZIP")
                {
                    _lastError = "ERROR: Invalid extension '" + extension + "' found in file name";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _lastError = "ERROR: Could not get/create the extraction path. Exception: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Stop the current thread till the item counts
        /// in both the folder objects are equal or a time out occurs.
        /// 
        /// This is basically a KLUDGE for lack of a better idea I could
        /// think of.
        /// </summary>
        /// <param name="folderObjSource">The folder object whose item count is to remain constant, typically source folder</param>
        /// <param name="folderObjDestination">The folder object whose item count is to change, typically destination folder</param>
        /// <returns>True if item counts were finally equal, False otherwise</returns>
        private static bool WaitTillItemCountIsEqual(Folder folderObjSource, Folder folderObjDestination)
        {
            try
            {
                _lastError = "";

                if (folderObjSource == null || folderObjDestination == null)
                {
                    _lastError = "ERROR: One or more Folder object(s) is/are null";
                    return false;
                }

                int sourceFolderItemCount = folderObjSource.Items().Count;
                int maxIterations = (TIMEOUT_VALUE * 1000) / SLEEP_DURATION;
                int numIterations = 0;
                while (folderObjDestination.Items().Count < sourceFolderItemCount)
                {
                    if (maxIterations <= numIterations++)
                    {
                        _lastError = "ERROR: Timeout occurred while processing archive";
                        return false;
                    }
                    System.Threading.Thread.Sleep(SLEEP_DURATION);
                }

                return true;
            }
            catch (Exception ex)
            {
                _lastError = "ERROR: Could not create archive. Exception: " + ex.Message;

                return false;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// String from last error that occurred in class
        /// </summary>
        public static string LastError
        {
            get { return _lastError; }
        }

        #endregion

        #region Public_Methods

        /// <summary>
        /// Creates archive from files in given directory
        /// </summary>
        /// <param name="archiveFile">The archive file to be created. Needs to have .zip extension</param>
        /// <param name="unArchiveFolder">The directory from which files/folders will be added to archive</param>
        /// <returns>True in case of success, false otherwise</returns>
        public static bool Archive(string archiveFile, string unArchiveFolder)
        {
            try
            {
                _lastError = "";

                // Check if the file has correct extension
                if (!VerifyExtension(archiveFile))
                {
                    return false;
                }

                // Create empty archive
                byte[] emptyArchiveContent = new byte[] { 80, 75, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                FileStream fs = File.Create(Path.GetFullPath(archiveFile));
                fs.Write(emptyArchiveContent, 0, emptyArchiveContent.Length);
                fs.Flush();
                fs.Close();
                fs = null;

                Shell shell = new Shell();
                Folder archive = shell.NameSpace(Path.GetFullPath(archiveFile));    // Get the archive folder object
                Folder sourceFolder = shell.NameSpace(Path.GetFullPath(unArchiveFolder)); // Get the source folder object
                archive.CopyHere(sourceFolder.Items(), 20);                         // Copy the contents from archive to extract path

                // While the above archive procedure works fine, there's
                // a caveat. The Copying is actually done in a different thread
                // and there is no direct way to wait while archiving completes.

                // The following function is a KLUDGE to wait till the item
                // counts in both source and destination are equal, or a 
                // timeout occurs
                if (!WaitTillItemCountIsEqual(sourceFolder, archive))
                {
                    // There was an error waiting for items to be copied
                    // We can directly return from here, the _lastError 
                    // has already been set by the function
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _lastError = "ERROR: Could not create archive. Exception: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Extracts the given archive file to default location
        /// i.e. Same directory as source with a new directory
        /// by the name of archive file
        /// </summary>
        /// <param name="archiveFile">The archive file to extract. Has to have .zip extension</param>
        /// <returns>True on Success, False otherwise</returns>
        public static bool UnArchive(string archiveFile)
        {
            try
            {
                _lastError = "";
                string extractionPath = "";

                //Get default extraction path
                if (!CheckAndGetExtractionPath(ref archiveFile, out extractionPath))
                {
                    // No need to set LastError here, its been already set by the function
                    return false;
                }

                // Use the generic UnArchive method to do the actual unarchive
                if (!UnArchive(archiveFile, extractionPath))
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _lastError = "ERROR: Could not unarchive. Exception: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// The main unarchive method. Will overwrite any files that are already there
        /// </summary>
        /// <param name="archiveFile">The file to be extracted. Needs to have zip extension</param>
        /// <param name="unArchiveFolder">The directory to which the file would be extracted</param>
        /// <returns>True on success, false otherwise</returns>
        public static bool UnArchive(string archiveFile, string unArchiveFolder)
        {
            try
            {
                _lastError = "";

                // Check if the archive file has correct extension
                if (!VerifyExtension(archiveFile))
                {
                    return false;
                }

                Shell shell = new Shell();
                Folder archive = shell.NameSpace(Path.GetFullPath(archiveFile));
                Folder extractFolder = shell.NameSpace(Path.GetFullPath(unArchiveFolder));

                // Copy each item one-by-one
                foreach (FolderItem f in archive.Items())
                {
                    extractFolder.CopyHere(f, 20);
                }

                // While the above unarchive procedure works fine, there's
                // a caveat. The Copying is actually done in a different thread
                // and there is no direct way to wait while extraction completes.

                // The following function is a KLUDGE to wait till the item
                // counts in both source and destination are equal, or a 
                // timeout occurs
                if (!WaitTillItemCountIsEqual(archive, extractFolder))
                {
                    // There was an error waiting for items to be copied
                    // We can directly return from here, the _lastError 
                    // has already been set by the function
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _lastError = "ERROR: Could not unarchive. Exception: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Copies permissions from one file to another
        /// </summary>
        /// <param name="sourceFile">The source file from which the permissions are to be copied</param>
        /// <param name="destFile">The destination file to which the permissins are to be applied</param>
        /// <returns>True on success, False otherwise</returns>
        public static bool CopyPermissions(string sourceFile, string destFile)
        {
            try
            {
                _lastError = "";

                FileInfo srcFileInfo = new FileInfo(Path.GetFullPath(sourceFile));
                FileInfo desFileInfo = new FileInfo(Path.GetFullPath(destFile));

                FileSecurity secSrc = srcFileInfo.GetAccessControl();
                secSrc.SetAccessRuleProtection(true, true);
                desFileInfo.SetAccessControl(secSrc);

                return true;
            }
            catch (Exception ex)
            {
                _lastError = "ERROR: Could not copy permissions. Exception: " + ex.Message;
                return false;
            }
        }

        #endregion
    }
}
