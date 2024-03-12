// Define a page specific shared/global object
window.page = {};

var toastService; // ??

// Add indexOf for old browsers (TODO: Move this?)
if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (elt /*, from*/) {
        var len = this.length >>> 0;

        var from = Number(arguments[1]) || 0;
        from = (from < 0)
            ? Math.ceil(from)
            : Math.floor(from);
        if (from < 0)
            from += len;

        for (; from < len; from++) {
            if (from in this &&
                this[from] === elt)
                return from;
        }
        return -1;
    };
}

$.validator.setDefaults({ ignore: null });

// Datepicker localization
//$.fn.datepicker.dates['nl'] = {
//    days: ["zondag", "maandag", "dinsdag", "woensdag", "donderdag", "vrijdag", "zaterdag", "zondag"],
//    daysShort: ["zo", "ma", "di", "wo", "do", "vr", "za", "zo"],
//    daysMin: ["zo", "ma", "di", "wo", "do", "vr", "za", "zo"],
//    months: [
//        "januari", "februari", "maart", "april", "mei", "juni", "juli", "augustus", "september", "oktober", "november",
//        "december"
//    ],
//    monthsShort: ["jan", "feb", "mrt", "apr", "mei", "jun", "jul", "aug", "sep", "okt", "nov", "dec"],
//    today: "Vandaag",
//    clear: "Wissen",
//    weekStart: 1,
//    format: "dd-mm-yyyy"
//};

(function ($) {
    $.fn.initializeContextBindings = function () {
        // Enable form validation
        $('form:not(.no-global-validation)', this).validation();

        // Datepicker
        //$('.input-date', this)
        //    .datepicker({
        //        language: "nl",
        //        format: "dd-mm-yyyy",
        //        autoclose: true,
        //        orientation: "top right",
        //        todayHighlight: true
        //    });

        $('.input-date', this)
            .next('.input-date-icon')
            .on('click',
                function () {
                    $(this).prev('.input-date').focus();
                });

        // Colorpicker
        $('.input-color')
            .minicolors({
                theme: 'bootstrap',
                letterCase: 'uppercase',
                control: 'hue',
                animationSpeed: 0
            });

        // Select boxes
        //var selectizeSettings = {
        //    delimiter: ";",
        //    selectOnTab: true,
        //    render: {
        //        item: function (data, escape) {
        //            var remove = '<div class="item-remove"><i class="fa fa-times"></i></div>';
        //            return '<div class="item">' +
        //                escape(data.text) +
        //                (this.settings.placeholder ? remove : "") +
        //                '</div>';
        //        }
        //    },
        //    onInitialize: function () {
        //        var self = this;

        //        this.revertSettings.$children.each(function () {
        //            $.extend(self.options[this.value], $(this).data());
        //        });

        //        this.$control.on('click',
        //            '.item-remove',
        //            function (e) {
        //                if (self.isLocked)
        //                    return;

        //                self.setValue(null);
        //            });
        //    }
        //};

        //$('select', this)
        //    .not('.no-selectize')
        //    .not('.input-allowed')
        //    .not('.multicolumn')
        //    .selectize(selectizeSettings)
        //    .trigger('selectized');

        //$('select.multicolumn', this)
        //    .not('.no-selectize')
        //    .selectize($.extend(selectizeSettings,
        //        {
        //            render: {
        //                'option': function (data, escape) {
        //                    var cells = data.text.split("|");
        //                    var cellHtml = "";
        //                    for (var i = 0; i < cells.length; i++) {
        //                        cellHtml += '<div class="option-cell">' + escape(cells[i]) + '</div>';
        //                    }

        //                    return '<div class="option option-row">' + cellHtml + '</div>';
        //                },
        //                'item': function (data, escape) {
        //                    return '<div class="item">' + escape(data.text.split("|")[0]) + '</div>';
        //                }
        //            }
        //        }))
        //    .trigger('selectized');

        //$('select.input-allowed', this)
        //    .not('.no-selectize')
        //    .not('.multicolumn')
        //    .selectize($.extend(selectizeSettings,
        //        {
        //            create: true,
        //            createOnBlur: true,
        //            persist: false,
        //            render: {
        //                'option_create': function (data, escape) {
        //                    return '<div class="create">Anders: <strong>' + escape(data.input) + '</strong>&hellip;</div>';
        //                }
        //            }
        //        }))
        //    .trigger('selectized');
    };
}(jQuery));

