/*!
* Copyright (c) 2012 - )|( Sanctuary Software Studio, Inc. - All rights reserved.
* 
* Filename:		    Utility.js
* Project:		    acrtinc.com/Scripts
* Description:	    Various site component events, effects etc...
* Dependencies:     jquery-1.8.2.min.js
*/

$(document).ready(function () {
    // Drop Down Navigation Script
    $(".navSection").hover(function () {
        $(this).children("div").show();
        $(".navSectionHere").children("div").hide();
    },
    function () {
        $(this).children("div").hide();
        $(".navSectionHere").children("div").hide();
    });

    $(".navSectionHere").hover(function () {
        $(this).children("div").show();
        $(".navSection").children("div").hide();
    },
    function () {
        $(this).children("div").hide();
        $(".navSection").children("div").hide();
    });

    // equal height columns for homepage boxes
    $(".homeBox").css({ "height": $(".homeBoxesContainer").height() - 75 });

    // notification message close button event
    $(".NotificationClose").bind(
    {
        click: function () {
            $(this).parent("div.NotificationBox").fadeOut();
        }
    });

    // textbox input placeholders
    var defaultPlaceholder = "PROMO CODE?";
    $("#txtPromoCode").bind(
	{
	    focus: function () {
	        var val = ($(this).attr("value") == defaultPlaceholder) ? "" : $(this).attr("value");
	        $(this).attr("value", val);
	    },
	    blur: function () {
	        var val = ($(this).attr("value") == "") ? defaultPlaceholder : $(this).attr("value");
	        $(this).attr("value", val);
	    }
	});
});