/*!
* Copyright (c) 2012 - )|( Sanctuary Software Studio, Inc. - All rights reserved.
* 
* Filename:		    Utility.js
* Project:		    acrtinc.com/Scripts
* Description:	    Various site component events, effects etc...
* Dependencies:     jquery-1.8.2.min.js
*/

// Mobile width trigger
var mobileTriggerWidth = 991;

$(document).ready(function () {
    // Encapsulate the window and body
    var $window = $(window);
    var $body = $("body");

    // Qualify mobile responsive
    if ($window.width() > mobileTriggerWidth) {
        return;
    }

    //
    //== Mobile Navigation
    //

    // Mobile menu/icons
    var $menus = $("#navigationWrapper").add("#user-menu-mobile");
    var $menuIcons = $("#mobile-menu-icons > img");

    $menuIcons.on({
        click: function () {
            var $clicked = $(this);
            var $menu = $($clicked.data("menu"));
            if ($menu.length <= 0) return;
            if ($menu.hasClass("open")) {
                $clicked.removeClass("active");
                $menu.removeClass("open");
                return;
            }
            // Reset targets
            $menuIcons.removeClass("active");
            $menus.removeClass("open");

            // Mark targets
            $clicked.addClass("active");
            $menu.addClass("open");
        }
    });

    // Sections/Dropdowns
    var $sections = $(".navSection").add(".navSectionHere");
    var $dropDowns = $(".navDropDown");
    $sections.on({
        click: function () {
            var $clicked = $(this);
            var $dropDown = $clicked.find(".navDropDown");
            if ($dropDown.length <= 0) return;
            if ($clicked.hasClass("open")) {
                $clicked.removeClass("open");
                $dropDown.removeClass("open");
                return;
            }
            $sections.removeClass("open");
            $dropDowns.removeClass("open");
            $clicked.addClass("open");
            $dropDown.addClass("open");
        }
    });

    // Append the open class to targeted dropdowns marked open on page load
    $(".navSectionHere").each(function () {
        var $dropDown = $(this).find(".navDropDown");
        if ($dropDown.length > 0) {
            $dropDown.addClass("open");
        }
    });

    //
    //== Responsive iframes:
    //   Primarily for YouTube video iframes and responsive formatting
    //   Ensure targeted iframes within the managed content container
    //   are inside the Bootstrap 'embed-responsive' wrapper
    $("iframe").each(function () {
        var $iframe = $(this);

        // Define the responsive iframe/embed css class
        var requireParent = "embed-responsive embed-responsive-4by3";

        // Check if the iframes' parent container has the
        // target class and continue next accordingly
        if ($iframe.parent().hasClass(requireParent)) return true;

        // Create/configure a new parent container
        // and insert to the dom before the iframe
        var $parent = $("<div/>")
          .addClass(requireParent)
          .html($iframe.html())
          .insertBefore($iframe);

        // Move/prepend the iframe to the new parent
        $iframe.prependTo($parent);
    });
});