$(function () {
    toastService = new ToastService();
    toastService.displayDropMessage();

    $("body").initializeContextBindings();

    //$.i18n().load({
    //    en: "/Scripts/strings/en.json",
    //    nl: "/Scripts/strings/nl.json"
    //});

    // Currency input

    if (window.navigator.userAgent.indexOf('Trident/') !== -1) {
        // Filter IE 11
        $('body')
            .on('input',
                '.input-currency',
                function () {
                    var input = $(this)[0];
                    var cursor = input.selectionStart;

                    // Replace DOT to COMMA (display only)
                    $(this).val($(this).val().replace(".", ","));
                    // IE 11 (win 7) ignores other events (onchange) when doing this

                    if ($(this).val().length > 0 && (cursor || cursor == '0')) {
                        input.selectionStart = cursor;
                        input.selectionEnd = cursor;
                    }
                });
    }

    $('body')
        .on('focus',
            '.input-currency',
            function () {
                if (parseFloat($(this).val()) == 0) {
                    $(this).data('defaultValue', $(this).val());
                    $(this).val("");
                }
            })
        .on('blur',
            '.input-currency',
            function () {
                if ($(this).val() == "" && $(this).data('defaultValue')) {
                    $(this).val($(this).data('defaultValue'));
                }

                // Format the number
                var value = $(this).val().replace(",", ".");
                if ($.isNumeric(value)) {
                    $(this).val(parseFloat(value).toFixed(2).replace(".", ","));
                }
            });

    $("body").on("click", "[data-lang]", function () {
        $.ajaxAction($(this).data("url")).done(function () {
            alert("piep!");
        });
    });

    // Timespan input
    $('body')
        .on('input',
            '.input-time',
            function () {
                var input = $(this)[0];
                var cursor = input.selectionStart;

                // Replace characters
                $(this).val($(this).val().replace(".", ":").replace("-", ":").replace(/[^0-9:]/g, ''));

                // IE doesn't like this
                if ($(this).val().length > 0 && (cursor || cursor == '0')) {
                    input.selectionStart = cursor;
                    input.selectionEnd = cursor;
                }
            })
        .on('blur',
            '.input-time',
            function () {
                var value = $(this).val().replace(/[^0-9]/g, '');
                var hours = 0;
                var minutes = 0;

                if (value.length > 3) {
                    hours = parseInt(value[0] + "" + value[1]);
                    minutes = parseInt(value[2] + "" + value[3]);
                } else if (value.length == 3) {
                    hours = parseInt(value[0]);
                    minutes = parseInt(value[1] + "" + value[2]);
                } else if (value > 0) {
                    hours = parseInt(value);
                } else {
                    $(this).val("");
                    return;
                }

                if ($(this).hasClass("input-time-hours-only")) {
                    hours = minutes > 30 ? hours + 1 : hours;
                    minutes = 0;
                }

                $(this)
                    .val(("0" + Math.max(0, Math.min(hours, 23))).slice(-2) +
                        ":" +
                        ("0" + Math.max(0, Math.min(minutes, 59))).slice(-2));
            });

    // Form submit via Ajax
    $('body')
        .on('submit',
            'form:not(.no-ajax)',
            function (e) {
                e.preventDefault();

                if (!$(this).valid() || $(this).data('ajax-busy') == true)
                    return;

                $(this).data('ajax-busy', true);

                var $form = $(this);
                var $files = $(this).find('input[type="file"]').filter(function () { return this.files.length > 0 });
                var postData;

                if ($files.length > 0) {
                    postData = new FormData();

                    var formFields = $form.serializeArray();
                    $.each(formFields,
                        function (i, field) {
                            postData.append(field.name, field.value);
                        });

                    $files.each(function (i, field) {
                        $.each(this.files,
                            function (j, file) {
                                postData.append(field.name, file);
                            });
                    });
                } else {
                    postData = $(this).serialize();
                }

                $.ajaxAction($(this).attr('action'), postData)
                    .done(function (data) {
                        var submitDoneEvent = $.Event("submitDone");
                        $form.trigger(submitDoneEvent, data);
                    })
                    .fail(function (data) {
                        var submitFailEvent = $.Event("submitFail");
                        $form.trigger(submitFailEvent, data);

                        if (submitFailEvent.isDefaultPrevented())
                            return;
                        if (data.ModelErrors && data.ModelErrors.length > 0)
                            toastr.error(data.ModelErrors.join("<br>"), "Error sending data");
                        else if (data.Message != undefined && data.Message != null) // Prevent default toast
                            toastr.error(data.Message, "");
                        //else if (!data.DropMessage) // Prevent default toast
                        //    toastr.error("No details available.", "Error sending data");
                    })
                    .always(function () {
                        $form.data('ajax-busy', false);
                    });
            });

    var hash = window.location.hash.substr(1);

    if (hash.indexOf("tab-pane-") === 0) {
        $('.tab-pane').hide();
        $('#' + hash).show();

        $('.nav-tabs li a.active').removeClass('active');
        //$("a[href='#" + hash + "']").addClass('active');
        $("a[href='#" + hash + "']").click();
    }
    //// Page tabs
    $('body')
        .on('click',
            '.nav-tabs li a',
            function (e) {
                e.preventDefault();
                var val = $(this).parent().closest('div');
                if (!$(this).hasClass('disabled') && !val.hasClass('statictab')) {
                    $('.tab-pane').hide();
                    $($(this).attr('href')).show();

                    $('.nav-tabs li a.active').removeClass('active');
                    //$(this).addClass('active');
                    setTimeout(function () {
                        $(this).click();
                    },
                        100);

                    if (history.replaceState)
                        history.replaceState(null, "", $(this).attr('href'));
                }
            });

    // Image uploader
    $('body')
        .on('click',
            '.uploaded-image-clickable',
            function () {
                $(this).imageUploader();
            });

    //$(".fieldList").fieldList();

    // Feedback button
    $('#button-feedback')
        .off('click')
        .on('click',
            function () {
                $.ajaxDialog('/Feedback/');
            });

    // General dialog and action links
    $(document.body)
        .on('click',
            '[data-click-dialog]',
            function (e) {
                e.preventDefault();
                var $this = $(this);
                var gridId = $this.attr('data-click-submit');

                $.ajaxDialog($this.attr('data-click-dialog'),
                    {
                        closeAll: ($this.data('dialog-closeall') !== undefined &&
                            $this.data('dialog-closeall').toLowerCase() !== "false"),
                        reloadGrid: { isReload: (gridId !== ""), reloadData: (gridId !== "" ? "#" + gridId : null) }
                    })
                    .done(function (e) {
                        if (e.Content && e.Content.Refresh) {
                            location.reload(true);
                        } else {
                            $this.trigger('dialogDone');
                        }
                    });
            });

    $(document.body)
        .on('click',
            '[data-click-action]',
            function (e) {
                e.preventDefault();
                var $this = $(this);

                var confirmText = $(this).attr('data-click-action-confirm');
                var actionUrl = $(this).attr('data-click-action');
                var gridId = $(this).attr('data-click-action-delete');

                if (confirmText !== null && confirmText !== undefined) {
                    bootbox.confirm({
                        buttons: {
                            confirm: {
                                label: 'OK',
                                className: 'btn-primary'
                            },
                            cancel: {
                                label: 'Cancel',
                                className: 'btn-default'
                            }
                        },
                        message: confirmText,
                        callback: function (result) {
                            if (result) {
                                $.ajaxAction(actionUrl)
                                    .done(function (e) {
                                        if (e.Content && e.Content.Refresh) {
                                            location.reload(true);
                                        } else {
                                            $this.trigger('actionDone');
                                            $("#" + gridId).fadeOut(200, function () {
                                                $('#' + gridId).data('kendoGrid').dataSource.read();
                                                $('#' + gridId).data('kendoGrid').refresh();
                                                $("#" + gridId).fadeIn(150);
                                            });
                                        }
                                    });
                            }
                        }
                    });
                } else {
                    $.ajaxAction(actionUrl)
                        .done(function (e) {
                            if (e.Content && e.Content.Refresh) {
                                location.reload(true);
                            } else {
                                $this.trigger('actionDone');
                            }
                        });
                }
            });

    $(document.body)
        .on('click',
            '[data-update-status]',
            function (e) {
                var $this = $(this);
                var confirmText = $(this).attr('data-click-action-confirm');
                var actionUrl = $(this).attr('data-update-status');
                var gridId = $(this).attr('data-click-action-cancle');

                if (confirmText !== null && confirmText !== undefined) {
                    bootbox.confirm({
                        buttons: {
                            confirm: {
                                label: 'OK',
                                className: 'btn-primary'
                            },
                            cancel: {
                                label: 'Cancel',
                                className: 'btn-default'
                            }
                        },
                        message: confirmText,
                        callback: function (result) {
                            if (result) {
                                $.ajaxAction(actionUrl)
                                    .done(function (e) {
                                        $this.trigger('actionDone');
                                    });
                            }
                            $("#" + gridId).fadeOut(200, function () {
                                $('#' + gridId).data('kendoGrid').dataSource.read();
                                $('#' + gridId).data('kendoGrid').refresh();
                                $("#" + gridId).fadeIn(150);
                            });
                        }
                    });
                } else {
                    $.ajaxAction(actionUrl)
                        .done(function (e) {
                            $this.trigger('actionDone');
                        });
                }
            });
});

