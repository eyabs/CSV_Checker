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

            //setting whether to check zipcodes against the database
            input_file.check_zipdb = chkbox_zipcode_dbcheck.Checked;

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
            using (StreamReader reader = new StreamReader(input_file.fpath))
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
                        //Error types:
                        //0: Format
                        //1: Special Character
                        //2: Zipcode
                        //3: Length
                        //4: Missing mandatory field

                        //storing the field currently being read in memory
                        current_field = input_file.current_line_a[index];


                        // THIS LINE CHECKS FOR ERRORS.
                        // copy the result from the error check
                        Array.Copy(input_file.multi_error_check(index, current_field), error_array, 5);

                        //Add sample errors, once for each type of error
                        for (int error_type = 0; error_type < 4; error_type++)
                        {
                            if (error_type == 2 && error_array[error_type])
                            {
                                invalid_zipcodes.Add(current_field);
                            }

                            //add sample errors for the errors sharing a report style
                            if (!added_sample_error_array[error_type] && error_array[error_type])
                            {
                                sample_errors.Add(String.Format("{0}, on line {1}, in the '{2}' column. '{3}' {4}.",
                                                   error_descriptions[error_type, 0],
                                                   line_num.ToString(),
                                                   input_file.header_a[index],
                                                   current_field,
                                                   error_descriptions[error_type, 1]));

                                added_sample_error_array[error_type] = true;

                            }
                            //add sample error for mandatory field missing
                            //this error has a distinct report style, so it has to be separated
                            if (!added_sample_error_array[4] && error_array[4])
                            {
                                sample_errors.Add(String.Format("{0}, on line {1}, in the '{2}' column. {3}.",
                                                      error_descriptions[4, 0],
                                                      line_num.ToString(),
                                                      input_file.header_a[index],
                                                      error_descriptions[4, 1]));
                                added_sample_error_array[4] = true;

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

            if (chkbox_sample_errors.Checked)
            {
                txtbox_file_errors.Text += Environment.NewLine + "***** Sample Errors: *****" + Environment.NewLine + Environment.NewLine;
                foreach (string item in sample_errors)
                {
                    txtbox_file_errors.Text += item;
                    txtbox_file_errors.Text += Environment.NewLine + Environment.NewLine;
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
            int lineNum = 1;
            int numInvalidZips = 0;
            int numStateMismatches = 0;
            List<string[]> invalidZips = new List<string[]>();
            List<string[]> stateMismatches = new List<string[]>();
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

                    // Check if zip code is valid.
                    if (!zipCheckArr[1].Equals("OK"))
                    {
                        // Get the zipcode, check if its blank, add it to the list of invalid zip codes.
                        string zipCode = input_file.current_line_a[input_file.zipcode_index];
                        zipCode = (String.IsNullOrWhiteSpace(zipCode)) ? "(blank)" : zipCode;
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
            if (numInvalidZips == 0)
            {
                txtbox_file_errors.Text += "No Invalid Zipcodes." + Environment.NewLine + Environment.NewLine;
            }
            else
            {
                txtbox_file_errors.Text += "****** Invalid Zipcodes ******" + Environment.NewLine + Environment.NewLine;
                txtbox_file_errors.Text += "Line Number:\tInvalid Zip Code:"+ Environment.NewLine;
                printList(invalidZips);
            }

            if (numStateMismatches != 0)
            {
                txtbox_file_errors.Text += "****** State Mismatches ******" + Environment.NewLine + Environment.NewLine;
                txtbox_file_errors.Text += "Line Number:\t"
                                            + "Zip Code:\t\t"
                                            + "State From File:\t" 
                                            + "State Expected:"
                                            + Environment.NewLine;
                printList(stateMismatches);
            }
            txtbox_status.Text = String.Format("{0} invalid zip code(s) found.\t {1} Zip code / state mismatches found.",
                                                    numInvalidZips,
                                                    numStateMismatches);
            txtbox_status.Refresh();
            return numInvalidZips + numStateMismatches;
        }
        
        // Prints the first 25 items in a list of string arrays w/ tab sepatated values
        private void printList(List<string[]> aList)
        {
            printList(aList, 25);

        }

        // Overload
        // Prints the specified numbner of items in a list in the error textbox w/ tab separated values.
        private void printList(List<string[]> aList, int aMax)
        {
            int loop = 0;
            foreach (string[] array in aList)
            {
                loop++;
                foreach (string item in array)
                {
                    if (loop <= aMax) txtbox_file_errors.Text += item + "\t\t";                    
                }
            txtbox_file_errors.Text += Environment.NewLine;
            }
            if (loop > aMax)
            {
                txtbox_file_errors.Text += "(" + (loop - 25).ToString() + " more)" + Environment.NewLine;
            }
            txtbox_file_errors.Text += Environment.NewLine;
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
