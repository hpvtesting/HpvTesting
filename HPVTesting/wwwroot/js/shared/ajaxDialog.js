(function ($) {
    // This is a sort of "static" counter that helps us increment modal anchors.
    // Thanks to Dipen for figuring this trick out.
    var dialogStack = 1;
    $.ajaxDialog = function (url, options) {
        $.loading(true);
        options = $.extend({}, { onSubmitDone: function () { }, closeAll: false, reloadPage: false, reloadGrid: { isReload: false, reloadData: null } }, options);
        var modalContainerId = "page-modal-container-" + dialogStack;

        if ($('#' + modalContainerId).length === 0) {
            $('<div id="' + modalContainerId + '"></div>').appendTo('body');
        }
        //IsSessionExists();

        return $.ajax({
            type: "GET",
            url: url,
            cache: false,
            headers: { 'X-Request-Href': location.href }
        })
            .done(function (data) {
                $.loading(false);

                if (options.closeAll) {
                    // TODO: this will close multiple windows. We should not have an id that can be used multiple times.
                    $("#page-modal").modal('hide');
                }
                var pageModalSelector = "#" + modalContainerId + " #page-modal";
                if ($(pageModalSelector).length > 0) {
                    $(pageModalSelector).modal('hide');
                }

                $('#' + modalContainerId).html(data);
                $(pageModalSelector).modal('show');

                // Initialize all bindings in the context of this dialog (From app.js)
                $(pageModalSelector).initializeContextBindings();

                // Resize textarea in dialogs
                $(pageModalSelector + " textarea")
                    .on('input dummy',
                        function () {
                            var initHeight = $(this).data('initheight');
                            if (!initHeight) {
                                initHeight = $(this).height();
                                $(this).data('initheight', initHeight);
                            }

                            $(this).height(0);
                            var newHeight = $(this)[0].scrollHeight + 10;
                            if (newHeight < initHeight) {
                                newHeight = initHeight;
                            } else if (newHeight > 220) {
                                newHeight = 220;
                            }

                            $(this).height(newHeight);
                        })
                    .trigger('dummy');

                var $firstControl = $(pageModalSelector + ' :input:visible').not(':button').first();
                if ($firstControl
                    .closest('.selectize-input')
                    .length <
                    1 &&
                    $firstControl.filter('.input-date').length < 1) {
                    $firstControl.focus(); // focus first input field if not a selectbox
                }

                $(pageModalSelector).on('hidden.bs.modal', function () {
                    $(this).parent().remove();
                });

                var $form = $(pageModalSelector + ' form');
                if ($form.length === 1) {
                    $form.on('submitDone',
                        function (e, formReturnData) {
                            $(pageModalSelector).modal('hide');
                            options.onSubmitDone.bind(this)();
                            if (options.reloadPage) {
                                window.location.reload();
                            }
                            //if (options.reloadGrid.isReload) {
                            //    var grid = $(options.reloadGrid.reloadData).data("kendoGrid");
                            //    grid.dataSource.read();
                            //}
                        });
                }
                dialogStack++;
            })

            .fail(function (xhr, textStatus, errorThrown) {
                $.loading(false);
                toastService.error('Oops something wrong', errorThrown);
            });
    };
}(jQuery));