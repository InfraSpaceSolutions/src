using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sancsoft.Web;

/// <summary>
/// Summary description for PayPalErrorHandler
/// </summary>
public class PaypalErrorHandler
{
	//public static RC GetResultCode ( string errorID )
	//{
	//	RC resultCode = RC.PPAccountError;
	//	long errID = WebConvert.ToLong( errorID, 0 );

	//	if ( errID == 0 )
	//	{
	//		return RC.Ok;
	//	}

	//	switch ( errID )
	//	{
	//		case 10512:
	//			resultCode = RC.PPMissingFirstName;
	//			break;
	//		case 105113:
	//			resultCode = RC.PPMissingLastName;
	//			break;
	//		case 10561: case 10565: case 10709:
	//		case 10710: case 10711: case 10712:
	//		case 10713: case 11587: case 10751:
	//			resultCode = RC.PPInvalidBillingAddress;
	//			break;
	//		case 10510: case 10504:
	//			resultCode = RC.PPCreditCardType;
	//			break;
	//		case 10508:
	//			resultCode = RC.CardExpirationDate;
	//			break;
	//		case 10748:
	//			resultCode = RC.PPInvalidCVV;
	//			break;
	//		case 11585: case 10535: case 10502: case 10527:
	//			resultCode = RC.PPInvalidCreditCard;
	//			break;
	//		case 13122:
	//			resultCode = RC.PPTransactionRefusal;
	//			break;
	//		default:
	//			if ( errID == 10001 || errID == 10003 || errID == 10004 || errID == 10478 || errID == 10505 || errID == 10507 || errID == 10509 )
	//			{
	//				resultCode = RC.PPInvalidArguments;
	//			}
	//			break;
	//	}
	//	return resultCode;
	//}
}