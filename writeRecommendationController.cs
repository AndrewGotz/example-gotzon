/*Written by: Andrew Gotzon
Date: 14-JULY-2017
Description: A asp.net example of posting information regarding if the recommendation submitted is a valid submission
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Microsoft.CSharp.RuntimeBinder;

namespace com.gotzon.SBU
{
    public class writeRecommendationController : ApiController
    {
        //Post Method to get the results from body and turn them into a list of strings to be processed
        public IHttpActionResult RecommendationPost([FromBody]dynamic value)
        {
            //Grabbing variables dynamically from the post API call in Angular to process
            var email = value.email.Value;
            databaseConnection con = new databaseConnection();
            List<string> l = new List<string>();

            //Method to check to see if the email submitted is a true employees email
            l = con.checkEmployee(email, 0);


            //check to see if email is an actual employee's email
            if (l[0] == "")
                return Ok("notemployee");

            //Grabs the employees email from the "single-sign-on" text file
            string resource_data = com.gotzon.SBU.Properties.Resources.userInformation;
            List<string> creds = resource_data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            string empEmail = creds[2];

            if (empEmail.Equals(email))
                return Ok("sameemployee");

            var lastname = value.last.Value;
            var description = value.description.Value;
            var values = value.values.Value;
            //Calling writeRec function to write the recommendation to the DB
            writeRec write = new writeRec();
            write.executeWrite(email, description, values);

            singleSignOn sign = new singleSignOn();
            var check = sign.executeStoredProcedure();
            //Test to see if post runs effectivly.
            if (check == 0)
            {
                return Ok("http://localhost:51463/EmployeeIndex.html#/form/begin");
            }
            else
            {
                return Ok("http://localhost:51463/ManagerIndex.html#/form/begin");
            }
        }
    }
}