using System.Runtime.InteropServices;
using System.Text;

namespace HTMLTOCGenerator
    {

    // ********************************************* class ShortenPath

    public class ShortenPath
        {

        [ DllImport ( "shlwapi.dll",
                      CharSet = CharSet.Auto ) ]
        static extern bool PathCompactPathEx (
                               [Out] StringBuilder  shortened_path,
                                     string         path_to_shorten,
                                     int            desired_length,
                                     int            flags );

        // ********************************************** shorten_path

        /// <summary>
        /// truncates a path to fit within a certain number of 
        /// characters by replacing path components with ellipses
        /// </summary>
        /// <param name="path_to_shorten">
        /// path that is to be shortened
        /// </param>
        /// <param name="desired_length">
        /// maximum length of the desired path (e.g., for a TextBox, 
        /// the value might be MaxLength)
        /// </param>
        /// <returns>
        /// shortened path
        /// </returns>
        public static string shorten_path ( string  path_to_shorten,
                                            int     desired_length )
            {
                                        // always define StringBuilder 
                                        // with size one larger than 
                                        // desired - failure to so  
                                        // will result in strange 
                                        // and wonderous exceptions
            StringBuilder  shortened_path = new StringBuilder (
                                        desired_length + 1 );

            PathCompactPathEx ( shortened_path,
                                path_to_shorten,
                                desired_length,
                                0 );

            return ( shortened_path.ToString ( ) );
            }

        } // class ShortenPath

    } // namespace HTMLTOCGenerator
