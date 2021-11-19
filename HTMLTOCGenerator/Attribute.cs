using System;

namespace HTMLTOCGenerator
    {

    // *********************************************** class Attribute

    /// <summary>
    /// container for an attribute name and value
    /// </summary>
    public class Attribute
        {

        string  attribute_name = String.Empty;
        string  attribute_value = String.Empty;

        // ************************************************* Attribute

        /// <summary>
        /// Construct a new Attribute; the name and value properties 
        /// must be specified
        /// </summary>
        /// <param name="name">
        /// The name of this attribute.
        /// </param>
        /// <param name="value">
        /// The value of this attribute.
        /// </param>
        public Attribute ( string  name, 
                           string  value )
            {

            Name = name;
            Value = value;
            }

        // ************************************************* Attribute

        /// <summary>
        /// Construct a new Attribute; the name property must be 
        /// specified
        /// </summary>
        /// <param name="name">
        /// The name of this attribute.
        /// </param>
        public Attribute ( string  name )
            {

            Name = name;
            Value = String.Empty;
            }

        // ************************************************* Attribute

        /// <summary>
        /// construct a blank attribute without any properties 
        /// specified
        /// </summary>
        public Attribute ( )
            {

            Name = String.Empty;
            Value = String.Empty;
            }

        // ****************************************************** Name

        /// <summary>
        /// The name for this attribute
        /// </summary>
        public string Name
            {

            get
                {
                return ( attribute_name );
                }

            set
                {
                attribute_name = value;
                }
            }

        // ***************************************************** Value

        /// <summary>
        /// The value for this attribute
        /// </summary>
        public string Value
            {

            get
                {
                return ( attribute_value );
                }

            set
                {
                attribute_value = value;
                }
            }

        } // class Attribute

    } // namespace HTMLTOCGenerator

