using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSV_checker
{
    public partial class frm_CSV_checker : Form
    {

        private string selected_file;
        private bool doNotClearTextbox = false;
        CSV_File input_file;

        public frm_CSV_checker()
        {
            //CSV_File input_file = new CSV_File();
       
            InitializeComponent();

            txtbox_selected_file.Text = "Please Select a file.";
            txtbox_file_errors.Text = "(any errors will be displayed here)";
        }

        //select the csv file to check
        private void btn_select_csv_Click(object sender, EventArgs e)
        {         
            //show the open file dialog box
            if (ofd_CVS_selector.ShowDialog() == DialogResult.OK)
            {
                //Assign the file to be checked
                txtbox_status.Clear();
                selected_file = ofd_CVS_selector.FileName;
                txtbox_selected_file.Text = selected_file;
                /*
                input_file = new CSV_File(ofd_CVS_selector.FileName);
                txtbox_selected_file.Text = input_file.fpath;*/
            }
        }
        
        private void txtbox_file_errors_TextChanged(object sender, EventArgs e)
        {
            //this.WriteLine("Hello, World!");
        }

        // Check the file for errors when the button is clicked.
        private void btn_check_errors_Click(object sender, EventArgs e)
        {
            List<string> format_report = new List<string>();
            List<string> sample_errors = new List<string>();
            List<string> invalid_zipcodes = new List<string>();
            bool[] added_sample_error_array = {false, false, false, false, false};
            string[,] error_descriptions = { {"Formating / General Data Error" , " does not have a valid format"}, 
                                             {"Special Character" , " has special characters"},
                                             {"Zipcode Error" , " is not a correctly formatted zipcode"},
                                             {"Record Too Long"," is over 40 characters"},
                                             {"Mandatory Field Missing"," This field cannot be blank"} };
            bool[] error_array = new bool[5]; //Stores the result from the multi error check
            string current_field;
            bool showSampleErrors = chkbox_sample_errors.Checked;

            List<string[]> badFomatRecords = new List<string[]>();
            List<string[]> specialCharacterRecords = new List<string[]>();
            List<string[]> badZipFormatRecords = new List<string[]>();
            List<string[]> tooLongRecords = new List<string[]>();
            List<string[]> missingFieldRecords = new List<string[]>();








            //if a file is selected, initialize the CSV_file object
            if (!String.IsNullOrWhiteSpace(selected_file))
            {
                input_file = new CSV_File(selected_file);
            }
            else
            {
                input_file = new CSV_File();
            }

            //check if initialized
            if( !input_file.is_initialized )
            {
                MessageBox.Show("No File Selected.");
                return;
            }


            ////////////////
            //////Starting to check for errors here
            //
            
            //clear the error text box
            if (!doNotClearTextbox)
            {
                txtbox_file_errors.Text = "";
            }
            doNotClearTextbox = !doNotClearTextbox;

            
            //check if the comma separated values are surrounded by quotes and display a message
            if (!input_file.has_quotes)
            {
                format_report.Add("WARNING: Values in file are not surrounded by quotes" + Environment.NewLine
                                  + "Missing of quotes may cause some checks to not work properly."
                                  + Environment.NewLine + Environment.NewLine);
            }

            //check if the header is incorrect and display a message
            if (!input_file.has_correct_header)
            {
                format_report.Add( "ERROR: Incorrect Header." + Environment.NewLine 
                                        + "Incorrect header may cause some checks to not work properly."
                                        + Environment.NewLine + Environment.NewLine );

                string title;
                int input_length = input_file.header_a.Length;
                int correct_length = input_file.correct_header_a.Length;
                
                //int loop_size = (input_length <= correct_length) ? input_length : correct_length;
                int index = 0;
                
                //case when correct header is larger or of equal size to header in file
                if ( correct_length >= input_length )
                {

                    while (index < input_length)
                    {
                        //temporarily rename field to "(Empty Field)" if blank
                        title = (String.IsNullOrWhiteSpace(input_file.header_a[index])) ? "(Empty Title)" : input_file.header_a[index];

                        if (title.Equals(input_file.correct_header_a[index], StringComparison.InvariantCultureIgnoreCase))
                        {
                            format_report.Add( "[" + index + "] : " + title + " - Correct." + Environment.NewLine);
                        }
                        else
                        {
                            format_report.Add( "-> [" + index + "] : '" + title + "' - Incorrect. The title should be: '" 
                                                + input_file.correct_header_a[index] + "'" + Environment.NewLine);
                        }

                        index++;
                    }
                    
                    while (index < correct_length)
                    {
                        //temporarily rename field to "(Empty Field)" if blank
                        //title = (String.IsNullOrWhiteSpace(input_file.header_a[index])) ? "(Empty Title)" : input_file.header_a[index];

                        format_report.Add( "-> [" + index + "] : field missing. The title should be: " 
                                                + input_file.correct_header_a[index] + Environment.NewLine);
                        index++;
                    }

                }
                //case when header in file has more fields than correct header.
                else
                {

                    while (index < correct_length)
                    {
                        //temporarily rename field to "(Empty Field)" if blank
                        title = (String.IsNullOrWhiteSpace(input_file.header_a[index])) ? "(Empty Title)" : input_file.header_a[index];

                        if (title.Equals(input_file.correct_header_a[index],StringComparison.InvariantCultureIgnoreCase))
                        {
                            format_report.Add("[" + index + "] : " + title + " - Correct." + Environment.NewLine);
                        }
                        else
                        {
                            format_report.Add("-> [" + index + "] : " + title + " - Incorrect. The title should be: "
                                                + input_file.correct_header_a[index] + Environment.NewLine);
                        }

                        index++;
                    } 
                    while(index < input_length)
                    {
                        //temporarily rename field to "(Empty Field)" if blank
                        title = (String.IsNullOrWhiteSpace(input_file.header_a[index])) ? "(Empty Title)" : input_file.header_a[index];

                        format_report.Add("-> [" + index + "] : " + title + " - Incorrect. " +
                                            "This is an extra item, or this item is misplaced." + Environment.NewLine);
                        index++;
                   }
                }
            }
            else
            {
                format_report.Add( "Header is correctly formatted." + Environment.NewLine);
            }


            ///
            /// Checking for Data Errors
            ///
                
            //line currently being read
            ulong line_num = 0;

            //open the csv file to be checked
            using (StreamReader reader = new StreamReader(input_file.fpath, Encoding.Default))
            {
                //read a line and store in memory as a string and as an array
                //reading the header , and do nothing
                input_file.current_line_s = reader.ReadLine();
                input_file.current_line_to_a();
                
                //read/check each line
                do
                {
                    //iterate the line number
                    line_num++;

                    //read the line, store in memory as string and array
                    input_file.current_line_s = reader.ReadLine();
                    input_file.current_line_to_a();

                    //display status in the status bar
                    txtbox_status.Text = "Checking Line: " + line_num.ToString() + "\t" + "Errors Found So Far: " + input_file.num_errors.ToString();
                    txtbox_status.Refresh();
                    
                    //check each field in the current line array
                    for (int index = 0; index < input_file.current_line_a.Length; index++)
                    {
                        //storing the field currently being read in memory
                        current_field = input_file.current_line_a[index];
                        
                        ///////////////////////////////////////////////
                        // THIS LINE CHECKS FOR ERRORS.
                        // copy the result from the error check
                        Array.Copy(input_file.multi_error_check(index, current_field), error_array, 5);
                        ///////////////////////////////////////////////
                        //Add sample errors, once for each type of error
                        for (int error_type = 0; error_type < 5; error_type++)
                        {
                            //Error types:
                            //0: Format
                            //1: Special Character
                            //2: Zipcode
                            //3: Length
                            //4: Missing mandatory field

                            // Option to add bad records to report.
                            if (showSampleErrors && error_array[error_type])
                            {
                                switch(error_type)
                                {
                                    case 0:
                                        if (input_file.num_bad_format == 1) badFomatRecords.Add(input_file.header_a);
                                        badFomatRecords.Add(input_file.current_line_a);
                                        break;
                                    case 1:
                                        if (input_file.num_illegal_chars == 1) specialCharacterRecords.Add(input_file.header_a);
                                        specialCharacterRecords.Add(input_file.current_line_a);
                                        break;
                                    case 2:
                                        if (input_file.num_bad_zipcodes == 1) badZipFormatRecords.Add(input_file.header_a);
                                        badZipFormatRecords.Add(input_file.current_line_a);
                                        break;
                                    case 3:
                                        if (input_file.num_over40_chars == 1) tooLongRecords.Add(input_file.header_a);
                                        tooLongRecords.Add(input_file.current_line_a);
                                        break;
                                    case 4:
                                        if (input_file.num_missing_fields == 1) missingFieldRecords.Add(input_file.header_a);
                                        missingFieldRecords.Add(input_file.current_line_a);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                    }

                    txtbox_file_errors.Refresh();

                } while (reader.Peek() != -1);
            }



            //////////////////////////////////////
            //Generating the Report
            /////

            //generating error report status message.
            txtbox_status.Text = "Generating Error Report. This may take a few moments.";
            txtbox_status.Refresh();


            txtbox_file_errors.Text += "***** NUMBER OF RECORDS IN FILE: " + line_num.ToString() + " *****" 
                                    + Environment.NewLine ;
            
            //success message
            if (input_file.num_errors == 0)
            {
                txtbox_file_errors.Text += "******* Number Of Errors: 0 *******" + Environment.NewLine;
                MessageBox.Show("No errors found.");
            }
            //error message
            else
            {
                txtbox_file_errors.Text += "******* Number Of Errors: " + input_file.num_errors + " *******" + Environment.NewLine;
                MessageBox.Show(input_file.num_errors + " errors found.");
            }
            
            //display a summary in the status box
            txtbox_status.Text = "Total Errors Found: " + input_file.num_errors.ToString()
                                + ((!input_file.has_correct_header) ? " | Incorrect Header " : "")
                                + " | " + input_file.num_bad_format + " general data errors(s)"
                                + " | " + input_file.num_illegal_chars + " special character(s)"
                                + "; " + input_file.num_bad_zipcodes + " are bad zip code(s)"
                                + " | " + input_file.num_over40_chars + " fields over 40 char(s)"
                                + " | " + input_file.num_missing_fields + " mandatory field(s) missing";
            txtbox_status.Refresh();


            txtbox_file_errors.Text += Environment.NewLine + "***** File Format Information: *****" + Environment.NewLine + Environment.NewLine;
            foreach (string item in format_report)
            {
                txtbox_file_errors.Text += item;
            }

            txtbox_file_errors.Text += Environment.NewLine + "***** Data Error Report: *****" + Environment.NewLine + Environment.NewLine;
            txtbox_file_errors.Text += input_file.num_bad_format + " item(s) with general data errors." + Environment.NewLine
                                     + input_file.num_illegal_chars + " field(s) with special character(s)." + Environment.NewLine
                                     + input_file.num_bad_zipcodes + " zip code(s) missing zeros or formatted wrong." + Environment.NewLine
                                     + input_file.num_over40_chars + " field(s) are over 40 char(s)." + Environment.NewLine
                                     + input_file.num_missing_fields + " mandatory field(s) missing." + Environment.NewLine;

            if (showSampleErrors)
            {
                txtbox_file_errors.Text += Environment.NewLine + "***** Sample Errors: *****" + Environment.NewLine + Environment.NewLine;
                if (input_file.num_errors == 0 || (input_file.num_errors == 1 && !input_file.has_correct_header))
                {
                    txtbox_file_errors.Text += "(No Errors to Show)";
                    txtbox_file_errors.Text += Environment.NewLine + Environment.NewLine;

                }
                else
                {
                    // Old method of printing report.
                    /* 
                    foreach (string item in sample_errors)
                    {
                        txtbox_file_errors.Text += item;
                        txtbox_file_errors.Text += Environment.NewLine + Environment.NewLine;
                    }
                    */

                    // Print sample Errors.
                    if(input_file.num_bad_format > 0)
                    {
                        txtbox_file_errors.Text += "***Records with Formatting Errors:***" + Environment.NewLine;
                        printList(badFomatRecords, 11 ,true);
                    }
                    if (input_file.num_illegal_chars > 0)
                    {
                        txtbox_file_errors.Text += "***Records with Illegal/Special Characters:***" + Environment.NewLine;
                        printList(specialCharacterRecords, 11, true);
                    }
                    if (input_file.num_bad_zipcodes > 0)
                    {
                        txtbox_file_errors.Text += "***Records with Bad Zip Code Formats:***" + Environment.NewLine;
                        printList(badZipFormatRecords, 11, true);
                    }
                    
                    if(input_file.num_over40_chars > 0)
                    {
                        txtbox_file_errors.Text += "***Records with Fields Over 40 Characters:***" + Environment.NewLine;
                        printList(tooLongRecords, 11, true);
                    }                    
                    if(input_file.num_missing_fields > 0)
                    {
                        txtbox_file_errors.Text += "***Records with Mandatory Fields Missing:***" + Environment.NewLine;
                        printList(missingFieldRecords, 11, true);
                    }
                }
            }

            //if (chkbox_zipcode_dbcheck.Checked)
            //{
            //    txtbox_file_errors.Text += Environment.NewLine + "***** List of Invalid Zipcodes: *****" + Environment.NewLine + Environment.NewLine;
            //    txtbox_file_errors.Text += "(first ten invalid zipcodes will be shown)" + Environment.NewLine;
            //    int loop = 0;
            //    foreach (string item in invalid_zipcodes)
            //    {
            //        loop++;
            //        txtbox_file_errors.Text += item;
            //        txtbox_file_errors.Text += Environment.NewLine;
            //        if (loop >= 10)
            //        {
            //            break;
            //        }
            //    }
            //}

        }



        private int zipCodeVerifier()
        {
            //if a file is selected, initialize the CSV_file object
            if (!String.IsNullOrWhiteSpace(selected_file))
            {
                input_file = new CSV_File(selected_file);
            }
            else
            {
                input_file = new CSV_File();
            }

            bool fileOK = verifyFileSelected(input_file);
            if (!fileOK) return -1;

            //clear the error text box
            if (!doNotClearTextbox)
            {
                txtbox_file_errors.Text = "";
            }
            doNotClearTextbox = !doNotClearTextbox;

            string line;
            bool hasLineReadErrors = false;
            int lineNum = 1;
            int numInvalidZips = 0;
            int numStateMismatches = 0;
            List<string[]> invalidZips = new List<string[]>();
            List<string[]> stateMismatches = new List<string[]>();
            List<string[]> lineReadErrors = new List<string[]>();
            string[] zipCheckArr;
            using(StreamReader reader = new StreamReader(selected_file))
            {
                line = reader.ReadLine();
                do
                {
                    txtbox_status.Text = String.Format("Checking Record #{0}. Invalid Zip Codes: {1}. Mismatched States/Zip Coes:{2}",
                                                       lineNum,
                                                       numInvalidZips,
                                                       numStateMismatches);
                    line = reader.ReadLine();
                    input_file.getLine(line);

                    zipCheckArr = input_file.VerifyZipcode(input_file.current_line_a);

                    // Check if CSV parsing worked.
                    if (String.IsNullOrEmpty(zipCheckArr[1]))
                    {
                        if (!hasLineReadErrors) lineReadErrors.Add(new string[] { "Line Number:", "Record:" });
                        hasLineReadErrors = true;
                        string[] unreadableLine = new string[input_file.current_line_a.Length + 1];
                        unreadableLine[0] = lineNum.ToString();
                        input_file.current_line_a.CopyTo(unreadableLine, 1);
                        lineReadErrors.Add(unreadableLine);
                        break;
                    }

                    // Check if zip code is valid.
                    if (!zipCheckArr[1].Equals("OK"))
                    {
                        // Get the zipcode, check if its blank, add it to the list of invalid zip codes.
                        string zipCode;
                        try
                        {
                            zipCode = input_file.current_line_a[input_file.zipcode_index];
                            zipCode = (String.IsNullOrWhiteSpace(zipCode)) ? "(blank)" : zipCode;
                        }
                        catch
                        {
                            zipCode = "Error Reading Zipcode";
                        }
                        invalidZips.Add(new String[2]{lineNum.ToString(), zipCode});
                        numInvalidZips++;
                    }
                    // Check if zip code matches state.
                    else if (!zipCheckArr[2].Equals("OK"))
                    {
                        // Get the zip code / stae, check if blank, add to list of mismathes.

                        string zipCode = input_file.current_line_a[input_file.zipcode_index];
                        string state = input_file.current_line_a[input_file.state_index];

                        zipCode = (String.IsNullOrWhiteSpace(zipCode)) ? "(blank)" : zipCode;
                        state = (String.IsNullOrWhiteSpace(state)) ? "(blank)" : state;

                        stateMismatches.Add(new String[4] { lineNum.ToString(), zipCode, state, zipCheckArr[2] });
                        numStateMismatches++;
                    }
                    lineNum++;
                    txtbox_status.Refresh();
                } while (reader.Peek() != -1);
            }


            // Print the report.
            if (numInvalidZips == 0 && !hasLineReadErrors)
            {
                txtbox_file_errors.Text += "No Invalid Zipcodes." + Environment.NewLine + Environment.NewLine;
            }
            else
            {
                txtbox_file_errors.Text += "****** Invalid Zipcodes ******" + Environment.NewLine + Environment.NewLine;
                txtbox_file_errors.Text += "Line Number:".PadRight(15) + "Invalid Zip Code:".PadRight(15) 
                                         + Environment.NewLine;
                printList(invalidZips);
            }

            if (numStateMismatches != 0)
            {
                txtbox_file_errors.Text += "****** State Mismatches ******" + Environment.NewLine + Environment.NewLine;
                txtbox_file_errors.Text += "Line Number:".PadRight(15)
                                            + "Zip Code:".PadRight(15)
                                            + "State Listed:".PadRight(15)
                                            + "State Expected:".PadRight(15)
                                            + Environment.NewLine;
                printList(stateMismatches);
            }
            if (hasLineReadErrors)
            {

                txtbox_file_errors.Text += "Some records could not be Interpreted."+ Environment.NewLine;
                printList(lineReadErrors);
            }
            txtbox_status.Text = String.Format("{0} invalid zip code(s) found.\t {1} Zip code / state mismatches found.",
                                                    numInvalidZips,
                                                    numStateMismatches);
            txtbox_status.Refresh();
            return numInvalidZips + numStateMismatches;
        }

        // printList() + overloads
        // Prints the specified numbner of items in a list in the error textbox w/ fixed width Default width = 15.
        private void printList(List<string[]> aList)
        {
            printList(aList, 25, 15);

        }
        private void printList(List<string[]> aList, int aMaxRecords, bool aFindWidth)
        {
            int[] widths;
            widths = allMaxLengths(aList,aMaxRecords);
            printList(aList, aMaxRecords, widths);
        }
        private void printList(List<string[]> aList, int aMaxRecords, int aWidth)
        {
            int loop = 0;
            int recordCount = aList.Count;
            foreach (string[] array in aList)
            {
                loop++;
                foreach (string item in array)
                {
                    txtbox_file_errors.Text += item.PadRight(aWidth);                    
                }
                txtbox_file_errors.Text += Environment.NewLine;
                if (loop >= aMaxRecords) break;
            }
            if (recordCount > aMaxRecords)
            {
                txtbox_file_errors.Text += "(" + (recordCount - aMaxRecords).ToString() + " more)" + Environment.NewLine;
            }
            txtbox_file_errors.Text += Environment.NewLine;
            txtbox_file_errors.Refresh();
        }
        private void printList(List<string[]> aList, int aMaxRecords, int[] aWidths)
        {
            int colNum;
            int recordCount = aList.Count;

            int loop = 0;
            foreach (string[] array in aList)
            {
                loop++;
                colNum = 0;
                foreach (string item in array)
                {
                    try
                    {
                        txtbox_file_errors.Text += item.PadRight(aWidths[colNum] + 3);
                    }
                    catch
                    {
                        txtbox_file_errors.Text += item.PadRight(40);
                    }
                    colNum++;
                }
                txtbox_file_errors.Text += Environment.NewLine;
                if (loop >= aMaxRecords) break;
            }
            if (recordCount > aMaxRecords)
            {
                txtbox_file_errors.Text += "(" + (recordCount - aMaxRecords).ToString() + " more)" + Environment.NewLine;
            }
            txtbox_file_errors.Text += Environment.NewLine;
            txtbox_file_errors.Refresh();
        }

        // maxLength( ... )
        // Find the largest width of an entry in a List<string[]> withing the first specified number of records.
        private int maxLength(List<string[]> aList, int aCutoff)
        {
            int max = 0;    
            string[] arr;

            // Set cutoff to the length of list if the list is smaller than the cutoff.
            aCutoff = (aCutoff < aList.Count) ? aCutoff : aList.Count;
            for (int loop = 0; loop < aCutoff; ++loop)
            {
                arr = aList[loop];
                foreach (string item in arr)
                {
                    max = (item.Length > max) ? item.Length : max;
                }
            }
            return max;
        }

        // allMaxLengths( ... )
        // Return the largest widths in each col of a list of string arrays as in int array
        private int[] allMaxLengths(List<string[]> aList, int aCutoff)
        {
            // Declare max size array as same size as the number of cols, initialize to 1.
            int[] maxima = new int[aList[0].Length];
            for (int loop = 0; loop < maxima.Length; ++loop) maxima[loop] = 1;
            
            string[] arr;

            // Set cutoff to the length of list if the list is smaller than the cutoff.
            aCutoff = (aCutoff < aList.Count) ? aCutoff : aList.Count;
            for (int line = 0; line < aCutoff; ++line)
            {
                arr = aList[line];
                for (int colLoop = 0; colLoop < maxima.Length; ++colLoop)
                {
                    try
                    {
                        maxima[colLoop] = (maxima[colLoop] < arr[colLoop].Length) ? arr[colLoop].Length : maxima[colLoop];
                    }
                    catch
                    {
                        maxima[colLoop] = 40;
                    }
                }
                /*
                foreach (string item in arr)
                {
                    maxima = (item.Length > max) ? item.Length : max;
                }*/
            }
            return maxima;

        }

        // Prints a string array in the error textbox w/ tab separated values.
        private void printArray(string[] aArray)
        {
            
            foreach (string detail in aArray )
            {
                txtbox_file_errors.Text += detail + "\t\t";
            }
            txtbox_file_errors.Text += Environment.NewLine;

        }

        //private string spaces(int numSpaces)
        //{
        //    string spaceStr = "";
        //    spaceStr.PadRight(numSpaces);
        //    return spaceStr;
        //}

        // Checks if file selected and exists.
        private bool verifyFileSelected(CSV_File aInput_file)
        {
            //check if initialized
            if (!aInput_file.is_initialized)
            {
                MessageBox.Show("No File Selected.");
                return false;
            }
            else if (!File.Exists(selected_file))
            {
                MessageBox.Show("Selected file does not exist.");
                return false;

            }
            return true;

        }
        // Display a notice once a file is selected.
        private void txtbox_selected_file_TextChanged(object sender, EventArgs e)
        {
            //txtbox_file_errors.Text = "(any errors will be displayed here)";

        }

        private void btnVerifyZips_Click(object sender, EventArgs e)
        {
            int numErrors = zipCodeVerifier();
            string message;

            if (numErrors == 0) message = "No zip code issues found!";
            else if (numErrors == -1) message = "Could not open File!";
            else  message = String.Format("{0} zip code issues found.", numErrors);

            MessageBox.Show(message);
        }

        private void btnClearWindow_Click(object sender, EventArgs e)
        {
            txtbox_file_errors.Text = "";
        }

        // Uncomment this and the event declaration to show the warning Message Box
        // Do somerthing once the "Check Zip Codes Against List of Valid Zip Codes" box is checked.
        //private void chkbox_zipcode_dbcheck_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (chkbox_zipcode_dbcheck.Checked)
        //    {
        //        MessageBox.Show("Note: This option may cause slower performance.");
        //    }
        //}
    }

}
