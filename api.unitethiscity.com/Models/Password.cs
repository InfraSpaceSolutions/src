/******************************************************************************
* Filename: Password.cs
* Project:  api.unitethiscity.com
* 
* Description:
* Enforce password standards and support random password generation.
******************************************************************************/
using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace com.unitethiscity.api.Models
{
    /// <summary>
    /// Static class to support password utility functions
    /// </summary>
    public static class Password
    {
        /// <summary>
        /// GenerateRandom
        /// </summary>
        /// <param name="len">Length of the password in # of characters</param>
        /// <returns>A random password</returns>
        public static string GenerateRandom(int len)
        {
            string password = "";
            Random rand = new Random();

            // Create the array of accepted characters.  Skip I's, L's and 1's to avoid "confusion"
            char[] characters = new char[34] 
			{ 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i',
			  'j', 'k', 'm', 'n', 'o', 'p', 'q', 'r',
			  's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
			  '0', '2', '3', '4', '5', '6', '7', '8', '9'
			};

            // Generate the password
            for (int i = 0; i < len; i++)
            {
                password += characters[rand.Next(0, 34)];
            }

            return password;
        }

        /// <summary>
        /// Generate a random password specifying the seed. Useful for generating multiple passwords at the same time.
        /// </summary>
        /// <param name="len">Length of the password in # of characters</param>
        /// <param name="seed">Seed number to generate against</param>
        /// <returns>A random password</returns>
        public static string GenerateRandom(int len, int seed)
        {
            string password = "";
            Random rand = new Random(seed);

            // Create the array of accepted characters.  Skip I's, L's and 1's to avoid "confusion"
            char[] characters = new char[34] 
			{ 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i',
			  'j', 'k', 'm', 'n', 'o', 'p', 'q', 'r',
			  's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
			  '0', '2', '3', '4', '5', '6', '7', '8', '9'
			};

            // Generate the password
            for (int i = 0; i < len; i++)
            {
                password += characters[rand.Next(0, 34)];
            }

            return password;
        }

        /// <summary>
        /// Expunge the characters of a password when writing to the screen
        /// </summary>
        /// <param name="passlength">Character count of the password</param>
        /// <returns>The expunged password</returns>
        public static string Expunge(int passlength)
        {
            string password = "";

            for (int i = 0; i < passlength; i++)
            {
                password += "*";
            }

            return password;
        }
    }
}