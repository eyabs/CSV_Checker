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
            {//Assign the file to be checked
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

        //check the file for errors when the button is clicked
        private void btn_check_errors_Click(object sender, EventArgs e)
        {
            List<string> format_report = new List<string>();
            List<string> data_report = new List<string>();

            string current_field;
            CSV_File input_file;
            
            //if a file is selected, initialize the CSV_file object
            if (!String.IsNullOrWhiteSpace(selected_file))
                input_file = new CSV_File(selected_file);
            else
                input_file = new CSV_File();

            //check if initialized
            if( !input_file.is_initialized )
            {
                MessageBox.Show("No File Selected.");
                return;
            }
           
            //check for errors

            //MessageBox.Show("Checking file for errors.");

            //clear the error text box
            txtbox_file_errors.Text = "";

            
            //check if the comma separated values are surrounded by quotes and displat a message
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

                        if (title == input_file.correct_header_a[index])
                        {
                            format_report.Add( "[" + index + "] : " + title + " - Correct." + Environment.NewLine);
                        }
                        else
                        {
                            format_report.Add( "-> [" + index + "] : " + title + " - Incorrect. The title should be: " 
                                                + input_file.correct_header_a[index] + Environment.NewLine);
                        }

                        index++;
                    }
                    
                    while (index < correct_length)
                    {
                        //temporarily rename field to "(Empty Field)" if blank
                        title = (String.IsNullOrWhiteSpace(input_file.header_a[index])) ? "(Empty Title)" : input_file.header_a[index];

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

                        if (title == input_file.correct_header_a[index])
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

            //declare an array to store the array from the multi error checker
            bool[] error_array = new bool[3];

            //open the csv file to be checked
            StreamReader reader = new StreamReader(input_file.fpath);

            //read a line and store in memory as a string and as an array
            input_file.current_line_s = reader.ReadLine();
            input_file.current_line_to_a();

            //line currently being read
            ulong line_num = 1;
            
            //read/check each line
            do
            {
                //read the line, store in memory
                input_file.current_line_s = reader.ReadLine();
                input_file.current_line_to_a();
                
                //display status in the status bar
                txtbox_status.Text = "Checking Line: " + line_num.ToString() + "\t" + "Errors Found So Far: " + input_file.num_errors.ToString();
                txtbox_status.Refresh();
                
                //check each field in the current line array
                for (int index = 0; index < input_file.current_line_a.Length; index++ )
                {
                    //storing the field currently being read in memory
                    current_field = input_file.current_line_a[index];
                    
                    //copy the result from the error check
                    Array.Copy(input_file.multi_error_check(index, current_field), error_array, 3);

                    //display error mesage if field has a special character
                    if ( error_array[0] )
                    {
                        data_report.Add( "ERROR on entry #" + line_num + ", field: \"" + input_file.header_a[index] +
                                         ".\" Field has a special character, or formatted incorrectly" + Environment.NewLine +
                                         "(" + input_file.header_a[index] + ": " + input_file.current_line_a[index] + ")" +
                                         Environment.NewLine + Environment.NewLine );

                    }

                    //display error message if the field is over 40 chars
                    if (error_array[1])
                    {
                        data_report.Add( "ERROR on entry #" + line_num + ", field: \"" + input_file.header_a[index] + 
                                         ".\" Field has over 40 characters." + Environment.NewLine +
                                         "(" + input_file.header_a[index] + ": " + current_field + ")" +
                                         Environment.NewLine + Environment.NewLine );
                    }

                    //display an error message if the field is missing and mandatory
                    if (error_array[2])
                    {
                        data_report.Add( "ERROR on entry #" + line_num + ", field: \"" + input_file.header_a[index] + 
                                         ".\" Mandatory field is missing." + Environment.NewLine + Environment.NewLine );
                    }
                    
                }

               
                //iterate the line number
                line_num++;
                txtbox_file_errors.Refresh();

            } while (reader.Peek() != -1);

            //////////////////////////////////////
            //Generating the Report
            /////

            //close the input csv
            reader.Close();

            //generating error report status message.
            txtbox_status.Text = "Generating Error Report. This may take a few moments.";
            txtbox_status.Refresh();
            
            //success message
            if (input_file.num_errors == 0)
            {
                txtbox_file_errors.Text = "******* Number Of Errors: 0 *******" + Environment.NewLine;
                MessageBox.Show("No errors found.");
            }
            //error message
            else
            {
                txtbox_file_errors.Text = "******* Number Of Errors: " + input_file.num_errors + " *******" + Environment.NewLine;
                MessageBox.Show(input_file.num_errors + " errors found.");
            }
            
            //display a summary in the status box
            txtbox_status.Text = "Total Errors Found: " + input_file.num_errors.ToString()
                                + ((!input_file.has_correct_header) ? " | Incorrect Header " : "")
                                + " | " + input_file.num_bad_format + " format errors(s)"
                                + " | " + input_file.num_illegal_chars + " special character(s)"
                                + "; " + input_file.num_bad_zipcodes + " are bad zip code(s)"
                                + " | " + input_file.num_over40_chars + " fields over 40 char(s)"
                                + " | " + input_file.num_missing_fields + " mandatory field(s) missing";
            txtbox_status.Refresh();
            
            
            txtbox_file_errors.Text += Environment.NewLine + "***** File Format Information: *****" + Environment.NewLine;
            foreach (string item in format_report)
            {
                txtbox_file_errors.Text += item;
            }

            txtbox_file_errors.Text += Environment.NewLine + "***** Data Errors: *****" + Environment.NewLine;
            txtbox_file_errors.Text += input_file.num_bad_format + " item(s) formatted wrong." + Environment.NewLine
                                     + input_file.num_illegal_chars + " field(s) with special character(s)." + Environment.NewLine
                                     + input_file.num_bad_zipcodes + " zip code(s) missing zeros or formatted wrong." + Environment.NewLine
                                     + input_file.num_over40_chars + " field(s) are over 40 char(s)." + Environment.NewLine
                                     + input_file.num_missing_fields + " mandatory field(s) missing." + Environment.NewLine;

            /*
            txtbox_file_errors.Text += Environment.NewLine + "***** Data Errors: *****" + Environment.NewLine;
            foreach (string item in data_report)
            {
                txtbox_file_errors.Text += item;
            }*/
        }

        private void txtbox_selected_file_TextChanged(object sender, EventArgs e)
        {
            txtbox_file_errors.Text = "(any errors will be displayed here)";

        }

        private void lbl_line_num_Click(object sender, EventArgs e)
        {

        }
    }

}
