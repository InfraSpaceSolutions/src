/******************************************************************************
* Filename: CreditCardUtility.cs
* Project:  UTC
* 
* Description:
* Credit card utility functions. Provides a general credit card test class 
* along with static methods for performing general tests.
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace com.unitethiscity.api.Models
{
    /// <summary>
    /// A collection of utilities for dealing with credit cards
    /// </summary>
    public class CreditCardUtility
    {
        public enum CardTypes { Unknown = 0, Visa, MasterCard, American_Express, Discover };
        public enum CardResults { OK = 0, InvalidNumber, InvalidNumberForType, InvalidLUHN, InvalidExpiration, InvalidExpired, Fault };
        private static Regex digitsOnly = new Regex(@"[^\d]");

        // credit card validation regular expressions by card type found on the web, checks the
        // prefix and the number of characters to validate the card number is of the card type,
        // detections should be in the order of the specified card types
        // Visa - prefixes = 41-49; length 13 or 16
        private static Regex visaCheck = new Regex(@"^4[0-9]{12}(?:[0-9]{3})?$");
        // MasterCard - prefixes = 51-55; length 16
        private static Regex masterCardCheck = new Regex(@"^5[1-5][0-9]{14}$");
        // Discover - prefix 6011 or 65; length 16
        private static Regex discoverCheck = new Regex(@"^6(?:011|5[0-9]{2})[0-9]{12}$");
        // American Express - prefix 34 or 37; 15 digits
        private static Regex amExCheck = new Regex(@"^3[47][0-9]{13}$");

        /// <summary>
        /// Credit card number for checking
        /// </summary>
        protected string cardNumber;
        public string CardNumber
        {
            get
            {
                return cardNumber;
            }
            set
            {
                cardNumber = CleanCardNumber(value);
            }
        }

        /// <summary>
        /// Credit card expiration month number 1-12
        /// </summary>
        public int CardExpMonth
        {
            get;
            set;
        }

        /// <summary>
        /// Credit card expiration year - should be a four digit year
        /// </summary>
        public int CardExpYear
        {
            get;
            set;
        }

        /// <summary>
        /// Credit card type - note: this can be inferred from number
        /// </summary>
        public CardTypes CardType
        {
            get;
            set;
        }

        /// <summary>
        /// Create a credit card utility object for checking the properties with
        /// default values.
        /// </summary>
        public CreditCardUtility()
        {
            CardNumber = "";
            CardExpMonth = 0;
            CardExpYear = 0;
        }

        /// <summary>
        /// Create a credit card utility object for checking the properties parametrically
        /// </summary>
        /// <param name="accountNumber">card number as a string</param>
        /// <param name="expMonth">expiration month 1-12</param>
        /// <param name="expYear">expiration year </param>
        /// <param name="ct"></param>
        public CreditCardUtility(string accountNumber, int expMonth, int expYear, CardTypes ct = CardTypes.Unknown)
        {
            CardNumber = accountNumber;
            CardExpMonth = expMonth;
            CardExpYear = expYear;
            CardType = ct;
        }

        /// <summary>
        /// Check all of the credit card information assigned to the object. The following checks are performed:
        /// - card number is valid for card type or can be detected
        /// - card number passes LUHN test
        /// - expiration date is valid
        /// - card has not yet expired
        /// </summary>
        /// <param name="detectCardType">false=use assigned card type; true=attempt to detect card type</param>
        /// <returns>CardResults error code; OK for no error</returns>
        public CardResults Check(bool detectCardType = false)
        {
            // detect or validate the card type
            if (detectCardType)
            {
                // attempt to infer the card type from the number
                CardType = DetectCardType(CardNumber);
                if (CardType == CardTypes.Unknown)
                {
                    return CardResults.InvalidNumber;
                }
            }
            else
            {
                // check to see if the number and card type are a match
                if (!CheckCardType(CardNumber, CardType))
                {
                    return CardResults.InvalidNumberForType;
                }
            }

            // perform the LUHN check on the number
            if (!CheckLUHN(CardNumber))
            {
                return CardResults.InvalidLUHN;
            }

            // see if the credit card is expired
            return CheckExpiration(CardExpMonth, CardExpYear);
        }

        /// <summary>
        /// Get the card number in an expunged format
        /// </summary>
        /// <returns>expunged version of card number</returns>
        public string ExpungedCardNumber()
        {
            return ExpungeCardNumber(CardNumber);
        }

        /// <summary>
        /// Convert a card type to a displayable string
        /// </summary>
        /// <param name="cardType">card type identifier</param>
        /// <returns>string version of card type</returns>
        public static string CardTypeName(CardTypes cardType)
        {
            string cardTypeName = cardType.ToString();
            cardTypeName.Replace('_', ' ');
            return cardTypeName;
        }

        /// <summary>
        /// Perform the LUHN check on the credit card. 
        /// </summary>
        /// <param name="rawAccount">account number to check</param>
        /// <returns>true if passes</returns>
        public static bool CheckLUHN(string rawAccount)
        {
            int sum = 0;
            int doubled = 0;
            int digit;

            // clean the credit card number to be only digits
            string account = CleanCardNumber(rawAccount);

            // walk back through the card number
            for (int i = 1; i <= account.Length; i++)
            {
                // convert the character to a numeric digit by subtracting ascii zero
                digit = (int)(account[account.Length - i] - '0');
                if ((i % 2) == 0)
                {
                    doubled = digit * 2;
                    sum += (doubled % 10);
                    if (doubled >= 10)
                    {
                        sum++;
                    }
                }
                else
                {
                    sum += digit;
                }
            }
            return ((sum % 10) == 0);
        }

        /// <summary>
        /// Attempt to detect the card type from the supplied account number
        /// </summary>
        /// <param name="rawAccount">card number to test</param>
        /// <returns>card type - Unknown if not detectable</returns>
        public static CardTypes DetectCardType(string rawAccount)
        {
            CardTypes ct = CardTypes.Unknown;

            // clean up the supplied account number
            string account = CleanCardNumber(rawAccount);

            // perform each of the checks and assign the card type on the first that passes
            if (visaCheck.IsMatch(account))
            {
                ct = CardTypes.Visa;
            }
            else if (masterCardCheck.IsMatch(account))
            {
                ct = CardTypes.MasterCard;
            }
            else if (discoverCheck.IsMatch(account))
            {
                ct = CardTypes.Discover;
            }
            else if (amExCheck.IsMatch(account))
            {
                ct = CardTypes.American_Express;
            }
            else
            {
                ct = CardTypes.Unknown;
            }

            return ct;
        }

        /// <summary>
        /// Checks to see if the supplied number is valid for the specified card type
        /// </summary>
        /// <param name="rawAccount">account number</param>
        /// <param name="ct">card type</param>
        /// <returns>true if card type is valid</returns>
        public bool CheckCardType(string rawAccount, CardTypes ct)
        {
            bool ret;

            // convert the account number to digits
            string account = CleanCardNumber(rawAccount);

            // check based on the specified card type
            switch (ct)
            {
                case CardTypes.Visa:
                    ret = visaCheck.IsMatch(account);
                    break;
                case CardTypes.MasterCard:
                    ret = visaCheck.IsMatch(account);
                    break;
                case CardTypes.Discover:
                    ret = discoverCheck.IsMatch(account);
                    break;
                case CardTypes.American_Express:
                    ret = amExCheck.IsMatch(account);
                    break;
                default:
                    // unknown card types are always invalid
                    ret = false;
                    break;
            }
            return ret;
        }

        /// <summary>
        /// Create an expunged representation of the credit card number supplied
        /// Expunged format is basically: ##xx xxxx xxxx #### without spaces
        /// </summary>
        /// <param name="rawAccount">raw account string</param>
        /// <returns>expunged version of card</returns>
        public static string ExpungeCardNumber(string rawAccount)
        {
            string expTemplate = "xxxxxxxxxxxxxxxx";

            // clean up the account number
            string account = CleanCardNumber(rawAccount);

            // create an expunged card with first two digits - bank of X - last four digits
            string exp = account.Substring(0, 2) + expTemplate.Substring(0, account.Length - 6) + account.Substring(account.Length - 4, 4);

            return exp;
        }

        /// <summary>
        /// Take a raw card number and clean up the formatting for length and only digits
        /// </summary>
        /// <param name="rawAccount">raw account string</param>
        /// <returns>sanitized account string</returns>
        public static string CleanCardNumber(string rawAccount)
        {
            string account = digitsOnly.Replace(rawAccount, "");
            // trim the card length to 16 digits or less
            if (account.Length > 16)
            {
                account = account.Substring(0, 16);
            }
            // work with a minimum of 13 digits
            account = account.PadRight(13, '0');
            return account;
        }

        /// <summary>
        /// Check the expiration date for validity
        /// </summary>
        /// <param name="month">expiration month 1-12</param>
        /// <param name="year">expiration year (four digit year)</param>
        /// <returns></returns>
        public static CardResults CheckExpiration(int month, int year)
        {
            // make sure that the month is valid
            if ((month < 1) || (month > 12))
            {
                return CardResults.InvalidExpiration;
            }
            // make sure that the year is reasonable
            if ((year < 2000) || (year > DateTime.Now.AddYears(20).Year))
            {
                return CardResults.InvalidExpiration;
            }

            // construct the first date where the card is expired
            DateTime expDate = new DateTime(year, month, 1);
            expDate = expDate.AddMonths(1);

            // see if the card is expired
            if (expDate <= DateTime.Now)
            {
                return CardResults.InvalidExpired;
            }

            return CardResults.OK;
        }
    }
}