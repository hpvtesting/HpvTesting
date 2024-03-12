(function ($) {
    $.loading = function (activate) {
        var overlay = '<div id="loading-overlay"><div id="loading-indicator"></div></div>';

        if (activate) {
            // Check if another request is already loading
            if ($('#loading-overlay').length > 0) {
                $('#loading-overlay').data('requests', $('#loading-overlay').data('requests') + 1);
                return;
            }

            $('#loading-overlay').remove();

            var timeout = setTimeout(function () {
                $('#loading-overlay').addClass('waiting');
            },
                500);

            $(overlay).appendTo('body').data('timeout', timeout).data('requests', 1);
        } else {
            if ($('#loading-overlay').data('requests')) {
                var requests = $('#loading-overlay').data('requests');

                if (requests > 1) {
                    $('#loading-overlay').data('requests', requests - 1);
                    return;
                }
            }

            if ($('#loading-overlay').data('timeout')) {
                clearTimeout($('#loading-overlay').data('timeout'));
            }

            $('#loading-overlay').remove();

            // IE doesn't like the combination of the CSS transition and animation

            //if ($('#loading-overlay').hasClass('waiting')) {
            //    $('#loading-overlay').removeClass('waiting');

            //    setTimeout(function() {
            //        $('#loading-overlay').remove();
            //    }, 500);

            //} else {
            //    $('#loading-overlay').remove();
            //}
        }
    };
}(jQuery));