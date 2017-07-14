/*
Created by: Andrew Gotzon's Senior Project Team 2017
Date last updated: 2/10/2017
Description: A class which makes a connection to the peer recognition DB to retrieve user credentials
of either a manager or employee.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text;

namespace com.gotzon.SBU
{
    public class databaseConnection
    {
        private SqlConnection con;
        public databaseConnection()
        {
            //Pulling login information to connection string from a configurable file
            string name = ConfigurationManager.AppSettings["userid"];
            string pass = ConfigurationManager.AppSettings["password"];
            string server = ConfigurationManager.AppSettings["server"];
            string db = ConfigurationManager.AppSettings["database"];

            //Passing the connection string to the object.
            con = new SqlConnection("user id =" + name + ";password=" + pass + ";" +
                                       "server=" + server + ";" +
                                       "Trusted_Connection=False;Encrypt=True;" +
                                       "database=" + db + "; " +
                                       "connection timeout=30");
            
            
        }//End of Constructor


        //SQL stored procedure to retrieve user credentials from Peer Recognition DB
        public List<string>  checkEmployee(string email, int checkMang)
        {
            //Initalizing variables.
            SqlCommand cmd = null;
            List<string> lst = new List<string>();


            try
            {
                //Opening connection to Database
                con.Open();
                
                //Assigning the SQL command the stored procedure
                cmd = new SqlCommand("[dbo].[CheckEmployee]", con);
                cmd.CommandType = CommandType.StoredProcedure;


                //Adding both the input and output parameters of our Stored proc
                cmd.Parameters.Add("@email", SqlDbType.VarChar, 255).Value = email;
                cmd.Parameters.Add("@mangNum", SqlDbType.Int).Value = checkMang;
                cmd.Parameters.Add("@returnEmail", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@returnDeptName", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;

                //Executing stored proc
                cmd.ExecuteNonQuery();
                
                //checking to see if either output variables return null
                if (cmd.Parameters["@returnEmail"].Value == null && cmd.Parameters["@returnDeptName"].Value == null)
                {
                    
                    //If null close connection to DB and return the empty list
                    con.Close();
                    return lst;
                 }
                 else
                 {

                    //Else return the output params as strings and add them to the list to return
                    string returnEmail = cmd.Parameters["@returnEmail"].Value.ToString();
                    string returnDeptName = cmd.Parameters["@returnDeptName"].Value.ToString();
                    lst.Add(returnDeptName);
                    lst.Add(returnEmail);
                    return lst;
                 }//End of if statement

            }//End of try block
            
            //Finally, check to see if the connection was never made, if so close the connection and throw exception
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
         
            }//End of finally block
            
        }//End of stored proc method

        //Method to retrieve the Managers email of the employee using the web app
        public string getManagersEmail(string email)
        {
            //Initalizing variables.
            SqlCommand cmd = null;
            string mangEmail = "";
            try
            {
                //Opening connection to Database
                con.Open();

                //Assigning the SQL command the stored procedure
                cmd = new SqlCommand("[dbo].[getManagerEmail]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //Adding both the input and output parameters of our Stored proc
                cmd.Parameters.Add("@emp_email_ID", SqlDbType.VarChar, 255).Value = email;
                cmd.Parameters.Add("@dm_email_ID", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;

                //Executing stored proc
                cmd.ExecuteNonQuery();

                //checking to see if either output variables return null
                if (cmd.Parameters["@dm_email_ID"].Value == null)
                {

                    //If null close connection to DB and return the empty list
                    con.Close();
                    return mangEmail;
                }
                else
                {

                    //Else return the output params as strings and add them to the list to return
                    mangEmail = cmd.Parameters["@dm_email_id"].Value.ToString();
                    return mangEmail;
                }
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }//End of finally block
        }
        
        //Method to write the Recommendation to the DB whenever executed
        public void writeRecom(string recemail, string recommendee, string detail, string value, string descript, string mang)
        {
            //Initalizing variables.
            SqlCommand cmd = null;
            try
            {
                //Opening connection to Database
                con.Open();

                //Assigning the SQL command the stored procedure
                cmd = new SqlCommand("[dbo].[WriteRecommendation]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //Adding both the input and output parameters of our Stored proc
                cmd.Parameters.Add("@recomID", SqlDbType.VarChar, 255).Value = recemail;
                cmd.Parameters.Add("@recommenID", SqlDbType.VarChar, 255).Value = recommendee;
                cmd.Parameters.Add("@statusDetail", SqlDbType.VarChar, 10).Value = detail;
                cmd.Parameters.Add("@recVals", SqlDbType.VarChar, 255).Value = value;
                cmd.Parameters.Add("@recDes", SqlDbType.VarChar, 750).Value = descript;
                cmd.Parameters.Add("@dmID", SqlDbType.VarChar, 255).Value = mang;
                
                //Executing stored proc
                cmd.ExecuteNonQuery();

                //checking to see if either output variables return null
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }//End of finally block
        }//End Method
       
        public Recommendation getRecommendationQuery(int rID)
        {
            Recommendation rec = new Recommendation();
            //Initalizing variables.
            SqlCommand cmd = null;
            try
            {
                //Opening connection to Database
                con.Open();

                //Assigning the SQL command the stored procedure
                cmd = new SqlCommand("[dbo].[getRecommendations]", con);
                cmd.CommandType = CommandType.StoredProcedure;
                //Adding both the input and output parameters of our Stored proc
                cmd.Parameters.Add("@rID", SqlDbType.Int).Value = rID;
                cmd.Parameters.Add("@rendersFirstName", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@rendersLastName", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@rendeesFirstName", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@rendeesLastName", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@date", SqlDbType.DateTime).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@values", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Description", SqlDbType.VarChar, 750).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@comment", SqlDbType.VarChar, 255).Direction = ParameterDirection.Output;
                //Executing stored proc
                cmd.ExecuteNonQuery();

                rec.RecommendersFirstName = cmd.Parameters["@rendersFirstName"].Value.ToString();
                rec.RecommendersLastName = cmd.Parameters["@rendersLastName"].Value.ToString();
                rec.RecommendeesFirstName = cmd.Parameters["@rendeesFirstName"].Value.ToString();
                rec.RecommendeesLastName = cmd.Parameters["@rendeesLastName"].Value.ToString();
                rec.Values = cmd.Parameters["@values"].Value.ToString();
                string[] token = cmd.Parameters["@date"].Value.ToString().Split(' ');
                rec.Date = token[0];
                rec.Description = cmd.Parameters["@Description"].Value.ToString();
                rec.comment = cmd.Parameters["@comment"].Value.ToString();
                return rec;
            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }//End of finally block
        }
        public List<Recommendation> getPendingRecommendations()
        {
            
            //Initalizing variables.
            SqlCommand cmd = null;
            SqlDataReader rdr = null;

            List<Recommendation> list = new List<Recommendation>();
            string resource_data = com.gotzon.SBU.Properties.Resources.userInformation;
            List<string> creds = resource_data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            string emailAddress = creds[2];
           

            try
            {
                //Opening connection to Database
                con.Open();

                //Assigning the SQL command the stored procedure
                cmd = new SqlCommand("[dbo].[getPendingRecs]", con);


                //Adding the input parameter of the stored procedure
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@dm_email_id", SqlDbType.VarChar, 255).Value = emailAddress;
                

                //Execution of Stored Procedure
                cmd.ExecuteNonQuery();
                

                //Using SqlDataReader to read in information from the stored procedure on a row by row basis
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    //Create recommendation object
                    Recommendation pendingRecommendation = new Recommendation();

                    //Get info from the database
                    int rID = rdr.GetInt32(0);    
                    string emp_first_name = rdr.GetString(1);  
                    string emp_last_name = rdr.GetString(2);
                   

                    //Setting variables from the Recommendation object as information from database
                    pendingRecommendation.rID = rID;
                    pendingRecommendation.RecommendeesFirstName = emp_first_name;
                    pendingRecommendation.RecommendeesLastName = emp_last_name;
                   

                    //Adding the Recommendation to the list
                    list.Add(pendingRecommendation);
                }

                //Returing the list
                return list;
            }

            //Finally closing connection
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }//End of finally block

        }



            
        

        //Method to update database with the approval/declination of a recommendation
        public void updateRec(int rID, string update, string comment)
        {
            SqlCommand cmd = null;
            try
            {
                con.Open();
                cmd = new SqlCommand("[dbo].[updateRec]", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@rID", SqlDbType.Int).Value = rID;
                cmd.Parameters.Add("@update", SqlDbType.VarChar, 255).Value = update;
                cmd.Parameters.Add("@comment", SqlDbType.VarChar, 255).Value = comment;
                cmd.ExecuteNonQuery();
                //Opening connection to Database
                


            }
            finally
            {
                if (con != null)
                {
                    con.Close();
                }
            }
        }

        //Method to return the recommendations searched for by a manager
        public List<Recommendation> searchRec(string firstName, string lastName, string status, int recommendingordee)
        {
            //Initalizing variables.
            SqlCommand cmd = null;
            SqlDataReader rdr = null;

            string resource_data = com.gotzon.SBU.Properties.Resources.userInformation;
            List<string> creds = resource_data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            string emailAddress = creds[2];

            List<Recommendation> lst = new List<Recommendation>();
            //lst = null;
            try
            {
                //Opening connection to Database
                con.Open();

                //Assigning the SQL command the stored procedure
                cmd = new SqlCommand("[dbo].[SearchRecommendation]", con);
                

                //Adding the input parameter of the stored procedure
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@firstname", SqlDbType.VarChar, 255).Value = firstName;
                cmd.Parameters.Add("@lastname", SqlDbType.VarChar, 255).Value = lastName;
                cmd.Parameters.Add("@status", SqlDbType.VarChar, 10).Value = status;
                cmd.Parameters.Add("@RecdeeOrDer", SqlDbType.Int).Value = recommendingordee;
                cmd.Parameters.Add("@managerID", SqlDbType.VarChar, 255).Value = emailAddress;

                
                //Execution of Stored Procedure
                cmd.ExecuteNonQuery();

                //Using SqlDataReader to read in information from the stored procedure on a row by row basis
                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    //Create recommendation object
                    Recommendation searchedRec = new Recommendation();

                    //Get info from the database and store in string
                    int rID = Int32.Parse(rdr.GetSqlValue(0).ToString());
                    string rendersFirstName = rdr.GetString(1);
                    string rendersLastName = rdr.GetString(2);
                    string rendeesFirstName = rdr.GetString(3);
                    string rendeesLastName = rdr.GetString(4);
                    string date =  rdr.GetSqlValue(5).ToString();
                    string values = rdr.GetString(6);
                    string Description = rdr.GetString(7);
                    string comment = rdr.GetString(8);

                    searchedRec.rID = rID;
                    searchedRec.RecommendersFirstName = rendersFirstName;
                    searchedRec.RecommendersLastName = rendersLastName;
                    searchedRec.RecommendeesFirstName = rendeesFirstName;
                    searchedRec.RecommendeesLastName = rendeesLastName;
                    searchedRec.Date = date;
                    searchedRec.Values = values;
                    searchedRec.Description = Description;
                    searchedRec.comment = comment;

                    lst.Add(searchedRec);
                }
                //Returing the list
                return lst;
            }
            //Finally closing connection
            finally
            {
                if (con != null)
                {
                    con.Close();
                }

            }//End of finally block

        }//End of Method




    }//End of class

}//End of namespace