(function ($) {
    var arrayExpression = /(\[[0-9]+?\])/g;

    var regReplace = function (val, index, newVal) {
        var parts = val.split(arrayExpression);
        parts[(index + 1) * 2 - 1] = '[' + newVal + ']';
        return parts.join('');
    };

    var replaceAttr = function (attribute, index, newVal) {
        var result = regReplace($(this).attr(attribute), index, newVal);
        $(this).attr(attribute, result);

        return result;
    };

    var isLetter = function (c) {
        if (65 <= c && c <= 90)
            return true;
        if (97 <= c)
            return c <= 122;
        return false;
    };

    var isDigit = function (c) {
        if (48 <= c)
            return c <= 57;
        return false;
    };

    var isAllowableSpecialCharacter = function (c) {
        return (c === "-" || c === ":" || c === "_");
    };

    var nameToId = function (name) {
        var id = [];

        for (var i = 0, len = name.length; i < len; i++) {
            var charCode = name.charCodeAt(i);

            if (!isLetter(charCode) && !isDigit(charCode) && !isAllowableSpecialCharacter(name[i])) {
                id.push("_");
            } else {
                id.push(name[i]);
            }
        }

        return id.join('');
    };

    $.fn.renumberMvcInput = function (arrayIndex) {
        arrayIndex = arrayIndex || 0;

        return this.each(function (i) {
            var $row = $(this);

            $(this)
                .find(':input')
                .each(function () {
                    var name = replaceAttr.call(this, 'name', arrayIndex, i); // Replace name
                    var id = $(this).attr('id');
                    var newId = nameToId(name);
                    $row.find('label[for="' + id + '"]').attr('for', newId); // Replace label for
                    $(this).attr('id', newId); // Replace id
                });
        });
    };
}(jQuery));

(function ($) {
    $.adjustDataTableColumns = function () {
        $($.fn.dataTable.tables(true)).DataTable()
            .columns.adjust();
    }
}(jQuery));