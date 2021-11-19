using System;
using System.IO;
using System.Threading;

namespace HTMLTOCGenerator
    {

    // ************************************************* class TryOpen

    public class TryOpen
        {

        // ******************************************* class variables

        static string   try_open_exception = String.Empty;

        // ***************************************** TryOpen_Exception

        public static string TryOpen_Exception
            {

            get
                {
                return ( try_open_exception );
                }

            set
                {
                try_open_exception = value;
                }
            }

        // ************************************************** try_open

        /// <summary>
        /// attempts to open a file, with a user defined number of 
        /// attempts with a Sleep delay between attempts
        /// </summary>
        /// <param name="file_path">
        /// the fully qualified path of the file to be opened
        /// </param>
        /// <param name="file_mode">
        /// file mode enum value(see MSDN documentation)
        /// </param>
        /// <param name="file_access">
        /// file access enum value(see MSDN documentation)
        /// </param>
        /// <param name="file_share">
        /// file share enum value(see MSDN documentation)
        /// </param>
        /// <param name="attempts">
        /// total number of attempts to make (multiply by delay for 
        /// the maximum time the function will try opening the file)
        /// </param>
        /// <param name="delay">
        /// delay in milliseconds between each attempt
        /// </param>
        /// <returns>
        /// the opened FileStream, if the file can be opened for the 
        /// mode, access, and share requirements; null, otherwise
        /// </returns>
        /// <see>
        /// http://stackoverflow.com/questions/265953/
        ///     how-can-you-easily-check-if-access-is-denied-for-a-
        ///     file-in-net
        /// </see>
        public static FileStream try_open ( string      file_path, 
                                            FileMode    file_mode, 
                                            FileAccess  file_access, 
                                            FileShare   file_share, 
                                            int         attempts, 
                                            int         delay )
            {
            int         attempt = 0;
            FileStream  file_stream = null;
                                        // allow multiple attempts
            while ( true )
                {
                try
                    {
                    file_stream = File.Open ( file_path, 
                                              file_mode, 
                                              file_access, 
                                              file_share );
                                        // successful, break out of 
                                        // the loop
                    TryOpen_Exception = "Success";
                    break;
                    }
                                        // failed to open
                catch ( Exception ex )
                    {
                                        // save exception message
                    TryOpen_Exception = ex.Message;

                    attempt++;
                                        // if tried too many times, 
                                        // break out of loop
                    if ( attempt >= attempts )
                        {
                        file_stream = null;
                        break;
                        }
                                        // still have attempts, delay 
                                        // and loop
                    else
                        {
                        Thread.Sleep ( delay );
                        }
                    }
                }

            return ( file_stream );
            }

        } // class TryOpen

    } // namespace HTMLTOCGenerator
