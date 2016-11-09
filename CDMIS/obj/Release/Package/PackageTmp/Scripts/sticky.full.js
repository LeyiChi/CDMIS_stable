// Sticky v1.0 by Daniel Raftery
// http://thrivingkings.com/sticky
//
// http://twitter.com/ThrivingKings

(function ($) {

    // Using it without an object
    $.sticky = function (note, options, callback) { return $.fn.sticky(note, options, callback); };

    $.fn.sticky = function (note, options, callback) {
        // Default settings
        var position = 'bottom-right'; // top-left, top-right, bottom-left, or bottom-right

        var settings =
			{
			    'speed': 'fast',  // animations: fast, slow, or integer
			    'duplicates': true,  // true or false
			    'autoclose': 5000  // integer or false
			};

        // Passing in the object instead of specifying a note
        if (!note) {
            note = this.html();
        }

        if (options)
        { $.extend(settings, options); }

        // Variables
        var display = true;
        var duplicate = 'no';

        // Somewhat of a unique ID
        var uniqID = Math.floor(Math.random() * 99999);

        // Handling duplicate notes and IDs
        $('.sticky-note').each(function () {
            if ($(this).html() == note && $(this).is(':visible')) {
                duplicate = 'yes';
                if (!settings['duplicates'])
                { display = false; }
            }
            if ($(this).attr('id') == uniqID)
            { uniqID = Math.floor(Math.random() * 9999999); }
        });

        // Make sure the sticky queue exists
        if (!$('body').find('.sticky-queue').html())
        { $('body').append('<div class="sticky-queue ' + position + '"></div>'); }

        // Can it be displayed?
        if (display) {
            // Building and inserting sticky note
            $('.sticky-queue').prepend('<div class="sticky border-' + position + '" id="' + uniqID + '"></div>');
            $('#' + uniqID).append('<img src="/Content/Image/close.png" class="sticky-close" rel="' + uniqID + '" title="关闭" />');
            $('#' + uniqID).append('<div class="sticky-note" rel="' + uniqID + '">' + note + '</div>');

            // Smoother animation
            var height = $('#' + uniqID).height();
            $('#' + uniqID).css('height', height);

            $('#' + uniqID).slideDown(settings['speed']);
            display = true;

        }

        // Listeners
        $('.sticky').ready(function () {
            // If 'autoclose' is enabled, set a timer to close the sticky
            if (settings['autoclose'])
            { $('#' + uniqID).delay(settings['autoclose']).fadeOut(settings['speed']); }
        });
        // Closing a sticky
        $('.sticky-close').click(function ()
        { $('#' + $(this).attr('rel')).dequeue().fadeOut(settings['speed']); });


        $('.sticky-note').click(function () {
            $('#' + $(this).attr('rel')).dequeue().fadeOut(settings['speed']);
            var Str = $(this).find("p").attr("id");
            //alert(Str);
            window.location.href = "/MailBox/ReceiveDetail/" + Str;
        });

        // Callback data
        var response =
			{
			    'id': uniqID,
			    'duplicate': duplicate,
			    'displayed': display,
			    'position': position
			}

        // Callback function?
        if (callback)
        { callback(response); }
        else
        { return (response); }

    }
})(jQuery);


//获取最新发送的MessageNo
function GetLatestMeNo(Txt) {
    var MeNo = "";
    $.ajax({
        url: "/MailBox/GetLatestMeNo",
        type: "GET",
        dataType: "json",
        async: false,
        data: {
            PiReceiver: Txt
        },
        success: function (res) {
            if ((res != "") && (res != null)) {
                MeNo = res;
            }
        }
    });
    return MeNo;
}