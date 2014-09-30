using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace CSV_checker
{

    ////
    //Class CSV_File
    //handles choosing the CVS file, checking for errors, printing a report, etc.
    class CSV_File
    {
        ///Private internal variables
        private string _fpath; //container for the path to a file
        private string _fname;
        private ulong _num_lines;
        private bool _is_initialized; //initilization check
        private string _header_s;
        private string[] _header_a;
        private string _current_line_s;
        private string[] _current_line_a;

        private bool _has_quotes;
        private bool _has_correct_header;
        private int _current_index = -1;
        private int _package_ID_index = -1;
        private int _name_index = -1;
        private int _address_index = -1;
        private int _city_index = -1;
        private int _state_index = -1;
        private int _zipcode_index = -1;
        private int _country_index = -1;
        private ulong _num_errors;
        private ulong _num_bad_format;
        private ulong _num_bad_zipcodes;
        private ulong _num_illegal_chars;
        private ulong _num_over40_chars;
        private ulong _num_missing_fields;

        private readonly string[] CSV_quote_delimiter = new string[] { "\",\"" };
        private readonly char[] CSV_comma_delimiter = new char[] { ',' };
        private readonly char[] CSV_trim_char = new char[] { '\"' };


        private readonly string _correct_header = "\"Package Id\",\"Company\",\"Full Name\",\"Address 1\",\"Address 2\",\"City\",\"State\",\"Zip\",\"Country\",\"Cost Center Id\",\"Reference 1\",\"Reference 2\",\"Reference 3\",\"Reference 4\"";
        private readonly string[] _correct_header_a = new string[] { "Package Id","Company","Full Name",
                                                                     "Address 1","Address 2","City",
                                                                     "State","Zip","Country",
                                                                     "Cost Center Id","Reference 1","Reference 2",
                                                                     "Reference 3","Reference 4"};
        

        private Regex _illegal_package_id_rx = new Regex(@"^[\.-]|[\?\$\)\(\*\^\+""\\<>,#@&%!='`]" ,RegexOptions.Compiled);
        private Regex _illegal_name_chars_rx = new Regex(@"[^a-zA-Z\.\-' ]", RegexOptions.Compiled);
        private Regex _legal_state_code_rx = new Regex(@"^[A-Z]{2}$" ,RegexOptions.Compiled);
        private Regex _legal_zipcode_chars_rx = new Regex(@"^\d{5}$" ,RegexOptions.Compiled);
        private Regex _legal_long_zipcode_chars_rx = new Regex(@"^\d{5}\-\d{4}$" ,RegexOptions.Compiled);
        //private Regex _legal_country_code_rx = "US";
        private Regex _illegal_chars_rx = new Regex(@"[^\p{IsBasicLatin}]" ,RegexOptions.Compiled);
        
        /////
        ///Public constructors, getters/setters, methods and member functions start here
        //

        //Default Constructor
        public CSV_File()
        {
            //fpath = System.Environment.GetEnvironmentVariable("windir");
            is_initialized = false;

            num_errors = 0;
        }
        
        //Constructor
        public CSV_File(string a_fpath)
        {
            fpath = a_fpath;
            //is_initialized = true;
            initialize();
        }

        ////
        ///getters/setters
        //
        public bool is_initialized
        {
            set { _is_initialized = value; }
            get { return _is_initialized; }
        }
        
        // fpath  
        //access the file path
        public string fpath    
        {
            set
            {
                _fpath = value;
                initialize();
            }
            get { return _fpath; }
        }
        
        public string fname
        { 
            set { _fname = value; }
            get { return _fname;  }
        }

        //string[] header_s
        //get or set the header in string form
        public string header_s
        {
            set { _header_s = value; }
            get { return _header_s; }
        }

        //string[] header_a
        //get or set the header in array form
        public string[] header_a
        {
            set { _header_a = value; }
            get { return _header_a;  }
        }

        //string[] correct_header_a
        //get the correct header in array form
        public string[] correct_header_a
        {
            get { return _correct_header_a; }
        }

        public bool has_correct_header
        {
            set { _has_correct_header = value; }
            get { return _has_correct_header;  }

        }

        public bool has_quotes
        {
            set { _has_quotes = value; }
            get { return _has_quotes; }

        }

        //string current_line_a
        //get or set the line currently being read in string` form
        public string current_line_s
        {
            set { _current_line_s = value; }
            get { return _current_line_s; }
        }

        //string[] current_line_a
        //get or set the line currently being read in array form
        public string[] current_line_a
        {
            set { _current_line_a = value; }
            get { return _current_line_a; }
        }
        
                        
        public Regex illegal_package_id_rx
        {
            get { return _illegal_package_id_rx; }
        }        
        
        public Regex legal_zipcode_chars_rx
        {
            get { return _legal_zipcode_chars_rx; }
        }

        public Regex illegal_name_chars_rx
        {
            get { return _illegal_name_chars_rx; }
        }

        public Regex legal_long_zipcode_chars_rx
        {
            get { return _legal_long_zipcode_chars_rx; }
        }

        public Regex legal_state_code_rx
        {
            get { return _legal_state_code_rx; }
        }
        
        //public Regex legal_country_code
        //{
        //    get { return _legal_country_code_rx; }
        //}
        
        public Regex illegal_chars_rx
        {
            get { return _illegal_chars_rx; }
        }
   
        public int current_index
        {
            set { _current_index = value; }
            get { return _current_index; }
        }

        public int package_ID_index
        {
            set { _package_ID_index = value; }
            get { return _package_ID_index; }
        }

        public int name_index
        {
            set { _name_index = value; }
            get { return _name_index; }
        }

        public int address_index
        {
            set { _address_index = value; }
            get { return _address_index; }
        }

        public int city_index
        {
            set { _city_index = value; }
            get { return _city_index; }
        }

        public int state_index
        {
            set { _state_index = value; }
            get { return _state_index; }
        }

        public int zipcode_index
        {
            set { _zipcode_index = value; }
            get { return _zipcode_index; }
        }

        public int country_index
        {
            set { _country_index = value; }
            get { return _country_index; }
        }

        public ulong num_errors
        {
            set { _num_errors = value; }
            get { return _num_errors; }
        }

        public ulong num_bad_format
        {
            set { _num_bad_format = value; }
            get { return _num_bad_format; }
        }

        public ulong num_bad_zipcodes
        {
            set { _num_bad_zipcodes = value; }
            get { return _num_bad_zipcodes; }
        }

        public ulong num_illegal_chars
        {
            set { _num_illegal_chars = value; }
            get { return _num_illegal_chars; }
        }

        public ulong num_over40_chars
        {
            set { _num_over40_chars = value; }
            get { return _num_over40_chars; }
        }
        
        public ulong num_missing_fields
        {
            set { _num_missing_fields = value; }
            get { return _num_missing_fields; }
        }

        public ulong num_lines
        {
            set { _num_lines = value; }
            get { return _num_lines; }
        }

        ////
        /// Public methods and member functions start here
        //

        //initialize()
        //public method
        //gets header from file, initializes error counts, etc
        public void initialize()
        {
            is_initialized = true;
            
            string[] fpath_a = fpath.Split(new char[]{'\\'});
            fname = fpath_a[fpath_a.Length - 1];

            get_header_ff();
            
            num_errors = 0;
            num_bad_format = 0;
            num_bad_zipcodes = 0;
            num_illegal_chars = 0;
            _num_over40_chars = 0;
            num_missing_fields = 0;

        }
        
        
        //void get_header_ff()
        //gets the header in string form from the input file
        public void get_header_ff()
        {
            if(is_initialized)
            {
                StreamReader reader = new StreamReader(fpath);
                if ((char)reader.Peek() == '\"')
                {
                    has_quotes = true;
                }
                else
                {
                    has_quotes = false;
                }
                header_s = reader.ReadLine();
                header_to_a();
                wrong_header_check();
                find_special_indices();
                reader.Close();
            }
            else { header_s = "No File Selected."; }
        }

        //bool header_check()
        //returns true if the header matches the correct header, false if it dosent
        public void wrong_header_check()
        {
            if (has_quotes)
            {
                if (_header_s == _correct_header)
                {
                    has_correct_header = true;
                }
                else
                {
                    num_errors++;
                    has_correct_header = false;
                }
            }
            else if (header_a.Length == correct_header_a.Length)
            {
                int loop_length = header_a.Length;
                for (int index = 0; index < loop_length; index++ )
                {
                    if(header_a[index] != correct_header_a[index])
                    {
                        num_errors++;
                        _has_correct_header = false;
                    }
                }
                _has_correct_header = true;
            }
            else
            {
                num_errors++;
                _has_correct_header = false;
            }
        }


        //header_to_a()
        //converts header string to an array
        public void header_to_a()
        {
            if (_has_quotes)
            {
                header_a = header_s.Trim(CSV_trim_char).Split(CSV_quote_delimiter, StringSplitOptions.None);
            }
            else
            {
                header_a = header_s.Split(CSV_comma_delimiter);
            }
        }

        //current_line_to_a()
        //converts the current line to an array w/ each individual field
        public void current_line_to_a()
        {
            current_line_a = null; 
            if (_has_quotes)
            {
                current_line_a = current_line_s.Trim(CSV_trim_char).Split(CSV_quote_delimiter, StringSplitOptions.None);
            }
            else
            {
                current_line_a = current_line_s.Split(CSV_comma_delimiter);
            }
        }
        
        //find_special_indices()
        //if the header is right, automatically assign the header indices for:
        //      package id, full name, address 1, city, state, zip, country
        //these fields are either mandatory or need to be in a specific format
        //tries to figure out where the special headers are if the header is in the wrong format
        public void find_special_indices()
        {
            if (has_correct_header)
            {
                package_ID_index = 0;
                name_index = 2;
                address_index = 3;
                city_index = 5;
                state_index = 6;
                zipcode_index = 7;
                country_index = 8;
            }
            else
            {
                foreach (string field in header_a)
                {
                    if (field.Trim().ToLower().Contains("id"))
                    {
                        if (package_ID_index == -1)
                        {
                            package_ID_index = Array.IndexOf(header_a, field);
                        }
                    }
                    else if (field.Trim().ToLower().Contains("name"))
                    {
                        if (name_index == -1)
                        {
                            name_index = Array.IndexOf(header_a, field);
                        }
                    }
                    else if (field.Trim().ToLower().Contains("addr"))
                    {
                        if (address_index == -1)
                        {
                            address_index = Array.IndexOf(header_a, field);
                        }
                    }
                    else if (field.Trim().ToLower().Contains("city"))
                    {
                        if (city_index == -1)
                        {
                            city_index = Array.IndexOf(header_a, field);
                        }
                    }
                    else if (field.Trim().ToLower().Contains("stat"))
                    {
                        if (state_index == -1)
                        {
                            state_index = Array.IndexOf(header_a, field);
                        }
                    }
                    else if (field.Trim().ToLower().Contains("coun"))
                    {
                        if (country_index == -1)
                        {
                            country_index = Array.IndexOf(header_a, field);
                        }
                    }
                    else if (field.Trim().ToLower().Contains("zip"))
                    {
                        if (zipcode_index == -1)
                        {
                            zipcode_index = Array.IndexOf(header_a, field);
                        }
                    }
                    else if (field.Trim().StartsWith("post"))
                    {
                        if (zipcode_index == -1)
                        {
                            zipcode_index = Array.IndexOf(header_a, field);
                        }
                    }
                }    
            }
        }

        //void rewrite_array(string[] old_array, string[] new_array)
        //rewrites the old_array to the new array
        public void rewrite_array(string[] old_array, string[] new_array)
        {
            if (old_array.Length < new_array.Length)
                Array.Resize(ref old_array, new_array.Length);

            else if (old_array.Length > new_array.Length)
                Array.Resize(ref new_array, old_array.Length);

            //else System.Windows.Forms.MessageBox.Show("If you are seeing this, something very strange happend.");

            for (uint index = 0; index < old_array.Length; index++)
                old_array[index] = new_array[index];
        }

        //bool[] multi_error_check(int a_index, string a_entry)
        //single function which checks for three different error types : { illegal characters/format, over 40 chars, missing mandatory field }
        //returns an array of bool's, one col for each check. true return if an error is found.
        public bool[] multi_error_check(int a_index, string a_entry)
        {
            bool[] error_array = new bool[3];
            error_array[0] = check_ill_chars(a_index, a_entry);
            error_array[1] = check_length(a_entry);
            error_array[2] = missing_mandatory_field(a_index, a_entry);
            return error_array;
        }

        //bool check_ill_chars(int a_index, string a_field )
        //check if the given field (a_field) has any illegal characters specific to the given header col (a_index)
        //returns true if it has special chars
        public bool check_ill_chars(int a_index, string a_field )
        {
            bool has_error = false;

            //first, check if isnt blank to save CPU power
            if (!String.IsNullOrEmpty(a_field))
            {
                //each if statement is for a specific header column
                //uses a specific regex for the spcific header col to see if in correct form
                if (a_index == package_ID_index)
                {
                    has_error = illegal_package_id_rx.IsMatch(a_field);
                }
                else if (a_index == name_index)
                {
                    has_error = illegal_name_chars_rx.IsMatch(a_field);
                }
                else if (a_index == address_index || a_index == address_index + 1 || a_index == city_index)
                {
                    has_error = illegal_chars_rx.IsMatch(a_field);
                }
                else if (a_index == state_index)
                {
                    has_error = !legal_state_code_rx.IsMatch(a_field);
                }
                else if (a_index == zipcode_index)
                {
                    has_error = check_zipcodes(a_field);
                }
                else if (a_index == country_index)
                {
                    has_error = (a_field == "US") ? false : true;
                }
                else
                {
                    has_error = illegal_chars_rx.IsMatch(a_field);
                }
            }

            if (has_error)
            {
                num_illegal_chars++;
                num_errors++;
                return true;
            }
            else
            {
                return false;
            }
        }

        //bool check_zipcodes( string a_zipcode )
        //Checks to see if the a given zipcode (string a_zipcode) in ##### or #####-#### form
        //returns true if zipcode has wrong format
        public bool check_zipcodes( string a_zipcode )
        {
            bool check1 = _legal_zipcode_chars_rx.IsMatch(a_zipcode); //##### form
            bool check2 = _legal_long_zipcode_chars_rx.IsMatch(a_zipcode); //#####-#### form
            
            //if (!Regex.IsMatch(a_zipcode, _legal_zipcode_chars))
            if (check1)
            {
                return false;
            }
            //else if (!Regex.IsMatch(a_zipcode, _legal_long_zipcode_chars))
            else if (check2)
            {
                return false;
            }
            else
            {
                num_bad_zipcodes++;
                return true;
            }
        }

        //bool check_length(string a_line)
        //checks to see a given item (string a_line) is over 40 chars
        //returns true if too long
        public bool check_length(string a_line)
        {
            if (a_line.Length <= 40)
                return false;
            else
            {
                _num_over40_chars++;
                num_errors++;
                return true;
            }
        }

        //bool missing_mandatory_field( int a_index, string a_field)
        //checks to see if a mandatory field (via a_index) is blank (via a_field)
        //returns true if mandatory && blank
        public bool missing_mandatory_field( int a_index, string a_field)
        {
            bool result;
            //check if the field is blank
            if (!String.IsNullOrWhiteSpace(a_field))
            {
                result = false;
            }
            //check if the field is mandatoty
            else if (a_index == package_ID_index ||
                a_index == name_index ||
                a_index == address_index ||
                a_index == city_index ||
                a_index == state_index ||
                a_index == zipcode_index ||
                a_index == country_index)
            {
                num_missing_fields++;
                num_errors++;
                result = true;
            }
            else 
            { 
                result = false; 
            }
            return result;
        }
    }
}
